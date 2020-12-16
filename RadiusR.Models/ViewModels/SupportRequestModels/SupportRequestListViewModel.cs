using RadiusR.DB;
using RezaB.Web.CustomAttributes;
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

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "HasCustomerResponse")]
        public bool HasCustomerResponse { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(RadiusR.DB.Enums.SupportRequests.SupportRequestStateID), typeof(RadiusR.Localization.Lists.SupportRequests.SupportRequestStateID))]
        [UIHint("LocalizedList")]
        public short StateID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "VisibleToCustomer")]
        public bool IsVisibleToCustomer { get; set; }

        public SupportRequestListViewModel() { }

        public SupportRequestListViewModel(SupportRequest dbRequest)
        {
            AssignedGroup = dbRequest.AssignedGroupID.HasValue ? dbRequest.AssignedSupportGroup.Name : null;
            AssignedUser = dbRequest.AssignedUserID.HasValue ? dbRequest.AssignedUser.Name : null;
            Date = dbRequest.Date;
            ID = dbRequest.ID;
            RedirectedToGroup = dbRequest.RedirectedGroupID.HasValue ? dbRequest.RedirectedSupportGroup.Name : null;
            RequestSubType = dbRequest.SupportRequestSubType.Name;
            RequestType = dbRequest.SupportRequestType.Name;
            SubscriberNo = dbRequest.SubscriptionID.HasValue ? dbRequest.Subscription.SubscriberNo : null;
            SubscriptionID = dbRequest.SubscriptionID;
            SupportPin = dbRequest.SupportPin;
            StateID = dbRequest.StateID;
            IsVisibleToCustomer = dbRequest.IsVisibleToCustomer;
            HasCustomerResponse = dbRequest.SupportRequestProgresses.Any() && !dbRequest.SupportRequestProgresses.OrderByDescending(srp => srp.Date).ThenByDescending(srp => srp.ID).FirstOrDefault().AppUserID.HasValue;
        }
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
                RequestSubType = sr.SupportRequestSubType.Name,
                RequestType = sr.SupportRequestType.Name,
                SubscriberNo = sr.SubscriptionID.HasValue ? sr.Subscription.SubscriberNo : null,
                SubscriptionID = sr.SubscriptionID,
                SupportPin = sr.SupportPin,
                StateID = sr.StateID,
                IsVisibleToCustomer = sr.IsVisibleToCustomer,
                HasCustomerResponse = sr.SupportRequestProgresses.Any() && !sr.SupportRequestProgresses.OrderByDescending(srp => srp.Date).ThenByDescending(srp => srp.ID).FirstOrDefault().AppUserID.HasValue
            });
        }
    }
}
