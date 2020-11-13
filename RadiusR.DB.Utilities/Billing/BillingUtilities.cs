using RadiusR.DB;
using RadiusR.DB.ModelExtentions;
using RadiusR.DB.Enums;
using RadiusR.DB.Enums.RecurringDiscount;
using System;
using System.Collections.Generic;
using System.Linq;
using RadiusR.Scheduler.SMS;

namespace RadiusR.DB.Utilities.Billing
{
    /// <summary>
    /// Utility functions for billing
    /// </summary>
    public static class BillingUtilities
    {
        /// <summary>
        /// Issues bills without saving to database
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="clientId">Customer id</param>
        /// <param name="issueToDate">Date for issuing bills till that date (today if not specified)</param>
        public static void IssueBill(this BillingReadySubscription currentSubscription, DateTime? issueToDate = null)
        {
            currentSubscription._issueBill(false, issueToDate);
        }

        /// <summary>
        /// Issues bills and forces the last bill without saving to database
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="clientId">Customer id</param>
        /// <param name="issueToDate">Date for issuing bills till that date (today if not specified)</param>
        public static void ForceIssueBill(this BillingReadySubscription currentSubscription, DateTime? issueToDate = null, bool settleAllInstallments = true, bool ignoreReferralDiscounts = false, IEnumerable<BillingExtraFee> extraFees = null)
        {
            currentSubscription._issueBill(true, issueToDate, settleAllInstallments, ignoreReferralDiscounts, extraFees);
        }

        private static void _issueBill(this BillingReadySubscription currentSubscription, bool forceLastBill, DateTime? issueToDate = null, bool settleAllInstallments = true, bool ignoreReferralDiscounts = false, IEnumerable<BillingExtraFee> extraFees = null)
        {
            var today = DateTime.Today;
            issueToDate = issueToDate ?? today;
            // check for active & valid billing type subscriptions
            if (!currentSubscription.Subscription.IsActive || !currentSubscription.Subscription.ActivationDate.HasValue || currentSubscription.Subscription.Service.BillingType == (short)ServiceBillingType.PrePaid)
                return;
            // get lower boundry to start issuing bills
            var lowerBoundry = currentSubscription.Subscription.Bills.Where(b => b.Source == (short)BillSources.System && b.PeriodEnd.HasValue).Select(b => b.PeriodEnd).DefaultIfEmpty(currentSubscription.Subscription.ActivationDate).Max().Value;
            lowerBoundry = lowerBoundry < currentSubscription.Subscription.ActivationDate ? currentSubscription.Subscription.ActivationDate.Value : lowerBoundry;
            lowerBoundry = lowerBoundry < currentSubscription.Subscription.LastTariffChangeDate ? currentSubscription.Subscription.LastTariffChangeDate.Value : lowerBoundry;
            lowerBoundry = lowerBoundry.Date;
            // count added bills for installments
            var addedBillsCount = 0;
            // find relevant period
            var currentPeriod = currentSubscription.Subscription.GetCurrentBillingPeriod(lowerBoundry, ignorePreviousBills: true);
            while (true)
            {
                var isLastBillWithNoTariff = false;
                // if all periods are done return
                if (currentPeriod.StartDate > issueToDate)
                {
                    // if it is pre-invoiced and has force bill on
                    if (forceLastBill && SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PreInvoicing)
                        isLastBillWithNoTariff = true;
                    else
                        return;
                }
                // get partiality for current period
                var partiality = currentPeriod.GetPartiality(forceLastBill ? issueToDate : (DateTime?)null);
                // invalid partiality
                if (partiality == null)
                    return;
                //// tariff change for pre-invoiced
                //if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PreInvoicing && currentSubscription.Subscription.SubscriptionTariffChange != null)
                //{
                //    currentSubscription.Subscription.Service = currentSubscription.Subscription.SubscriptionTariffChange.Service;
                //    currentSubscription.Subscription.PaymentDay = currentSubscription.Subscription.SubscriptionTariffChange.NewBillingPeriod;
                //    currentSubscription.Subscription.SubscriptionTariffChange = null;
                //}
                // is last bill for invoiced subscription
                bool isLastPartialBill = false;
                // no more bills for post paid if we are currently in the period
                if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PostInvoicing && currentPeriod.EndDate > issueToDate)
                {
                    // it is last bill before cancellation
                    if (forceLastBill)
                        isLastPartialBill = true;
                    // no more bills
                    else
                        return;
                }

                //-------------- CALCULATE TARIFF FEE -----------------
                var tariffFee = currentSubscription.Subscription.GetCurrentTariffCost(currentSubscription.Usage.Select(u => new SubscriptionUtilities.DailyUsageInfo() { Date = u.Date, DownloadBytes = u.DownloadBytes, UploadBytes = u.UploadBytes }), currentPeriod, isLastPartialBill ? issueToDate : (DateTime?)null);
                // invalid tariff fee
                if (!tariffFee.HasValue)
                    return;
                //-------------- CALCULATE OTHER FEES -----------------
                // query signiture for retrieving fees
                var clientValidFees = currentSubscription.ValidFees.ValidBillFees(addedBillsCount, (isLastPartialBill || isLastBillWithNoTariff) && settleAllInstallments).ToList();
                // partiality for all time fees
                foreach (var billFee in clientValidFees.Where(bf => bf.FeeID == null))
                {
                    billFee.CurrentCost *= partiality.GeneralPartiality;
                }
                //-------------- ADD TARIFF FEE -----------------------
                if (!isLastBillWithNoTariff && tariffFee.Value > 0m)
                {
                    clientValidFees.Add(new BillFee()
                    {
                        CurrentCost = tariffFee.Value,
                        FeeTypeID = (short)FeeType.Tariff,
                        InstallmentCount = 1,
                        Description = currentSubscription.Subscription.Service.Name
                    });
                }
                //-------------- ADD EXTRA FEES IN LAST BILL -----------------------
                if (extraFees != null && (isLastBillWithNoTariff || isLastPartialBill))
                {
                    clientValidFees.AddRange(extraFees.Select(ef => new BillFee()
                    {
                        CurrentCost = ef.Cost,
                        InstallmentCount = 1,
                        Fee = !ef.IsAllTime ? new Fee()
                        {
                            Cost = ef.Cost,
                            Date = DateTime.Now,
                            FeeTypeID = (short)ef.FeeType,
                            FeeTypeVariantID = ef.VariantID,
                            InstallmentBillCount = 1,
                            SubscriptionID = currentSubscription.Subscription.ID
                        } : null
                    }));
                }
                //-------------- CREATE BILL --------------------------
                // adding new bill
                if (clientValidFees.Any())
                {
                    var newBill = new Bill()
                    {
                        BillStatusID = (short)BillState.Unpaid,
                        SubscriptionID = currentSubscription.Subscription.ID,
                        Subscription = currentSubscription.Subscription,
                        BillFees = clientValidFees.ToList(),
                        IssueDate = DateTime.Today,
                        DueDate = DateTime.Today.AddDays(currentSubscription.Subscription.Service.PaymentTolerance),
                        PaymentTypeID = (short)PaymentType.None,
                        Source = (short)BillSources.System,
                        PeriodStart = currentPeriod.StartDate,
                        PeriodEnd = isLastPartialBill ? issueToDate : currentPeriod.EndDate
                    };
                    // add SMS schedule
                    newBill.AddNewBillSMS();
                    //-------------- RECURRING DISCOUNTS --------------------------
                    // apply recurring discounts
                    var activeRecurringDiscounts = currentSubscription.ValidRecurringDiscounts.ToArray();
                    // only full length
                    if (partiality.GeneralPartiality < 1m)
                    {
                        activeRecurringDiscounts = activeRecurringDiscounts.Where(rd => !rd.OnlyFullInvoice).ToArray();
                    }
                    // ignore referral discounts
                    if (ignoreReferralDiscounts)
                    {
                        activeRecurringDiscounts = activeRecurringDiscounts.Where(rd => !rd.ReferralSubscriptionID.HasValue).ToArray();
                    }
                    // apply discounts               
                    foreach (var discount in activeRecurringDiscounts)
                    {
                        // check for discount uses 
                        if (discount.AppliedTimes >= discount.ApplicationTimes)
                            continue;
                        // ------------------ PREVIOUS PAYMENTS CHECK (REFERRAL ONLY) ----------------------
                        // check for previous payment expiration in referral discounts
                        var recurringDiscountApplicationState = RecurringDiscountApplicationState.Applied;
                        if (!discount.HasBeenPenalized && discount.ReferralSubscriptionID.HasValue)
                        {
                            var lastPairedBill = discount.PairedSubscriptionBills.Where(b => b.State == (short)BillState.Paid || b.DueDate < today).FirstOrDefault();
                            var currentSubPreviousBills = currentSubscription.Subscription.Bills.Where(b => b.ID > 0 && b.Source == (short)BillSources.System && b.BillStatusID != (short)BillState.Cancelled).OrderByDescending(b => b.IssueDate).ThenByDescending(b => b.ID).ToArray();
                            var currentSubscriptionLastBill = currentSubPreviousBills.Where(b => b.BillStatusID == (short)BillState.Paid || b.DueDate < today).FirstOrDefault();
                            // skip if one of the pair has not passed their first payment threshold
                            if (currentSubscriptionLastBill == null || lastPairedBill == null)
                            {
                                continue;
                            }
                            //
                            else if (currentSubscriptionLastBill.BillStatusID == (short)BillState.Unpaid || lastPairedBill.State == (short)BillState.Unpaid || currentSubscriptionLastBill.PayDate > currentSubscriptionLastBill.DueDate || lastPairedBill.PayDate > lastPairedBill.DueDate)
                            {
                                recurringDiscountApplicationState = RecurringDiscountApplicationState.Passed;
                                discount.HasBeenPenalized = true;
                            }
                        }
                        // ---------------- APPLY RECURRING DISCOUNT --------------------
                        // pass discount (referral discounts)
                        if (recurringDiscountApplicationState == RecurringDiscountApplicationState.Passed)
                        {
                            newBill.AppliedRecurringDiscounts.Add(new AppliedRecurringDiscount() { RecurringDiscount = discount.RecurringDiscount, ApplicationState = (short)RecurringDiscountApplicationState.Passed });
                        }
                        // apply discount
                        else
                        {
                            // fee discounts
                            if (discount.RecurringDiscount.ApplicationType == (short)RecurringDiscountApplicationType.FeeBased && discount.RecurringDiscount.FeeTypeID.HasValue && newBill.BillFees.Any(bf => bf.FeeTypeID == discount.RecurringDiscount.FeeTypeID.Value))
                            {
                                // applicable fees
                                var validFees = newBill.BillFees.Where(bf => bf.FeeTypeID == discount.RecurringDiscount.FeeTypeID.Value).ToArray();
                                foreach (var currentFee in validFees)
                                {
                                    var previousDiscountAmount = currentFee.Discount != null ? currentFee.Discount.Amount : 0m;
                                    var currentPayableAmount = currentFee.CurrentCost - previousDiscountAmount;
                                    var discountAmount = discount.RecurringDiscount.DiscountType == (short)RecurringDiscountType.Static ? discount.RecurringDiscount.Amount : discount.RecurringDiscount.Amount * currentFee.CurrentCost;
                                    var newDiscountAmount = Math.Min(discountAmount + previousDiscountAmount, currentFee.CurrentCost);
                                    if (newDiscountAmount > 0m)
                                        currentFee.Discount = new Discount()
                                        {
                                            Amount = newDiscountAmount,
                                            DiscountType = (short)DiscountType.System
                                        };
                                }
                                // set bill recurring discount
                                newBill.AppliedRecurringDiscounts.Add(new AppliedRecurringDiscount() { RecurringDiscount = discount.RecurringDiscount, ApplicationState = (short)RecurringDiscountApplicationState.Applied });
                            }
                            // bill discounts (only percentage type acceptable)
                            else if (discount.RecurringDiscount.ApplicationType == (short)RecurringDiscountApplicationType.BillBased && discount.RecurringDiscount.DiscountType == (short)RecurringDiscountType.Percentage)
                            {
                                // for all fees
                                foreach (var currentFee in newBill.BillFees)
                                {
                                    var previousDiscountAmount = currentFee.Discount != null ? currentFee.Discount.Amount : 0m;
                                    var currentPayableAmount = currentFee.CurrentCost - previousDiscountAmount;
                                    var discountAmount = discount.RecurringDiscount.Amount * currentFee.CurrentCost;
                                    var newDiscountAmount = Math.Min(discountAmount + previousDiscountAmount, currentFee.CurrentCost);
                                    if (newDiscountAmount > 0m)
                                        currentFee.Discount = new Discount()
                                        {
                                            Amount = newDiscountAmount,
                                            DiscountType = (short)DiscountType.System
                                        };
                                }
                                // set bill recurring discount
                                newBill.AppliedRecurringDiscounts.Add(new AppliedRecurringDiscount() { RecurringDiscount = discount.RecurringDiscount, ApplicationState = (short)RecurringDiscountApplicationState.Applied });
                            }
                        }
                        // add to uses
                        discount.AppliedTimes++;
                    }
                    //-------------- ZERO SUM BILLS --------------------------
                    // cancel bills which are 0 payment total and update last allowed date
                    if (newBill.GetPayableCost() <= 0m)
                    {
                        newBill.BillStatusID = (short)BillState.Cancelled;
                        currentSubscription.Subscription.UpdateLastAllowedDate(newlyAddedBill: newBill);
                    }
                    //-------------- CREDIT PAYMENT --------------------------
                    // if client does not have any unpaid bills check to pay with credits
                    else if (!currentSubscription.Subscription.Bills.Any(bill => bill.BillStatusID == (short)BillState.Unpaid))
                    {
                        // check against credits
                        var billCost = newBill.GetPayableCost();
                        if (billCost <= currentSubscription.CreditsTotal)
                        {
                            newBill.BillStatusID = (short)BillState.Paid;
                            newBill.PaymentTypeID = (short)PaymentType.Credit;
                            newBill.PayDate = DateTime.Now;
                            currentSubscription.CreditsTotal -= billCost;
                            newBill.SubscriptionCredits = new List<SubscriptionCredit>()
                            {
                                new SubscriptionCredit()
                                {
                                    SubscriptionID = currentSubscription.Subscription.ID,
                                    Amount = -1m * billCost,
                                    Date = DateTime.Now,
                                }
                            };
                            // add SMS schedule
                            newBill.AddPaymentBillSMS();
                            // update last allowed date if neccessary
                            currentSubscription.Subscription.UpdateLastAllowedDate(newlyAddedBill: newBill);
                        }
                    }
                    // add bill entity to context
                    currentSubscription.Subscription.Bills.Add(newBill);
                    // increment added bill count
                    addedBillsCount++;
                }
                // -----------------------------------------------------------
                // if it is last bill with no tariff stop
                if (isLastBillWithNoTariff)
                    return;
                // go to next period
                currentPeriod = currentSubscription.Subscription.GetCurrentBillingPeriod(currentPeriod.EndDate);
            }
        }

        public static void IssuePrePaidBill(this Subscription dbSubscription, int periodCount, bool hasSetupFee)
        {
            // retrieve client added fees
            var addedClientFees = dbSubscription.Fees.Where(fee => !fee.BillFees.Any() || fee.FeeTypeCost.IsAllTime).Select(fee => new BillFee()
            {
                CurrentCost = fee.FeeTypeID == (short)FeeType.Tariff ? dbSubscription.Service.Price : fee.Cost ?? fee.FeeTypeCost.Cost ?? fee.FeeTypeVariant.Price,
                FeeID = fee.FeeTypeCost.IsAllTime ? (long?)null : fee.ID,
                FeeTypeID = fee.FeeTypeCost.IsAllTime ? fee.FeeTypeID : (short?)null,
                InstallmentCount = 1
            }).ToList();

            foreach (var billFee in addedClientFees.Where(bf => !bf.FeeID.HasValue))
            {
                billFee.CurrentCost *= periodCount;
            }

            // setup fee
            if (hasSetupFee)
            {
                FeeTypeCost setupFee;
                using (RadiusREntities db = new RadiusREntities())
                {
                    setupFee = db.FeeTypeCosts.Find((short)FeeType.Setup);
                    addedClientFees.Add(new BillFee()
                    {
                        CurrentCost = setupFee.Cost.Value,
                        Fee = new Fee()
                        {
                            Cost = setupFee.Cost,
                            InstallmentBillCount = 1,
                            FeeTypeID = setupFee.FeeTypeID,
                            Date = DateTime.Now,
                            SubscriptionID = dbSubscription.ID
                        }
                    });
                }
            }

            // add bill
            dbSubscription.Bills.Add(new Bill()
            {
                BillStatusID = (short)BillState.Unpaid,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now,
                //PartialPercentage = periodCount,
                PaymentTypeID = (short)PaymentType.None,
                BillFees = addedClientFees,
                //ServiceID = dbSubscription.ServiceID,
                Source = (short)BillSources.System
            });
            // extend last allowed date
            dbSubscription.LastAllowedDate = DateTime.Now.Date.AddMonths(periodCount);
        }

        public static void CreateManualBill(this Subscription subscription, IEnumerable<long> FeeIDs)
        {
            var selectedFees = subscription.Fees.Where(f => FeeIDs.Contains(f.ID)).ToArray();
            if (FeeIDs.Except(selectedFees.Select(f => f.ID)).Any() || selectedFees.Any(f => f.IsCancelled || f.FeeTypeCost.IsAllTime || f.InstallmentBillCount != 1))
                throw new InvalidOperationException("Invalid selected fees for manual bill.");
            subscription.Bills.Add(new Bill()
            {
                BillStatusID = (short)BillState.Unpaid,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(subscription.Service.PaymentTolerance),
                PaymentTypeID = (short)PaymentType.None,
                BillFees = selectedFees.Select(f => new BillFee()
                {
                    CurrentCost = f.Cost ?? f.FeeTypeCost.Cost ?? f.FeeTypeVariant.Price,
                    FeeID = f.ID,
                    InstallmentCount = 1
                }).ToList(),
                Source = (short)BillSources.Manual
            });
        }

        /// <summary>
        /// Removes expired fees from query
        /// </summary>
        /// <param name="fees"></param>
        /// <returns>Active fees</returns>
        public static IEnumerable<BillFee> ValidBillFees(this IEnumerable<BillingReadySubscriptionFee> fees, int addedBillCount = 0, bool settleAllInstallments = true)
        {
            return fees.Where(fee => fee.CountedInstallments + addedBillCount < fee.TotalInstallments)
                .Select(fee => new BillFee()
                {
                    CurrentCost = Math.Round(fee.Cost / ((decimal)fee.TotalInstallments / (decimal)((!fee.IsAllTime && settleAllInstallments) ? (fee.TotalInstallments - fee.CountedInstallments - addedBillCount) : 1)), 2),
                    FeeID = fee.IsAllTime ? (long?)null : fee.ID,
                    FeeTypeID = fee.IsAllTime ? fee.FeeTypeID : (short?)null,
                    InstallmentCount = (!fee.IsAllTime && settleAllInstallments) ? fee.TotalInstallments - fee.CountedInstallments - addedBillCount : 1
                });
        }
    }
}
