using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class NASVerticalIPMapViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "NAS")]
        public int NASID { get; set; }

        public NASViewModel NAS { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIPStart")]
        [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
        public string LocalIPStart { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIPEnd")]
        [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
        public string LocalIPEnd { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RealIPStart")]
        [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
        public string RealIPStart { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RealIPEnd")]
        [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
        public string RealIPEnd { get; set; }

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
                return LocalIPStart + "-" + LocalIPEnd + " -> " + RealIPStart + "-" + RealIPEnd;
            }
        }
    }
}
