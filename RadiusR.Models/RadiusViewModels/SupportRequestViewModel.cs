using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SupportRequestViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Customer")]
        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Message")]
        [MaxLength(250, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Message { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(RadiusR.DB.Enums.SupportRequestStateID), typeof(RadiusR.Localization.Lists.SupportRequestStateID))]
        [UIHint("LocalizedList")]
        public short StateID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        public long? CustomerSetupTaskID { get; set; }

        public SubscriptionListDisplayViewModel Subscription { get; set; }

        public CustomerSetupServiceTaskViewModel SetupServiceTask { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
        public string AdminMessage { get; set; }

        public int? IssuerID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Issuer")]
        public string IssuerName { get; set; }
    }
}