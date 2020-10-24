using RadiusR.BTKLogging;
using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.BTKLogging
{
    static class SettingsCache
    {
        private static MemoryCache _cache = new MemoryCache("SettingsCache");

        public static void Load()
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                _cache.Set("Settings",  db.BTKSchedulerSettings.ToArray().Select(settings => new SchedulerSettings(settings)).ToArray(), DateTimeOffset.Now.AddMinutes(10));
            }
        }

        public static SchedulerSettings[] Get()
        {
            var results = _cache.Get("Settings");
            if (results == null)
            {
                Load();
            }

            return _cache.Get("Settings") as SchedulerSettings[];
        }
    }
}
