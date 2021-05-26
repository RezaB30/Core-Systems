using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class AgentTariffViewModel
    {
        public int TariffID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ServiceName")]
        public string TariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [UIHint("Currency")]
        public string Price
        {
            get
            {
                return _price.HasValue ? _price.Value.ToString("###,##0.00") : null;
            }
            set
            {
                decimal price;
                if (decimal.TryParse(value, out price))
                    _price = price;
            }
        }

        public decimal? _price { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsActive")]
        public bool IsActive { get; set; }

        public int DomainID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        public string DomainName { get; set; }

    }
}
