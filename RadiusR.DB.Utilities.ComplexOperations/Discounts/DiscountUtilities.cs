using RadiusR.SystemLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Discounts
{
    public static class DiscountUtilities
    {
        public static void CancelRecurringDiscount(RadiusREntities db, RecurringDiscount recurringDiscount, DiscountOperationOptions options)
        {
            var isReferralDiscount = recurringDiscount.ReferrerRecurringDiscount != null || recurringDiscount.ReferringRecurringDiscounts.Any();
            if (!recurringDiscount.AppliedRecurringDiscounts.Any() && !isReferralDiscount)
            {
                var id = recurringDiscount.ID;
                var subscriptionId = recurringDiscount.SubscriptionID;
                db.RecurringDiscounts.Remove(recurringDiscount);
                db.SystemLogs.Add(SystemLogProcessor.RemoveRecurringDiscount(id, options.CancellationCause, options.AppUserID, subscriptionId, options.LogInterface, options.LogInterfaceUsername));
            }
            else
            {
                recurringDiscount.IsDisabled = true;
                recurringDiscount.CancellationCause = (short)options.CancellationCause;
                recurringDiscount.CancellationDate = DateTime.Now;
                db.SystemLogs.Add(SystemLogProcessor.DisableRecurringDiscount(recurringDiscount.ID, options.CancellationCause, options.AppUserID, recurringDiscount.SubscriptionID, options.LogInterface, options.LogInterfaceUsername));

                if (isReferralDiscount)
                {
                    if (recurringDiscount.ReferrerRecurringDiscount != null)
                    {
                        recurringDiscount.ReferrerRecurringDiscount.IsDisabled = true;
                        recurringDiscount.ReferrerRecurringDiscount.CancellationCause = (short)options.CancellationCause;
                        recurringDiscount.ReferrerRecurringDiscount.CancellationDate = DateTime.Now;
                        db.SystemLogs.Add(SystemLogProcessor.DisableRecurringDiscount(recurringDiscount.ReferrerRecurringDiscount.ID, options.CancellationCause, options.AppUserID, recurringDiscount.ReferrerRecurringDiscount.SubscriptionID, options.LogInterface, options.LogInterfaceUsername));
                    }
                    if (recurringDiscount.ReferringRecurringDiscounts.Any())
                    {
                        foreach (var discount in recurringDiscount.ReferringRecurringDiscounts.ToArray())
                        {
                            discount.IsDisabled = true;
                            discount.CancellationCause = (short)options.CancellationCause;
                            discount.CancellationDate = DateTime.Now;
                            db.SystemLogs.Add(SystemLogProcessor.DisableRecurringDiscount(discount.ID, options.CancellationCause, options.AppUserID, discount.SubscriptionID, options.LogInterface, options.LogInterfaceUsername));
                        }
                    }
                }
            }
        }
    }
}
