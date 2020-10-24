using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums
{
    public enum TTWorkOrderType
    {
        Sale_DSL = 1,
        Sale_FTTX = 2,
        TariffChange = 3,
        TypeChange = 4,
        ISPChange = 5,
        Transport_DSL = 6,
        Transport_FTTX = 7,
        Cancellation = 8,
        ChangeToNaked = 9,
        // add here
        Others = 99
    }
}
