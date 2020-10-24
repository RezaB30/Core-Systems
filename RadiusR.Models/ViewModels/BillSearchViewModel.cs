using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class BillSearchViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerName")]
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Accountant")]
        public int? AccountantID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public DateTime? IssueDateStart { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public DateTime? IssueDateEnd { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DueDate")]
        public DateTime? DueDateStart { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DueDate")]
        public DateTime? DueDateEnd { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        public DateTime? PaymentDateStart { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        public DateTime? PaymentDateEnd { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(BillState), typeof(RadiusR.Localization.Lists.BillState))]
        [UIHint("LocalizedList")]
        public short State { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentType")]
        [EnumType(typeof(PaymentType), typeof(RadiusR.Localization.Lists.PaymentType))]
        [UIHint("LocalizedList")]
        public short PaymentTypeID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentGateway")]
        public int? PaymentGateway { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingPeriod")]
        public int? BillingPeriod { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillNo")]
        public string EBillCode { get; set; }
    }
}