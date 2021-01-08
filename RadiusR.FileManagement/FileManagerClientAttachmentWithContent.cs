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
            if (fileContent.CanSeek)
            {
                fileContent.Seek(0, SeekOrigin.Begin);
            }
            MD5 = string.Join(string.Empty, System.Security.Cryptography.MD5.Create().ComputeHash(fileContent).Select(b => b.ToString("x2")));
            if (fileContent.CanSeek)
            {
                fileContent.Seek(0, SeekOrigin.Begin);
            }
        }

        internal FileManagerClientAttachmentWithContent(Stream fileContent, string serverSideName) : base(serverSideName)
        {
            Content = fileContent;
        }

        internal FileManagerClientAttachmentWithContent(Stream fileContent, DateTime creationDate, string md5, ClientAttachmentTypes attachmentType, string fileExtention) : base(creationDate, md5, attachmentType, fileExtention)
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
