using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RadiusR_Manager.Models.CSVModels
{
    public class BillTaxDetailsCSVModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public string IssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EBillIssueDate")]
        public string EBillIssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EBillType")]
        public string EBillType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillNo")]
        public string BillCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxOffice")]
        public string TaxRegion { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxNo")]
        public string TaxNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
        public string TCKNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceName")]
        public string ServiceName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string ClientValidName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        public string Total { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxBase")]
        public string TaxBase { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.TaxTypeID), Name = "VAT")]
        public string VAT { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.TaxTypeID), Name = "SCT")]
        public string SCT { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Discount")]
        public string Discount { get; set; }
    }
}
