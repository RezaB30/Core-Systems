using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class PartnerCollectionViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        [UIHint("ExactTime")]
        public DateTime CreationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Creator")]
        public string Creator { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        [UIHint("ExactTime")]
        public DateTime? PaymentDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Payer")]
        public string Payer { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        [UIHint("Currency")]
        public string Total { get; set; }

        public decimal? _total
        {
            get
            {
                decimal result;
                if (decimal.TryParse(Total, out result))
                {
                    return result;
                }
                return null;
            }

            set
            {
                Total = value.HasValue ? value.Value.ToString("###,##0.00") : null;
            }
        }
    }
}
