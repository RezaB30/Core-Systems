using RadiusR.DB;
using RadiusR.DB.BTKLogging;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using RadiusR.FileManagement;
using RadiusR.FileManagement.BTKLogging;
using RezaB.Files.FTP;
using NLog;

namespace RadiusR.BTKLogging
{
    public static class BTKLogManager
    {
        private static Logger logger = LogManager.GetLogger("BTK Log Scheduler");

        public static void CreateLogs(SchedulerSettings schedulerSettings)
        {
            // create log files
            switch (schedulerSettings.LogType)
            {
                case BTKLogTypes.ClientCatalog:
                    CreateClientCatologLogs(schedulerSettings);
                    break;
                case BTKLogTypes.ClientChanges:
                    CreateClientChangeLogs(schedulerSettings);
                    break;
                case BTKLogTypes.IPDR:
                    CreateIPDRLogs(schedulerSettings);
                    break;
                case BTKLogTypes.IPBlock:
                    CreateIPBlockLogs(schedulerSettings);
                    break;
                case BTKLogTypes.Sessions:
                    CreateSessionsLog(schedulerSettings);
                    break;
                case BTKLogTypes.ClientOld:
                    CreateClientOldLogs(schedulerSettings);
                    break;
                default:
                    break;
            }
        }

        public static void UploadCreatedFiles(SchedulerSettings schedulerSettings)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.ListBTKLogs(schedulerSettings.LogType, schedulerSettings.LastUploadTime, schedulerSettings.CurrentOperationTime);
            if (result.InternalException != null)
            {
                throw result.InternalException;
            }
            var validLogFiles = result.Result.Where(lf => lf.BTKDate.HasValue).ToArray();
            logger.Trace("Upload file list for {0}:{1}{2}",
                schedulerSettings.LogType.ToString(),
                Environment.NewLine,
                string.Join(Environment.NewLine, validLogFiles.Select(lf => lf.FileName).ToArray()));
            var ftpClient = FTPClientFactory.CreateFTPClient(schedulerSettings.FTPFolder, schedulerSettings.FTPUsername, schedulerSettings.FTPPassword);
            foreach (var logFileWithDate in validLogFiles)
            {
                logger.Trace("Uploading {0}.", logFileWithDate.FileName);
                using (var fileResult = fileManager.GetBTKLog(schedulerSettings.LogType, logFileWithDate.BTKDate.Value, logFileWithDate.FileName))
                {
                    if(fileResult.InternalException != null)
                    {
                        throw fileResult.InternalException;
                    }
                    var uploadResult = ftpClient.SaveFile(fileResult.Result.FileName, fileResult.Result.Content);
                    if(uploadResult.InternalException != null)
                    {
                        throw uploadResult.InternalException;
                    }
                    else if (!uploadResult.Result)
                    {
                        throw new Exception($"error uploading [{logFileWithDate.FileName}].");
                    }
                    else
                    {
                        schedulerSettings.LastUploadTime = logFileWithDate.BTKDate.Value;
                    }
                }
            }
        }

        private static void CreateClientCatologLogs(SchedulerSettings schedulerSettings)
        {
            var operationTime = schedulerSettings.CurrentOperationTime;
            var fileNo = 1;
            var lastMaxSubID = (long?)null;
            StringBuilder contents = new StringBuilder(string.Empty);
            while (true)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                    var query = db.Subscriptions.GetValidEntriesForClientCatalog(schedulerSettings.NextOperationTime.Value);

                    if (lastMaxSubID.HasValue)
                        query = query.Where(subscription => subscription.ID > lastMaxSubID);
                    query = query.Take(1000);
                    var currentCount = query.Count();
                    if (currentCount <= 0)
                        break;

                    contents.Append(string.Join(Environment.NewLine, query.GetClientsCatalogLog()));

                    if (schedulerSettings.PartitionFiles)
                    {
                        SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.ClientCatalog, schedulerSettings.NextOperationTime.Value, fileNo));
                        
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxSubID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.ClientCatalog, schedulerSettings.NextOperationTime.Value, fileNo));
            }
        }

        private static void CreateClientChangeLogs(SchedulerSettings schedulerSettings)
        {
            var operationTime = schedulerSettings.CurrentOperationTime;
            var fileNo = 1;
            var lastMaxSubID = (long?)null;
            StringBuilder contents = new StringBuilder(string.Empty);
            while (true)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                    var query = db.Subscriptions.GetValidEntriesForClientChanges(schedulerSettings.LastOperationTime, schedulerSettings.NextOperationTime.Value);

                    if (lastMaxSubID.HasValue)
                        query = query.Where(subscription => subscription.ID > lastMaxSubID);
                    query = query.Take(1000);
                    var currentCount = query.Count();
                    if (currentCount <= 0)
                        break;

                    contents.Append(string.Join(Environment.NewLine, query.GetClientsChangeLogs(schedulerSettings.LastOperationTime)));

                    if (schedulerSettings.PartitionFiles)
                    {
                        SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.ClientChanges, schedulerSettings.NextOperationTime.Value, fileNo));
                        
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxSubID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.ClientChanges, schedulerSettings.NextOperationTime.Value, fileNo));
            }
        }

        private static void CreateIPDRLogs(SchedulerSettings schedulerSettings)
        {
            var operationTime = schedulerSettings.CurrentOperationTime;

            var lastMaxAccRecordID = (long?)null;
            var fileNo = 1;
            StringBuilder contents = new StringBuilder(string.Empty);

            while (true)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                    var query = db.RadiusAccountings
                        .GetActiveInTimeSpan(schedulerSettings.LastOperationTime, schedulerSettings.NextOperationTime.Value)
                        .Where(ra => string.IsNullOrEmpty(ra.Subscription.RadiusAuthorization.StaticIP))
                        .OrderBy(ra => ra.ID).AsQueryable();

                    if (lastMaxAccRecordID.HasValue)
                        query = query.Where(ra => ra.ID > lastMaxAccRecordID);
                    query = query.Take(1000);
                    var currentCount = query.Count();
                    if (currentCount <= 0)
                        break;

                    contents.Append(string.Join(Environment.NewLine, query.GetIPDRLog(schedulerSettings.LastOperationTime, schedulerSettings.NextOperationTime.Value)));

                    if (schedulerSettings.PartitionFiles)
                    {
                        SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.IPDR, schedulerSettings.NextOperationTime.Value, fileNo));
                        
                        fileNo++;
                        contents.Clear();
                    }
                    else
                    {
                        contents.Append(Environment.NewLine);
                    }
                    lastMaxAccRecordID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.IPDR, schedulerSettings.NextOperationTime.Value, fileNo));
            }
        }

        private static void CreateIPBlockLogs(SchedulerSettings schedulerSettings)
        {
            var operationTime = schedulerSettings.CurrentOperationTime;
            var fileNo = 1;
            var lastMaxBlockID = (long?)null;
            StringBuilder contents = new StringBuilder(string.Empty);

            while (true)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                    var query = db.BTKIPBlocks.OrderBy(block => block.ID).AsQueryable();

                    if (lastMaxBlockID.HasValue)
                        query = query.Where(block => block.ID > lastMaxBlockID);
                    query = query.Take(1000);
                    var currentCount = query.Count();
                    if (currentCount <= 0)
                        break;

                    contents.Append(string.Join(Environment.NewLine, query.GetIPBlockLog()));

                    if (schedulerSettings.PartitionFiles)
                    {
                        contents.Append(Environment.NewLine);
                        SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.IPBlock, schedulerSettings.NextOperationTime.Value, fileNo));
                        
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxBlockID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                contents.Append(Environment.NewLine);
                SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.IPBlock, schedulerSettings.NextOperationTime.Value, fileNo));
            }
        }

        private static void CreateSessionsLog(SchedulerSettings schedulerSettings)
        {
            var operationTime = schedulerSettings.CurrentOperationTime;

            IEnumerable<ServiceInfrastructureTypes> infrastructureTypes;
            using (RadiusREntities db = new RadiusREntities())
            {
                infrastructureTypes = db.Services.GroupBy(service => service.InfrastructureType).Select(g => g.Key).ToArray().Select(it => (ServiceInfrastructureTypes)it).ToArray();
            }

            foreach (var infrastructureType in infrastructureTypes)
            {
                var fileNo = 1;
                var lastMaxAccRecordID = (long?)null;
                StringBuilder contents = new StringBuilder(string.Empty);

                while (true)
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                        var query = db.RadiusAccountings.Where(ra => ra.Subscription.Service.InfrastructureType == (short)infrastructureType).GetActiveInTimeSpan(schedulerSettings.LastOperationTime, schedulerSettings.NextOperationTime.Value).OrderBy(ra => ra.ID).Where(ra => !string.IsNullOrEmpty(ra.Subscription.RadiusAuthorization.StaticIP)).AsQueryable();

                        if (lastMaxAccRecordID.HasValue)
                            query = query.Where(ra => ra.ID > lastMaxAccRecordID);
                        query = query.Take(1000);
                        var currentCount = query.Count();
                        if (currentCount <= 0)
                            break;

                        contents.Append(string.Join(Environment.NewLine, query.GetSessionsLog()));

                        if (schedulerSettings.PartitionFiles)
                        {
                            SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.Sessions, schedulerSettings.NextOperationTime.Value, fileNo, Enum.GetName(typeof(ServiceInfrastructureTypes), infrastructureType).Replace("_", " ")));
                            
                            fileNo++;
                            contents.Clear();
                        }
                        lastMaxAccRecordID = query.Max(subscription => subscription.ID);
                    }
                }

                if (!schedulerSettings.PartitionFiles && contents.Length > 0)
                {
                    SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.Sessions, schedulerSettings.NextOperationTime.Value, fileNo, Enum.GetName(typeof(ServiceInfrastructureTypes), infrastructureType).Replace("_", " ")));
                }
            }
        }

        private static void CreateClientOldLogs(SchedulerSettings schedulerSettings)
        {
            var operationTime = schedulerSettings.CurrentOperationTime;
            var fileNo = 1;
            var lastMaxSubID = (long?)null;
            StringBuilder contents = new StringBuilder(string.Empty);

            while (true)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                    var query = db.Subscriptions.Include(sub => sub.Customer.BillingAddress).Include(sub => sub.Customer.CustomerIDCard).Include(sub => sub.Address).Include(sub => sub.Service).Include(sub => sub.SubscriptionTelekomInfo);
                    query = db.Subscriptions.GetValidEntriesForClientCatalog(schedulerSettings.NextOperationTime.Value);

                    if (lastMaxSubID.HasValue)
                        query = query.Where(subscription => subscription.ID > lastMaxSubID);
                    query = query.Take(1000);
                    var currentCount = query.Count();
                    if (currentCount <= 0)
                        break;

                    if (contents.Length > 0)
                        contents.Append(Environment.NewLine);
                    contents.Append(string.Join(Environment.NewLine, query.GetClientOldLog()));

                    if (schedulerSettings.PartitionFiles)
                    {
                        SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.ClientOld, schedulerSettings.NextOperationTime.Value, fileNo));
                        
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxSubID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                SaveLogFile(new BTKLogFile(contents.ToString(), BTKLogTypes.ClientOld, schedulerSettings.NextOperationTime.Value, fileNo));
            }
        }

        private static void SaveLogFile(BTKLogFile logFile)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.SaveBTKLogFile(logFile);
            if (result.InternalException != null)
            {
                throw result.InternalException;
            }
            else if (!result.Result)
            {
                throw new Exception($"Saving {logFile.LogType} log file was not successful.");
            }
        }
    }
}
