using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CancelledClientUnpaidBillsViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PhoneNo")]
        public string PhoneNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationDate")]
        public DateTime? CancellationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnpaidBillCount")]
        public int UnpaidBillCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalDebt")]
        [UIHint("Currency")]
        public string TotalDebt
        {
            get
            {
                return _totalDebt.ToString("###,##0.00");
            }
        }

        public decimal _totalDebt { get; set; }
    }
}
