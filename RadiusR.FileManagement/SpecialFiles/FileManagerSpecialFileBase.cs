using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement.SpecialFiles
{
    public abstract class FileManagerSpecialFileBase
    {
        private readonly char[] InvalidCharacters = new char[] { '.', '/', '\\' };
        public const int MaxNameLength = 65;
        private string _name;

        protected FileManagerSpecialFileBase(string name, string fileExtention)
        {
            Name = CleanFileName(name);
            CreationDate = DateTime.Now;
            MD5 = string.Empty;
            FileExtention = fileExtention.ToLower();
        }

        protected FileManagerSpecialFileBase(string name, DateTime creationDate, string md5, string fileExtention)
        {
            Name = CleanFileName(name);
            CreationDate = creationDate;
            MD5 = md5;
            FileExtention = fileExtention.ToLower();
        }

        protected FileManagerSpecialFileBase(string serverSideName)
        {
            var parts = serverSideName.Split('.');
            Name = parts[0];
            CreationDate = new DateTime(Convert.ToInt64(parts[1]));
            MD5 = parts[2];
            FileExtention = string.Join(".", parts.Where((item, index) => index > 2));
        }

        protected string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = CleanFileName(value);
            }
        }

        public DateTime CreationDate { get; protected set; }

        public string MD5 { get; internal set; }

        public string FileExtention { get; protected set; }

        public string MIMEType
        {
            get
            {
                return MIMEUtility.GetMIMETypeFromFileExtention(FileExtention);
            }
        }

        public string ServerSideName
        {
            get
            {
                return $"{Name}.{CreationDate.Ticks}.{MD5}.{FileExtention}";
            }
        }

        private string CleanFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            return new string(fileName.ToArray().Take(MaxNameLength).Where(c => !InvalidCharacters.Contains(c)).ToArray());
        }

        public override string ToString()
        {
            return ServerSideName;
        }
    }
}
