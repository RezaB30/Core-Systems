using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ServiceClientCountViewModel
    {
        public int ServiceID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ServiceName")]
        public string ServiceName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientCount")]
        public string ClientCount
        {
            get
            {
                return _clientCount.ToString("###,###,##0");
            }
        }

        public long _clientCount { get; set; }
    }
}
