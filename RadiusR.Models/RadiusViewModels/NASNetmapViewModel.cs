using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class NASNetmapViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "NAS")]
        public int NASID { get; set; }

        public NASViewModel NAS { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIPSubnet")]
        [IPSubnet(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IPSubnet")]
        public string LocalIPSubnet { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RealIPSubnet")]
        [IPSubnet(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IPSubnet")]
        public string RealIPSubnet { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PortCount")]
        [PortCount(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PortCount")]
        public string PortCount { get; set; }

        public int? _PortCount
        {
            get
            {
                int parsed;
                if (int.TryParse(PortCount, out parsed))
                {
                    return parsed;
                }
                return null;
            }
            set
            {
                PortCount = value.ToString();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Map")]
        public string MapView
        {
            get
            {
                return LocalIPSubnet + " -> " + RealIPSubnet;
            }
        }


        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PreserveLastByte")]
        public bool PreserveLastByte { get; set; }
    }
}
