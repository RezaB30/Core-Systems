using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public static class SchedulerSettings
    {
        public static TimeSpan SchedulerStartTime
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<TimeSpan>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static TimeSpan SchedulerStopTime
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<TimeSpan>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static TimeSpan SchedulerRetryDelay
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<TimeSpan>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static TimeSpan SMSSchedulerStartTime
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<TimeSpan>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static TimeSpan SMSSchedulerStopTime
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<TimeSpan>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static int SMSSchedulerPaymentReminderThreshold
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static int SMSSchedulerPrepaidReminderThreshold
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static short SchedulerBillingType
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<short>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #region METHODS
        /// <summary>
        /// Clears settings cache.
        /// </summary>
        public static void ClearCache()
        {
            var props = typeof(SchedulerSettings).GetProperties();
            foreach (var property in props)
            {
                DBSetting<RadiusREntities, AppSetting>.ClearCache(property.Name);
            }
        }
        /// <summary>
        /// Updates database values and clears cache.
        /// </summary>
        /// <param name="settings">Settings object.(only effective with "SettingElement" attribute)</param>
        public static void Update(object settings)
        {
            DBSetting<RadiusREntities, AppSetting>.Update(settings);
        }
        #endregion
    }
}
