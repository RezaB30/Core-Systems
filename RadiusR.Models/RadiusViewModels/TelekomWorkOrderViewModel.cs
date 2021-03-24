using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class TelekomWorkOrderViewModel
    {
        public long ID { get; set; }

        public long SubscriberID { get; set; }

        public long TelekomCustomerCode { get; set; }

        public int DomainID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "User")]
        public string AppUserName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        [UIHint("ExactTime")]
        public DateTime CreationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastRetryDate")]
        [UIHint("ExactTime")]
        public DateTime? LastRetryDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string SubscriberName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ContactPhoneNo")]
        [UIHint("PhoneNo")]
        public string PhoneNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TelekomSubscriberNo")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        //[MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string XDSLNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "OperationType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(RadiusR.DB.Enums.TelekomOperations.TelekomOperationType), typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationType))]
        [UIHint("LocalizedList")]
        public short? OperationType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "OperationSubType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType), typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationSubType))]
        [UIHint("LocalizedList")]
        public short? OperationSubType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "QueueNo")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveLong(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveLong")]
        [MaxLength(19, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string QueueNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ManagementCode")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string ManagementCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ProvinceCode")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string ProvinceCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransitionTransactionID")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string TransactionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsOpen")]
        public bool IsOpen { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClosingDate")]
        [UIHint("ExactTime")]
        public DateTime? ClosingDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [UIHint("TelekomWorkOrderState")]
        public short? State { get; set; }

        public long? _queueNo
        {
            get
            {
                long parsed;
                if (long.TryParse(QueueNo, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                QueueNo = value.ToString();
            }
        }

        public int? _managementCode
        {
            get
            {
                int parsed;
                if (int.TryParse(ManagementCode, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                ManagementCode = value.ToString();
            }
        }

        public int? _provinceCode
        {
            get
            {
                int parsed;
                if (int.TryParse(ProvinceCode, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                ProvinceCode = value.ToString();
            }
        }

        public long? _transactionID
        {
            get
            {
                long parsed;
                if (long.TryParse(TransactionID, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                TransactionID = value.ToString();
            }
        }

        public bool CanRetry
        {
            get
            {
                return OperationType != (short?)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition;
            }
        }

        public bool CanFinish
        {
            get
            {
                return OperationType != (short?)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition || State != (short?)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.InProgress;
            }
        }
    }
}
