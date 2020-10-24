using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums
{
    public enum CancellationReason
    {
        //NotNeeded = 1,
        //Dept = 2,
        //Technical = 3,
        //AddressChange = 4,
        //Support = 5
        ChangedNumber = 2,
        Others = 3,
        FakeDocuments = 4,
        CustomerRequest = 5,
        Transfer = 6,
        WrongOwner = 7,
        BlackList = 8,
        OutOfUse = 9,
        MissingDocuments = 10,
        AccidentalInput = 11,
        RelatedProductCancelled = 12,
    }
}
