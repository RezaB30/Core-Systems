using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SupportGroupViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(200, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Leader")]
        public string LeaderName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Leader")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? LeaderID { get; set; }

        public bool? IsActive { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RelevantTypes")]
        public IEnumerable<SupportRequestTypeViewModel> RelevantTypes { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ActiveUsers")]
        public int ActiveUsers { get; set; }
    }
}
