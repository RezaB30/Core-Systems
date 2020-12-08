using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB;
using System.Data.Entity;
using System.Linq.Expressions;

namespace RadiusR.DB
{
    public partial class Subscription
    {
        public string ClientNo
        {
            get
            {
                return ID.ToString("0000000");
            }
        }

        [Obsolete("Use HasBilling instead.")]
        public bool IsPrePaid
        {
            get
            {
                if (Service.BillingType == (short)Enums.ServiceBillingType.PrePaid)
                    return true;
                return false;
            }
        }

        public bool HasBilling
        {
            get
            {
                if (Service.BillingType == (short)Enums.ServiceBillingType.Invoiced)
                {
                    return true;
                }

                return false;
            }
        }
        /// <summary>
        /// Gets the name surname combination. should not be used in a database query!
        /// </summary>
        public string ValidDisplayName
        {
            get
            {
                return Customer.ValidDisplayName;
            }
        }

        public bool IsActive
        {
            get
            {
                return State == (short)Enums.CustomerState.Active || State == (short)Enums.CustomerState.Reserved;
            }
        }

        public bool CanLoginOnline
        {
            get
            {
                return State == (short)Enums.CustomerState.Active || State == (short)Enums.CustomerState.Disabled;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return State == (short)Enums.CustomerState.Cancelled;
            }
        }

        public string DaysRemaining
        {
            get
            {
                if (LastAllowedDate.HasValue)
                {
                    var days = (LastAllowedDate.Value <= DateTime.Now.Date) ? 0 : (LastAllowedDate.Value - DateTime.Now.Date).Days;
                    return (days > 0) ? days.ToString() : "-";
                }
                return "-";
            }
        }


        /// <summary>
        /// The last usage date shown to client
        /// </summary>
        public DateTime? AlertDate
        {
            get
            {
                return LastAllowedDate.HasValue ? LastAllowedDate.Value.AddDays(-1) : (DateTime?)null;
            }
        }

        public class LastPeriodUsageInfo
        {
            public long TotalUpload { get; set; }

            public long TotalDownload { get; set; }
        }
    }
}
