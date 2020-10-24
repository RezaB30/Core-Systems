using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class TelekomWorkOrderSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "User")]
        public int? AppUserID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        public DateTime? StartDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        public DateTime? EndDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "OperationType")]
        [EnumType(typeof(RadiusR.DB.Enums.TelekomOperations.TelekomOperationType), typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationType))]
        [UIHint("LocalizedList")]
        public short? OperationType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "OperationSubType")]
        [EnumType(typeof(RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType), typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationSubType))]
        [UIHint("LocalizedList")]
        public short? OperationSubType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ShowClosed")]
        public bool? ShowClosed { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState), typeof(RadiusR.Localization.Lists.RegistrationState))]
        [UIHint("LocalizedList")]
        public short? State { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Address")]
        public AddressViewModel Address { get; set; }
    }
}
