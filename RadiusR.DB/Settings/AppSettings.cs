using RadiusR.DB;
using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;

namespace RadiusR.DB
{
    /// <summary>
    /// Manages settings for application.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Number of rows in a data table.
        /// </summary>
        public static int TableRows
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// <summary>
        /// Number of page buttons in a data table paging.
        /// </summary>
        public static int PagesLinkCount
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// <summary>
        /// Max number of steps in a line chart.
        /// </summary>
        public static int ChartMaxSteps
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        /// <summary>
        /// Phone No code prefix. (ex: +90)
        /// </summary>
        public static string CountryPhoneCode
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        // e-bill defaults
        public static bool EBillIsActive
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static decimal PastDueFlatPenalty
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<decimal>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static decimal PastDuePenaltyPercentage
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<decimal>(MethodBase.GetCurrentMethod().Name.Substring(4)) * 0.01m;
            }
        }

        public static string InvoiceArchiveIDPrefix
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string InvoiceBillIDPrefix
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderCentralSystemNo
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderCityName
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderCountryName
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderProvinceName
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderCompanyTaxRegion
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderCompanyTitle
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderEmail
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderPhoneNo
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderFaxNo
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderRegistrationNo
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string SenderTaxNo
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static short EBillsThreshold
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<short>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static short ReviewDelay
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<short>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        // e-bill credentials
        public static string EBillCompanyCode
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string EBillApiUsername
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static string EBillApiPassword
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        // bill settings
        public static int PaymentTolerance
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        public static int ExpirationTolerance
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<int>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        // google API
        public static string GeocodingAPIKey
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }
        // company
        public static string CompanyName
        {
            get
            {
                return DBSetting<RadiusREntities, AppSetting>.Retrieve<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #region METHODS
        /// <summary>
        /// Clears settings cache.
        /// </summary>
        public static void ClearCache()
        {
            var props = typeof(AppSettings).GetProperties();
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
