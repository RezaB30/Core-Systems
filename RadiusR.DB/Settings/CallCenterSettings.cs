using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public static class CallCenterSettings
    {
        public static string CallCenterAPIKey
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string CallCenterInternalIDs
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static IEnumerable<string> CallCenterInternalIDList
        {
            get
            {
                return CallCenterInternalIDs.Split(',');
            }
        }

        #region METHODS
        /// <summary>
        /// Clears settings cache.
        /// </summary>
        public static void ClearCache()
        {
            var props = typeof(CallCenterSettings).GetProperties();
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
