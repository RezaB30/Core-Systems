using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ClientCountReportViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalCount")]
        public long TotalCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancelledCount")]
        public long CancelledCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ActiveCount")]
        public long ActiveCount
        {
            get
            {
                return TotalCount - CancelledCount;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PassiveCount")]
        public long PassiveCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FreezedCount")]
        public long FreezedCount { get; set; }

        public List<object> DiagramData { get; set; }
    }
}
