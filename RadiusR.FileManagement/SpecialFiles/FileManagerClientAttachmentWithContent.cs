using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public class FileManagerClientAttachmentWithContent : FileManagerSpecialFileWithContent<FileManagerClientAttachment>
    {
        public FileManagerClientAttachmentWithContent(Stream content, FileManagerClientAttachment fileDetails) : base(content, fileDetails) { }
    }
}
