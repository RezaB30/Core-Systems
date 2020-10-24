using RadiusR.DB.Enums;
using RadiusR.DB.Enums.RecurringDiscount;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ReferralDiscountReportViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ReferrerSubscriptionNo")]
        public string ReferrerSubscriptionNo { get; set; }

        public long ReferrerSubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ReferredSubscriptionNo")]
        public string ReferredSubscriptionNo { get; set; }

        public long ReferredSubscriptionID { get; set; }

        public RadiusViewModels.RecurringDiscountViewModel RecurringDiscount { get; set; }
    }
}
