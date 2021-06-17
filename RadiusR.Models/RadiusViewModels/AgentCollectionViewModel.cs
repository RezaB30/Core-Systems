using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class AgentCollectionViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AgentName")]
        public string AgentName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        [UIHint("ExactTime")]
        public DateTime CreationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Creator")]
        public string CreatorName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        [UIHint("ExactTime")]
        public DateTime? PaymentDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Payer")]
        public string PayerName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Allowance")]
        [UIHint("Currency")]
        public string AllowanceAmount { get; private set; }

        public decimal _allowanceAmount
        {
            set
            {
                AllowanceAmount = value.ToString("###,###,##0.00");
            }
        }
    }
}
