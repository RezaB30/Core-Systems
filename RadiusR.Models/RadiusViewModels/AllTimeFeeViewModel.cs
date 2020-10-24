using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class AllTimeFeeViewModel
    {
        public long BillID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeTypeID")]
        [EnumType(typeof(FeeType), typeof(RadiusR.Localization.Lists.FeeType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short FeeTypeID { get; set; }

        public decimal _cost { get; set; }

        [UIHint("Currency")]
        public string Cost
        {
            get
            {
                return _cost.ToString("###,##0.00");
            }
            set
            {
                decimal parsed;
                if (decimal.TryParse(value, out parsed))
                {
                    _cost = parsed;
                }
            }
        }


    }
}