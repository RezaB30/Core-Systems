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
        public FileManagerResult<FileManagerBasicFile> GetContractMailBody(string culture)
        {
            culture = string.IsNullOrWhiteSpace(culture) ? string.Empty : $".{culture}";
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
            var fileResult = InternalFileManager.GetFile($"{PathRepository.MailContractBodyFile}{culture}.html");
            if (fileResult.InternalException != null)
            {
                if (!string.IsNullOrEmpty(culture))
                {
                    culture = string.Empty;
                    fileResult = InternalFileManager.GetFile($"{PathRepository.MailContractBodyFile}{culture}.html");
                    if (fileResult.InternalException != null)
                    {
                        return new FileManagerResult<FileManagerBasicFile>(fileResult.InternalException);
                    }
                }
                else
                {
                    return new FileManagerResult<FileManagerBasicFile>(fileResult.InternalException);
                }
            }

            return new FileManagerResult<FileManagerBasicFile>(new FileManagerBasicFile($"{PathRepository.MailContractBodyFile}{culture}.html", fileResult.Result));
        }

        public FileManagerResult<FileManagerBasicFile> GetContractMailBodyByName(string fileName)
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
            var fileResult = InternalFileManager.GetFile(fileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(fileResult.InternalException);
            }

            return new FileManagerResult<FileManagerBasicFile>(new FileManagerBasicFile(fileName, fileResult.Result));
        }

        public FileManagerResult<bool> SaveContractMailBody(FileManagerBasicFile file, string culture)
        {
            culture = string.IsNullOrWhiteSpace(culture) ? string.Empty : $".{culture}";
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.MailFiles.Concat(PathRepository.MailContractFiles));
            var result = CreateAndEnterPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<bool>(result.InternalException);
            }
            result = InternalFileManager.SaveFile($"{PathRepository.MailContractBodyFile}{culture}.html", file.Content, true);
            return result;
        }

        public FileManagerResult<bool> RemoveContractMailBody(string culture)
        {
            culture = string.IsNullOrWhiteSpace(culture) ? string.Empty : $".{culture}";
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
            result = InternalFileManager.RemoveFile($"{PathRepository.MailContractBodyFile}{culture}.html");
            return result;
        }

        public FileManagerResult<bool> RemoveContractMailBodyByName(string fileName)
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
            result = InternalFileManager.RemoveFile(fileName);
            return result;
        }

        public FileManagerResult<IEnumerable<string>> ListContractMailBodies()
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.MailFiles.Concat(PathRepository.MailContractFiles));
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<IEnumerable<string>>(result.InternalException);
            }
            else if (!result.Result)
            {
                return new FileManagerResult<IEnumerable<string>>(new InvalidOperationException("Path does not exist."));
            }
            var listResult = InternalFileManager.GetFileList();
            if (listResult.InternalException != null)
            {
                return new FileManagerResult<IEnumerable<string>>(listResult.InternalException);
            }
            else if (listResult.Result == null)
            {
                return new FileManagerResult<IEnumerable<string>>(Enumerable.Empty<string>());
            }
            return new FileManagerResult<IEnumerable<string>>(listResult.Result);
        }
    }
}
