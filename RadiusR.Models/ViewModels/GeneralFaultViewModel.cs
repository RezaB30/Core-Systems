using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class GeneralFaultViewModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<FaultViewModel> Faults { get; set; }

        public class FaultViewModel
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StartDate")]
            public string StartDate { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EndDate")]
            public string EndDate { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Title")]
            public string Topic { get; set; }
        }
    }
}
