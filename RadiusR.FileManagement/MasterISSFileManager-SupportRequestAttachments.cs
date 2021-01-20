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
        private string GetSupportRequestAttachmentsPath(long supportRequestId)
        {
            var resultPathParts = PathRepository.SupportRequestAttachments.Concat(GetIdPathPartition(supportRequestId));
            var resulPath = string.Join(InternalFileManager.PathSeparator, resultPathParts);

            return resulPath;
        }

        public FileManagerResult<IEnumerable<FileManagerSupportRequestAttachment>> GetSupportRequestAttachmentList(long supportRequestId)
        {
            var searchPath = GetSupportRequestAttachmentsPath(supportRequestId);
            InternalFileManager.GoToRootDirectory();
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (!result.Result)
            {
                return new FileManagerResult<IEnumerable<FileManagerSupportRequestAttachment>>(Enumerable.Empty<FileManagerSupportRequestAttachment>(), result.InternalException);
            }
            var fileListResult = InternalFileManager.GetFileList();
            if (fileListResult.InternalException != null)
            {
                return new FileManagerResult<IEnumerable<FileManagerSupportRequestAttachment>>(null, fileListResult.InternalException);
            }
            else if (fileListResult.Result != null)
            {
                return new FileManagerResult<IEnumerable<FileManagerSupportRequestAttachment>>(fileListResult.Result.Select(fileName => new FileManagerSupportRequestAttachment(fileName)));
            }
            return new FileManagerResult<IEnumerable<FileManagerSupportRequestAttachment>>(Enumerable.Empty<FileManagerSupportRequestAttachment>());
        }

        public FileManagerResult<bool> SaveSupportRequestAttachment(long supportRequestId, FileManagerSupportRequestAttachmentWithContent attachment)
        {
            var destinationPath = GetSupportRequestAttachmentsPath(supportRequestId);
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

        public FileManagerResult<FileManagerSupportRequestAttachmentWithContent> GetSupportRequestAttachment(long supportRequestId, string fileName)
        {
            var destinationPath = GetSupportRequestAttachmentsPath(supportRequestId);
            InternalFileManager.GoToRootDirectory();
            var result = InternalFileManager.EnterDirectoryPath(destinationPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<FileManagerSupportRequestAttachmentWithContent>(result.InternalException);
            }
            else if (!result.Result)
            {
                return new FileManagerResult<FileManagerSupportRequestAttachmentWithContent>(new InvalidOperationException("File not found!"));
            }

            var fileResult = InternalFileManager.GetFile(fileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerSupportRequestAttachmentWithContent>(fileResult.InternalException);
            }

            return new FileManagerResult<FileManagerSupportRequestAttachmentWithContent>(new FileManagerSupportRequestAttachmentWithContent(fileResult.Result, new FileManagerSupportRequestAttachment(fileName)));
        }

        public FileManagerResult<bool> RemoveSupportRequestAttachment(long supportRequestId, string fileName)
        {
            var destinationPath = GetSupportRequestAttachmentsPath(supportRequestId);
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
