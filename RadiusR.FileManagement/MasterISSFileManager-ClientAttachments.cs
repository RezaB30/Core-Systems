﻿using RezaB.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement
{
    public partial class MasterISSFileManager
    {
        private string GetClientAttachmentsPath(long subscriberId)
        {
            var upperStage = subscriberId / 1000;
            //var lowerStage = subscriberId % 1000;
            var upperPath = $"{((upperStage * 1000) + 1):0000000}-{((upperStage + 1) * 1000):0000000}";
            var lowerPath = $"{subscriberId:#0000}";
            var resultPathParts = PathRepository.ClientAttachments.Concat(new string[] { upperPath, lowerPath });
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
            result = InternalFileManager.SaveFile(attachment.ServerSideName, attachment.Content);
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

            return new FileManagerResult<FileManagerClientAttachmentWithContent>(new FileManagerClientAttachmentWithContent(fileResult.Result, fileName));
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
