using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Extentions
{
    public static class SubscriptionStateUtilities
    {
        public static void AddStateHistory(this Subscription subscription, CustomerState newState, int? userId = null)
        {
            subscription.SubscriptionStateHistories.Add(new SubscriptionStateHistory()
            {
                OldState = subscription.State,
                NewState = (short)newState,
                ChangeDate = DateTime.Now,
                UserID = userId
            });
        }

        public static bool IsValidChange(CustomerState oldState, CustomerState newState)
        {
            switch (newState)
            {
                case CustomerState.Registered:
                    if (oldState != CustomerState.Reserved)
                    {
                        return false;
                    }
                    break;
                case CustomerState.Reserved:
                    if (oldState != CustomerState.Registered)
                    {
                        return false;
                    }
                    break;
                case CustomerState.Active:
                    if (oldState != CustomerState.Disabled && oldState != CustomerState.Reserved)
                    {
                        return false;
                    }
                    break;
                case CustomerState.Disabled:
                    if (oldState != CustomerState.Active)
                    {
                        return false;
                    }
                    break;
                case CustomerState.Cancelled:
                    return true;
                default:
                    return false;
            }
            return true;
        }
    }
}
