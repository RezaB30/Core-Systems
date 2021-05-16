using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RezaB.Files.FTP;
using RadiusR_Manager.Models.ViewModels;
using System.IO;
using RadiusR.OfflinePayment;
using RadiusR.DB.Utilities.Billing;
using System.Data.Entity;
using RadiusR.SystemLogs;
using System.Runtime.Caching;
using RezaB.Web.Authentication;
using NLog;

namespace RadiusR_Manager.Controllers
{
    public class OfflinePaymentController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        private static MemoryCache statusCache = new MemoryCache("OfflinePaymentStatus");
        private static Logger offlinePaymentLogger = LogManager.GetLogger("offline-payments");

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        // GET: OfflinePayment
        public ActionResult Index(int? page)
        {
            var viewResults = db.OfflinePaymentGateways.OrderBy(opg => opg.ID).Select(opg => new OfflinePaymentGatewayViewModel()
            {
                ID = opg.ID,
                FTPAddress = opg.FTPAddress,
                FTPPassword = opg.FTPPassword,
                FTPUsername = opg.FTPUsername,
                IsActive = opg.IsActive,
                LastOperationTime = opg.LastOperationTime,
                LastProcessedFileName = opg.LastProcessedFileName,
                Name = opg.Name,
                Type = opg.Type,
                CanBeRemoved = !opg.ExternalPayments.Any()
            });

            SetupPages(page, ref viewResults);

            return View(viewResults.ToArray());
        }

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        // GET: OfflinePayment/Add
        public ActionResult Add()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: OfflinePayment/Add
        public ActionResult Add(OfflinePaymentGatewayViewModel paymentGateway)
        {
            if (ModelState.IsValid)
            {
                db.OfflinePaymentGateways.Add(new OfflinePaymentGateway()
                {
                    FTPAddress = paymentGateway.FTPAddress,
                    FTPPassword = paymentGateway.FTPPassword,
                    FTPUsername = paymentGateway.FTPUsername,
                    SendFolder = paymentGateway.SendFolder,
                    ReceiveFolder = paymentGateway.ReceiveFolder,
                    IsActive = true,
                    Name = paymentGateway.Name,
                    Type = paymentGateway.Type.Value
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(paymentGateway);
        }

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        // GET: OfflinePayment/Edit
        public ActionResult Edit(int id)
        {
            var dbGateway = db.OfflinePaymentGateways.Find(id);
            if (dbGateway == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var gateway = new OfflinePaymentGatewayViewModel()
            {
                FTPAddress = dbGateway.FTPAddress,
                FTPPassword = dbGateway.FTPPassword,
                FTPUsername = dbGateway.FTPUsername,
                SendFolder = dbGateway.SendFolder,
                ReceiveFolder = dbGateway.ReceiveFolder,
                ID = dbGateway.ID,
                IsActive = dbGateway.IsActive,
                Name = dbGateway.Name,
                Type = dbGateway.Type
            };

            return View(viewName: "Add", model: gateway);
        }

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: OfflinePayment/Edit
        public ActionResult Edit(int id, OfflinePaymentGatewayViewModel gateway)
        {
            var dbGateway = db.OfflinePaymentGateways.Find(id);
            if (dbGateway == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbGateway.FTPAddress = gateway.FTPAddress;
                dbGateway.FTPPassword = gateway.FTPPassword;
                dbGateway.FTPUsername = gateway.FTPUsername;
                dbGateway.SendFolder = gateway.SendFolder;
                dbGateway.ReceiveFolder = gateway.ReceiveFolder;
                dbGateway.Name = gateway.Name;
                dbGateway.Type = gateway.Type.Value;

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(viewName: "Add", model: gateway);
        }

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: OfflinePayment/ToggleState
        public ActionResult ToggleState(int id)
        {
            var gateway = db.OfflinePaymentGateways.Find(id);
            if (gateway == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            gateway.IsActive = !gateway.IsActive;
            db.SaveChanges();
            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Offline Payment Gateways")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: OfflinePayment/Remove
        public ActionResult Remove(int id)
        {
            var gateway = db.OfflinePaymentGateways.Find(id);
            if (gateway == null || gateway.ExternalPayments.Any())
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            db.OfflinePaymentGateways.Remove(gateway);
            db.SaveChanges();
            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Offline Payments")]
        [HttpGet]
        // GET: OfflinePayment/ProcessPayments
        public ActionResult ProcessPayments()
        {
            var statusReport = GetPaymentFileList();

            ViewBag.CheckList = statusReport;
            ViewBag.StatusReportTime = statusCache.Get("LastUpdateTime") as DateTime?;
            return View();
        }

        [AuthorizePermission(Permissions = "Offline Payments")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ProcessPayments")]
        // GET: OfflinePayment/ProcessPayments
        public ActionResult ProcessPaymentsConfirm()
        {
            offlinePaymentLogger.Debug("payment started.");

            var results = new List<OfflinePaymentStatusReportViewModel>();
            // load payment files list
            offlinePaymentLogger.Debug("getting payment file list.");
            var fileList = GetPaymentFileList();
            foreach (var item in fileList)
            {
                if (!item.IsSuccess)
                {
                    results.Add(new OfflinePaymentStatusReportViewModel()
                    {
                        Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.GetPaymentList,
                        IsSuccess = false,
                        Results = item.FileNames.Any() ? item.FileNames.FirstOrDefault() : null
                    });
                    offlinePaymentLogger.Warn($"loading payment files was unsuccessful {results.LastOrDefault()}");
                }
                else
                {
                    results.Add(new OfflinePaymentStatusReportViewModel()
                    {
                        Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.GetPaymentList,
                        IsSuccess = true,
                        Results = RadiusR.Localization.Pages.OfflinePaymentReportStages.Success
                    });
                    offlinePaymentLogger.Trace($"loading payment files was successful {results.LastOrDefault()}");
                    // load gateway settings
                    var currentGateway = db.OfflinePaymentGateways.Find(item.ID);
                    if (currentGateway == null)
                    {
                        results.Add(new OfflinePaymentStatusReportViewModel()
                        {
                            Gateway = item.ID.ToString(),
                            Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.LoadGatewaySettings,
                            Results = RadiusR.Localization.Pages.OfflinePaymentReportStages.GatewaySettingsNotFound,
                            IsSuccess = false
                        });
                        offlinePaymentLogger.Warn($"error loading gateway settings {results.LastOrDefault()}");
                    }
                    else
                    {
                        // file format
                        var format = FormatTypes.Finansbank;
                        switch ((RadiusR.DB.Enums.OfflineGatewayTypes)currentGateway.Type)
                        {
                            case RadiusR.DB.Enums.OfflineGatewayTypes.Finans:
                                format = FormatTypes.Finansbank;
                                break;
                            case RadiusR.DB.Enums.OfflineGatewayTypes.Halk:
                                format = FormatTypes.Halkbank;
                                break;
                            default:
                                results.Add(new OfflinePaymentStatusReportViewModel()
                                {
                                    Gateway = currentGateway.Name,
                                    Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.LoadGatewaySettings,
                                    Results = "Invalid Gateway Type",
                                    IsSuccess = false
                                });
                                offlinePaymentLogger.Warn($"invalid gateway type {results.LastOrDefault()}");
                                continue;
                        }
                        // fatal error switch
                        var shouldSkip = false;
                        // continue
                        results.Add(new OfflinePaymentStatusReportViewModel()
                        {
                            Gateway = currentGateway.Name,
                            Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.LoadGatewaySettings,
                            IsSuccess = true,
                            Results = RadiusR.Localization.Pages.OfflinePaymentReportStages.Success
                        });
                        offlinePaymentLogger.Trace($"loading gateway settings was successful {results.LastOrDefault()}");
                        // access files
                        var ftpClient = FTPClientFactory.CreateFTPClient(currentGateway.FTPAddress, currentGateway.FTPUsername, currentGateway.FTPPassword);
                        var response = ftpClient.EnterDirectoryPath(currentGateway.ReceiveFolder);
                        if (response.InternalException != null)
                        {
                            results.Add(new OfflinePaymentStatusReportViewModel()
                            {
                                Gateway = currentGateway.Name,
                                Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.LoadPaymentFiles,
                                Results = response.InternalException.Message,
                                IsSuccess = false
                            });
                            offlinePaymentLogger.Warn(response.InternalException, $"ftp error {results.LastOrDefault()}");
                        }
                        else
                        {
                            // read files
                            foreach (var fileName in item.FileNames)
                            {
                                if (shouldSkip)
                                {
                                    results.Add(new OfflinePaymentStatusReportViewModel()
                                    {
                                        Gateway = currentGateway.Name,
                                        Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.LoadPaymentFiles,
                                        Results = RadiusR.Localization.Pages.OfflinePaymentReportStages.Skipped,
                                        IsSuccess = false,
                                        DetailedList = new[] { fileName }
                                    });
                                    offlinePaymentLogger.Warn($"skipped loading files {results.LastOrDefault()}");
                                    break;
                                }
                                using (var fileResponse = ftpClient.GetFile(fileName))
                                {
                                    if (fileResponse.InternalException != null)
                                    {
                                        results.Add(new OfflinePaymentStatusReportViewModel()
                                        {
                                            Gateway = currentGateway.Name,
                                            Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.LoadPaymentFiles,
                                            Results = fileResponse.InternalException.Message,
                                            IsSuccess = false,
                                            DetailedList = new[] { fileName }
                                        });
                                        offlinePaymentLogger.Warn(response.InternalException, $"ftp error {results.LastOrDefault()}");
                                    }
                                    else
                                    {
                                        // prepare for processing
                                        IEnumerable<RadiusR.OfflinePayment.Receiving.BatchPaidBill> processResults = null;
                                        // processing file
                                        offlinePaymentLogger.Debug("processing file.");
                                        try
                                        {
                                            fileResponse.Result.Seek(0, SeekOrigin.Begin);
                                            processResults = BatchProcessor.ProcessStream(fileResponse.Result, format);
                                            processResults = processResults.ToArray();
                                            offlinePaymentLogger.Debug("file process successful.");
                                        }
                                        catch (Exception ex)
                                        {
                                            results.Add(new OfflinePaymentStatusReportViewModel()
                                            {
                                                Gateway = currentGateway.Name,
                                                Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.ProcessPaymentFile,
                                                Results = ex.Message,
                                                IsSuccess = false,
                                                DetailedList = new[] { fileName }
                                            });
                                            offlinePaymentLogger.Warn(response.InternalException, $"process file error {results.LastOrDefault()}");
                                            shouldSkip = true;
                                            continue;
                                        }
                                        // load bills from db
                                        offlinePaymentLogger.Debug("loading bills from DB.");
                                        var billIds = processResults.Select(pr => Convert.ToInt64(pr.BillNo)).ToArray();
                                        IEnumerable<Bill> dbBills = null;
                                        try
                                        {
                                            dbBills = db.Bills.OrderBy(b => b.IssueDate).ThenBy(b => b.ID).Where(b => billIds.Contains(b.ID))
                                            .Include(b => b.ExternalPayment.RadiusRBillingService).Include(b => b.ExternalPayment.OfflinePaymentGateway)
                                            .Include(b => b.PartnerBillPayment).Include(b => b.ScheduledSMSes)
                                            .Include(b => b.BillFees.Select(bf => bf.Discount))
                                            .ToArray();
                                            offlinePaymentLogger.Debug("loading bills successful.");
                                        }
                                        catch (Exception ex)
                                        {
                                            results.Add(new OfflinePaymentStatusReportViewModel()
                                            {
                                                Gateway = currentGateway.Name,
                                                Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.GetBillListFromDatabase,
                                                Results = ex.Message,
                                                IsSuccess = false
                                            });
                                            offlinePaymentLogger.Warn(response.InternalException, $"error loading bills from db {results.LastOrDefault()}");
                                            shouldSkip = true;
                                            continue;
                                        }
                                        // create stage status details
                                        var stageDetails = new List<string>();
                                        // bills which are not found
                                        var notFound = billIds.Except(dbBills.Select(b => b.ID)).ToArray();
                                        stageDetails.AddRange(notFound.Select(bid => string.Format(RadiusR.Localization.Pages.OfflinePaymentReportStages.BillNotFound, bid)));
                                        // already paid bills
                                        var alreadyPaid = dbBills.Where(b => b.BillStatusID == (short)RadiusR.DB.Enums.BillState.Paid || b.BillStatusID == (short)RadiusR.DB.Enums.BillState.Cancelled).Select(b => new { BillNo = b.ID, SubscriberNo = b.Subscription.SubscriberNo }).ToArray();
                                        foreach (var bill in alreadyPaid)
                                        {
                                            stageDetails.Add(string.Format(RadiusR.Localization.Pages.OfflinePaymentReportStages.AlreadyPaidBill, bill.BillNo, bill.SubscriberNo));
                                        }
                                        // pay
                                        dbBills = dbBills.Where(b => b.BillStatusID == (short)RadiusR.DB.Enums.BillState.Unpaid).ToArray();
                                        var failedPays = new List<Bill>();
                                        foreach (var bill in dbBills)
                                        {
                                            var current = db.PayBills(new[] { bill }, RadiusR.DB.Enums.PaymentType.OfflineBanking, BillPayment.AccountantType.Admin, User.GiveUserId(), new BillPaymentGateway()
                                            {
                                                OfflineGateway = currentGateway
                                            });
                                            if (current != BillPayment.ResponseType.Success)
                                                failedPays.Add(bill);
                                            else
                                                db.SystemLogs.Add(SystemLogProcessor.BillPayment(new[] { bill.ID }, User.GiveUserId(), bill.SubscriptionID, RadiusR.DB.Enums.SystemLogInterface.MasterISS, null, RadiusR.DB.Enums.PaymentType.OfflineBanking, currentGateway.Name));
                                        }
                                        stageDetails.AddRange(failedPays.Select(b => string.Format(RadiusR.Localization.Pages.OfflinePaymentReportStages.FailedPayment, b.ID, b.Subscription.SubscriberNo)));
                                        // add status
                                        results.Add(new OfflinePaymentStatusReportViewModel()
                                        {
                                            Gateway = currentGateway.Name,
                                            Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.BillPayment + " (" + fileName + ")",
                                            DetailedList = stageDetails,
                                            IsSuccess = true,
                                            Results = string.Format(RadiusR.Localization.Pages.OfflinePaymentReportStages.PaymentResult, dbBills.Count() - failedPays.Count(), alreadyPaid.Count(), notFound.Count(), failedPays.Count())
                                        });
                                        offlinePaymentLogger.Debug($"payment successful {results.LastOrDefault()}");
                                        // update gateway's last operation
                                        currentGateway.LastOperationTime = DateTime.Now;
                                        currentGateway.LastProcessedFileName = fileName;
                                        // save to db
                                        offlinePaymentLogger.Debug("saving changes to DB.");
                                        try
                                        {
                                            db.SaveChanges();
                                            results.Add(new OfflinePaymentStatusReportViewModel()
                                            {
                                                Gateway = currentGateway.Name,
                                                Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.UpdateDatabase + " (" + fileName + ")",
                                                IsSuccess = true,
                                                Results = RadiusR.Localization.Pages.OfflinePaymentReportStages.Success
                                            });
                                            offlinePaymentLogger.Debug($"saving to DB successful {results.LastOrDefault()}");
                                        }
                                        catch (Exception ex)
                                        {
                                            results.Add(new OfflinePaymentStatusReportViewModel()
                                            {
                                                Gateway = currentGateway.Name,
                                                Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.UpdateDatabase + " (" + fileName + ")",
                                                IsSuccess = false,
                                                Results = ex.Message
                                            });
                                            offlinePaymentLogger.Warn(response.InternalException, $"error saving to DB {results.LastOrDefault()}");
                                            shouldSkip = true;
                                            continue;
                                        }
                                    }
                                }
                            }
                            // skip on fatal error
                            if (shouldSkip)
                                continue;
                            // prepare for uploading unpaid bills
                            // load unpaid bills from db
                            try
                            {
                                using (var uploadStream = CreateUploadFileFromDB(format))
                                {
                                    var client = FTPClientFactory.CreateFTPClient(currentGateway.FTPAddress, currentGateway.FTPUsername, currentGateway.FTPPassword);
                                    var uploadError = client.EnterDirectoryPath(currentGateway.SendFolder);
                                    if (uploadError.InternalException != null)
                                    {
                                        results.Add(new OfflinePaymentStatusReportViewModel()
                                        {
                                            Gateway = currentGateway.Name,
                                            Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.UploadBillLists,
                                            IsSuccess = false,
                                            Results = uploadError.InternalException.Message
                                        });
                                        shouldSkip = true;
                                        continue;
                                    }
                                    var uploadFileName = BatchProcessor.GetUploadFileName(DateTime.Now, format);
                                    var uploadResults = client.SaveFile(uploadFileName, uploadStream);
                                    if (uploadResults.InternalException != null)
                                    {
                                        results.Add(new OfflinePaymentStatusReportViewModel()
                                        {
                                            Gateway = currentGateway.Name,
                                            Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.UploadBillLists,
                                            IsSuccess = false,
                                            Results = uploadError.InternalException.Message,
                                            DetailedList = new[] { uploadFileName }
                                        });
                                        shouldSkip = true;
                                        continue;
                                    }
                                    results.Add(new OfflinePaymentStatusReportViewModel()
                                    {
                                        Gateway = currentGateway.Name,
                                        Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.UploadBillLists,
                                        IsSuccess = true,
                                        Results = RadiusR.Localization.Pages.OfflinePaymentReportStages.Success,
                                        DetailedList = new[] { uploadFileName }
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                results.Add(new OfflinePaymentStatusReportViewModel()
                                {
                                    Gateway = currentGateway.Name,
                                    Stage = RadiusR.Localization.Pages.OfflinePaymentReportStages.GetBillListFromDatabase,
                                    IsSuccess = false,
                                    Results = ex.Message
                                });
                                continue;
                            }
                        }
                    }
                }
            }

            statusCache.Set("LastStatusReport", results, DateTimeOffset.Now.AddHours(2));
            statusCache.Set("LastUpdateTime", DateTime.Now, DateTimeOffset.Now.AddHours(2));

            return RedirectToAction("StatusReport");
            //return View(viewName: "StatusReport", model: results);
        }

        [AuthorizePermission(Permissions = "Offline Payments")]
        [HttpGet]
        // GET: OfflinePayment/StatusReport
        public ActionResult StatusReport()
        {
            var viewResults = statusCache.Get("LastStatusReport") as IEnumerable<OfflinePaymentStatusReportViewModel>;
            if (viewResults != null)
            {
                ViewBag.ReportTime = statusCache.Get("LastUpdateTime") as DateTime?;
            }

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Batch Expiration Date Update")]
        [HttpGet]
        // GET: OfflinePayment/BatchExpDateUpdate
        public ActionResult BatchExpDateUpdate()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Batch Expiration Date Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: OfflinePayment/BatchExpDateUpdate
        public ActionResult BatchExpDateUpdate(string subscriberNos)
        {
            subscriberNos = subscriberNos ?? string.Empty;
            var lines = subscriberNos.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            var dbSubs = db.Subscriptions.Include(s => s.RadiusAuthorization).Where(s => s.ActivationDate.HasValue && s.RadiusAuthorization.ExpirationDate.HasValue).Where(s => lines.Contains(s.SubscriberNo)).ToArray();
            if (dbSubs.Count() != lines.Length)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Validation.Common.InvalidInput + " (" + (lines.Length - dbSubs.Count()) + ")";
                ViewBag.Values = subscriberNos;
                return View();
            }

            foreach (var dbSubscription in dbSubs)
            {
                if (dbSubscription.State == (short)RadiusR.DB.Enums.CustomerState.Cancelled || dbSubscription.State == (short)RadiusR.DB.Enums.CustomerState.Disabled)
                    continue;
                var oldLastAlloweddate = dbSubscription.RadiusAuthorization.ExpirationDate.Value.Date;
                if (oldLastAlloweddate < DateTime.Today.AddDays(5))
                {
                    dbSubscription.RadiusAuthorization.ExpirationDate = dbSubscription.RadiusAuthorization.ExpirationDate.Value.Date.AddDays(5);
                    db.SystemLogs.Add(SystemLogProcessor.ExtendExpirationDate(User.GiveUserId(), dbSubscription.ID, RadiusR.DB.Enums.SystemLogInterface.MasterISS, null, oldLastAlloweddate.ToString("yyyy-MM-dd"), dbSubscription.RadiusAuthorization.ExpirationDate.Value.ToString("yyyy-MM-dd")));
                }
            }

            db.SaveChanges();
            return RedirectToAction("BatchExpDateUpdate", new { errorMessage = 0 });
        }

        #region Private Methods
        public class PaymentFileList
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public IEnumerable<string> FileNames { get; set; }

            public bool IsSuccess { get; set; }
        }

        private IEnumerable<PaymentFileList> GetPaymentFileList()
        {
            var statusReport = new List<PaymentFileList>();
            var gateways = db.OfflinePaymentGateways.Where(opg => opg.IsActive).ToArray();
            foreach (var gateway in gateways)
            {
                var client = FTPClientFactory.CreateFTPClient(gateway.FTPAddress, gateway.FTPUsername, gateway.FTPPassword);
                {
                    var currentResult = client.EnterDirectoryPath(gateway.ReceiveFolder);
                    if (currentResult.InternalException != null)
                    {
                        statusReport.Add(new PaymentFileList() { ID = gateway.ID, Name = gateway.Name, FileNames = new[] { currentResult.InternalException.Message }, IsSuccess = false });
                        continue;
                    }
                }
                {
                    var currentResult = client.GetFileList();
                    if (currentResult.InternalException != null)
                    {
                        statusReport.Add(new PaymentFileList() { ID = gateway.ID, Name = gateway.Name, FileNames = new[] { currentResult.InternalException.Message }, IsSuccess = false });
                        continue;
                    }

                    if (currentResult.Result.Contains(gateway.LastProcessedFileName))
                    {
                        var allFiles = currentResult.Result.ToArray();
                        for (int i = 0; i < allFiles.Length; i++)
                        {
                            if (allFiles[i] == gateway.LastProcessedFileName)
                            {
                                statusReport.Add(new PaymentFileList() { ID = gateway.ID, Name = gateway.Name, FileNames = allFiles.Skip(i + 1).ToArray(), IsSuccess = true });
                                break;
                            }
                        }
                    }
                    else
                    {
                        statusReport.Add(new PaymentFileList() { ID = gateway.ID, Name = gateway.Name, FileNames = currentResult.Result.ToArray(), IsSuccess = true });
                    }
                }
            }

            return statusReport;
        }

        private Stream CreateUploadFileFromDB(FormatTypes format)
        {
            var finalStream = new MemoryStream();
            var startTime = DateTime.Now;
            long startingID = 0;
            while (true)
            {
                var baseQuery = db.Bills.OrderBy(b => b.ID).Where(b => b.BillStatusID == (short)RadiusR.DB.Enums.BillState.Unpaid || (b.BillStatusID == (short)RadiusR.DB.Enums.BillState.Paid && b.PayDate > startTime)).Select(b => new
                {
                    BillNo = b.ID,
                    Amount = b.BillFees.Select(bf => bf.CurrentCost - (bf.DiscountID.HasValue ? bf.Discount.Amount : 0m)).DefaultIfEmpty(0m).Sum(),
                    DueDate = b.DueDate,
                    FullName = b.Subscription.Customer.CorporateCustomerInfo != null ? b.Subscription.Customer.CorporateCustomerInfo.Title : b.Subscription.Customer.FirstName + " " + b.Subscription.Customer.LastName,
                    SubscriberNo = b.Subscription.SubscriberNo
                });
                var query = baseQuery.Where(b => b.BillNo > startingID);
                var results = query.Take(1000).ToArray();
                if (results.Count() == 0)
                {
                    var totalAmount = baseQuery.Select(b => b.Amount).DefaultIfEmpty(0m).Sum();
                    var totalCount = baseQuery.Count();
                    BatchProcessor.CopyToStream(finalStream, format, Enumerable.Empty<RadiusR.OfflinePayment.Sending.BatchReadyBill>(), new RadiusR.OfflinePayment.Sending.FinishLine()
                    {
                        TotalAmount = totalAmount,
                        TotalCount = totalCount
                    });
                    break;
                }

                BatchProcessor.CopyToStream(finalStream, format, results.Select(r => new RadiusR.OfflinePayment.Sending.BatchReadyBill()
                {
                    Amount = r.Amount,
                    BillNo = r.BillNo.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    DueDate = r.DueDate,
                    FullName = r.FullName,
                    SubscriberNo = r.SubscriberNo
                }), WriteHeaderLine: startingID == 0);

                startingID = results.Max(r => r.BillNo);
            }

            return finalStream;
        }
        #endregion
    }
}