using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ExpiredPoolViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ExpiredPoolName")]
        [MaxLength(64, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string ExpiredPoolName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIPSubnet")]
        [IPSubnet(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IPSubnet")]
        public string LocalIPSubnet { get; set; }
    }
}
