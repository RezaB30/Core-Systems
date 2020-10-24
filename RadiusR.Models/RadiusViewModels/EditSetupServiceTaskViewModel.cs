using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class EditSetupServiceTaskViewModel : NewSetupServiceTaskViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaskType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(RadiusR.DB.Enums.CustomerSetup.TaskTypes), typeof(RadiusR.Localization.Lists.CustomerSetup.TaskType))]
        [UIHint("LocalizedList")]
        public short TaskType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerName")]
        public string ClientName { get; set; }
    }
}
