using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Files
{
    public static partial class FileManager
    {
        public class FileInfo
        {
            public string Path { get; set; }

            public string Name { get; set; }

            public DateTime CreationDate { get; set; }

            public string FileType { get; set; }

            public string NakedName { get; set; }
        }
    }
}
