using RadiusR.DB;
using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SubscriberFeesAddViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeTypeID")]
        [EnumType(typeof(FeeType), typeof(RadiusR.Localization.Lists.FeeType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short? FeeTypeID { get; set; }

        public string FeeTypeName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InstallmentCount")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int InstallmentCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeVariant")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? SelectedVariantID { get; set; }

        public bool IsAllTime { get; set; }

        public IEnumerable<FeeVariant> Variants { get; set; }

        public IEnumerable<CustomFee> CustomFees { get; set; }

        public class FeeVariant
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeVariant")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            public int ID { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Title")]
            public string Name { get; set; }
        }

        public class CustomFee
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Title")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            [MaxLength(50, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
            public string Title { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
            [MaxLength(10, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
            [NonZero(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "NonZero")]
            [UIHint("Currency")]
            public string Price
            {
                get
                {
                    return (_price.HasValue) ? _price.Value.ToString("###,##0.00") : null;
                }
                set
                {
                    _price = (string.IsNullOrEmpty(value)) ? (decimal?)null : decimal.Parse(value);
                }
            }

            public decimal? _price { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InstallmentCount")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            public int Installment { get; set; }
        }

        public List<Fee> GetDBObject(IEnumerable<FeeTypeCost> allDBFees)
        {
            if (!FeeTypeID.HasValue || allDBFees.FirstOrDefault(f => f.FeeTypeID == FeeTypeID) == null)
                return null;
            var currentFee = allDBFees.FirstOrDefault(f => f.FeeTypeID == FeeTypeID);
            var currentVariant = currentFee.FeeTypeVariants.Any() ? currentFee.FeeTypeVariants.FirstOrDefault(fv => fv.ID == SelectedVariantID) : null;
            if (currentFee.FeeTypeVariants.Any() && currentVariant == null)
                return null;
            if (!currentFee.Cost.HasValue && currentVariant == null && (CustomFees == null || !CustomFees.Any()))
                return null;
            // for custom fee
            if(CustomFees != null && CustomFees.Any())
            {
                return CustomFees.Select(cf => new Fee()
                {
                    Cost = cf._price.Value,
                    Date = DateTime.Today,
                    Description = cf.Title,
                    //FeeDescription = new FeeDescription() { Description = cf.Title},
                    FeeTypeID = currentFee.FeeTypeID,
                    InstallmentBillCount = currentFee.IsAllTime ? (short)1 : (short)cf.Installment,
                }).ToList();
            }
            // for variant fee
            if(currentVariant != null)
            {
                return new List<Fee>()
                {
                    new Fee()
                    {
                        Cost = currentVariant.Price,
                        Date = DateTime.Today,
                        FeeTypeID = currentFee.FeeTypeID,
                        FeeTypeVariantID = currentVariant.ID,
                        InstallmentBillCount = currentFee.IsAllTime ? (short)1 : (short)InstallmentCount
                    }
                };
            }
            // no variant fee
            return new List<Fee>()
            {
                new Fee()
                {
                    Cost = currentFee.Cost,
                    InstallmentBillCount = currentFee.IsAllTime ? (short)1 : (short)InstallmentCount,
                    Date = DateTime.Today,
                    FeeTypeID = currentFee.FeeTypeID
                }
            };
        }
    }
}
