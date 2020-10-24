using RadiusR.DB.DomainsCache;
using RezaB.TurkTelekom.WebServices;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class TelekomTariffHelperViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "XDSLType")]
        [EnumType(typeof(XDSLType), typeof(RadiusR.Localization.Lists.XDSLType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short? XDSLType { get; set; }

        public decimal MonthlyStaticFee { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Speed")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? SpeedCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Speed")]
        public string SpeedName { get; set; }

        public string SpeedDetails { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        public string TariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? PacketCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? TariffCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsPaperworkNeeded")]
        public bool IsPaperworkNeeded { get; set; }

        public TelekomTariffHelperViewModel() { }

        public TelekomTariffHelperViewModel(CachedTelekomTariff cachedTariff)
        {
            XDSLType = (short)cachedTariff.XDSLType;
            MonthlyStaticFee = cachedTariff.MonthlyStaticFee;
            SpeedCode = cachedTariff.SpeedCode;
            SpeedName = cachedTariff.SpeedName;
            TariffName = cachedTariff.TariffName;
            TariffCode = cachedTariff.TariffCode;
            PacketCode = cachedTariff.PacketCode;
        }
    }
}
