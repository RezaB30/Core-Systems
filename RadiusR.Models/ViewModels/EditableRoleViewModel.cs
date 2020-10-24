using RadiusR_Manager.Models.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class EditableRoleViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Title")]
        [MaxLength(50, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Permissions")]
        [TreeCheckerDisplay(typeof(RadiusR.Localization.Model.Permissions))]
        public IEnumerable<TreeCollection> Permissions { get; set; }
    }
}