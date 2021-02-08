using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RadiusR.DB;
using RadiusR.DB.Enums;

namespace RadiusR.FileManagement.BTKLogging
{
    public static class BTKLogUtilities
    {
        private static readonly Regex fileDateRegex = new Regex(@"(?<=_ABONE_REHBER_|_ABONE_HAREKET_|_NAT_IPDR_|_IPBLOK_|_OTURUM_|_ABONE_)\d+", RegexOptions.Compiled);

        public static string GetLogFileName(BTKLogTypes logType, DateTime operationTime, int index = 1, string serviceInfrastructureType = null)
        {
            switch (logType)
            {
                case BTKLogTypes.ClientCatalog:
                    return string.Format("{0}_{1}_{2}_ABONE_REHBER_{3}_{4}.abn.gz", BTKSettings.BTKOperatorName, BTKSettings.BTKOperatorCode, BTKSettings.BTKOperatorType, operationTime.ToString("yyyyMMddHHmmss"), index.ToString("000"));
                case BTKLogTypes.ClientChanges:
                    return string.Format("{0}_{1}_{2}_ABONE_HAREKET_{3}_{4}.abn.gz", BTKSettings.BTKOperatorName, BTKSettings.BTKOperatorCode, BTKSettings.BTKOperatorType, operationTime.ToString("yyyyMMddHHmmss"), index.ToString("000"));
                case BTKLogTypes.IPDR:
                    return string.Format("{0}_NAT_IPDR_{1}_{2}.log.gz", BTKSettings.BTKOperatorName, operationTime.ToString("yyyyMMddHHmmss"), index.ToString());
                case BTKLogTypes.IPBlock:
                    return string.Format("{0}_IPBLOK_{1}_{2}.log.gz", BTKSettings.BTKOperatorName, operationTime.ToString("yyyyMMddHHmmss"), index.ToString("00"));
                case BTKLogTypes.Sessions:
                    return string.Format("{0}-{1}_{2}_OTURUM_{3}_{4}.log.gz", BTKSettings.BTKOperatorName, BTKSettings.BTKOperatorDepartment.ToString("00"), serviceInfrastructureType, operationTime.ToString("yyyyMMddHHmmss"), index.ToString());
                case BTKLogTypes.ClientOld:
                    return string.Format("{0}_ABONE_{1}.abn.gz", BTKSettings.BTKOperatorName, operationTime.ToString("yyyyMMdd"));
                default:
                    return null;
            }
        }

        public static DateTime? GetDateTimeFromFileName(string serverSideName)
        {
            var dateMatch = fileDateRegex.Match(serverSideName);
            if (dateMatch == null)
                return null;
            DateTime result;
            if (DateTime.TryParseExact(dateMatch.Value, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }
            else if (DateTime.TryParseExact(dateMatch.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        public static Stream CreateZipStream(string content, Encoding encoding)
        {
            using (MemoryStream tempStream = new MemoryStream())
            {
                var buffer = encoding.GetBytes(content);
                tempStream.Write(buffer, 0, buffer.Length);
                tempStream.Seek(0, SeekOrigin.Begin);

                MemoryStream results = new MemoryStream();
                using (GZipStream zipStream = new GZipStream(results, CompressionMode.Compress, true))
                {
                    tempStream.CopyTo(zipStream);
                }

                results.Seek(0, SeekOrigin.Begin);
                return results;
            }
        }
    }
}
