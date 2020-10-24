using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.CSVModels
{
    public class BatchEBillCSVModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillNo")]
        public string BillId { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string ClientValidName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PhoneNo")]
        public string ClientPhoneNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public string IssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DueDate")]
        public string DueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        public string PayDate { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Address")]
        //public string Address { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Province")]
        //public string Province { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "City")]
        //public string City { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
        //public string TCKNo { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ServiceName")]
        //public string Service { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        public string Total { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Lists.TaxTypeID), Name = "VAT")]
        //public string VAT { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Lists.TaxTypeID), Name = "SCT")]
        //public string SCT { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxBase")]
        //public string TaxBase { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentType")]
        public string PaymentType { get; set; }

        public string URL { get; set; }
    }
}
