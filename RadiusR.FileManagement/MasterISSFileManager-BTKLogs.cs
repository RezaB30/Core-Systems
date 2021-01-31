using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.FileManagement.BTKLogging;
using RezaB.Files;

namespace RadiusR.FileManagement
{
    public partial class MasterISSFileManager
    {
        private string GetBTKLogPath(RadiusR.DB.Enums.BTKLogTypes logType, DateTime date)
        {
            return string.Join(InternalFileManager.PathSeparator, PathRepository.BTKLogs.Concat(new string[]
            {
                Enum.GetName(typeof(RadiusR.DB.Enums.BTKLogTypes), logType),
                date.Year.ToString("0000"),
                date.Month.ToString("00"),
                date.Day.ToString("00")
            }));
        }

        public FileManagerResult<bool> SaveBTKLogFile(BTKLogFile file)
        {
            var searchPath = GetBTKLogPath(file.LogType, file.FileDate);
            InternalFileManager.GoToRootDirectory();
            var result = CreateAndEnterPath(searchPath);
            if (result.InternalException != null)
            {
                return result;
            }
            result = InternalFileManager.SaveFile(file.ServerSideName, BTKLogging.BTKLogUtilities.CreateZipStream(file.Content, Encoding.GetEncoding("ISO-8859-9")), true);
            return result;
        }

        public FileManagerResult<IEnumerable<LogFileWithDate>> ListBTKLogs(RadiusR.DB.Enums.BTKLogTypes logType, DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                return new FileManagerResult<IEnumerable<LogFileWithDate>>(Enumerable.Empty<LogFileWithDate>());
            }
            var currentDate = startDate.Date;
            var finalResult = new List<LogFileWithDate>();
            while (currentDate <= endDate.Date)
            {
                var searchPath = GetBTKLogPath(logType, currentDate);
                InternalFileManager.GoToRootDirectory();
                var result = InternalFileManager.EnterDirectoryPath(searchPath);
                if (result.InternalException != null)
                {
                    return new FileManagerResult<IEnumerable<LogFileWithDate>>(result.InternalException);
                }
                var listResult = InternalFileManager.GetFileList();
                if (listResult.InternalException != null)
                {
                    return new FileManagerResult<IEnumerable<LogFileWithDate>>(listResult.InternalException);
                }
                var filteredNames = listResult.Result.Select(fn => new LogFileWithDate() { FileName = fn, BTKDate = BTKLogging.BTKLogUtilities.GetDateTimeFromFileName(fn) }).Where(fn => fn.BTKDate >= startDate && fn.BTKDate < endDate).OrderBy(fn => fn.BTKDate).ToArray();
                finalResult.AddRange(filteredNames);

                currentDate = currentDate.AddDays(1).Date;
            }

            return new FileManagerResult<IEnumerable<LogFileWithDate>>(finalResult.ToArray());
        }

        public FileManagerResult<FileManagerBasicFile> GetBTKLog(RadiusR.DB.Enums.BTKLogTypes logType, DateTime date, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return new FileManagerResult<FileManagerBasicFile>(new InvalidOperationException("fileName can not be empty!"));
            }
            var searchPath = GetBTKLogPath(logType, date);
            InternalFileManager.GoToRootDirectory();
            var result = InternalFileManager.EnterDirectoryPath(searchPath);
            if (result.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(result.InternalException);
            }
            var fileResult = InternalFileManager.GetFile(fileName);
            if (fileResult.InternalException != null)
            {
                return new FileManagerResult<FileManagerBasicFile>(fileResult.InternalException);
            }
            return new FileManagerResult<FileManagerBasicFile>(new FileManagerBasicFile(fileName, fileResult.Result));
        }
    }
}
