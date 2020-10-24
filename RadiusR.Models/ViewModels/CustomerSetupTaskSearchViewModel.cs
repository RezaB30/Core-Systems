using RadiusR.DB.Enums.CustomerSetup;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CustomerSetupTaskSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Operator")]
        public int? OperatorID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaskType")]
        [EnumType(typeof(TaskTypes), typeof(RadiusR.Localization.Lists.CustomerSetup.TaskType))]
        [UIHint("LocalizedList")]
        public short TaskType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(TaskStatuses), typeof(RadiusR.Localization.Lists.CustomerSetup.TaskStatuses))]
        [UIHint("LocalizedList")]
        public short TaskState { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EndDate")]
        public DateTime? EndDate { get; set; }
    }
}
