using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer.SubscriptionItems
{
    public class UsernameEditViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Username")]
        [WordAndNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "WordAndNumber")]
        [MaxLength(64, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Password")]
        [WordAndNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "WordAndNumber")]
        [MaxLength(64, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [MinLength(5, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MinLength")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        public string DomainName { get; set; }
    }
}
