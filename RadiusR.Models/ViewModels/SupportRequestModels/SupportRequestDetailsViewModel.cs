using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.SupportRequestModels
{
    public class SupportRequestDetailsViewModel
    {
        public SupportRequestListViewModel RequestInfo { get; set; }

        public IEnumerable<SupportRequestStageViewModel> Stages { get; set; }
    }
}
