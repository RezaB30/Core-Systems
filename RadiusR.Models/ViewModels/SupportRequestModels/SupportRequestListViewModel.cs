using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.SupportRequestModels
{
    public class SupportRequestListViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        public long? SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SupportPin")]
        public string SupportPin { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RequestType")]
        public string RequestType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RequestSubType")]
        public string RequestSubType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AssignedGroup")]
        public string AssignedGroup { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AssignedStaff")]
        public string AssignedUser { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RedirectedToGroup")]
        public string RedirectedToGroup { get; set; }
    }

    public static class SupportRequestListExtentions
    {
        public static IQueryable<SupportRequestListViewModel> GetViewModels(this IQueryable<SupportRequest> query)
        {
            return query.Select(sr => new SupportRequestListViewModel()
            {
                AssignedGroup = sr.AssignedGroupID.HasValue ? sr.AssignedSupportGroup.Name : null,
                AssignedUser = sr.AssignedUserID.HasValue ? sr.AssignedUser.Name : null,
                Date = sr.Date,
                ID = sr.ID,
                RedirectedToGroup = sr.RedirectedGroupID.HasValue ? sr.RedirectedSupportGroup.Name : null,
                RequestSubType = sr.SubTypeID.HasValue ? sr.SupportRequestSubType.Name : null,
                RequestType = sr.TypeID.HasValue ? sr.SupportRequestType.Name : null,
                SubscriberNo = sr.SubscriptionID.HasValue ? sr.Subscription.SubscriberNo : null,
                SubscriptionID = sr.SubscriptionID,
                SupportPin = sr.SupportPin
            });
        }
    }
}
