using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public class FileManagerSupportRequestAttachment : FileManagerSpecialFileBase
    {
        public long StageId
        {
            get
            {
                return Convert.ToInt64(Name.Split('-').LastOrDefault());
            }
        }

        public string FileName
        {
            get
            {
                return Name.Split('-').FirstOrDefault();
            }
        }

        public FileManagerSupportRequestAttachment(string serverSideName) : base(serverSideName) { }

        public FileManagerSupportRequestAttachment(long stageId, string fileName, string fileExtention) : base(null, fileExtention)
        {
            fileName = new string(fileName.ToCharArray().Where(c => c != '-').ToArray());
            Name = $"{fileName}-{stageId}";
        }

        public FileManagerSupportRequestAttachment(long stageId, string fileName, DateTime creationDate, string md5, string fileExtention) : base(null, creationDate, md5, fileExtention)
        {
            fileName = new string(fileName.ToCharArray().Where(c => c != '-').ToArray());
            Name = $"{fileName}-{stageId}";
        }
    }
}
