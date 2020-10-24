using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums
{
    public enum PaymentType
    {
        None = 1,
        Cash = 2,
        VirtualPos = 3,
        PhysicalPos = 4,
        OnlineBanking = 5,
        Transfer = 6,
        MobilExpress = 7,
        Credit = 8,
        Partner = 9,
        OfflineBanking = 10,
    }
}
