using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class RiskyClientViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnpaidBillCount")]
        public int UnpaidBillCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnpaidBillAmount")]
        [UIHint("Currency")]
        public string UnpaidAmount
        {
            get
            {
                return _unpaidAmount.ToString("###,###,##0.00");
            }
        }

        public decimal _unpaidAmount { get; set; }
    }
}