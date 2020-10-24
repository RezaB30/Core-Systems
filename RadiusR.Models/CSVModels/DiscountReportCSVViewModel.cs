using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.CSVModels
{
    public class DiscountReportCSVViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public string IssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        public string Total { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalDiscount")]
        public string DiscountTotal { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "PayingAmount")]
        public string PaymentTotal { get; set; }
    }
}
