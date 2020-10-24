using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.BTKLogging.Enums
{
    public enum AcctTerminateCause
    {
        user_request = 1,
        lost_carrier = 2,
        lost_service = 3,
        idle_timeout = 4,
        session_timeout = 5,
        admin_reset = 6,
        admin_reboot = 7,
        port_error = 8,
        nas_error = 9,
        nas_request = 10,
        nas_reboot = 11,
        port_unneeded = 12,
        port_preempted = 13,
        port_suspended = 14,
        service_unavailable = 15,
        callback = 16,
        user_error = 17,
        host_request = 18,
        supplicant_restart = 19,
        reauthentication_failure = 20,
        port_reinitialized = 21,
        port_administratively_disabled = 22,
        lost_power = 23
    }
}
