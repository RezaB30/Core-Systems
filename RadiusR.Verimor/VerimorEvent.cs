using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Verimor
{
    /// <summary>
    /// Represents a verimor call event.
    /// </summary>
    public class VerimorEvent
    {
        public string event_type { get; set; }
        public string domain_id { get; set; }
        public string direction { get; set; }
        public string caller_id_number { get; set; }
        public string outbound_caller_id_number { get; set; }
        public string destination_number { get; set; }
        public string dialed_user { get; set; }
        public string call_uuid { get; set; }
        public string start_stamp { get; set; }
        public string connected_user { get; set; }
        public string answer_stamp { get; set; }
        public string end_stamp { get; set; }
        public string duration { get; set; }
        public bool recording_present { get; set; }
        public bool answered { get; set; }
        public string queue { get; set; }
        public string queue_wait_duration { get; set; }
        public string sip_hangup_disposition { get; set; }
        public string hangup_cause { get; set; }
        public string failure_status { get; set; }
        public string failure_phrase { get; set; }

        public string InternalID
        {
            get
            {
                return connected_user ?? dialed_user;
            }
        }
    }
}
