using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SupportGroupRenameViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(200, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Name { get; set; }
    }
}
