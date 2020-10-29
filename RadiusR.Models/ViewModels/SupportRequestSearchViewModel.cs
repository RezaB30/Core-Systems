using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SupportRequestSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(RadiusR.DB.Enums.SubscriptionSupportRequestStateID), typeof(RadiusR.Localization.Lists.SubscriptionSupportRequestStateID))]
        [UIHint("LocalizedList")]
        public short? StateID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        public DateTime? DateStart { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        public DateTime? DateEnd { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Issuer")]
        [EnumType(typeof(IssuerType), typeof(RadiusR.Localization.Lists.SupportRequestIssuerType))]
        [UIHint("LocalizedList")]
        public short? Issuer { get; set; }

        public enum IssuerType
        {
            Customer = 1,
            Operator = 2
        }
    }
}
