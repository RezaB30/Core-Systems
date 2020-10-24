using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.DomainsCache
{
    public class CachedDomain
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string UsernamePrefix { get; set; }

        public string SubscriberNoPrefix { get; set; }

        public int MaxFreezeDuration { get; set; }

        public short MaxFreezesPerYear { get; set; }

        public short? AccessMethod { get; set; }

        public TelekomCredentials TelekomCredential { get; set; }

        public class TelekomCredentials
        {
            public string XDSLWebServiceUsername { get; set; }

            public string XDSLWebServicePassword { get; set; }

            public string XDSLWebServiceCustomerCode { get; set; }

            public string OLOPortalUsername { get; set; }

            public string OLOPortalPassword { get; set; }

            public string OLOPortalCustomerCode { get; set; }

            public int OLOPortalCustomerCodeInt
            {
                get
                {
                    int result;
                    if (int.TryParse(OLOPortalCustomerCode, out result))
                        return result;
                    return 0;
                }
            }

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

            public long XDSLWebServiceCustomerCodeInt
            {
                get
                {
                    long result;
                    if (long.TryParse(XDSLWebServiceCustomerCode, out result))
                        return result;
                    return 0;
                }
            }
        }
    }
}
