using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SavedFileViewModel
    {
        public string FileName { get; set; }

        [UIHint("ExactTime")]
        public DateTime CreationDate { get; set; }

        [EnumType(typeof(RadiusR.FileManagement.ClientAttachmentTypes), typeof(RadiusR.Localization.Lists.ClientAttachmentTypes))]
        [UIHint("LocalizedList")]
        public short AttachmentType { get; set; }

        public string FileExtention { get; set; }
    }
}
