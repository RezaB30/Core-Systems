using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Verimor
{
    public class EventCacheObject
    {
        public VerimorEvent RawEvent { get; set; }

        public IEnumerable<long> SubscriptionIDs { get; set; }

        public string Url { get; set; }

        public string PhoneNo { get; set; }

        public string DisplayName { get; set; }

        public string Message { get; set; }

        public string LinkText { get; set; }
    }
}
