using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AgentPaymentViewModel
    {
        public long BillID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentType")]
        [EnumType(typeof(RadiusR.DB.Enums.PaymentType), typeof(RadiusR.Localization.Lists.PaymentType))]
        [UIHint("LocalizedList")]
        public short PaymentType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        [UIHint("ExactTime")]
        public DateTime PaymentDate { get; set; }

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

        public decimal _total { get; set; }

        public decimal _allowance { get; set; }

        public decimal _commission { get; set; }
    }
}
