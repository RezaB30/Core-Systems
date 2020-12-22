using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Files
{
    /// <summary>
    /// Manages files for all RadiusR apps.
    /// </summary>
    public static partial class FileManager
    {
        private static string root
        {
            get
            {
                var _root = Environment.GetEnvironmentVariable("RadiusR_Repo", EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(_root))
                    throw new Exception("Environement variable not set!");
                return _root;
            }
        }

        public static string SaveClientAttachment(Stream source, long clientId, string fileType)
        {
            var filePath = GetNewAttachmentPath(clientId, fileType);
            SaveFile(source, filePath);
            return filePath.Substring(filePath.LastIndexOf('\\') + 1);
        }

        public static void SavePDFFormTemplate(Stream source, int formType, string fileType)
        {
            var filePath = GetPDFTemplatePath(formType, fileType);
            SaveFile(source, filePath);
        }

        public static void SaveFile(Stream source, string path)
        {
            path = root + "\\" + path;
            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
            using (var file = File.Create(path))
            {
                source.Seek(0, SeekOrigin.Begin);
                source.CopyTo(file);
                file.Flush();
            }
        }

        public static IEnumerable<FileInfo> GetClientFilePaths(long clientId)
        {
            var folderPath = string.Format("{0}{1}\\", RadiusRFolders.ClientAttachments, clientId);
            return GetFolderFiles(folderPath);
        }

        public static Stream GetClientAttachment(long clientId, string fileName)
        {
            var filePath = string.Format("{0}{1}\\{2}", RadiusRFolders.ClientAttachments, clientId, fileName);
            return GetRepositoryFile(filePath);
        }

        public static Stream GetPDFTemplate(int formType)
        {
            var filePath = GetPDFTemplatePath(formType);
            if (filePath == null)
                return null;
            return GetRepositoryFile(filePath);

        }

        public static bool ContractAppendixExists()
        {
            var fullPath = root + "\\" + RadiusRFolders.PDFFormTemplates + RadiusRFolders.ContractAppendixFileName;
            return File.Exists(fullPath);
        }

        public static void SaveContractAppendix(Stream pdfStream)
        {
            SaveFile(pdfStream, RadiusRFolders.PDFFormTemplates + RadiusRFolders.ContractAppendixFileName);
        }

        public static Stream GetContractAppendix()
        {
            return GetRepositoryFile(RadiusRFolders.PDFFormTemplates + RadiusRFolders.ContractAppendixFileName);
        }

        public static void RemoveContractAppendix()
        {
            if (ContractAppendixExists())
            {
                var fullpath = root + "\\" + RadiusRFolders.PDFFormTemplates + RadiusRFolders.ContractAppendixFileName;
                File.Delete(fullpath);
            }
        }

        public static bool PDFTemplateExists(int formType)
        {
            var filePath = GetPDFTemplatePath(formType);
            return filePath != null;
        }

        public static void DeleteClientAttachment(long clientId, string fileName)
        {
            var filePath = string.Format("{0}{1}\\{2}", RadiusRFolders.ClientAttachments, clientId, fileName);
            File.Delete(string.Format("{0}\\{1}", root, filePath));
        }

        public static void DeletePDFTemplate(int formType)
        {
            var filePath = GetPDFTemplatePath(formType);
            if (filePath != null)
                File.Delete(filePath);
        }

        public static Stream GetRepositoryFile(string repositoryPath)
        {
            if (repositoryPath.StartsWith("\\"))
                repositoryPath = repositoryPath.Substring(1);
            var fullPath = string.Format("{0}\\{1}", root, repositoryPath);
            return File.Exists(fullPath) ? File.OpenRead(fullPath) : null;
        }

        public static IEnumerable<FileInfo> GetFolderFiles(string folderPath)
        {
            var fullFolderPath = root + "\\" + folderPath;
            DirectoryInfo dir = new DirectoryInfo(fullFolderPath);
            if (!dir.Exists)
                return Enumerable.Empty<FileInfo>();
            var files = dir.GetFiles();
            return files.Select(file => new FileInfo()
            {
                Name = file.Name,
                CreationDate = file.CreationTime,
                Path = folderPath + file.Name,
                FileType = file.Extension.Replace(".", ""),
                NakedName = file.Name.Substring(0, file.Name.LastIndexOf('.') >= 0 ? file.Name.LastIndexOf('.') : file.Name.Length)
            });
        }

        private static string GetNewAttachmentPath(long clientId, string fileType)
        {
            return string.Format("\\{0}\\{1}\\{2}.{3}", RadiusRFolders.ClientAttachments, clientId, Guid.NewGuid().ToString(), fileType);
        }

        private static string GetPDFTemplatePath(int formType, string fileType)
        {
            return string.Format("\\{0}{1}.{2}", RadiusRFolders.PDFFormTemplates, formType, fileType);
        }

        private static string GetPDFTemplatePath(int formType)
        {
            var folderPath = string.Format("{0}", RadiusRFolders.PDFFormTemplates);
            var targetFile = GetFolderFiles(folderPath).FirstOrDefault(f => f.NakedName == formType.ToString());
            return targetFile == null ? null : targetFile.Path;
        }

        public static IEnumerable<FileInfo> GetContractMailBodies()
        {
            return GetFolderFiles(RadiusRFolders.MailContarctFiles);
        }

        public static Stream GetContractMailBodyByCulture(string culture = null)
        {
            var suffix = string.IsNullOrEmpty(culture) ? ".html" : $".{culture}.html";
            var fullPath = $"{RadiusRFolders.MailContarctFiles}{RadiusRFolders.MailContractFileName}{suffix}";
            var results = GetRepositoryFile(fullPath);
            if (results == null && culture != null)
            {
                return GetContractMailBodyByCulture();
            }
            return results;
        }

        public static Stream GetContractMailBodyByFileName(string fileName)
        {
            var fullPath = $"{RadiusRFolders.MailContarctFiles}{fileName}";
            return GetRepositoryFile(fullPath);
        }

        public static void SaveContractMailBody(Stream htmlFileStream, string culture)
        {
            var suffix = string.IsNullOrEmpty(culture) ? ".html" : $".{culture}.html";
            var fullPath = $"{RadiusRFolders.MailContarctFiles}{RadiusRFolders.MailContractFileName}{suffix}";
            SaveFile(htmlFileStream, fullPath);
        }

        public static void DeleteContractMailBody(string fileName)
        {
            var fullPath = $"{root}\\{RadiusRFolders.MailContarctFiles}{fileName}";
            File.Delete(fullPath);
        }

        internal static void ClearRepositoryFolder(string folderPath)
        {
            var fullFolderPath = root + "\\" + folderPath;
            DirectoryInfo dir = new DirectoryInfo(fullFolderPath);
            if (!dir.Exists)
                return;
            dir.Delete(true);
            Directory.CreateDirectory(fullFolderPath);
        }
    }
}
