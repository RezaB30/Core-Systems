using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Verimor
{
    /// <summary>
    /// Handles verimor events cache.
    /// </summary>
    public static class EventCache
    {
        private static MemoryCache _cache = new MemoryCache("VerimorEvents");
        /// <summary>
        /// Gets a cached item.
        /// </summary>
        /// <param name="internalID">The user internal number.</param>
        /// <param name="lastUUID">Last loaded UUID to prevent duplicates.</param>
        /// <returns></returns>
        public static EventCacheObject RetrieveEvent(string internalID, string lastUUID = null, string lastEventType = null)
        {
            var eventObject = _cache.Get(internalID) as EventCacheObject;
            if (eventObject == null)
                return null;
            if (string.IsNullOrEmpty(lastUUID) || lastUUID != eventObject.RawEvent.call_uuid || lastEventType != eventObject.RawEvent.event_type)
                return eventObject;
            return null;
        }
        /// <summary>
        /// Saves incoming event into cache.
        /// </summary>
        /// <param name="eventObject">The event to cache.</param>
        public static void SaveEvent(EventCacheObject eventObject)
        {
            if (eventObject.RawEvent.InternalID != null)
                _cache.Set(eventObject.RawEvent.InternalID, eventObject, DateTimeOffset.Now.AddSeconds(5));
        }
    }
}
