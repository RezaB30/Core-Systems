using RadiusR.DB;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class RoleViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Title")]
        [MaxLength(50, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SystemRole")]
        public bool IsSystemRole { get; set; }

        public bool CanBeManuallyAssigned { get; set; }

        public bool HasUsers { get; set; }

        [UIHint("Role")]
        [EnumType(typeof(RadiusR.DB.Enums.CommonRole), typeof(RadiusR.Localization.Lists.CommonRole))]
        public Role _source { get; set; }
    }
}