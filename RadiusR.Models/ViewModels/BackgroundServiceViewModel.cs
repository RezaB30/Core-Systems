using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class BackgroundServiceViewModel
    {

        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType( typeof(ServiceState), typeof(RadiusR.Localization.Lists.BackgroundServiceState))]
        [UIHint("LocalizedList")]
        public short State { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Name")]
        public string DisplayName { get; set; }

        public enum ServiceState
        {
            Running = 1,
            Stopped = 2
        }
    }
}
