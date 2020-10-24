using RadiusR.DB;
using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class NASViewModel
    {
        [ReadOnly(true)]
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Backbone")]
        public int? BackboneNASID { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasIP")]
        [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
        public string IP { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasName")]
        public string Name { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasType")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        //public int TypeID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NATType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(NATType), typeof(RadiusR.Localization.Lists.NATType))]
        [UIHint("LocalizedList")]
        public short NATType { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "RadiusIncomingPort")]
        [PortNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PortNumber")]
        public string RadiusIncomingPort { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasSecret")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Secret { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ApiUsername")]
        public string ApiUsername { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ApiPassword")]
        public string ApiPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ApiPort")]
        [PortNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PortNumber")]
        public string ApiPort { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NASType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(NASTypes), typeof(RadiusR.Localization.Lists.NASTypes))]
        [UIHint("BigLocalizedList")]
        public int? NASType { get; set; }

        public bool Disabled { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RealIPPool")]
        [UIHint("VerticalIPMap")]
        public IEnumerable<NASVerticalIPMapViewModel> NASVerticalIPMaps { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IPPool")]
        public string IPMapInfo
        {
            get
            {
                if (NASVerticalIPMaps == null || NASVerticalIPMaps.Count() <= 0)
                    return "-";
                return string.Join(Environment.NewLine, NASVerticalIPMaps.Select(map => map.MapView + "(" + map.PortCount + ")"));
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NetmapList")]
        public string NetmapInfo
        {
            get
            {
                if (NASNetmaps == null || NASNetmaps.Count() <= 0)
                    return "-";
                return string.Join(Environment.NewLine, NASNetmaps.Select(netmap => netmap.RealIPSubnet + "(" + netmap.PortCount + ")"));
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NATInfo")]
        public string NATInfo
        {
            get
            {
                switch (NATType)
                {
                    case (short)RadiusR.DB.Enums.NATType.Horizontal:
                        return NetmapInfo;
                    case (short)RadiusR.DB.Enums.NATType.Vertical:
                    case (short)RadiusR.DB.Enums.NATType.VerticalDSL:
                        return IPMapInfo;
                    default:
                        return "-";
                }
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Backbone")]
        public string BackboneNASName
        {
            get
            {
                if (BackboneNAS == null)
                    return "-";
                return BackboneNAS.Name;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Backbone")]
        public NAS BackboneNAS { get; set; }

        public IEnumerable<NASViewModel> SubNASes { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NetmapList")]
        [UIHint("Netmap")]
        public IEnumerable<NASNetmapViewModel> NASNetmaps { get; set; }
    }
}