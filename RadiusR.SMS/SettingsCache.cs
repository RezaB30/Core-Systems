using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.SMS
{
    internal static class SettingsCache
    {
        private static MemoryCache cache = new MemoryCache("SMSSettingsCache");
        private static object activeListLock = new object();
        /// <summary>
        /// How long until cache item expires.
        /// </summary>
        public static TimeSpan CachingLength
        {
            get
            {
                return RetriveFromCache<TimeSpan>("CachingLength", true);
            }
        }
        /// <summary>
        /// SMS service username.
        /// </summary>
        public static string ServiceUsername
        {
            get
            {
                return RetriveFromCache<string>("ServiceUsername");
            }
        }
        /// <summary>
        /// SMS service password.
        /// </summary>
        public static string ServicePassword
        {
            get
            {
                return RetriveFromCache<string>("ServicePassword");
            }
        }
        /// <summary>
        /// SMS title.
        /// </summary>
        public static string ServiceTitle
        {
            get
            {
                return RetriveFromCache<string>("ServiceTitle");
            }
        }
        /// <summary>
        /// If SMS service should send the SMS.
        /// </summary>
        public static bool IsActive
        {
            get
            {
                return RetriveFromCache<bool>("IsActive");
            }
        }
        /// <summary>
        /// The type of SMS API to use.
        /// </summary>
        public static short APIType
        {
            get
            {
                return RetriveFromCache<short>("API");
            }
        }
        /// <summary>
        /// Gives active SMS types as a dictionary.
        /// </summary>
        public static Dictionary<short, bool> ActiveTypes
        {
            get
            {
                return RetriveActiveTypesFromCache("ActiveTypes");
            }
        }

        private static CacheItemPolicy policy
        {
            get
            {
                return new CacheItemPolicy()
                {
                    Priority = CacheItemPriority.Default,
                    AbsoluteExpiration = DateTime.Now.Add(CachingLength)
                };
            }
        }

        private static CacheItemPolicy getCachingLengthPolicy(TimeSpan cachingLength)
        {

            return new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default,
                AbsoluteExpiration = DateTime.Now.Add(cachingLength)
            };
        }

        private static T RetriveFromCache<T>(string key, bool IsCachingLength = false)
        {
            var value = cache.Get(key);
            if (value != null)
                return (T)value;
            using (RadiusREntities entities = new RadiusREntities())
            {
                value = entities.SMSSettings.FirstOrDefault(setting => setting.Key == key).Value;
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter == null)
                {
                    return default(T);
                }
                value = (T)converter.ConvertFromString((string)value);
                // this prevents infinite loop
                if (IsCachingLength)
                    cache.Set(key, value, getCachingLengthPolicy((TimeSpan)value));
                else
                    cache.Set(key, value, policy);
                return (T)value;
            }
        }

        private static Dictionary<short, bool> RetriveActiveTypesFromCache(string key)
        {
            var value = cache.Get(key);
            if (value != null)
                return (Dictionary<short, bool>)value;
            if (Monitor.TryEnter(activeListLock, 2000))
            {
                try
                {
                    value = cache.Get(key);
                    if (value != null)
                        return (Dictionary<short, bool>)value;
                    using (RadiusREntities entities = new RadiusREntities())
                    {
                        value = entities.SMSTexts.Where(sms => !sms.IsDisabled).Select(sms => new { Type = sms.TypeID, IsActive = !sms.IsDisabled }).ToArray().Distinct().ToDictionary(sms => sms.Type, sms => sms.IsActive);
                        cache.Set(key, value, policy);
                        return (Dictionary<short, bool>)value;
                    }
                }
                finally
                {
                    Monitor.Exit(activeListLock);
                }
            }

            return new Dictionary<short, bool>();            
        }

        public static void Reload()
        {
            var keys = cache.Select(item => item.Key).ToArray();
            foreach (var key in keys)
            {
                cache.Remove(key);
            }
        }
    }
}
