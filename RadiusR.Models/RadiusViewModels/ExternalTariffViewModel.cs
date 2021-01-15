using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class ExternalTariffViewModel
    {
        public int TariffID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Name")]
        [MaxLength(300, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string DisplayName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int DomainID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "XDSL")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public bool XDSL { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Fiber")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public bool Fiber { get; set; }
    }
}
