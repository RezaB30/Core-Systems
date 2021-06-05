using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums
{
    public enum PDFFormType
    {
        IndividualContract = 1,
        CorporateContract = 2,
        IndividualTransition = 3,
        CorporateTransition = 4,
        PSTNtoNaked = 5,
        Transfer = 6,
        Transport = 7,
        BillReceipt = 8,
    }
}
