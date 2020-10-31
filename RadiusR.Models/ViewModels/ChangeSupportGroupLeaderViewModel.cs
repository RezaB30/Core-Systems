using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ChangeSupportGroupLeaderViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Leader")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? LeaderID { get; set; }
    }
}
