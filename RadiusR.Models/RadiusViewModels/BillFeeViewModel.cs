using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class BillFeeViewModel
    {
        public long ID { get; set; }

        public long BillID { get; set; }

        public string DisplayName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [UIHint("Currency")]
        public string CurrentCost { get; set; }

        public decimal? _currentCost
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(CurrentCost, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                if (value.HasValue)
                    CurrentCost = value.Value.ToString("###,###,##0.00");
                else
                    CurrentCost = null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Discount")]
        [UIHint("Currency")]
        public string DiscountAmount { get; set; }

        public decimal? _discountAmount
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(DiscountAmount, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                if (value.HasValue)
                    DiscountAmount = value.Value.ToString("###,###,##0.00");
                else
                    DiscountAmount = null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InstallmentCount")]
        public int InstallmentCount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
