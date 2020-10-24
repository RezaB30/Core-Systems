using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class EBillBatchResultsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SuccessfulCount")]
        public int SuccessfulCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UnsuccessfulCount")]
        public int UnsuccessfulCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalCount")]
        public int TotalCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InvalidCount")]
        public int InvalidCount
        {
            get
            {
                return TotalCount - SuccessfulCount - UnsuccessfulCount;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Results")]
        [EnumType(typeof(ResultType), typeof(RadiusR.Localization.Lists.EBillResultType))]
        [UIHint("LocalizedList")]
        public short ErrorCode { get; set; }

        public enum ResultType
        {
            Success = 0,
            CuncurrencyDetected = 1,
            FatalError = 2,
            PartialError = 3
        }
    }
}
