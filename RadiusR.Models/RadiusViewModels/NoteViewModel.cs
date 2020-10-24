using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class NoteViewModel
    {
        public long ID { get; set; }

        public long SubscriptionID { get; set; }

        public int WriterID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Message")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Message { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        // ---------------- View Only --------------------

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Author")]
        public string WriterName { get; set; }
    }
}