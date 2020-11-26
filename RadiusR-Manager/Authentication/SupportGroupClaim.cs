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

        public bool CanCreate { get; set; }

        public bool CanChangeState { get; set; }

        public SupportGroupClaim(int groupId, bool isLeader, bool canCreate, bool canChangeState )
        {
            GroupId = groupId;
            IsLeader = isLeader;
            CanCreate = canCreate;
            CanChangeState = canChangeState;
        }

        protected SupportGroupClaim() { }
    }
}