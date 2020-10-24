using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Enums.CustomerSetup;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class CustomerSetupServiceTaskViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaskID")]
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Operator")]
        public string User { get; set; }

        public SubscriptionListDisplayViewModel Client { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaskType")]
        [EnumType(typeof(TaskTypes), typeof(RadiusR.Localization.Lists.CustomerSetup.TaskType))]
        [UIHint("LocalizedList")]
        public short TaskType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "HasModem")]
        public bool HasModem { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ModemName")]
        public string ModemName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "XDSLType")]
        [EnumType(typeof(XDSLTypes), typeof(RadiusR.Localization.Lists.CustomerSetup.XDSLTypes))]
        [UIHint("LocalizedList")]
        public short XDSLType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        [UIHint("ExactTime")]
        public DateTime IssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ReservationDate")]
        public DateTime? ReservationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompletionDate")]
        [UIHint("ExactTime")]
        public DateTime? CompletionDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(TaskStatuses), typeof(RadiusR.Localization.Lists.CustomerSetup.TaskStatuses))]
        [UIHint("LocalizedList")]
        public short Status { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
        public string Details { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsChargeable")]
        public bool IsCharged { get; set; }
    }
}
