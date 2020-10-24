using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SubscriptionCommitmentViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentLength")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(RadiusR.DB.Enums.CommitmentLength), typeof(RadiusR.Localization.Lists.CommitmentLength))]
        [UIHint("LocalizedList")]
        public short? CommitmentLength { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentExpirationDate")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public DateTime? CommitmentExpirationDate { get; set; }
    }
}
