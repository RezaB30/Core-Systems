using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums.TelekomOperations
{
    public enum TelekomOperationSubType
    {
        XDSL = 1,
        FTTX = 2,
        XDSL_to_FTTX = 3,
        FTTX_to_XDSL = 4,
        Normal_to_Handicap = 5
    }
}
