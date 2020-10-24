using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class TelekomAccessCredential
    {
        public long XDSLWebServiceUsernameInt
        {
            get
            {
                long result;
                if (long.TryParse(XDSLWebServiceUsername, out result))
                    return result;
                return 0;
            }
        }
    }
}
