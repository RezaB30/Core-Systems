using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR_Manager
{
    public class SupportGroupClaim
    {
        public int GroupId { get; set; }

        public bool IsLeader { get; set; }

        public bool CanChangeState { get; set; }

        public bool CanRedirect { get; set; }

        public bool CanWriteToCustomer { get; set; }

        public bool CanAssignToStaff { get; set; }
    }

    public class ExtendedSupportGroupClaim
    {
        public SupportGroupClaim BaseClaim { get; private set; }
        public bool CanRead { get; set; }

        public ExtendedSupportGroupClaim(SupportGroupClaim baseClaim, bool canRead = true)
        {
            BaseClaim = baseClaim;
            CanRead = canRead;
        }
    }
}