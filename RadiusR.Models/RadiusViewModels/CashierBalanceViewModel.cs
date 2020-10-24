using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class CashierBalanceViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Dealer")]
        public int CashierID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Details")]
        public string Details { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Amount")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [UIHint("Currency")]
        public string Amount
        {
            get
            {
                return _amount.ToString("###,##0.00");
            }
            set
            {
                _amount = decimal.Parse(value);
            }
        }

        public decimal _amount { get; set; }

        public Cashier Cashier { get; set; }
    }
}