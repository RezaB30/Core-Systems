using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement
{
    public class FileManagerBasicFile : IDisposable
    {
        public string FileName { get; set; }

        public string FileExtention
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                    return string.Empty;
                return FileName.Split('.').LastOrDefault() ?? string.Empty;
            }
        }

        public Stream Content { get; set; }

        public FileManagerBasicFile(string fileName, Stream content)
        {
            FileName = fileName;
            Content = content;
        }

        public void Dispose()
        {
            Content.Dispose();
        }
    }
}
