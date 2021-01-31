using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.BTKLogging
{
    public class BTKLogFile
    {
        public string Content { get; private set; }

        public RadiusR.DB.Enums.BTKLogTypes LogType { get; private set; }

        public DateTime FileDate { get; private set; }

        public int FileIndex { get; private set; }

        public string ServiceInfrastructureType { get; private set; }

        public string ServerSideName
        {
            get
            {
                return BTKLogging.BTKLogUtilities.GetLogFileName(LogType, FileDate, FileIndex, ServiceInfrastructureType);
            }
        }

        public BTKLogFile(string content, RadiusR.DB.Enums.BTKLogTypes logType, DateTime fileDate, int fileIndex = 1, string serviceInfrastructureType = null)
        {
            Content = content;
            LogType = logType;
            FileDate = fileDate;
            FileIndex = fileIndex;
            ServiceInfrastructureType = serviceInfrastructureType;
        }
    }
}
