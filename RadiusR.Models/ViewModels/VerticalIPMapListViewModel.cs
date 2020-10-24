using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class VerticalIPMapListViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasName")]
        public string NASName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "VerticalIPMapList")]
        [UIHint("VerticalIPMap")]
        public IEnumerable<RadiusViewModels.NASVerticalIPMapViewModel> NASVerticalIPMaps { get; set; }
    }
}
