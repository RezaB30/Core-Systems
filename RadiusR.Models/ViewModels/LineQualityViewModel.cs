using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class LineQualityViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DSLAM")]
        public string DSLAM { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ShelfCardPort")]
        public string ShelfCardPort { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CRMSpeedProfile")]
        public string CRMSpeedProfile { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NMSSpeedProfile")]
        public string NMSSpeedProfile { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LineState")]
        [EnumType(typeof(TTLineState), typeof(RadiusR.Localization.Lists.TTLineState))]
        [UIHint("LocalizedList")]
        public short LineState { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DownloadAttn")]
        public string DownloadAttn { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UploadAttn")]
        public string UploadAttn { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DownloadNoiseMargin")]
        public string DownloadNoiseMargin { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UploadNoiseMargin")]
        public string UploadNoiseMargin { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CurrentDownloadSpeed")]
        public string CurrentDownloadSpeed { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CurrentUploadSpeed")]
        public string CurrentUploadSpeed { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DownloadSpeedCapacity")]
        public string DownloadSpeedCapacity { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UploadSpeedCapacity")]
        public string UploadSpeedCapacity { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DownloadOutputPower")]
        public string DownloadOutputPower { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "UploadOutputPower")]
        public string UploadOutputPower { get; set; }


    }
}
