using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class SubscriptionRegistrationInfoViewModel
    {
        [EnumType(typeof(RadiusR.DB.Enums.SubscriptionRegistrationType), typeof(RadiusR.Localization.Lists.SubscriptionRegistrationType))]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RegistrationType")]
        [UIHint("LocalizedList")]
        public short RegistrationType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransferHistory")]
        public IEnumerable<TransferHistoryViewModel> TransferHistory { get; set; }
    }
}
