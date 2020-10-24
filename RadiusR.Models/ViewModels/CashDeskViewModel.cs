using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CashDeskViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IncomeType")]
        [UIHint("LocalizedList")]
        [EnumType(typeof(MoneyInputType), typeof(RadiusR.Localization.Lists.MoneyInputType))]
        public short PaymentType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Accountant")]
        public string AccountantName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Total")]
        [UIHint("Currency")]
        public string Total
        {
            get
            {
                return _total.ToString("###,###,##0.00");
            }
        }

        public decimal _total { get; set; }

    }
}