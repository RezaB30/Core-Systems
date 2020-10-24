using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class PartnerAvailableTariffViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int DomianID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        public string DomainName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int TariffID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        public string TariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentLength")]
        [EnumType(typeof(RadiusR.DB.Enums.CommitmentLength), typeof(RadiusR.Localization.Lists.CommitmentLength))]
        [UIHint("LocalizedList")]
        public short? Commitment { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Allowance")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string Allowance { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AllowanceThreshold")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string AllowanceThreshold { get; set; }

        public decimal? _allowance
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(Allowance, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                if (value.HasValue)
                    Allowance = value.Value.ToString("###,##0.00");
                else
                    Allowance = null;
            }
        }

        public decimal? _allowanceThreshold
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(AllowanceThreshold, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                if (value.HasValue)
                    AllowanceThreshold = value.Value.ToString("###,##0.00");
                else
                    AllowanceThreshold = null;
            }
        }
    }
}
