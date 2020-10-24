using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RadiusR.DB.Enums.CustomerSetup;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SetupServiceTaskDetailsViewModel : CustomerSetupServiceTaskViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerName")]
        public string ClientName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Stages")]
        public IEnumerable<Stage> Stages { get; set; }

        public class Stage
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
            [EnumType(typeof(FaultCodes), typeof(RadiusR.Localization.Lists.CustomerSetup.FaultCodes))]
            [UIHint("LocalizedList")]
            public short Status { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
            public string Details { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
            [UIHint("ExactTime")]
            public DateTime Date { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ReservationDate")]
            public DateTime? ReservationDate { get; set; }
        }
    }
}
