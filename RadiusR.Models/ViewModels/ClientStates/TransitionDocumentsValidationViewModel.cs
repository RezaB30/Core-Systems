using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.ClientStates
{
    public class TransitionDocumentsValidationViewModel
    {
        public string ValidationMessage { get; set; }

        public IEnumerable<DocumentValidation> Documents { get; set; }

        public class DocumentValidation
        {
            [EnumType(typeof(RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes), typeof(RadiusR.Localization.Lists.ClientAttachmentTypes))]
            [UIHint("LocalizedList")]
            public short DocumentType { get; set; }

            public bool IsValid { get; set; }
        }
    }
}
