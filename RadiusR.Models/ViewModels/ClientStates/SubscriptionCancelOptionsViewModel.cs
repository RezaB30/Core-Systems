using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.ClientStates
{
    public class SubscriptionCancelOptionsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Reason")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(CancellationReason), typeof(RadiusR.Localization.Lists.CancellationReason))]
        [UIHint("LocalizedList")]
        public short ReasonID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
        [MaxLength(300, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string ReasonDescription { get; set; }
    }
}
