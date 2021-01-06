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
                return $"{Enum.GetName(typeof(ClientAttachmentTypes), AttachmentType)}.{CreationDate.Ticks}.{MD5}.{FileExtention}";
            }
        }

        public DateTime CreationDate { get; private set; }

        public ClientAttachmentTypes AttachmentType { get; private set; }

        public string FileExtention { get; private set; }

        public string MD5 { get; protected set; }

        public string MIMEType
        {
            get
            {
                return MIMEUtility.GetMIMETypeFromFileExtention(FileExtention);
            }
        }

        public FileManagerClientAttachment(ClientAttachmentTypes attachmentType, string fileExtention)
        {
            CreationDate = DateTime.Now;
            MD5 = string.Empty;//Guid.NewGuid().ToString("N");
            AttachmentType = attachmentType;
            FileExtention = fileExtention;
        }

        internal FileManagerClientAttachment(DateTime creationDate, string md5, ClientAttachmentTypes attachmentType, string fileExtention)
        {
            CreationDate = creationDate;
            MD5 = md5;
            AttachmentType = attachmentType;
            FileExtention = fileExtention;
        }

        internal FileManagerClientAttachment(string serverSideName)
        {
            var parts = serverSideName.Split('.');
            ClientAttachmentTypes attachmentType;
            if (Enum.TryParse(parts[0], false, out attachmentType))
            {
                AttachmentType = attachmentType;
            }
            else
            {
                AttachmentType = ClientAttachmentTypes.Others;
            }
            CreationDate = new DateTime(Convert.ToInt64(parts[1]));
            MD5 = parts[2];
            FileExtention = string.Join(".", parts.Where((item, index) => index > 2));
        }

        public override string ToString()
        {
            return ServerSideName;
        }
    }
}
