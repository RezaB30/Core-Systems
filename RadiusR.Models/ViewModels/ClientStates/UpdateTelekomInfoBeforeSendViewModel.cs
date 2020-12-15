using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.ClientStates
{
    public class UpdateTelekomInfoBeforeSendViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TTPacket")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public TelekomTariffHelperViewModel TelekomTariffInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PSTN")]
        [PhoneNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PhoneNumber")]
        [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string PSTN { get; set; }
    }
}
