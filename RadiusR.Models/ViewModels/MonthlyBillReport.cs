using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class MonthlyBillReport
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        public int _year { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        public int _month { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalBillCount")]
        public int TotalBillCount { get; set; }

        public decimal _totalBillAmount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaidBillCount")]
        public int PaidBillCount { get; set; }

        public decimal _paidBillAmount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnpaidBillCount")]
        public int UnpaidBillCount { get; set; }

        public decimal _unpaidBillAmount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalBillAmount")]
        [UIHint("Currency")]
        public string TotalBillAmount
        {
            get
            {
                return _totalBillAmount.ToString("###,###,##0.00");
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaidBillAmount")]
        [UIHint("Currency")]
        public string PaidBillAmount
        {
            get
            {
                return _paidBillAmount.ToString("###,###,##0.00");
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnpaidBillAmount")]
        [UIHint("Currency")]
        public string UnpaidBillAmount
        {
            get
            {
                return _unpaidBillAmount.ToString("###,###,##0.00");
            }
        }
    }
}