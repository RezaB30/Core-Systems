using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Files.BTKLogging
{
    public static class BTKLogFileManager
    {
        public static void CreateLogFile(LogFileTypes logType, string fileContent, OperatorInfo operatorInfo, DateTime operationTime, int count = 1, string serviceInfrastructureType = null)
        {
            var fileName = string.Empty;
            switch (logType)
            {
                case LogFileTypes.ClientCatalog:
                    fileName = string.Format("{0}_{1}_{2}_ABONE_REHBER_{3}_{4}.abn.gz", operatorInfo.OperatorName, operatorInfo.OperatorCode, operatorInfo.OperatorType, operationTime.ToString("yyyyMMddHHmmss"), count.ToString("000"));
                    break;
                case LogFileTypes.ClientChanges:
                    fileName = string.Format("{0}_{1}_{2}_ABONE_HAREKET_{3}_{4}.abn.gz", operatorInfo.OperatorName, operatorInfo.OperatorCode, operatorInfo.OperatorType, operationTime.ToString("yyyyMMddHHmmss"), count.ToString("000"));
                    break;
                case LogFileTypes.IPDR:
                    fileName = string.Format("{0}_NAT_IPDR_{1}_{2}.log.gz", operatorInfo.OperatorName, operationTime.ToString("yyyyMMddHHmmss"), count.ToString());
                    break;
                case LogFileTypes.IPBlock:
                    fileName = string.Format("{0}_IPBLOK_{1}_{2}.log.gz", operatorInfo.OperatorName, operationTime.ToString("yyyyMMddHHmmss"), count.ToString("00"));
                    break;
                case LogFileTypes.Sessions:
                    fileName = string.Format("{0}-{1}_{2}_OTURUM_{3}_{4}.log.gz", operatorInfo.OperatorName, operatorInfo.Department.ToString("00"), serviceInfrastructureType, operationTime.ToString("yyyyMMddHHmmss"), count.ToString());
                    break;
                case LogFileTypes.ClientOld:
                    fileName = string.Format("{0}_ABONE_{1}.abn.gz", operatorInfo.OperatorName, operationTime.ToString("yyyyMMdd"));
                    break;
                default:
                    return;
            }

            string logPath = GetLogFolder(logType);
            using (var zipStream = CreateZipStream(fileContent))
            {
                FileManager.SaveFile(zipStream, logPath + fileName);
            }
        }

        public static IEnumerable<LogFileInfo> GetRecentLogFiles(LogFileTypes logType)
        {
            return FileManager.GetFolderFiles(GetLogFolder(logType)).Select(fi => new LogFileInfo()
            {
                FileName = fi.Name,
                PathWithName = fi.Path
            });
        }

        private static string GetLogFolder(LogFileTypes logType)
        {
            switch (logType)
            {
                case LogFileTypes.ClientCatalog:
                    return RadiusRFolders.BTKTempLogCatalog;
                case LogFileTypes.ClientChanges:
                    return RadiusRFolders.BTKTempLogChanges;
                case LogFileTypes.IPDR:
                    return RadiusRFolders.BTKTempLogIPDR;
                case LogFileTypes.IPBlock:
                    return RadiusRFolders.BTKTempLogIPBlock;
                case LogFileTypes.Sessions:
                    return RadiusRFolders.BTKTempLogSessions;
                case LogFileTypes.ClientOld:
                    return RadiusRFolders.BTKTempLogClientOld;
                default:
                    return null;
            }
        }

        private static void CreateZipFile(string path, string fileName, string fileContents)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var encoding = Encoding.GetEncoding("ISO-8859-9");
                var buffer = encoding.GetBytes(fileContents);
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                using (FileStream compressionStream = File.Create(path + "\\" + fileName + ".gz"))
                {
                    using (GZipStream zipStream = new GZipStream(compressionStream, CompressionMode.Compress))
                    {
                        stream.CopyTo(zipStream);
                    }
                }
            }
        }

        private static Stream CreateZipStream(string contents)
        {
            using (MemoryStream tempStream = new MemoryStream())
            {
                var encoding = Encoding.GetEncoding("ISO-8859-9");
                var buffer = encoding.GetBytes(contents);
                tempStream.Write(buffer, 0, buffer.Length);
                tempStream.Seek(0, SeekOrigin.Begin);

                MemoryStream results = new MemoryStream();
                using (GZipStream zipStream = new GZipStream(results, CompressionMode.Compress, true))
                {
                    tempStream.CopyTo(zipStream);
                }

                return results;
            }
        }

        public static void ClearTempFolder(LogFileTypes logType)
        {
            FileManager.ClearRepositoryFolder(GetLogFolder(logType));
        }

        public class LogFileInfo
        {
            public string FileName { get; set; }

            public string PathWithName { get; set; }
        }
    }
}
