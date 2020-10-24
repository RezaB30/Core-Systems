using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class MikrotikRateLimitViewModel
    {
        #region regexes
        private static Regex ValueWithSuffix = new Regex(@"^\d{1,}[k|M]?/\d{1,}[k|M]?$");
        private static Regex ValueWithoutSuffix = new Regex(@"^\d+/\d+$");
        private static Regex PriorityValue = new Regex(@"^[1-8]$");
        private static Regex NumberPart = new Regex(@"^\d+");
        private static Regex SuffixPart = new Regex(@"[k|M]$");
        #endregion

        public int? Rx
        {
            get
            {
                int value;
                if (int.TryParse(RxView, out value))
                    return value;
                return null;
            }
            set
            {
                RxView = value.ToString();
            }
        }

        public int? Tx
        {
            get
            {
                int value;
                if (int.TryParse(TxView, out value))
                    return value;
                return null;
            }
            set
            {
                TxView = value.ToString();
            }
        }

        public string RxSuffix { get; set; }

        public string TxSuffix { get; set; }

        public int? RxBurst
        {
            get
            {
                int value;
                if (int.TryParse(RxBurstView, out value))
                    return value;
                return null;
            }
            set
            {
                RxBurstView = value.ToString();
            }
        }

        public int? TxBurst
        {
            get
            {
                int value;
                if (int.TryParse(TxBurstView, out value))
                    return value;
                return null;
            }
            set
            {
                TxBurstView = value.ToString();
            }
        }

        public string RxBurstSuffix { get; set; }

        public string TxBurstSuffix { get; set; }

        public int? RxBurstThreshold
        {
            get
            {
                int value;
                if (int.TryParse(RxBurstThresholdView, out value))
                    return value;
                return null;
            }
            set
            {
                RxBurstThresholdView = value.ToString();
            }
        }

        public int? TxBurstThreshold
        {
            get
            {
                int value;
                if (int.TryParse(TxBurstThresholdView, out value))
                    return value;
                return null;
            }
            set
            {
                TxBurstThresholdView = value.ToString();
            }
        }

        public string RxBurstThresholdSuffix { get; set; }

        public string TxBurstThresholdSuffix { get; set; }

        public int? RxBurstTime
        {
            get
            {
                int value;
                if (int.TryParse(RxBurstTimeView, out value))
                    return value;
                return null;
            }
            set
            {
                RxBurstTimeView = value.ToString();
            }
        }

        public int? TxBurstTime
        {
            get
            {
                int value;
                if (int.TryParse(TxBurstTimeView, out value))
                    return value;
                return null;
            }
            set
            {
                TxBurstTimeView = value.ToString();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Priority")]
        public int? Priority { get; set; }

        #region methods
        public static MikrotikRateLimitViewModel Parse(string rawValue)
        {
            var results = new MikrotikRateLimitViewModel();
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;
            var parts = rawValue.Split(' ');
            if (parts.Count() < 1 || parts.Count() > 5)
                return null;
            for (int i = 0; i < parts.Count(); i++)
            {
                var isValid = false;
                var rxPart = 0;
                var txPart = 0;
                if (i < 3)
                {
                    isValid = ValueWithSuffix.IsMatch(parts[i]);
                    if (!isValid)
                        return null;
                    var rxSuffix = string.Empty;
                    var txSuffix = string.Empty;
                    var pair = parts[i].Split('/');
                    rxPart = int.Parse(NumberPart.Match(pair[0]).Value);
                    rxSuffix = rxPart > 0 ? SuffixPart.Match(pair[0]).Value : string.Empty;
                    txPart = int.Parse(NumberPart.Match(pair[1]).Value);
                    txSuffix = txPart > 0 ? SuffixPart.Match(pair[1]).Value : string.Empty;

                    switch (i)
                    {
                        case 0:
                            results.Rx = rxPart > 0 ? rxPart : (int?)null;
                            results.RxSuffix = rxSuffix;
                            results.Tx = txPart > 0 ? txPart : (int?)null;
                            results.TxSuffix = txSuffix;
                            break;
                        case 1:
                            results.RxBurst = rxPart > 0 ? rxPart : (int?)null;
                            results.RxBurstSuffix = rxSuffix;
                            results.TxBurst = txPart > 0 ? txPart : (int?)null;
                            results.TxBurstSuffix = txSuffix;
                            break;
                        case 2:
                            results.RxBurstThreshold = rxPart > 0 ? rxPart : (int?)null;
                            results.RxBurstThresholdSuffix = rxSuffix;
                            results.TxBurstThreshold = txPart > 0 ? txPart : (int?)null;
                            results.TxBurstThresholdSuffix = txSuffix;
                            break;
                        default:
                            return null;
                    }
                }
                if (i == 3)
                {
                    isValid = ValueWithoutSuffix.IsMatch(parts[i]);
                    if (!isValid)
                        return null;
                    var pair = parts[i].Split('/');
                    rxPart = int.Parse(NumberPart.Match(pair[0]).Value);
                    txPart = int.Parse(NumberPart.Match(pair[1]).Value);

                    results.RxBurstTime = rxPart > 0 ? rxPart : (int?)null;
                    results.TxBurstTime = txPart > 0 ? txPart : (int?)null;
                }
                if (i == 4)
                {
                    isValid = PriorityValue.IsMatch(parts[i]);
                    if (!isValid)
                        return null;
                    results.Priority = int.Parse(parts[i]);
                }
            }

            return results;
        }

        public override string ToString()
        {
            var retValue = Rx + RxSuffix + "/" + Tx + TxSuffix;
            if (RxBurst > 0 || TxBurst > 0 || RxBurstThreshold > 0 || TxBurstThreshold > 0 || RxBurstTime > 0 || TxBurstTime > 0 || Priority.HasValue)
            {
                retValue += " " + (RxBurst > 0 ? RxBurst + RxBurstSuffix : "0") + "/" + (TxBurst > 0 ? TxBurst + TxBurstSuffix : "0");
            }
            if (RxBurstThreshold > 0 || TxBurstThreshold > 0 || RxBurstTime > 0 || TxBurstTime > 0 || Priority.HasValue)
            {
                retValue += " " + (RxBurstThreshold > 0 ? RxBurstThreshold + RxBurstThresholdSuffix : "0") + "/" + (TxBurstThreshold > 0 ? TxBurstThreshold + TxBurstThresholdSuffix : "0");
            }
            if (RxBurstTime > 0 || TxBurstTime > 0 || Priority.HasValue)
            {
                retValue += " " + (RxBurstTime > 0 ? RxBurstTime.ToString() : "0") + "/" + (TxBurstTime > 0 ? TxBurstTime.ToString() : "0");
            }
            if (Priority.HasValue)
            {
                retValue += " " + Priority.Value;
            }

            return retValue;
        }

        #endregion

        #region view_props

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "UploadRate")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string RxView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "DownloadRate")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string TxView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "UploadBurstRate")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string RxBurstView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "DownloadBurstRate")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string TxBurstView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "UploadBurstThreshold")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string RxBurstThresholdView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "DownloadBurstThreshold")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string TxBurstThresholdView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "UploadBurstTime")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string RxBurstTimeView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "DownloadBurstTime")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        public string TxBurstTimeView { get; set; }

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Priority")]
        //[RegularExpression(@"^[1-8]?$", ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        //public string PriorityView { get; set; }

        #endregion
    }
}
