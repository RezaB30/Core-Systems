using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class SubscriptionScheduledTariffChangeViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NewTariffName")]
        public string NewScheduledTariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NewTariffActivationDate")]
        public DateTime? NewScheduledTariffActivationDate { get; set; }
    }
}
