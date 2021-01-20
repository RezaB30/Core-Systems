using RezaB.Files;
using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Files.Local;
using RezaB.Files.FTP;

namespace RadiusR.FileManagement
{
    public partial class MasterISSFileManager
    {
        private IFileManager InternalFileManager { get; set; }

        public MasterISSFileManager()
        {
            switch (FileManagerSettings.FileManagerType)
            {
                case DB.Enums.FileManagerTypes.Local:
                    {
                        var _root = Environment.GetEnvironmentVariable("RadiusR_Repo", EnvironmentVariableTarget.Machine);
                        if (string.IsNullOrEmpty(_root))
                            throw new Exception("Environement variable not set!");
                        InternalFileManager = new LocalFileManager(_root);
                    }
                    break;
                case DB.Enums.FileManagerTypes.Remote:
                    {
                        InternalFileManager = FTPClientFactory.CreateFTPClient(FileManagerSettings.FileManagerHost, FileManagerSettings.FileManagerUsername, FileManagerSettings.FileManagerPassword);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Invalid file manager type. ({FileManagerSettings.FileManagerType})");
            }
        }

        private FileManagerResult<bool> CreateAndEnterPath(string path)
        {
            InternalFileManager.GoToRootDirectory();
            var results = InternalFileManager.DirectoryExists(path);
            if (!results.Result)
            {
                results = InternalFileManager.CreateDirectory(path);
                if (!results.Result)
                {
                    return results;
                }
            }
            results = InternalFileManager.EnterDirectoryPath(path);
            return results;
        }

        private IEnumerable<string> GetIdPathPartition(long id)
        {
            var upperStage = id / 1000;
            var upperPath = $"{((upperStage * 1000) + 1):0000000}-{((upperStage + 1) * 1000):0000000}";
            var lowerPath = $"{id:#0000}";
            return new string[] { upperPath, lowerPath };
        }
    }
}
