using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public static class FileHashUtility
    {
        public static string GetMD5Hash(Stream content)
        {
            if (content.CanSeek)
            {
                content.Seek(0, SeekOrigin.Begin);
            }
            var MD5 = string.Join(string.Empty, System.Security.Cryptography.MD5.Create().ComputeHash(content).Select(b => b.ToString("x2")));
            if (content.CanSeek)
            {
                content.Seek(0, SeekOrigin.Begin);
            }

            return MD5;
        }
    }
}
