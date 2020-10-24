using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class SubscriptionListDisplayViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ArchiveNo")]
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TelekomSubscriberNo")]
        public string DSLNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(CustomerState), typeof(RadiusR.Localization.Lists.CustomerState))]
        [UIHint("LocalizedList")]
        public short State { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RegistrationDate")]
        public DateTime? RegistrationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationDate")]
        public DateTime? CancellationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [UIHint("Currency")]
        public string ServicePrice { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InstallationAddress")]
        [UIHint("Address")]
        public AddressViewModel InstallationAddress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        public string TariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ContactPhoneNo")]
        [UIHint("PhoneNo")]
        public string ContactPhoneNo { get; set; }

        public decimal? _servicePrice
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(ServicePrice, out parsed))
                {
                    return parsed;
                }
                return null;
            }
            set
            {
                ServicePrice = value.HasValue ? value.Value.ToString("###,##0.00") : string.Empty;
            }
        }

        public IEnumerable<CustomerState> ValidStateChanges
        {
            get
            {
                return StateChangeUtilities.GetValidStateChanges((CustomerState)State);
            }
        }
    }
}
