using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AddSubscriptionSpecialOfferViewModel
    {
        [Display(ResourceType = typeof( RadiusR.Localization.Pages.Common), Name = "SpecialOffers")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? SpecialOfferID { get; set; }
    }
}
