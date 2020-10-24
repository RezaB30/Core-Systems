using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace RadiusR.DB.Utilities
{
    /// <summary>
    /// Default DB attributes for radius server
    /// </summary>
    public static class RadiusDefaults
    {
        public const string StringDateFormat = "yyyy-MM-dd hh:mm:ss";

        public static string FramedProtocol
        {
            get
            {
                return (string)RetrieveFromCache("Framed-Protocol");
            }
        }

        public static string AcctInterimInterval
        {
            get
            {
                return (string)RetrieveFromCache("Acct-Interim-Interval");
            }
        }

        public static string SimultaneousUse
        {
            get
            {
                return (string)RetrieveFromCache("Simultaneous-Use");
            }
        }

        /// <summary>
        /// Updates radius default settings.
        /// </summary>
        /// <param name="settings">New settings</param>
        public static void Change(dynamic settings)
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                var allEntries = db.RadiusDefaults.ToList();

                allEntries.Where(s => s.Attribute == "Framed-Protocol").SingleOrDefault().Value = settings.FramedProtocol.ToString();
                allEntries.Where(s => s.Attribute == "Acct-Interim-Interval").SingleOrDefault().Value = settings.AcctInterimInterval.ToString();
                allEntries.Where(s => s.Attribute == "Simultaneous-Use").SingleOrDefault().Value = settings.SimultaneousUse.ToString();

                allEntries.ForEach(s => db.Entry(s).State = EntityState.Modified);
                db.SaveChanges();

                HttpContext.Current.Cache.Remove("Framed-Protocol");
                HttpContext.Current.Cache.Remove("Acct-Interim-Interval");
                HttpContext.Current.Cache.Remove("Simultaneous-Use");
            }
        }

        /// <summary>
        /// Retrieves a value from cache.
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <returns>Cache value</returns>
        private static object RetrieveFromCache(string cacheKey)
        {
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    var value = db.RadiusDefaults.Find(cacheKey).Value;
                    HttpContext.Current.Cache.Insert(cacheKey, value, null, DateTime.UtcNow.AddDays(1), Cache.NoSlidingExpiration);
                }
            }
            return HttpContext.Current.Cache[cacheKey];
        }
    }
}
