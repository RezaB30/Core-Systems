using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public string ClientName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Total")]
        [UIHint("Currency")]
        public string Total { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public IEnumerable<DateTime> IssueDates { get; set; }

        public string RedirectUrl { get; set; }
    }
}