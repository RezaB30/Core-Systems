using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.TurkTelekom.WebServices;

namespace RadiusR.DB.DomainsCache
{
    public class CachedTelekomTariff
    {
        public XDSLType XDSLType { get; set; }

        public decimal MonthlyStaticFee { get; set; }

        public int SpeedCode { get; set; }

        public string SpeedName { get; set; }

        public string SpeedDetails { get; set; }

        public string TariffName { get; set; }

        public int PacketCode { get; set; }

        public int TariffCode { get; set; }
    }
}
