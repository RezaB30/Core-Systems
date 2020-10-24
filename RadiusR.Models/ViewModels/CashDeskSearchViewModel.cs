using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CashDeskSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
        public string FullName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IncomeType")]
        [UIHint("LocalizedList")]
        [EnumType(typeof(MoneyInputType), typeof(RadiusR.Localization.Lists.MoneyInputType))]
        public short PaymentType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EndDate")]
        public DateTime? EndDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Accountant")]
        public int? AccountantID { get; set; }
    }
}