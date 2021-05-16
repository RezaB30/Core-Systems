using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class OfflinePaymentStatusReportViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Gateway")]
        public string Gateway { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Stage")]
        public string Stage { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Results")]
        public string Results { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Success")]
        public bool IsSuccess { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Details")]
        public IEnumerable<string> DetailedList { get; set; }

        public override string ToString()
        {
            return $"[Gateway: {Gateway}],[Stage: {Stage}],[Results: {Results}],[Success: {IsSuccess}],[DetailedList: {string.Join("|", DetailedList ?? Enumerable.Empty<string>())}]";
        }
    }
}
