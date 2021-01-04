using RezaB.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement
{
    public partial class MasterISSFileManager
    {
        public FileManagerResult<FileManagerBasicFile> GetPDFForm(RadiusR.DB.Enums.PDFFormType pdfFormType)
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.PDFForms);
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(result.InternalException);
            }
            var fileName = Enum.GetName(typeof(RadiusR.DB.Enums.PDFFormType), pdfFormType);
            var listResult = InternalFileManager.GetFileList();
            if (listResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(listResult.InternalException);
            }
            else if (listResult.Result == null || listResult.Result.Where(fileNameOnServer => fileNameOnServer.StartsWith($"{fileName}.")).Count() != 1)
            {
                return new FileManagerResult<FileManagerBasicFile>(new NotSupportedException("File not found!"));
            }
            var foundFileName = listResult.Result.FirstOrDefault(fileNameOnServer => fileNameOnServer.StartsWith($"{fileName}."));
            var fileResult = InternalFileManager.GetFile(foundFileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(result.InternalException);
            }
            return new FileManagerResult<FileManagerBasicFile>(new FileManagerBasicFile(foundFileName, fileResult.Result));
        }

        public FileManagerResult<bool> RemovePDFForm(RadiusR.DB.Enums.PDFFormType pdfFormType)
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.PDFForms);
            var result = CreateAndEnterPath(searchPath);
            if (result.InternalException != null)
            {
                return result;
            }
            var fileName = Enum.GetName(typeof(RadiusR.DB.Enums.PDFFormType), pdfFormType);
            var listResult = InternalFileManager.GetFileList();
            if (listResult.InternalException != null)
            {
                return new FileManagerResult<bool>(listResult.InternalException);
            }
            else if (listResult.Result == null || listResult.Result.Where(fileNameOnServer => fileNameOnServer.StartsWith($"{fileName}.")).Count() < 1)
            {
                return new FileManagerResult<bool>(new NotSupportedException("File not found!"));
            }
            var foundFileNames = listResult.Result.Where(fileNameOnServer => fileNameOnServer.StartsWith($"{fileName}.")).ToArray();
            foreach (var foundFileName in foundFileNames)
            {
                result = InternalFileManager.RemoveFile(foundFileName);
                if (result.InternalException != null)
                {
                    return result;
                }
            }
            return result;
        }

        public FileManagerResult<bool> SavePDFForm(RadiusR.DB.Enums.PDFFormType pdfFormType, FileManagerBasicFile file)
        {
            RemovePDFForm(pdfFormType);
            var result = InternalFileManager.SaveFile($"{Enum.GetName(typeof(RadiusR.DB.Enums.PDFFormType), pdfFormType)}.{file.FileExtention}", file.Content, true);
            return result;
        }

        public FileManagerResult<FileManagerBasicFile> GetContractAppendix()
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.PDFForms);
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(result.InternalException);
            }
            var fileResult = InternalFileManager.GetFile(PathRepository.ContractAppendixFileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(fileResult.InternalException);
            }
            return new FileManagerResult<FileManagerBasicFile>(new FileManagerBasicFile(PathRepository.ContractAppendixFileName, fileResult.Result));
        }

        public FileManagerResult<bool> SaveContractAppendix(FileManagerBasicFile file)
        {
            if (file.FileExtention.ToLower() != "pdf")
                return new FileManagerResult<bool>(false, new InvalidDataException("Only pdf files are acceptable."));
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.PDFForms);
            var result = CreateAndEnterPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<bool>(result.InternalException);
            }
            result = InternalFileManager.SaveFile(PathRepository.ContractAppendixFileName, file.Content, true);
            return result;
        }

        public FileManagerResult<bool> RemoveContractAppendix()
        {
            InternalFileManager.GoToRootDirectory();
            var searchPath = string.Join(InternalFileManager.PathSeparator, PathRepository.PDFForms);
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<bool>(result.InternalException);
            }
            result = InternalFileManager.RemoveFile(PathRepository.ContractAppendixFileName);
            return result;
        }
    }
}
