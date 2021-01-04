using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Files;

namespace RadiusR.FileManagement
{
    public partial class MasterISSFileManager
    {
        public FileManagerResult<FileManagerBasicFile> GetContractMailBody()
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.MailFiles.Concat(PathRepository.MailContractFiles));
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(result.InternalException);
            }
            else if (!result.Result)
            {
                return new FileManagerResult<FileManagerBasicFile>(new InvalidOperationException("Path does not exist."));
            }
            var fileResult = InternalFileManager.GetFile(PathRepository.MailContractBodyFile);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(fileResult.InternalException);
            }

            return new FileManagerResult<FileManagerBasicFile>(new FileManagerBasicFile(PathRepository.MailContractBodyFile, fileResult.Result));
        }

        public FileManagerResult<bool> SaveContractMailBody(FileManagerBasicFile file)
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.MailFiles.Concat(PathRepository.MailContractFiles));
            var result = CreateAndEnterPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<bool>(result.InternalException);
            }
            result = InternalFileManager.SaveFile(PathRepository.MailContractBodyFile, file.Content, true);
            return result;
        }

        public FileManagerResult<bool> RemoveContractMailBody()
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.MailFiles.Concat(PathRepository.MailContractFiles));
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<bool>(result.InternalException);
            }
            else if (!result.Result)
            {
                return new FileManagerResult<bool>(new InvalidOperationException("Path does not exist."));
            }
            result = InternalFileManager.RemoveFile(PathRepository.MailContractBodyFile);
            return result;
        }
    }
}
