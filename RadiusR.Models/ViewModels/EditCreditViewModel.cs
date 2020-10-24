using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class EditCreditViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AddingAmount")]
        [UIHint("Currency")]
        public string AddingAmount
        {
            get
            {
                return _addingAmount.HasValue ? _addingAmount.Value.ToString("###,##0.00") : null;
            }
            set
            {
                decimal addingAmount;
                if (decimal.TryParse(value, out addingAmount))
                    _addingAmount = addingAmount;
            }
        }

        public decimal? _addingAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubtractingAmount")]
        [UIHint("Currency")]
        public string SubtractingAmount
        {
            get
            {
                return _subtractingAmount.HasValue ? _subtractingAmount.Value.ToString("###,##0.00") : null;
            }
            set
            {
                decimal subtractingAmount;
                if (decimal.TryParse(value, out subtractingAmount))
                    _subtractingAmount = subtractingAmount;
            }
        }

        public decimal? _subtractingAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Details")]
        public string Details { get; set; }
    }
}