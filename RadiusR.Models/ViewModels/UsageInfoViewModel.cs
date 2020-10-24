using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class UsageInfoViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        public DateTime Date { get; set; }

        public int? _month { get; set; }

        public int? _year { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Download")]
        [UIHint("FormattedBytes")]
        public decimal Download { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Upload")]
        [UIHint("FormattedBytes")]
        public decimal Upload { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        [UIHint("FormattedBytes")]
        public decimal Total
        {
            get
            {
                return Download + Upload;
            }
        }
    }
}