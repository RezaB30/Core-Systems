using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class CustomerSubscriptionViewModel
    {

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RegistrationType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(SubscriptionRegistrationType), typeof(RadiusR.Localization.Lists.SubscriptionRegistrationType))]
        [UIHint("LocalizedList")]
        public short? RegistrationType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransferringSubscription")]
        public long? TransferringSubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransferringSubscription")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [MinLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MinLength")]
        [Number(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Number")]
        public string TransferringSubscriptionNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransitionXDSLNo")]
        [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [MinLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MinLength")]
        [Number(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Number")]
        public string TransitionXDSLNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransitionPSTN")]
        [PhoneNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PhoneNumber")]
        public string TransitionPSTN { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int DomainID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ServiceName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int ServiceID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InstallationAddress")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [UIHint("Address")]
        public AddressViewModel SetupAddress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Username")]
        [WordAndNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "WordAndNumber")]
        [MaxLength(64, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StaticIP")]
        [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
        public string StaticIP { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingPeriod")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int BillingPeriod { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Groups")]
        public IEnumerable<int> GroupIds { get; set; }

        public SubscriptionCommitmentViewModel CommitmentInfo { get; set; }

        public SubscriptionTelekomInfoViewModel TelekomDetailedInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ExtraFees")]
        public List<SubscriberFeesAddViewModel> AddedFeesInfo { get; set; }

        public SubscriptionReferralDiscountViewModel ReferralDiscount { get; set; }

        public CustomerSubscriptionViewModel()
        {
            AddedFeesInfo = new List<SubscriberFeesAddViewModel>();
            ReferralDiscount = new SubscriptionReferralDiscountViewModel();
        }
    }
}
