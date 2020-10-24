using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.NetInvoice.RadiusRDBAdapter
{
    public class EBillDefaults
    {
        public bool IsActive { get; set; }

        public decimal PastDueFlatPenalty { get; set; }

        public decimal PastDuePenaltyPercentage { get; set; }

        public string InvoiceArchiveIDPrefix { get; set; }

        public string InvoiceBillIDPrefix { get; set; }

        public string SenderCentralSystemNo { get; set; }

        public string SenderCityName { get; set; }

        public string SenderCountryName { get; set; }

        public string SenderProvinceName { get; set; }

        public string SenderCompanyTaxRegion { get; set; }

        public string SenderCompanyTitle { get; set; }

        public string SenderEmail { get; set; }

        public string SenderPhoneNo { get; set; }

        public string SenderFaxNo { get; set; }

        public string SenderRegistrationNo { get; set; }

        public string SenderTaxNo { get; set; }

        public EBillDefaults(bool useDBDefaults = true)
        {
            if (useDBDefaults)
            {
                IsActive = AppSettings.EBillIsActive;
                InvoiceArchiveIDPrefix = AppSettings.InvoiceArchiveIDPrefix;
                InvoiceBillIDPrefix = AppSettings.InvoiceBillIDPrefix;
                PastDueFlatPenalty = AppSettings.PastDueFlatPenalty;
                PastDuePenaltyPercentage = AppSettings.PastDuePenaltyPercentage;
                SenderCentralSystemNo = AppSettings.SenderCentralSystemNo;
                SenderCityName = AppSettings.SenderCityName;
                SenderCompanyTaxRegion = AppSettings.SenderCompanyTaxRegion;
                SenderCompanyTitle = AppSettings.SenderCompanyTitle;
                SenderCountryName = AppSettings.SenderCountryName;
                SenderEmail = AppSettings.SenderEmail;
                SenderPhoneNo = AppSettings.SenderPhoneNo;
                SenderProvinceName = AppSettings.SenderProvinceName;
                SenderRegistrationNo = AppSettings.SenderRegistrationNo;
                SenderTaxNo = AppSettings.SenderTaxNo;
                SenderFaxNo = AppSettings.SenderFaxNo;
            }
        }
    }
}
