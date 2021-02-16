using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class GroupViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "GroupName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsActive")]
        public bool IsActive { get; set; }

        public int _subscriptionCount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Clients")]
        public string SubscriptionsCount
        {
            get
            {
                return _subscriptionCount.ToString("###,###,##0");
            }
        }

        public bool CanBeChanged { get; set; }
    }
}