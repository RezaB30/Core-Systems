using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public class FileManagerClientAttachment : FileManagerSpecialFileBase
    {

        //public string ServerSideName
        //{
        //    get
        //    {
        //        return $"{Enum.GetName(typeof(ClientAttachmentTypes), AttachmentType)}.{CreationDate.Ticks}.{MD5}.{FileExtention}";
        //    }
        //}

        //public DateTime CreationDate { get; private set; }

        public ClientAttachmentTypes AttachmentType
        {
            get
            {
                return (ClientAttachmentTypes)Enum.Parse(typeof(ClientAttachmentTypes), Name);
            }
            protected set
            {
                Name = Enum.GetName(typeof(ClientAttachmentTypes), value);
            }
        }

        //public string FileExtention { get; private set; }

        //public string MD5 { get; protected set; }

        //public string MIMEType
        //{
        //    get
        //    {
        //        return MIMEUtility.GetMIMETypeFromFileExtention(FileExtention);
        //    }
        //}

        public FileManagerClientAttachment(ClientAttachmentTypes attachmentType, string fileExtention) : base(null, fileExtention)
        {
            AttachmentType = attachmentType;
        }

        internal FileManagerClientAttachment(DateTime creationDate, string md5, ClientAttachmentTypes attachmentType, string fileExtention) : base(null, creationDate, md5, fileExtention)
        {
            AttachmentType = attachmentType;
        }

        internal FileManagerClientAttachment(string serverSideName) : base(serverSideName) { }
    }
}
