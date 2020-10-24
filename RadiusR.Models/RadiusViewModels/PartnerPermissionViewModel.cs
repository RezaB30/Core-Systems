using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class PartnerPermissionViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Permission")]
        [EnumType(typeof(RadiusR.DB.Enums.PartnerPermissions), typeof(RadiusR.Localization.Lists.PartnerPermissions))]
        [UIHint("LocalizedList")]
        public short Permission { get; set; }
    }
}
