using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public static class VPOSSettings
    {
        private static MemoryCache internalCache = new MemoryCache("VPOSSettings");

        public static Enums.VPOSTypes VPOSType
        {
            get
            {
                return GetInternalSettings().VPOSType;
            }
        }

        public static string MerchantID
        {
            get
            {
                return GetInternalSettings().MerchantID;
            }
        }

        public static string StoreKey
        {
            get
            {
                return GetInternalSettings().StoreKey;
            }
        }

        public static string UserID
        {
            get
            {
                return GetInternalSettings().UserID;
            }
        }

        public static string Userpassword
        {
            get
            {
                return GetInternalSettings().UserPassword;
            }
        }

        public static int CurrentVPOSID
        {
            get
            {
                return GetInternalSettings().CurrentVPOSID;
            }
        }

        #region Caching
        class Settings
        {
            public Enums.VPOSTypes VPOSType { get; set; }

            public string MerchantID { get; set; }

            public string StoreKey { get; set; }

            public string UserID { get; set; }

            public string UserPassword { get; set; }

            public int CurrentVPOSID { get; set; }
        }

        private static Settings GetInternalSettings()
        {
            var results = internalCache.Get("Settings") as Settings;
            if(results == null)
            {
                if(Monitor.TryEnter(internalCache, 5000))
                {
                    try
                    {
                        results = internalCache.Get("Settings") as Settings;
                        if (results != null)
                            return results;

                        using (RadiusREntities db = new RadiusREntities())
                        {
                            var selectedVPOS = db.AppSettings.FirstOrDefault(s => s.Key == "SelectedVPOS");
                            if (selectedVPOS != null)
                            {
                                var selectedVPOSID = Convert.ToInt32(selectedVPOS.Value);
                                var currentSettings = db.VPOSLists.Find(selectedVPOSID);
                                results = new Settings()
                                {
                                    VPOSType = (Enums.VPOSTypes)currentSettings.VPOSTypeID,
                                    MerchantID = currentSettings.MerchantID,
                                    StoreKey = currentSettings.StoreKey,
                                    UserID = currentSettings.UserID,
                                    UserPassword = currentSettings.UserPass,
                                    CurrentVPOSID = selectedVPOSID
                                };
                                internalCache.Set("Settings", results, DateTimeOffset.Now.AddMinutes(5));

                                return results;
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(internalCache);
                    }
                }
            }

            return results;
        }

        public static void RefreshCache()
        {
            internalCache.Remove("Settings");
        }
        #endregion
    }
}
