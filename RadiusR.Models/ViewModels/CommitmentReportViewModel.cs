using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CommitmentReportViewModel
    {
        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string SubscriberName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentLength")]
        [EnumType(typeof(RadiusR.DB.Enums.CommitmentLength), typeof(RadiusR.Localization.Lists.CommitmentLength))]
        [UIHint("LocalizedList")]
        public short? CommitmentLength { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentExpirationDate")]
        public DateTime? ExpirationDate { get; set; }
    }
}
