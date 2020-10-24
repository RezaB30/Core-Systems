using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AppUserSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Role")]
        [UIHint("Role")]
        [EnumType(typeof(RadiusR.DB.Enums.CommonRole), typeof(RadiusR.Localization.Lists.CommonRole))]
        public int? RoleID { get; set; }
    }
}
