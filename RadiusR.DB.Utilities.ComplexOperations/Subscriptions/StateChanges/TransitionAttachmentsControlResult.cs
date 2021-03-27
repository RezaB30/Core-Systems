using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class TransitionAttachmentsControlResult
    {
        public bool IsValid
        {
            get
            {
                return FileManagerException == null && (RequiredDocuments?.All(rd => rd.IsAvailable) ?? false);
            }
        }

        public IEnumerable<RequiredDocument> RequiredDocuments { get; set; }

        public IEnumerable<RadiusR.FileManagement.SpecialFiles.FileManagerClientAttachment> ValidDocuments { get; set; }

        public Exception FileManagerException { get; set; }

        public class RequiredDocument
        {
            public bool IsAvailable { get; set; }

            public RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes DocumentType { get; set; }
        }
    }
}
