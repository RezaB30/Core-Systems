using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.SupportRequestModels
{
    public class SupportRequestStageViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        public string Message { get; set; }

        public bool IsVisibleToCustomer { get; set; }

        public string CommittingUser { get; set; }

        [EnumType(typeof(RadiusR.DB.Enums.SupportRequests.SupportRequestActionTypes), typeof(RadiusR.Localization.Lists.SupportRequests.SupportRequestActionTypes))]
        [UIHint("LocalizedList")]
        public short ActionType { get; set; }

        public short? OldState { get; set; }

        public short? NewState { get; set; }

        public string GroupName { get; set; }
    }
}
