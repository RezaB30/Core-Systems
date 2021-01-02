using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement
{
    public class FileManagerClientAttachmentWithContent : FileManagerClientAttachment, IDisposable
    {
        public Stream Content { get; private set; }

        public FileManagerClientAttachmentWithContent(Stream fileContent, ClientAttachmentTypes attachmentType, string fileExtention) : base(attachmentType, fileExtention)
        {
            Content = fileContent;
        }

        internal FileManagerClientAttachmentWithContent(Stream fileContent, string serverSideName) : base(serverSideName)
        {
            Content = fileContent;
        }

        internal FileManagerClientAttachmentWithContent(Stream fileContent, DateTime creationDate, string randomSegment, ClientAttachmentTypes attachmentType, string fileExtention) : base(creationDate, randomSegment, attachmentType, fileExtention)
        {
            Content = fileContent;
        }

        public void Dispose()
        {
            if (Content != null)
                Content.Dispose();
        }
    }
}
