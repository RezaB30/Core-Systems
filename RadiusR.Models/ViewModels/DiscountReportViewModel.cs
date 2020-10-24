using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class DiscountReportViewModel
    {

        public IEnumerable<BillRow> Rows { get; set; }

        [UIHint("Currency")]
        public string Total { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalDiscount")]
        [UIHint("Currency")]
        public string DiscountTotal { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "PayingAmount")]
        [UIHint("Currency")]
        public string PaymentTotal { get; set; }

        public decimal _total
        {
            set
            {
                Total = value.ToString("###,##0.00");
            }
        }

        public decimal _discountTotal
        {
            set
            {
                DiscountTotal = value.ToString("###,##0.00");
            }
        }

        public decimal _paymentTotal
        {
            set
            {
                PaymentTotal = value.ToString("###,##0.00");
            }
        }

        public class BillRow
        {
            public long SubscriberId { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
            public string SubscriberNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
            public DateTime IssueDate { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
            [UIHint("Currency")]
            public string Total { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalDiscount")]
            [UIHint("Currency")]
            public string DiscountTotal { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "PayingAmount")]
            [UIHint("Currency")]
            public string PaymentTotal { get; set; }

            public bool IsCancelled { get; set; }

            public decimal _total
            {
                set
                {
                    Total = value.ToString("###,##0.00");
                }
            }

            public decimal _discountTotal
            {
                set
                {
                    DiscountTotal = value.ToString("###,##0.00");
                }
            }

            public decimal _paymentTotal
            {
                set
                {
                    PaymentTotal = value.ToString("###,##0.00");
                }
            }
        }
    }
}
