using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR_Manager
{
    public class SupportGroupClaim
    {
        public int GroupId { get; private set; }

        public bool IsLeader { get; private set; }

        public SupportGroupClaim(int groupId, bool isLeader)
        {
            GroupId = groupId;
            IsLeader = isLeader;
        }
    }
}