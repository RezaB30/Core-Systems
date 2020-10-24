using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ExtendPackageViewModel
    {

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PeriodCount")]
        public int AddedPeriods { get; set; }

        public long ClientID { get; set; }

        public string ClientName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Total")]
        [UIHint("Currency")]
        public string TotalFee
        {
            get
            {
                if (!_totalFee.HasValue)
                    return 0m.ToString("###,##0.00");
                return _totalFee.Value.ToString("###,##0.00");
            }
            set
            {
                decimal parsed;
                if (decimal.TryParse(value, out parsed))
                    _totalFee = parsed;
            }
        }

        public decimal? _totalFee { get; set; }
    }
}