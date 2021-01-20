using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public class FileManagerSpecialFileWithContent<T> : IDisposable where T : FileManagerSpecialFileBase
    {
        public T FileDetail { get; private set; }

        public Stream Content { get; private set; }

        public FileManagerSpecialFileWithContent(Stream content, T fileDetails)
        {
            Content = content;
            FileDetail = fileDetails;
            FileDetail.MD5 = FileHashUtility.GetMD5Hash(content);
        }

        public void Dispose()
        {
            if (Content != null)
                Content.Dispose();
        }
    }
}
