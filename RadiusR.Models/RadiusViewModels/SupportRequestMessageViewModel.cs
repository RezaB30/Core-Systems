using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SupportRequestMessageViewModel
    {
        public long RequestID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Message { get; set; }
    }
}