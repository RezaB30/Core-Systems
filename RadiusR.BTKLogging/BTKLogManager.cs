using RadiusR.DB;
using RadiusR.DB.BTKLogging;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.Files.BTKLogging;
using RezaB.FTPUtilities;
using RadiusR.Files;
using System.Data.Entity;

namespace RadiusR.BTKLogging
{
    public static class BTKLogManager
    {
        public static void CreateLogs(SchedulerSettings schedulerSettings)
        {
            // clear temp folder
            BTKLogFileManager.ClearTempFolder((LogFileTypes)schedulerSettings.LogType);
            // create temp files
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
            // ftp upload
            var files = BTKLogFileManager.GetRecentLogFiles((LogFileTypes)schedulerSettings.LogType);
            var ftpClient = FTPClientFactory.CreateFTPClient(schedulerSettings.FTPFolder, schedulerSettings.FTPUsername, schedulerSettings.FTPPassword);
            foreach (var file in files)
            {
                using (var fileStream = FileManager.GetRepositoryFile(file.PathWithName))
                {
                    ftpClient.Upload(file.FileName, fileStream);
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
                        BTKLogFileManager.CreateLogFile(LogFileTypes.ClientCatalog, contents.ToString(), new OperatorInfo()
                        {
                            OperatorName = BTKSettings.BTKOperatorName,
                            OperatorCode = BTKSettings.BTKOperatorCode,
                            OperatorType = BTKSettings.BTKOperatorType,
                            Department = BTKSettings.BTKOperatorDepartment
                        }, schedulerSettings.NextOperationTime.Value, fileNo);
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxSubID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                BTKLogFileManager.CreateLogFile(LogFileTypes.ClientCatalog, contents.ToString(), new OperatorInfo()
                {
                    OperatorName = BTKSettings.BTKOperatorName,
                    OperatorCode = BTKSettings.BTKOperatorCode,
                    OperatorType = BTKSettings.BTKOperatorType,
                    Department = BTKSettings.BTKOperatorDepartment
                }, schedulerSettings.NextOperationTime.Value, fileNo);
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
                        BTKLogFileManager.CreateLogFile(LogFileTypes.ClientChanges, contents.ToString(), new OperatorInfo()
                        {
                            OperatorName = BTKSettings.BTKOperatorName,
                            OperatorCode = BTKSettings.BTKOperatorCode,
                            OperatorType = BTKSettings.BTKOperatorType,
                            Department = BTKSettings.BTKOperatorDepartment
                        }, schedulerSettings.NextOperationTime.Value, fileNo);
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxSubID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                BTKLogFileManager.CreateLogFile(LogFileTypes.ClientChanges, contents.ToString(), new OperatorInfo()
                {
                    OperatorName = BTKSettings.BTKOperatorName,
                    OperatorCode = BTKSettings.BTKOperatorCode,
                    OperatorType = BTKSettings.BTKOperatorType,
                    Department = BTKSettings.BTKOperatorDepartment
                }, schedulerSettings.NextOperationTime.Value, fileNo);
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
                    var query = db.RadiusAccountings.GetActiveInTimeSpan(schedulerSettings.LastOperationTime, schedulerSettings.NextOperationTime.Value).OrderBy(ra => ra.ID).AsQueryable();

                    if (lastMaxAccRecordID.HasValue)
                        query = query.Where(ra => ra.ID > lastMaxAccRecordID);
                    query = query.Take(1000);
                    var currentCount = query.Count();
                    if (currentCount <= 0)
                        break;

                    contents.Append(string.Join(Environment.NewLine, query.GetIPDRLog(schedulerSettings.LastOperationTime)));

                    if (schedulerSettings.PartitionFiles)
                    {
                        BTKLogFileManager.CreateLogFile(LogFileTypes.IPDR, contents.ToString(), new OperatorInfo()
                        {
                            OperatorName = BTKSettings.BTKOperatorName,
                            OperatorCode = BTKSettings.BTKOperatorCode,
                            OperatorType = BTKSettings.BTKOperatorType,
                            Department = BTKSettings.BTKOperatorDepartment
                        }, schedulerSettings.NextOperationTime.Value, fileNo);
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
                BTKLogFileManager.CreateLogFile(LogFileTypes.IPDR, contents.ToString(), new OperatorInfo()
                {
                    OperatorName = BTKSettings.BTKOperatorName,
                    OperatorCode = BTKSettings.BTKOperatorCode,
                    OperatorType = BTKSettings.BTKOperatorType,
                    Department = BTKSettings.BTKOperatorDepartment
                }, schedulerSettings.NextOperationTime.Value, fileNo);
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
                        BTKLogFileManager.CreateLogFile(LogFileTypes.IPBlock, contents.ToString(), new OperatorInfo()
                        {
                            OperatorName = BTKSettings.BTKOperatorName,
                            OperatorCode = BTKSettings.BTKOperatorCode,
                            OperatorType = BTKSettings.BTKOperatorType,
                            Department = BTKSettings.BTKOperatorDepartment
                        }, schedulerSettings.NextOperationTime.Value, fileNo);
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxBlockID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                contents.Append(Environment.NewLine);
                BTKLogFileManager.CreateLogFile(LogFileTypes.IPBlock, contents.ToString(), new OperatorInfo()
                {
                    OperatorName = BTKSettings.BTKOperatorName,
                    OperatorCode = BTKSettings.BTKOperatorCode,
                    OperatorType = BTKSettings.BTKOperatorType,
                    Department = BTKSettings.BTKOperatorDepartment
                }, schedulerSettings.NextOperationTime.Value, fileNo);
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
                        var query = db.RadiusAccountings.Where(ra => ra.Subscription.Service.InfrastructureType == (short)infrastructureType).GetActiveInTimeSpan(schedulerSettings.LastOperationTime, schedulerSettings.NextOperationTime.Value).OrderBy(ra => ra.ID).Where(ra => !string.IsNullOrEmpty(ra.Subscription.StaticIP)).AsQueryable();

                        if (lastMaxAccRecordID.HasValue)
                            query = query.Where(ra => ra.ID > lastMaxAccRecordID);
                        query = query.Take(1000);
                        var currentCount = query.Count();
                        if (currentCount <= 0)
                            break;

                        contents.Append(string.Join(Environment.NewLine, query.GetSessionsLog()));

                        if (schedulerSettings.PartitionFiles)
                        {
                            BTKLogFileManager.CreateLogFile(LogFileTypes.Sessions, contents.ToString(), new OperatorInfo()
                            {
                                OperatorName = BTKSettings.BTKOperatorName,
                                OperatorCode = BTKSettings.BTKOperatorCode,
                                OperatorType = BTKSettings.BTKOperatorType,
                                Department = BTKSettings.BTKOperatorDepartment
                            }, schedulerSettings.NextOperationTime.Value, fileNo, Enum.GetName(typeof(ServiceInfrastructureTypes), infrastructureType).Replace("_", " "));
                            fileNo++;
                            contents.Clear();
                        }
                        lastMaxAccRecordID = query.Max(subscription => subscription.ID);
                    }
                }

                if (!schedulerSettings.PartitionFiles && contents.Length > 0)
                {
                    BTKLogFileManager.CreateLogFile(LogFileTypes.Sessions, contents.ToString(), new OperatorInfo()
                    {
                        OperatorName = BTKSettings.BTKOperatorName,
                        OperatorCode = BTKSettings.BTKOperatorCode,
                        OperatorType = BTKSettings.BTKOperatorType,
                        Department = BTKSettings.BTKOperatorDepartment
                    }, schedulerSettings.NextOperationTime.Value, fileNo, Enum.GetName(typeof(ServiceInfrastructureTypes), infrastructureType).Replace("_", " "));
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
                        BTKLogFileManager.CreateLogFile(LogFileTypes.ClientOld, contents.ToString(), new OperatorInfo()
                        {
                            OperatorName = BTKSettings.BTKOperatorName,
                            OperatorCode = BTKSettings.BTKOperatorCode,
                            OperatorType = BTKSettings.BTKOperatorType,
                            Department = BTKSettings.BTKOperatorDepartment
                        }, schedulerSettings.NextOperationTime.Value, fileNo);
                        fileNo++;
                        contents.Clear();
                    }
                    lastMaxSubID = query.Max(subscription => subscription.ID);
                }
            }

            if (!schedulerSettings.PartitionFiles && contents.Length > 0)
            {
                BTKLogFileManager.CreateLogFile(LogFileTypes.ClientOld, contents.ToString(), new OperatorInfo()
                {
                    OperatorName = BTKSettings.BTKOperatorName,
                    OperatorCode = BTKSettings.BTKOperatorCode,
                    OperatorType = BTKSettings.BTKOperatorType,
                    Department = BTKSettings.BTKOperatorDepartment
                }, schedulerSettings.NextOperationTime.Value, fileNo);
            }
        }
    }
}
