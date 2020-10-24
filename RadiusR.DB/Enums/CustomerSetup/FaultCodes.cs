using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR.DB.Enums.CustomerSetup
{
    public enum FaultCodes
    {
        NoFault = 1,
        InvalidAddress = 2,
        TelekomLineFault = 3,
        ClientCancelled = 4,
        ModemFault = 5,
        TelekomGeneralFault = 6,
        BuildingInstallationFault = 7,
        CustomerCouldNotBeReached = 8,
        RendezvousMade = 9,
        SetupComplete = 10,
        CustomerDidTheSetup = 11,
        WaitingForNewRendezvous = 12
    }
}