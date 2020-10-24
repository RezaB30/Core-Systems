using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public static class BTKSettings
    {
        public static string BTKOperatorCode
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string BTKOperatorName
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string BTKOperatorType
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static int BTKOperatorDepartment
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #region METHODS
        /// <summary>
        /// Clears settings cache.
        /// </summary>
        public static void ClearCache()
        {
            var props = typeof(BTKSettings).GetProperties();
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
