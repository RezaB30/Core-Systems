using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class PartnerCollectionDetailsViewModel
    {
        public long ID { get; set; }

        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        [UIHint("ExactTime")]
        public DateTime IssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompletionDate")]
        [UIHint("ExactTime")]
        public DateTime? CompletionDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Allowance")]
        [UIHint("Currency")]
        public string Allowance { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AllowanceState")]
        [EnumType(typeof(RadiusR.DB.Enums.PartnerAllowanceState), typeof(RadiusR.Localization.Lists.PartnerAllowanceState))]
        [UIHint("LocalizedList")]
        public short AllowanceState { get; set; }

        public decimal? _allowance
        {
            get
            {
                decimal result;
                if (decimal.TryParse(Allowance, out result))
                {
                    return result;
                }
                return null;
            }

            set
            {
                Allowance = value.HasValue ? value.Value.ToString("###,##0.00") : null;
            }
        }
    }
}
