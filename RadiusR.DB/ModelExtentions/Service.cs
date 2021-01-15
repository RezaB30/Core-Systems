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
                if (BillingType == (short)Enums.ServiceBillingType.Invoiced)
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
        /// <summary>
        /// Gets the best day of month for this tariff based on given day of month.
        /// </summary>
        /// <param name="currentDayOfMonth">The day of month to calculate for.</param>
        /// <returns></returns>
        public int? GetBestBillingPeriod(int currentDayOfMonth)
        {
            var availableOptions = ServiceBillingPeriods.OrderBy(sbp => sbp.DayOfMonth).Select(sbp => sbp.DayOfMonth).ToArray();
            if (!availableOptions.Any())
            {
                return null;
            }
            var result = availableOptions.Where(d => d <= currentDayOfMonth).LastOrDefault();
            if(result == default(short))
            {
                result = availableOptions.FirstOrDefault();
            }

            return result;
        }
    }
}
