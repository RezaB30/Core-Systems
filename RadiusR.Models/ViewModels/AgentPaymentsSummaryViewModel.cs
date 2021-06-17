using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AgentPaymentsSummaryViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        [UIHint("Currency")]
        public string Total
        {
            get
            {
                return _total.ToString("###,###,##0.00");
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Allowance")]
        [UIHint("Currency")]
        public string Allowance
        {
            get
            {
                return _allowance.ToString("###,###,##0.00");
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Commission")]
        [UIHint("Currency")]
        public string Commission
        {
            get
            {
                return _commission.ToString("###,###,##0.00");
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalAllowance")]
        [UIHint("Currency")]
        public string TotalAllowance
        {
            get
            {
                return (_allowance - _commission).ToString("###,###,##0.00");
            }
        }

        public decimal _total { get; set; }

        public decimal _allowance { get; set; }

        public decimal _commission { get; set; }
    }
}
