using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class VerticalDSLIPMapViewModel
    {

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIPPools")]
        public IPSubnet[] LocalIPSubnets { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DSLLineIPs")]
        public IP[] DSLLines { get; set; }

        public string LocalIPSubnetsStringValue
        {
            get
            {
                return LocalIPSubnets != null && LocalIPSubnets.Any() ? string.Join("|", LocalIPSubnets.Select(item => item.Value)) : null;
            }

            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    LocalIPSubnets = value.Split('|').Select(s => new IPSubnet() { Value = s }).ToArray();
                }
            }
        }

        public string DSLLinesStringValue
        {
            get
            {
                return DSLLines != null && DSLLines.Any() ? string.Join("|", DSLLines.Select(item => item.Value)) : null;
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    DSLLines = value.Split('|').Select(s => new IP() { Value = s }).ToArray();
                }
            }
        }

        public class IPSubnet
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIPSubnet")]
            [IPSubnet(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IPSubnet")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            public string Value { get; set; }
        }

        public class IP
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IP")]
            [IP(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IP")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            public string Value { get; set; }
        }
    }
}
