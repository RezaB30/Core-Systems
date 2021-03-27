using RezaB.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.FileManagement.SpecialFiles;

namespace RadiusR.FileManagement
{
    public partial class MasterISSFileManager
    {
        private string GetClientAttachmentsPath(long subscriberId)
        {
            var resultPathParts = PathRepository.ClientAttachments.Concat(GetIdPathPartition(subscriberId));
            var resulPath = string.Join(InternalFileManager.PathSeparator, resultPathParts);

            return resulPath;
        }

        public FileManagerResult<IEnumerable<FileManagerClientAttachment>> GetClientAttachmentsList(long subscriberId)
        {
            var searchPath = GetClientAttachmentsPath(subscriberId);
            InternalFileManager.GoToRootDirectory();
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (!result.Result)
            {
                return new FileManagerResult<IEnumerable<FileManagerClientAttachment>>(Enumerable.Empty<FileManagerClientAttachment>(), result.InternalException);
            }
            var fileListResult = InternalFileManager.GetFileList();
            if (fileListResult.InternalException != null)
            {
                return new FileManagerResult<IEnumerable<FileManagerClientAttachment>>(null, fileListResult.InternalException);
            }
            else if (fileListResult.Result != null)
            {
                return new FileManagerResult<IEnumerable<FileManagerClientAttachment>>(fileListResult.Result.Select(fileName => new FileManagerClientAttachment(fileName)));
            }
            return new FileManagerResult<IEnumerable<FileManagerClientAttachment>>(Enumerable.Empty<FileManagerClientAttachment>());
        }

        public FileManagerResult<bool> SaveClientAttachment(long subscriberId, FileManagerClientAttachmentWithContent attachment)
        {
            var destinationPath = GetClientAttachmentsPath(subscriberId);
            InternalFileManager.GoToRootDirectory();
            var result = CreateAndEnterPath(destinationPath);
            if (!result.Result)
            {
                return result;
            }
            // check file hash to prevent dupes
            var listResult = InternalFileManager.GetFileList();
            if (listResult.InternalException != null)
            {
                return new FileManagerResult<bool>(listResult.InternalException);
            }
            if (listResult.Result != null && listResult.Result.Any(fileName => fileName.Contains($".{attachment.FileDetail.MD5}")))
            {
                return new FileManagerResult<bool>(true);
            }
            // save file
            result = InternalFileManager.SaveFile(attachment.FileDetail.ServerSideName, attachment.Content);
            return result;
        }

        public FileManagerResult<FileManagerClientAttachmentWithContent> GetClientAttachment(long subscriberId, string fileName)
        {
            var destinationPath = GetClientAttachmentsPath(subscriberId);
            InternalFileManager.GoToRootDirectory();
            var result = InternalFileManager.EnterDirectoryPath(destinationPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<FileManagerClientAttachmentWithContent>(result.InternalException);
            }
            else if (!result.Result)
            {
                return new FileManagerResult<FileManagerClientAttachmentWithContent>(new InvalidOperationException("File not found!"));
            }

            var fileResult = InternalFileManager.GetFile(fileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerClientAttachmentWithContent>(fileResult.InternalException);
            }

            return new FileManagerResult<FileManagerClientAttachmentWithContent>(new FileManagerClientAttachmentWithContent(fileResult.Result, new FileManagerClientAttachment(fileName)));
        }

        public FileManagerResult<bool> RemoveClientAttachment(long subscriberId, string fileName)
        {
            var destinationPath = GetClientAttachmentsPath(subscriberId);
            InternalFileManager.GoToRootDirectory();
            var result = InternalFileManager.EnterDirectoryPath(destinationPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<bool>(result.InternalException);
            }
            else if (!result.Result)
            {
                return new FileManagerResult<bool>(new InvalidOperationException("File not found!"));
            }

            var fileResult = InternalFileManager.RemoveFile(fileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<bool>(fileResult.InternalException);
            }

            return new FileManagerResult<bool>(true);
        }
    }
}
