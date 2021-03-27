using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class OutgoingTransitionRejectViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(TransitionRejectionReason), typeof(RadiusR.Localization.Lists.TelekomOperations.TransitionRejectionReason))]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RejectionReason")]
        [UIHint("LocalizedList")]
        public short? RejectionReason { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RejectionDescription")]
        [MaxLength(300, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [DataType(DataType.MultilineText)]
        public string RejectionDescription { get; set; }

        public enum TransitionRejectionReason
        {
            Expired = 1,
            WrongCustomerInfo = 2,
            Fraud = 3,
            InvalidTCKNo = 4,
            InsufficientDocuments = 5,
            InvalidForeignerID = 6,
            InvalidTaxNo = 7,
            InvalidCompanyTitle = 8,
            RequestFormNotValid = 9,
        }
    }
}
