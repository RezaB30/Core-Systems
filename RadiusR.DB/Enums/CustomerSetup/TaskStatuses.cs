using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR.DB.Enums.CustomerSetup
{
    public enum TaskStatuses
    {
        New = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        Halted = 5
    }
}