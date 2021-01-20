using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public class FileManagerSupportRequestAttachmentWithContent : FileManagerSpecialFileWithContent<FileManagerSupportRequestAttachment>
    {
        public FileManagerSupportRequestAttachmentWithContent(Stream content, FileManagerSupportRequestAttachment fileDetails) : base(content, fileDetails) { }
    }
}
