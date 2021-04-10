using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Enums;

namespace RadiusR.DB.Utilities.Billing
{
    public static class ExtendPackage
    {
        /// <summary>
        /// Extends client service period for pre-paid clients and adds necessary data related to database.
        /// </summary>
        /// <param name="entities">Data entities.</param>
        /// <param name="dbSubscription">The customer to extend it's service.</param>
        /// <param name="addedPeriods">The count of extending periods.</param>
        /// <param name="paymentType">Type of payment.</param>
        /// <param name="accountantType">Type of accountant.</param>
        /// <param name="accountantId">The accountant identifier.</param>
        /// <returns>If was success or not.</returns>
        public static BillPayment.ResponseType ExtendClientPackage(this RadiusREntities entities, Subscription dbSubscription, int addedPeriods, PaymentType paymentType, BillPayment.AccountantType accountantType, int? accountantId = null, RadiusRBillingService paymentServiceUser = null)
        {
            var allTimeFees = dbSubscription.Fees.Where(fee => fee.FeeTypeCost.IsAllTime && fee.FeeTypeCost.FeeTypeID != (short)FeeType.Tariff);
            //var totalFee = dbSubscription.Service.Price * (decimal)addedPeriods + allTimeFees.Sum(fee => fee.FeeTypeCost.Cost ?? fee.FeeTypeVariant.Price) * (decimal)addedPeriods;
            var totalFee = dbSubscription.GetSubscriberPackageExtentionUnitPrice() * (decimal)addedPeriods;
            dbSubscription.RadiusAuthorization.ExpirationDate = (dbSubscription.RadiusAuthorization.ExpirationDate.HasValue) ? (DateTime.Now > dbSubscription.RadiusAuthorization.ExpirationDate) ? DateTime.Now.AddMonths(addedPeriods) : dbSubscription.RadiusAuthorization.ExpirationDate.Value.AddMonths(addedPeriods) : DateTime.Now.AddMonths(addedPeriods);

            var addedBill = new Bill()
            {
                AccountantID = accountantId,
                BillStatusID = (short)BillState.Paid,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now,
                PayDate = DateTime.Now,
                PaymentTypeID = (short)paymentType,
                Source = (short)BillSources.Manual,
                ExternalPayment = paymentServiceUser != null ? new ExternalPayment() { RadiusRBillingService = paymentServiceUser } :null,
                BillFees = allTimeFees.Select(fee => new BillFee()
                {
                    CurrentCost = (fee.FeeTypeCost.Cost ?? fee.FeeTypeVariant.Price) * (decimal)addedPeriods,
                    FeeTypeID = fee.FeeTypeID,
                    InstallmentCount = 1
                }).Concat(new List<BillFee>()
                    {
                        new BillFee()
                        {
                            CurrentCost = dbSubscription.Service.Price * (decimal)addedPeriods,
                            FeeTypeID = (short)FeeType.Tariff,
                            InstallmentCount = 1,
                            Description = dbSubscription.Service.Name
                        }
                    }).ToList()
            };
            dbSubscription.Bills.Add(addedBill);

            if (accountantType == BillPayment.AccountantType.Cashier)
            {
                var cashier = entities.Cashiers.FirstOrDefault(c => c.AppUserID == accountantId.Value);
                var cashierDeposit = cashier.CashierBalances.Sum(balance => balance.Amount);
                var cashierProfitCut = cashier.ProfitCut;

                if (totalFee < cashierProfitCut)
                {
                    // payment amount is less than cashier profit cut
                    return BillPayment.ResponseType.InvalidPaymentAmount;
                }
                if (cashierDeposit < totalFee)
                {
                    // not enough credits
                    return BillPayment.ResponseType.NotEnoughCredit;
                }

                cashier.CashierBalances.Add(new CashierBalance()
                {
                    Date = DateTime.Now,
                    Amount = (-1m * totalFee) + cashierProfitCut
                });
            }

            return BillPayment.ResponseType.Success;
        }

        public static decimal GetSubscriberPackageExtentionUnitPrice(this Subscription dbSubscription)
        {
            if (dbSubscription.HasBilling)
                return 0m;

            var allTimeFees = dbSubscription.Fees.Where(fee => fee.FeeTypeCost.IsAllTime && fee.FeeTypeCost.FeeTypeID != (short)FeeType.Tariff);
            var totalFee = dbSubscription.Service.Price + allTimeFees.Sum(fee => fee.FeeTypeCost.Cost ?? fee.FeeTypeVariant.Price);

            return totalFee;
        }
    }
}
