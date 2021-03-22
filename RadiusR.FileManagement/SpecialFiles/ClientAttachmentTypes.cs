using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public enum ClientAttachmentTypes
    {
        Contract = 1,
        IDCard = 2,
        PSTN = 3,
        CHURN = 4,
        Setup = 5,
        Transport = 6,
        Transfer = 7,
        Cancellation = 8,
        TaxPermit = 9,
        AuthorizedSignitures = 10,
        Docket = 11,
        ActivityCertificate = 12,
        TradeRegistryPaper = 13,
        WarrantOfAttorney = 14,
        Others = 199
    }
}
