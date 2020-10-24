using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SystemLogViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "User")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LogType")]
        [EnumType(typeof(SystemLogTypes), typeof(RadiusR.Localization.Lists.SystemLogTypes))]
        [UIHint("BigLocalizedList")]
        public int LogType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LogInterfaceType")]
        [EnumType(typeof(SystemLogInterface), typeof(RadiusR.Localization.Lists.SystemLogInterface))]
        [UIHint("LocalizedList")]
        public short LogInterfaceType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LogInterfaceUsername")]
        public string LogInterfaceUsername { get; set; }

        public long? SubscriptionID { get; set; }

        public long? CustomerID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Details")]
        [UIHint("RawHtml")]
        public string ProcessedLog { get; set; }
    }
}
