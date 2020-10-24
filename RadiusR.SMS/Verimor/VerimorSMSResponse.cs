using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.SMS.Verimor
{
    public class VerimorSMSResponse
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public bool WasSuccessful { get; set; }
    }
}
