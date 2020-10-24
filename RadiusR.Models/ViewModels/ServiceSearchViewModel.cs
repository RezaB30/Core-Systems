using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ServiceSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ServiceName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceDomains")]
        public int? DomianID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingType")]
        [EnumType(typeof(ServiceBillingType), typeof(RadiusR.Localization.Lists.ServiceBillingType))]
        [UIHint("LocalizedList")]
        public short? BillingType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "QuotaType")]
        [EnumType(typeof(QuotaType), typeof(RadiusR.Localization.Lists.QuotaType))]
        [UIHint("LocalizedList")]
        public short? QuotaType { get; set; }
    }
}
