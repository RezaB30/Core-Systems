using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class Service
    {
        public bool HasBilling
        {
            get
            {
                if (BillingType == (short)Enums.ServiceBillingType.Invoiced || BillingType == (short)Enums.ServiceBillingType.PreInvoiced)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Valid time table based on the current time of day.
        /// </summary>
        public ServiceRateTimeTable CurrentTimeTable
        {
            get
            {
                return ServiceRateTimeTables.FirstOrDefault(tt => (tt.StartTime < tt.EndTime && DateTime.Now.TimeOfDay >= tt.StartTime && DateTime.Now.TimeOfDay < tt.EndTime) || (tt.StartTime > tt.EndTime && (DateTime.Now.TimeOfDay >= tt.StartTime || DateTime.Now.TimeOfDay < tt.EndTime)));
            }
        }
        /// <summary>
        /// Valid rate limit based on time of day.
        /// </summary>
        public string CurrentRateLimit
        {
            get
            {
                return CurrentTimeTable != null ? CurrentTimeTable.RateLimit : RateLimit;
            }
        }
        /// <summary>
        /// Whether can have quota sale.
        /// </summary>
        public bool CanHaveQuotaSale
        {
            get
            {
                return QuotaType == (short)Enums.QuotaType.HardQuota || QuotaType == (short)Enums.QuotaType.SoftQuota;
            }
        }
    }
}
