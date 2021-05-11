using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.PartnerRegisters
{
    public class PartnerRegisteredSubscriptionViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RegistrationDate")]
        [UIHint("ExactTime")]
        public DateTime RegistrationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        public short State { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Partner")]
        public string PartnerName { get; set; }

        public string Username { get; set; }

        public long SubscriptionID { get; set; }

        public IEnumerable<CustomerState> ValidStateChanges
        {
            get
            {
                return StateChangeUtilities.GetValidStateChanges((CustomerState)State);
            }
        }
    }
}
