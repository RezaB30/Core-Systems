using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ExpiredPoolListViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasName")]
        public string NASName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ExpiredPoolList")]
        [UIHint("ExpiredPoolList")]
        public IEnumerable<ExpiredPoolViewModel> ExpiredPools { get; set; }
    }
}
