using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.CSVModels
{
    public class CancelledClientsUnpaidBillsCSVModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PhoneNo")]
        public string PhoneNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationDate")]
        public string CancellationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnpaidBillCount")]
        public string UnpaidBillsCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalDebt")]
        public string TotalDebt { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillsDetail")]
        public string BillsDetail { get; set; }
    }
}
