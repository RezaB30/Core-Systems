using RadiusR.DB;
using RadiusR_Manager.Models.CustomAttributes;
using RezaB.DBUtilities;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AppSettingsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "TableRows")]
        [SettingElement]
        public int TableRows { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "PagesLinkCount")]
        [SettingElement]
        public int PagesLinkCount { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "ChartMaxSteps")]
        [SettingElement]
        public int ChartMaxSteps { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "CountryPhoneCode")]
        [CountryPhoneCode(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "CountryPhoneCode")]
        [SettingElement]
        public string CountryPhoneCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "EBillIsActive")]
        [SettingElement]
        public bool EBillIsActive { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "PastDueFlatPenalty")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        [SettingElement]
        public string PastDueFlatPenalty { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "PastDuePenaltyPercentage")]
        [Percentage(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Percentage")]
        [UIHint("Percent")]
        [SettingElement]
        public string PastDuePenaltyPercentage { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "InvoiceArchiveIDPrefix")]
        [EBillPrefix(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "EBillPrefix")]
        [SettingElement]
        public string InvoiceArchiveIDPrefix { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "InvoiceBillIDPrefix")]
        [EBillPrefix(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "EBillPrefix")]
        [SettingElement]
        public string InvoiceBillIDPrefix { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderCentralSystemNo")]
        [SettingElement]
        public string SenderCentralSystemNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderCityName")]
        [SettingElement]
        public string SenderCityName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderCountryName")]
        [SettingElement]
        public string SenderCountryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderProvinceName")]
        [SettingElement]
        public string SenderProvinceName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderCompanyTaxRegion")]
        [SettingElement]
        public string SenderCompanyTaxRegion { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderCompanyTitle")]
        [SettingElement]
        public string SenderCompanyTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderEmail")]
        [Email(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "EmailValidation")]
        [SettingElement]
        public string SenderEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderPhoneNo")]
        [SettingElement]
        public string SenderPhoneNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderFaxNo")]
        [SettingElement]
        public string SenderFaxNo { get; set; }


        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderRegistrationNo")]
        [SettingElement]
        public string SenderRegistrationNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SenderTaxNo")]
        [TaxNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TaxNumber")]
        [SettingElement]
        public string SenderTaxNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "EBillCompanyCode")]
        [SettingElement]
        public string EBillCompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "EBillApiUsername")]
        [SettingElement]
        public string EBillApiUsername { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "EBillApiPassword")]
        [SettingElement]
        public string EBillApiPassword { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "EBillsThreshold")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [SettingElement]
        public short EBillsThreshold { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "ReviewDelay")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [SettingElement]
        public string ReviewDelay { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "GeocodingAPIKey")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string GeocodingAPIKey { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "CompanyName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(200, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string CompanyName { get; set; }

        public decimal? _PastDueFlatPenalty
        {
            get
            {
                decimal result;
                if (decimal.TryParse(PastDueFlatPenalty, out result))
                    return result;
                return null;
            }
            set
            {
                PastDueFlatPenalty = value.ToString();
            }
        }

        public decimal? _PastDuePenaltyPercentage
        {
            get
            {
                decimal result;
                if (decimal.TryParse(PastDuePenaltyPercentage, out result))
                    return result / 100m;
                return null;
            }
            set
            {
                PastDuePenaltyPercentage = (value.Value * 100m).ToString("##0.##");
            }
        }

        public short? _ReviewDelay
        {
            get
            {
                short result;
                if (short.TryParse(ReviewDelay, out result))
                    return result;
                return null;
            }
            set
            {
                ReviewDelay = value.Value.ToString();
            }
        }

        public AppSettingsViewModel() { }

        public AppSettingsViewModel(bool loadup)
        {
            if (loadup)
            {
                TableRows = AppSettings.TableRows;
                PagesLinkCount = AppSettings.PagesLinkCount;
                ChartMaxSteps = AppSettings.ChartMaxSteps;
                CountryPhoneCode = AppSettings.CountryPhoneCode;
                _PastDueFlatPenalty = AppSettings.PastDueFlatPenalty;
                _PastDuePenaltyPercentage = AppSettings.PastDuePenaltyPercentage;
                EBillIsActive = AppSettings.EBillIsActive;
                InvoiceArchiveIDPrefix = AppSettings.InvoiceArchiveIDPrefix;
                InvoiceBillIDPrefix = AppSettings.InvoiceBillIDPrefix;
                SenderCentralSystemNo = AppSettings.SenderCentralSystemNo;
                SenderCityName = AppSettings.SenderCityName;
                SenderCountryName = AppSettings.SenderCountryName;
                SenderProvinceName = AppSettings.SenderProvinceName;
                SenderCompanyTaxRegion = AppSettings.SenderCompanyTaxRegion;
                SenderCompanyTitle = AppSettings.SenderCompanyTitle;
                SenderEmail = AppSettings.SenderEmail;
                SenderPhoneNo = AppSettings.SenderPhoneNo;
                SenderFaxNo = AppSettings.SenderFaxNo;
                SenderRegistrationNo = AppSettings.SenderRegistrationNo;
                SenderTaxNo = AppSettings.SenderTaxNo;
                EBillCompanyCode = AppSettings.EBillCompanyCode;
                EBillApiUsername = AppSettings.EBillApiUsername;
                EBillApiPassword = AppSettings.EBillApiPassword;
                EBillsThreshold = AppSettings.EBillsThreshold;
                _ReviewDelay = AppSettings.ReviewDelay;
                GeocodingAPIKey = AppSettings.GeocodingAPIKey;
                CompanyName = AppSettings.CompanyName;
            }
        }

    }
}