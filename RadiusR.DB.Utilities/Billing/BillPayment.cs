using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public static class BillPayment
    {
        /// <summary>
        /// Pays bills and adds necessary data related to database.
        /// </summary>
        /// <param name="entities">Data entities.</param>
        /// <param name="bills">Bills to be paid.</param>
        /// <param name="paymentType">Type of payment.</param>
        /// <param name="accountantType">Type of accountant.</param>
        /// <param name="accountantId">The accountant identifier.</param>
        /// <returns>If was success or not.</returns>
        public static ResponseType PayBills(this RadiusREntities entities, IEnumerable<Bill> bills, PaymentType paymentType, AccountantType accountantType, int? accountantId = null, BillPaymentGateway gateway = null)
        {
            if (bills.Any())
            {
                foreach (var bill in bills)
                {
                    bill.BillStatusID = (short)BillState.Paid;
                    bill.PaymentTypeID = (short)paymentType;
                    bill.PayDate = DateTime.Now;
                    bill.AccountantID = accountantId;
                    // agent control
                    if (bill.Subscription.AgentID.HasValue && paymentType == PaymentType.Cash && bill.Subscription.AgentID != gateway?.PaymentAgent?.ID)
                    {
                        return ResponseType.CannotPayForAgentSubscriptionInCash;
                    }
                    // gateway check and set
                    if (gateway != null)
                    {
                        var gatewayCount = 0;
                        if (gateway.OfflineGateway != null) gatewayCount++;
                        if (gateway.PaymentPartner != null) gatewayCount++;
                        if (gateway.PaymentServiceUser != null) gatewayCount++;
                        if (gateway.PaymentAgent != null) gatewayCount++;

                        if (gatewayCount != 1)
                            return ResponseType.OnlyOneGatewayAllowed;

                        // agent allowance and commission
                        if (gateway.PaymentAgent != null)
                        {
                            if (gateway.PaymentAgent.ID != bill.Subscription.AgentID)
                            {
                                return ResponseType.SubscriptionDoesNotBelongToAgent;
                            }
                            var tariffFees = bill.BillFees.Where(bf => bf.FeeTypeID == (short)FeeType.Tariff).ToArray();
                            if (tariffFees.Length > 0)
                            {
                                var totalTariffPayableFee = tariffFees.Select(tf => tf.CurrentCost - (tf.Discount != null ? tf.Discount.Amount : 0m)).Sum();
                                var allowance = gateway.PaymentAgent.Allowance * totalTariffPayableFee;
                                if (paymentType == PaymentType.Cash)
                                    allowance -= bill.GetPayableCost();
                                var commission = 0m;
                                if (paymentType == PaymentType.MobilExpress || paymentType == PaymentType.VirtualPos || paymentType == PaymentType.PhysicalPos)
                                    commission = AgentsSettings.AgentsNonCashPaymentCommission;
                                bill.AgentRelatedPayments.Add(new AgentRelatedPayment()
                                {
                                    AgentID = gateway.PaymentAgent.ID,
                                    Allowance = allowance,
                                    ExtraCommission = commission,
                                });
                            }
                        }

                        if (gateway.PaymentPartner != null)
                            bill.PartnerBillPayment = gateway != null && gateway.PaymentPartner != null ? new PartnerBillPayment() { Partner = gateway.PaymentPartner, PartnerSubUser = gateway.PaymentPartnerSubUser } : null;
                        else
                            bill.ExternalPayment = new ExternalPayment() { OfflinePaymentGateway = gateway.OfflineGateway, RadiusRBillingService = gateway.PaymentServiceUser };

                    }
                    var expiredSMSes = bill.ScheduledSMSes.Where(ss => (ss.SMSType == (short)SMSType.PaymentReminder || ss.SMSType == (short)SMSType.FailedAutomaticPayment) && (!ss.ExpirationDate.HasValue || ss.ExpirationDate > DateTime.Now) && !ss.SendTime.HasValue).ToArray();
                    foreach (var sms in expiredSMSes)
                    {
                        entities.ScheduledSMS.Find(sms.ID).ExpirationDate = DateTime.Today;
                    }

                    entities.Entry(bill).State = EntityState.Modified;
                }

                // total client credit payments
                var totalCreditPayed = 0m;
                // set last allowed date
                var dbBillGroups = bills.GroupBy(bill => bill.Subscription);
                foreach (var group in dbBillGroups)
                {
                    var dbSubscription = group.Key;

                    // calculate new last allowed date
                    dbSubscription.UpdateLastAllowedDate();
                    entities.Entry(dbSubscription).State = EntityState.Modified;

                    // subtract client credits
                    {
                        var totalClientCredits = dbSubscription.SubscriptionCredits.Sum(credit => credit.Amount);
                        foreach (var bill in group.OrderBy(b => b.IssueDate).ThenBy(b => b.ID))
                        {
                            if (totalClientCredits <= 0m)
                                break;
                            var currentBillCost = bill.GetPayableCost();
                            var payedCredit = currentBillCost;
                            payedCredit = payedCredit > totalClientCredits ? totalClientCredits : payedCredit;
                            dbSubscription.SubscriptionCredits.Add(new SubscriptionCredit()
                            {
                                Amount = -1m * payedCredit,
                                BillID = bill.ID,
                                Date = DateTime.Now,
                                AccountantID = accountantId
                            });
                            totalClientCredits -= payedCredit;
                            totalCreditPayed += payedCredit;
                        }
                    }
                }

                // take from deposit
                if (accountantType == AccountantType.Seller || accountantType == AccountantType.Cashier)
                {
                    var totalPayment = bills.ToList().Sum(bill => bill.GetPayableCost());
                    totalPayment -= totalCreditPayed;
                    // take from cashier
                    if (accountantType == AccountantType.Cashier)
                    {
                        var cashier = entities.Cashiers.FirstOrDefault(c => c.AppUserID == accountantId.Value);
                        var cashierDeposit = cashier.CashierBalances.Sum(balance => balance.Amount);
                        var cashierProfitCut = cashier.ProfitCut;

                        if (totalPayment < cashierProfitCut)
                        {
                            // payment amount is less than cashier profit cut
                            return ResponseType.InvalidPaymentAmount;
                        }
                        if (cashierDeposit < totalPayment)
                        {
                            // not enough credits
                            return ResponseType.NotEnoughCredit;
                        }

                        cashier.CashierBalances.Add(new CashierBalance()
                        {
                            Date = DateTime.Now,
                            Amount = (-1m * totalPayment) + cashierProfitCut
                        });
                    }
                }
                // partner credit
                if (gateway != null && gateway.PaymentPartner != null)
                {
                    var totalPayment = bills.ToList().Sum(bill => bill.GetPayableCost()) - totalCreditPayed;
                    var totalPartnerCredits = gateway.PaymentPartner.PartnerCredits.Select(pc => pc.Amount).DefaultIfEmpty(0m).Sum();
                    var neededCredit = Math.Max(0m, totalPayment - (bills.Count() * gateway.PaymentPartner.PaymentAllowance));
                    if (neededCredit > totalPartnerCredits)
                    {
                        // not enough credits
                        return ResponseType.NotEnoughCredit;
                    }
                    foreach (var bill in bills)
                    {
                        var payedAmountWithoutCredit = bill.GetPayableCost() - (bill.SubscriptionCredits.Any() ? bill.SubscriptionCredits.Sum(sc => sc.Amount) : 0m);
                        // pay with allowance
                        if (payedAmountWithoutCredit > Math.Max(gateway.PaymentPartner.PaymentAllowance, gateway.PaymentPartner.MinAmountForAllowance))
                        {
                            gateway.PaymentPartner.PartnerCredits.Add(new PartnerCredit()
                            {
                                Amount = (-1m * payedAmountWithoutCredit) + gateway.PaymentPartner.PaymentAllowance,
                                BillID = bill.ID,
                                Date = DateTime.Now
                            });
                        }
                        // pay without allowance
                        else
                        {
                            gateway.PaymentPartner.PartnerCredits.Add(new PartnerCredit()
                            {
                                Amount = (-1m * payedAmountWithoutCredit),
                                BillID = bill.ID,
                                Date = DateTime.Now
                            });
                        }
                    }
                }

            }

            return ResponseType.Success;
        }

        public static ResponseType CancelPayment(this RadiusREntities entities, IEnumerable<Bill> bills)
        {
            bills = bills.Where(bill => bill.BillStatusID == (short)BillState.Paid);
            // if not empty
            if (bills.Any())
            {
                // clear client credit pays
                entities.SubscriptionCredits.RemoveRange(bills.Select(bill => bill.SubscriptionCredits).SelectMany(cc => cc));
                // reverse cashier/seller credit expenditure
                var cashierPaidBills = bills.Where(bill => bill.AccountantID.HasValue && bill.AppUser.Cashiers.Any());
                foreach (var bill in cashierPaidBills)
                {
                    var cashier = bill.AppUser.Cashiers.FirstOrDefault();
                    cashier.CashierBalances.Add(new CashierBalance()
                    {
                        Date = DateTime.Now,
                        Amount = bill.GetPayableCost() - cashier.ProfitCut
                    });
                }
                // reverse payments
                foreach (var bill in bills)
                {
                    bill.PayDate = null;
                    bill.PaymentTypeID = (short)PaymentType.None;
                    bill.AccountantID = null;
                    bill.BillStatusID = (short)BillState.Unpaid;
                    if (bill.ExternalPayment != null)
                        entities.ExternalPayments.Remove(bill.ExternalPayment);
                    if (bill.PartnerBillPayment != null)
                        entities.PartnerBillPayments.Remove(bill.PartnerBillPayment);
                    entities.Entry(bill).State = EntityState.Modified;
                    // reverse add partner credits
                    if (bill.PartnerCredits.Any())
                    {
                        foreach (var credit in bill.PartnerCredits)
                        {
                            entities.PartnerCredits.Add(new PartnerCredit()
                            {
                                Amount = Math.Abs(credit.Amount),
                                BillID = credit.BillID,
                                Date = DateTime.Now,
                                PartnerID = credit.PartnerID
                            });
                        }
                    }
                }

                // remove agent allowances
                var agentAllowances = bills.SelectMany(b => b.AgentRelatedPayments).ToArray();
                if (agentAllowances.Any())
                {
                    if (bills.Any(b => b.AgentCollection != null))
                    {
                        return ResponseType.CannotCancelBillsInAgentCollections;
                    }
                    entities.AgentRelatedPayments.RemoveRange(agentAllowances);
                }
                // set last allowed date
                var dbBillGroups = bills.GroupBy(bill => bill.Subscription);
                foreach (var group in dbBillGroups)
                {
                    var dbSubscription = group.Key;
                    dbSubscription.UpdateLastAllowedDate(true);
                    entities.Entry(dbSubscription).State = EntityState.Modified;
                }

                return ResponseType.Success;
            }
            return ResponseType.Success;
        }

        public static ResponseType CancelBills(this RadiusREntities entities, IEnumerable<Bill> bills)
        {
            if (bills.Any(b => b.BillStatusID != (short)BillState.Unpaid))
                return ResponseType.InvalidBillState;
            // cancel bills
            foreach (var bill in bills)
            {
                bill.BillStatusID = (short)BillState.Cancelled;
                entities.Entry(bill).State = EntityState.Modified;
            }
            // update last allowed dates
            var dbBillGroups = bills.GroupBy(bill => bill.Subscription);
            foreach (var group in dbBillGroups)
            {
                var dbSubscription = group.Key;
                dbSubscription.UpdateLastAllowedDate(true);
                entities.Entry(dbSubscription).State = EntityState.Modified;
            }

            return ResponseType.Success;
        }

        public enum AccountantType
        {
            Admin,
            Seller,
            Cashier
        }

        public enum ResponseType
        {
            Success,
            NotEnoughCredit,
            InvalidPaymentAmount,
            HasCashierPays,
            InvalidBillState,
            OnlyOneGatewayAllowed,
            CannotPayForAgentSubscriptionInCash,
            SubscriptionDoesNotBelongToAgent,
            CannotCancelBillsInAgentCollections
        }
    }
}
