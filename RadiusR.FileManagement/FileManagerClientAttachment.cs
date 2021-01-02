using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement
{
    public class FileManagerClientAttachment
    {

        public string ServerSideName
        {
            get
            {
                return $"{Enum.GetName(typeof(ClientAttachmentTypes), AttachmentType)}.{CreationDate.Ticks}.{RandomSegment}.{FileExtention}";
            }
        }

        public DateTime CreationDate { get; private set; }

        public ClientAttachmentTypes AttachmentType { get; private set; }

        public string FileExtention { get; private set; }

        public string RandomSegment { get; private set; }

        public FileManagerClientAttachment(ClientAttachmentTypes attachmentType, string fileExtention)
        {
            CreationDate = DateTime.Now;
            RandomSegment = Guid.NewGuid().ToString("N");
            AttachmentType = attachmentType;
            FileExtention = fileExtention;
        }

        internal FileManagerClientAttachment(DateTime creationDate, string randomSegment, ClientAttachmentTypes attachmentType, string fileExtention)
        {
            CreationDate = creationDate;
            RandomSegment = randomSegment;
            AttachmentType = attachmentType;
            FileExtention = fileExtention;
        }

        internal FileManagerClientAttachment(string serverSideName)
        {
            var parts = serverSideName.Split('.');
            AttachmentType = (ClientAttachmentTypes)Enum.Parse(typeof(ClientAttachmentTypes), parts[0]);
            CreationDate = new DateTime(Convert.ToInt64(parts[1]));
            RandomSegment = parts[2];
            FileExtention = string.Join(".", parts.Where((item, index) => index > 2));
        }

        public override string ToString()
        {
            return ServerSideName;
        }
    }
}
