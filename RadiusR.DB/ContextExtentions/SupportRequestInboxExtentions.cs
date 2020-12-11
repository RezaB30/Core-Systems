using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class RadiusREntities
    {
        public IQueryable<SupportRequest> GetSupportGroupInbox(int id)
        {
            return SupportRequests.Where(sr => sr.SupportRequestType.SupportGroups.Select(sg => sg.ID).Contains(id) && sr.StateID != (short)Enums.SupportRequests.SupportRequestStateID.Done && !sr.AssignedGroupID.HasValue && !sr.RedirectedGroupID.HasValue).OrderByDescending(sr => sr.Date);
        }

        public IQueryable<SupportRequest> GetSupportGroupRedirectInbox(int id)
        {
            return SupportRequests.Where(sr => sr.RedirectedGroupID == id && sr.AssignedGroupID != id && sr.StateID != (short)Enums.SupportRequests.SupportRequestStateID.Done).OrderByDescending(sr => sr.Date);
        }

        public IQueryable<SupportRequest> GetSupportGroupInProgressInbox(int id)
        {
            return SupportRequests.Where(sr => sr.AssignedGroupID == id && sr.StateID != (short)Enums.SupportRequests.SupportRequestStateID.Done).OrderByDescending(sr => sr.Date);
        }

        public IQueryable<SupportRequest> GetSupportUserInbox(int groupId, int userId)
        {
            return SupportRequests.Where(sr => sr.AssignedUserID == userId && sr.AssignedGroupID == groupId && sr.StateID != (short)Enums.SupportRequests.SupportRequestStateID.Done).OrderByDescending(sr => sr.Date);
        }

        public IQueryable<SupportRequest> GetSupportGroupFinishedRequests(int id)
        {
            return SupportRequests.Where(sr => sr.AssignedGroupID == id && sr.StateID == (short)Enums.SupportRequests.SupportRequestStateID.Done).OrderByDescending(sr => sr.Date);
        }
    }
}
