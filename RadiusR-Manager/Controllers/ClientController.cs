using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Helpers;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.CSVModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using RadiusR.DB.Utilities.Billing;
using System.Threading.Tasks;
using System.Data.Entity;
using RezaB.Mikrotik.Extentions;
using RezaB.Mikrotik.Extentions.MultiTasking;
using RadiusR.SMS;
using RadiusR.DB.ModelExtentions;
using RadiusR_Manager.Models.Extentions;
using RadiusR_Manager.Models;
using RadiusR.FileManagement;
using RezaB.Files;
using RadiusR.DB.Utilities.Extentions;
using System.Web;
using RadiusR.SystemLogs;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;
using RezaB.TurkTelekom.WebServices.Exceptions;
using RezaB.Data.Files;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;
using RezaB.Web;
using RezaB.Data.Localization;
using RezaB.Data.Formating;
using RezaB.Web.Authentication;
using RadiusR.FileManagement.SpecialFiles;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Clients", Roles = "cashier")]
    public partial class ClientController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Clients")]
        [HttpGet]
        // GET: Client
        public ActionResult Index(int? page, CustomerSearchViewModel search)
        {
            // query all sql rows
            var dbSubscriptions = db.Subscriptions.OrderByDescending(c => c.ID).AsQueryable();

            // apply search to sql rows
            dbSubscriptions = dbSubscriptions.FilterBySearchViewModel(search, db, User);

            // clear search address validations
            {
                var names = ModelState.Where(s => s.Key.StartsWith("Address.")).Select(s => s.Key).ToArray();
                foreach (var name in names)
                {
                    ModelState.Remove(name);
                }
            }

            // page data
            SetupPages(page, ref dbSubscriptions);

            var viewResults = dbSubscriptions.Select(s => new SubscriptionListDisplayViewModel()
            {
                Name = s.Customer.CorporateCustomerInfo != null ? s.Customer.CorporateCustomerInfo.Title : s.Customer.FirstName + " " + s.Customer.LastName,
                Username = s.Username,
                SubscriberNo = s.SubscriberNo,
                ID = s.ID,
                ContactPhoneNo = s.Customer.ContactPhoneNo,
                DSLNo = s.SubscriptionTelekomInfo != null ? s.SubscriptionTelekomInfo.SubscriptionNo : null,
                TariffName = s.Service.Name,
                State = s.State
            }).ToArray();

            ViewBag.Services = new SelectList(db.Services.ToArray(), "Name", "Name", search.ServiceName);
            ViewBag.Groups = new SelectList(db.Groups.ToArray(), "ID", "Name", search.GroupID);

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Download Subscriber List")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client
        public ActionResult Index(CustomerSearchViewModel search)
        {
            // query all sql rows
            var dbSubscriptions = db.Subscriptions.OrderByDescending(c => c.ID).AsQueryable();

            // apply search to sql rows
            dbSubscriptions = dbSubscriptions.FilterBySearchViewModel(search, db, User);

            dbSubscriptions = dbSubscriptions
                .Include(sub => sub.Service)
                .Include(sub => sub.Customer.CorporateCustomerInfo)
                .Include(sub => sub.Address)
                .Include(sub => sub.SubscriptionTelekomInfo)
                .Include(sub => sub.Groups);

            var StateList = new LocalizedList<CustomerState, RadiusR.Localization.Lists.CustomerState>();
            var results = dbSubscriptions.ToArray().Select(sub => new SubscriberListCSVModel()
            {
                Name = sub.ValidDisplayName,
                PhoneNo = AppSettings.CountryPhoneCode + sub.Customer.ContactPhoneNo,
                ServiceName = sub.Service.Name,
                ServiceNo = sub.SubscriptionTelekomInfo != null ? sub.SubscriptionTelekomInfo.SubscriptionNo : string.Empty,
                SubscriberNo = sub.SubscriberNo,
                Username = sub.Username,
                State = StateList.GetDisplayText(sub.State),
                GroupName = sub.Groups.Any() ? string.Join(", ", sub.Groups.Select(g => g.Name)) : string.Empty,
                RegistrationDate = sub.MembershipDate
            }).ToArray();

            return File(CSVGenerator.GetStream(results, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.ClientList + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Subscriber State")]
        // POST: Client/ChangeStatus
        public ActionResult ChangeStatus(long id, int State, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);

            CustomerState status;
            try
            {
                status = (CustomerState)State;
            }
            catch
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "5", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            if (dbSubscription.State == State)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            // ------------------new-----------------

            if (!StateChangeUtilities.GetValidStateChanges((CustomerState)dbSubscription.State).Contains(status))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            switch (status)
            {
                case CustomerState.Registered:
                    // new registration
                    if (dbSubscription.RegistrationType == (short)SubscriptionRegistrationType.NewRegistration)
                        return RedirectToAction("SendTelekomRegistration", new { id = dbSubscription.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                    // transition
                    else if (dbSubscription.RegistrationType == (short)SubscriptionRegistrationType.Transition)
                    {
                        return RedirectToAction("PrepareTransition", new { id = dbSubscription.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                    }
                    // transfer
                    else
                    {
                        var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new RegisterSubscriptionOptions()
                        {
                            AppUserID = User.GiveUserId(),
                            LogInterface = SystemLogInterface.MasterISS,
                            ScheduleSMSes = false
                        });

                        if (results.IsFatal)
                        {
                            throw results.InternalException;
                        }
                        else if (results.IsSuccess)
                        {
                            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        }
                        else
                        {
                            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                            TempData["ErrorResults"] = results;
                            return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                        }
                    }
                case CustomerState.Reserved:
                    return RedirectToAction("ReserveClientActions", new { redirectUrl = redirectUrl, id = id });
                case CustomerState.Active:
                    {
                        //try
                        //{
                        var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new ActivateSubscriptionOptions()
                        {
                            AppUserID = User.GiveUserId(),
                            LogInterface = SystemLogInterface.MasterISS,
                            ForceUnfreeze = false
                        });

                        if (results.IsFatal)
                        {
                            throw results.InternalException;
                        }
                        else if (results.IsSuccess)
                        {
                            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        }
                        else
                        {
                            // show force if user has permission
                            if (User.HasPermission("Force Unfreeze"))
                            {
                                TempData["UnfreezeError"] = results.ErrorMessage;
                                return RedirectToAction("UnfreezeError", new { id = id, redirectUrl = redirectUrl });
                            }
                            // else go to error page
                            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                            TempData["ErrorResults"] = results;
                            return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                        }
                        //}
                        //catch (TTWebServiceException ex)
                        //{
                        //    // show tt error
                        //    if (User.HasPermission("Force Unfreeze"))
                        //    {
                        //        TempData["UnfreezeError"] = ex.GetShortMessage();
                        //        return RedirectToAction("UnfreezeError", new { id = id, redirectUrl = redirectUrl });
                        //    }

                        //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", uri);
                        //    TempData["ErrorMessageDetails"] = ex.GetShortMessage();
                        //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        //}
                    }
                case CustomerState.Disabled:
                    return RedirectToAction("FreezeSubscription", new { redirectUrl = redirectUrl, id = id });
                case CustomerState.Cancelled:
                    return RedirectToAction("CancelSubscription", new { redirectUrl = redirectUrl, id = id });
                default:
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "5", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpGet]
        // GET: Client/ReserveClientActions
        public ActionResult ReserveClientActions(long id, string redirectUrl)
        {
            ViewBag.ClientID = id;
            ViewBag.RedirectURL = redirectUrl;

            var backUri = new UriBuilder(redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", backUri);
            ViewBag.BackLink = backUri.Uri.PathAndQuery;

            return View();
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ReserveClientActions")]
        // POST: Client/ReserveClientActions
        public ActionResult ReserveClientActionsConfirm(long id, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            if (dbSubscription.State != (short)CustomerState.Registered)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            // change state
            var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new ReserveSubscriptionOptions()
            {
                AppUserID = User.GiveUserId(),
                SetupServiceRequest = null,
                LogInterface = SystemLogInterface.MasterISS
            });

            if (results.IsFatal)
            {
                throw results.InternalException;
            }
            else if (results.IsSuccess)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            else
            {
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                TempData["ErrorResults"] = results;
                return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
            }
            
        }

        //[AuthorizePermission(Permissions = "Modify Clients")]
        //[HttpGet]
        //public ActionResult TransferActions(long id, string redirectUrl)
        //{
        //    ViewBag.ClientID = id;
        //    ViewBag.RedirectURL = redirectUrl;

        //    var backUri = new UriBuilder(redirectUrl);
        //    UrlUtilities.RemoveQueryStringParameter("errorMessage", backUri);
        //    ViewBag.BackLink = backUri.Uri.PathAndQuery;

        //    return View();
        //}

        //[AuthorizePermission(Permissions = "Modify Clients")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("TransferActions")]
        //// POST: Client/ReserveClientActions
        //public ActionResult TransferActionsConfirm(long id, string redirectUrl)
        //{
        //    var uri = new UriBuilder(redirectUrl);
        //    var dbSubscription = db.Subscriptions.Find(id);
        //    if (dbSubscription == null)
        //    {
        //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
        //        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        //    }
        //    if (dbSubscription.SubscriptionTelekomInfo == null || !dbSubscription.TTWorkOrders.Where(wo => wo.WorkType == (short)TTWorkOrderType.))
        //    {
        //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
        //        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        //    }

        //    dbSubscription.SubscriptionTelekomInfo.QueueNo = null;
        //    dbSubscription.SubscriptionTelekomInfo.ManagementCode = null;
        //    dbSubscription.SubscriptionTelekomInfo.ProvinceCode = null;
        //    // save
        //    db.SaveChanges();

        //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
        //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        //}

        //[AuthorizePermission(Permissions = "Modify Clients")]
        //[HttpGet]
        //// GET: Client/TransferBySetupService
        //public ActionResult TransferBySetupService(long id, string redirectUrl)
        //{
        //    ViewBag.ClientID = id;
        //    ViewBag.RedirectURL = redirectUrl;

        //    var backUri = new UriBuilder(redirectUrl);
        //    UrlUtilities.RemoveQueryStringParameter("errorMessage", backUri);
        //    ViewBag.BackLink = backUri.Uri.PathAndQuery;

        //    ViewBag.ServiceUsers = new SelectList(db.CustomerSetupUsers.ActiveUsers().Select(user => new { ID = user.ID, Name = user.Name }).ToList(), "ID", "Name");

        //    return View("SetupBySetupService", new NewSetupServiceTaskViewModel());
        //}

        //[AuthorizePermission(Permissions = "Modify Clients")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //// POST: Client/TransferBySetupService
        //public ActionResult TransferBySetupService(long id, string redirectUrl, [Bind(Include = "SetupUserID,HasModem,ModemName,XDSLType,TaskDescription")]NewSetupServiceTaskViewModel task)
        //{
        //    if (!db.CustomerSetupUsers.ActiveUsers().Any(user => user.ID == task.SetupUserID))
        //    {
        //        ModelState.AddModelError("SetupUserID", RadiusR.Localization.Pages.ErrorMessages._9);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var uri = new UriBuilder(redirectUrl);
        //        var dbSubscription = db.Subscriptions.Find(id);
        //        if (dbSubscription == null)
        //        {
        //            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
        //            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        //        }

        //        dbSubscription.CustomerSetupTasks.Add(new CustomerSetupTask()
        //        {
        //            Details = task.TaskDescription,
        //            HasModem = task.HasModem,
        //            IsCharged = false,
        //            ModemName = task.ModemName,
        //            SetupUserID = task.SetupUserID.Value,
        //            TaskIssueDate = DateTime.Now,
        //            TaskStatus = (short)RadiusR.DB.Enums.CustomerSetup.TaskStatuses.New,
        //            XDSLType = task.XDSLType,
        //            TaskType = (short)RadiusR.DB.Enums.CustomerSetup.TaskTypes.Transfer
        //        });

        //        dbSubscription.SubscriptionTelekomInfo.ManagementCode = null;
        //        dbSubscription.SubscriptionTelekomInfo.ProvinceCode = null;
        //        dbSubscription.SubscriptionTelekomInfo.QueueNo = null;

        //        // save
        //        db.SaveChanges();

        //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
        //        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        //    }

        //    ViewBag.ClientID = id;
        //    ViewBag.RedirectURL = redirectUrl;

        //    var backUri = new UriBuilder(redirectUrl);
        //    UrlUtilities.RemoveQueryStringParameter("errorMessage", backUri);
        //    ViewBag.BackLink = backUri.Uri.PathAndQuery;

        //    ViewBag.ServiceUsers = new SelectList(db.CustomerSetupUsers.ActiveUsers().Select(user => new { ID = user.ID, Name = user.Name }).ToList(), "ID", "Name");

        //    return View("SetupBySetupService", task);
        //}

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        public ActionResult SetupBySetupService(long id, string redirectUrl)
        {
            ViewBag.ClientID = id;
            ViewBag.RedirectURL = redirectUrl;

            var backUri = new UriBuilder(redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", backUri);
            ViewBag.BackLink = backUri.Uri.PathAndQuery;

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", backUri);
                return Redirect(backUri.Uri.PathAndQuery + backUri.Fragment);
            }

            ViewBag.ServiceUsers = new SelectList(db.CustomerSetupUsers.ActiveUsers().ValidPartnersForArea(dbSubscription.Address).Select(user => new { ID = user.ID, Name = user.Name }).ToList(), "ID", "Name");

            return View(new NewSetupServiceTaskViewModel());
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetupBySetupService(long id, string redirectUrl, [Bind(Include = "SetupUserID,HasModem,ModemName,XDSLType,TaskDescription")] NewSetupServiceTaskViewModel task)
        {
            var uri = new UriBuilder(redirectUrl);
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            if (dbSubscription.State != (short)CustomerState.Registered)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (!db.CustomerSetupUsers.ActiveUsers().ValidPartnersForArea(dbSubscription.Address).Any(user => user.ID == task.SetupUserID))
            {
                ModelState.AddModelError("SetupUserID", RadiusR.Localization.Pages.ErrorMessages._9);
            }

            if (ModelState.IsValid)
            {
                // change state
                var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new ReserveSubscriptionOptions()
                {
                    AppUserID = User.GiveUserId(),
                    LogInterface = SystemLogInterface.MasterISS,
                    SetupServiceRequest = new ReserveSubscriptionOptions.SetupRequest()
                    {
                        HasModem = task.HasModem,
                        ModemName = task.ModemName,
                        SetupUserID = task.SetupUserID.Value,
                        SetupTaskDescription = task.TaskDescription,
                        XDSLType = (RadiusR.DB.Enums.CustomerSetup.XDSLTypes)task.XDSLType
                    }
                });

                if (results.IsFatal)
                {
                    throw results.InternalException;
                }
                else if (results.IsSuccess)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                }
                else
                {
                    UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                    TempData["ErrorResults"] = results;
                    return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                }
            }

            ViewBag.ClientID = id;
            ViewBag.RedirectURL = redirectUrl;

            var backUri = new UriBuilder(redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", backUri);
            ViewBag.BackLink = backUri.Uri.PathAndQuery;

            ViewBag.ServiceUsers = new SelectList(db.CustomerSetupUsers.ActiveUsers().ValidPartnersForArea(dbSubscription.Address).Select(user => new { ID = user.ID, Name = user.Name }).ToList(), "ID", "Name");

            return View(task);
        }

        [AuthorizePermission(Permissions = "Clients")]
        [HttpGet]
        [ActionName("AdditionalFees")]
        // GET: Client/AdditionalFees
        public ActionResult ChangeStateAdditionalFees()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Clients")]
        // GET: Client/Details
        public ActionResult Details(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (dbSubscription.Service == null)
            {
                return RedirectToAction("Index", new { errorMessage = 2 });
            }

            var customer = new CustomerDetailsViewModel(dbSubscription, db);
            ViewBag.TelekomSyncError = TempData["tt-sync-error"];
            return View(customer);
        }

        [AuthorizePermission(Permissions = "Clients")]
        // GET,POST: Client/TrafficUsage
        public ActionResult TrafficUsage([Bind(Include = "StartDate,EndDate,ClientID")] TrafficUsageViewModel trafficUsage)
        {
            var dbSubscription = db.Subscriptions.Find(trafficUsage.ClientID);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!trafficUsage.EndDate.HasValue)
            {
                trafficUsage.EndDate = DateTime.Now.Date;
            }
            if (!trafficUsage.StartDate.HasValue)
            {
                trafficUsage.StartDate = trafficUsage.EndDate.Value.AddMonths(-1).Date;
            }
            var diagramData = new List<LinearDiagramDataArray>();
            var dailyData = db.RadiusDailyAccountings.Where(rda => rda.SubscriptionID == trafficUsage.ClientID && rda.Date <= trafficUsage.EndDate && rda.Date >= trafficUsage.StartDate)
                .GroupBy(rda => rda.Date).Select(rdag => new { date = rdag.Key, bytes = rdag.Sum(rda => rda.DownloadBytes + rda.UploadBytes) })
                .OrderBy(rda => rda.date)
                .ToArray().Select(t => (decimal)t.bytes);
            diagramData.Add(new LinearDiagramDataArray(RadiusR.Localization.Pages.Common.DailyUsage, dailyData, bytes => RateLimitFormatter.ToTrafficStandard(bytes, true)));

            ViewBag.DiagramData = diagramData;
            return View(trafficUsage);
        }

        [AuthorizePermission(Permissions = "Clients")]
        // GET: Client/UsageDetails
        public ActionResult UsageDetails(int? page, long id, string monthly)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._4);
            }

            var service = dbSubscription.Service;

            //ViewBag.HasFUP = dbSubscription.FUPUsedBytes < service.FUPTrafficLimit;

            if (string.IsNullOrEmpty(monthly))
            {
                var dailyUsage = dbSubscription.RadiusDailyAccountings.GroupBy(daily => daily.Date).OrderByDescending(dailyGroup => dailyGroup.Key).Select(dailyGroup => new UsageInfoViewModel()
                {
                    Date = dailyGroup.Key,
                    Download = dailyGroup.Sum(daily => (decimal)daily.DownloadBytes),
                    Upload = dailyGroup.Sum(daily => (decimal)daily.UploadBytes)
                }).AsQueryable();

                SetupPages(page, ref dailyUsage);

                ViewBag.Monthly = false;
                return View(dailyUsage);
            }

            var monthlyUsage = dbSubscription.RadiusDailyAccountings.GroupBy(daily => daily.Date).OrderByDescending(dailyGroup => dailyGroup.Key).Select(dailyGroup => new
            {
                groupingKey = new
                {
                    year = dailyGroup.Key.Year,
                    month = dailyGroup.Key.Month
                },
                download = dailyGroup.Sum(daily => (decimal)daily.DownloadBytes),
                upload = dailyGroup.Sum(daily => (decimal)daily.UploadBytes)
            }).GroupBy(monthlyGroup => monthlyGroup.groupingKey).Select(monthlyGroup => new UsageInfoViewModel()
            {
                _year = monthlyGroup.Key.year,
                _month = monthlyGroup.Key.month,
                Download = monthlyGroup.Sum(dailyGroup => dailyGroup.download),
                Upload = monthlyGroup.Sum(dailyGroup => dailyGroup.upload)
            }).OrderByDescending(usage => usage._year).ThenByDescending(usage => usage._month).Take(12).AsQueryable();

            SetupPages(page, ref monthlyUsage);

            ViewBag.Monthly = true;
            return View(monthlyUsage);
        }

        [AuthorizePermission(Permissions = "Manage Credit")]
        // GET: Client/Credit
        public ActionResult Credit(int? page, long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);

            var clientCredit = new ClientCreditViewModel(db, id);
            var credits = clientCredit.Credits.AsQueryable();
            SetupPages(page, ref credits);
            clientCredit.Credits = credits.ToList();

            return View(clientCredit);
        }

        [AuthorizePermission(Permissions = "Manage Credit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/AddCredit
        public ActionResult AddCredit(long id, [Bind(Prefix = "editCreditModel", Include = "AddingAmount")] EditCreditViewModel editCredit)
        {
            ModelState.Remove("editCreditModel.SubtractingAmount");
            ModelState.Remove("editCreditModel.Details");
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null || !dbSubscription.HasBilling)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                dbSubscription.SubscriptionCredits.Add(new SubscriptionCredit()
                {
                    Amount = editCredit._addingAmount.Value,
                    Date = DateTime.Now,
                    AccountantID = User.GiveUserId()
                });

                db.SaveChanges();

                // send credit ack sms
                // currently disabled due to not having a separate sales page
                //SMSService.SendCreditAck(new string[] { dbClient.Phone }, dbClient.Culture, editCredit._addingAmount.Value);

                return RedirectToAction("Credit", new { id = id, errorMessage = 0 });
            }

            return RedirectToAction("Credit", new { id = id, errorMessage = 11 });
        }

        [AuthorizePermission(Permissions = "Manage Credit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/SubtractCredit
        public ActionResult SubtractCredit(long id, [Bind(Prefix = "editCreditModel", Include = "SubtractingAmount")] EditCreditViewModel editCredit)
        {
            ModelState.Remove("editCreditModel.AddingAmount");
            ModelState.Remove("editCreditModel.Details");
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null || !dbSubscription.HasBilling)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                dbSubscription.SubscriptionCredits.Add(new SubscriptionCredit()
                {
                    Amount = editCredit._subtractingAmount.Value * -1m,
                    Date = DateTime.Now,
                    AccountantID = User.GiveUserId()
                });

                db.SaveChanges();

                return RedirectToAction("Credit", new { id = id, errorMessage = 0 });
            }

            return RedirectToAction("Credit", new { id = id, errorMessage = 11 });
        }

        [AuthorizePermission(Permissions = "Payment", Roles = "cashier")]
        [HttpGet]
        // GET: Client/ExtendPackage
        public ActionResult ExtendPackage(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);

            var extendPackage = new ExtendPackageViewModel()
            {
                ClientID = id,
                AddedPeriods = 1,
                ClientName = dbSubscription.ValidDisplayName
            };
            return View(extendPackage);
        }

        [AuthorizePermission(Permissions = "Payment", Roles = "cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ExtendPackage
        public ActionResult ExtendPackage([Bind(Include = "AddedPeriods,ClientID,TotalFee,ClientName")] ExtendPackageViewModel extendPackage, PaymentType? paymentType = PaymentType.None, bool HasPrintRequested = false)
        {
            var dbSubscription = db.Subscriptions.Find(extendPackage.ClientID);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (extendPackage._totalFee.HasValue)
            {
                if (paymentType == PaymentType.None || paymentType == PaymentType.OnlineBanking)
                {
                    if (User.IsInRole("cashier"))
                        return RedirectToAction("Index", "Home", new { errorMessage = 9 });

                    return Redirect(Url.Action("Details", "Client", new { id = dbSubscription.ID, errorMessage = 9 }) + "#bills");
                }

                if (paymentType == PaymentType.VirtualPos)
                {
                    // send to vpos payment
                    var tokenCode = PaymentTokenManager.AddToken(new PaymentTokenManager.PacketExtentionToken()
                    {
                        ExtentionPeriodCount = extendPackage.AddedPeriods,
                        ClientId = dbSubscription.ID,
                        ReturnUrl = Url.Action("Details", "Client", new { id = dbSubscription.ID }, Request.Url.Scheme) + "#bills",
                        Amount = dbSubscription.Service.Price * extendPackage.AddedPeriods,
                        ClientAddress = dbSubscription.Address.AddressText,
                        ClientName = dbSubscription.ValidDisplayName,
                        ClientTel = dbSubscription.Customer.ContactPhoneNo,
                        Language = dbSubscription.Customer.Culture.Split('-')[0],
                        SubscriberNo = dbSubscription.SubscriberNo,
                        ServiceName = dbSubscription.Service.Name
                    });

                    return RedirectToAction("Index", "VPOSPayment", new { id = tokenCode });
                }

                var results = db.ExtendClientPackage(dbSubscription, extendPackage.AddedPeriods, paymentType.Value, User.GiveAccountantType(), User.GiveUserId());
                if (results == BillPayment.ResponseType.NotEnoughCredit)
                    return RedirectToAction("ExtendPackage", "Client", new { id = dbSubscription.ID, errorMessage = 12 });

                db.SystemLogs.Add(SystemLogProcessor.ExtendPackage(User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null, extendPackage.AddedPeriods));

                SMSServiceAsync SMSAsync = new SMSServiceAsync();
                db.SMSArchives.AddSafely(SMSAsync.SendSubscriberSMS(dbSubscription, SMSType.ExtendPackage, new Dictionary<string, object>()
                {
                    { SMSParamaterRepository.SMSParameterNameCollection.ExtendedMonths, extendPackage.AddedPeriods.ToString() }
                }));

                db.SaveChanges();

                if (User.IsInRole("cashier"))
                    return RedirectToAction("Index", "Home", new { errorMessage = 0 });

                if (HasPrintRequested)
                {
                    var issuedBill = dbSubscription.Bills.OrderByDescending(bill => bill.PayDate).FirstOrDefault();
                    if (issuedBill != null)
                    {
                        return RedirectToAction("Receipt", "Bill", new { billId = issuedBill.ID, redirectUrl = Url.Action("Details", "Bill", new { id = issuedBill.ID }, Request.Url.Scheme) + "#bills" });
                    }
                }
                return Redirect(Url.Action("Details", "Client", new { id = dbSubscription.ID, errorMessage = 0 }) + "#bills");
            }

            extendPackage._totalFee = (dbSubscription.Fees.Where(fee => fee.FeeTypeCost.IsAllTime && fee.FeeTypeID != (short)FeeType.Tariff).Sum(fee => fee.FeeTypeCost.Cost ?? fee.FeeTypeVariant.Price) + dbSubscription.Service.Price) * extendPackage.AddedPeriods;

            ViewBag.PaymentType = paymentType;
            return View(extendPackage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Clients")]
        [AuthorizePermission(Permissions = "SMS")]
        // POST: Client/SendCredentials
        public ActionResult SendCredentials(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            SMSService SMS = new SMSService();
            db.SMSArchives.AddSafely(SMS.SendSubscriberSMS(dbSubscription, SMSType.UserCredentials));
            db.SaveChanges();
            return Redirect(Url.Action("Details", new { id = id }) + "#sms");
        }

        [AuthorizePermission(Permissions = "Clients")]
        [HttpPost]
        [OutputCache(Duration = 10)]
        public async Task<ActionResult> GetOnlineUsers()
        {
            var dbNASes = db.NAS.Where(nas => !nas.SubNASes.Any() && !nas.Disabled).ToList();
            var credentials = dbNASes.Select(credential => new MikrotikApiCredentials(credential.IP, 8728, credential.ApiUsername, credential.ApiPassword)).ToArray();
            var results = await credentials.GetOnlineUsersList();
            results = results.Select(res => new UserListResults() { Users = res.Users, NASIP = dbNASes.FirstOrDefault(nas => nas.IP == res.NASIP).Name + "(" + res.NASIP + ")" });
            return Json(results);
        }

        [AuthorizePermission(Permissions = "Clients")]
        // Get: Client/ConnectionHistory
        public ActionResult ConnectionHistory(long id, int? page)
        {
            var dbSubscription = db.Subscriptions.FirstOrDefault(client => client.ID == id);
            if (dbSubscription == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._4);
            }

            var viewResults = db.RadiusAccountings.Include(ra => ra.RadiusAccountingIPInfo).Where(ra => ra.SubscriptionID == dbSubscription.ID).OrderByDescending(ra => ra.StartTime).Select(ra => new AccountingRecord()
            {
                ID = ra.ID,
                NasName = ra.NASIP,
                StartTime = ra.StartTime,
                _stopTime = ra.StopTime,
                _updateTime = ra.UpdateTime,
                RealIP = ra.RadiusAccountingIPInfo != null ? ra.RadiusAccountingIPInfo.RealIP : "-",
                LocalIP = ra.RadiusAccountingIPInfo != null ? ra.RadiusAccountingIPInfo.LocalIP : ra.FramedIPAddress,
                CallingStation = ra.CallingStationID
            });

            SetupPages(page, ref viewResults);

            var nases = db.NAS.Select(nas => new { IP = nas.IP, Name = nas.Name }).ToArray();
            var results = viewResults.ToList();

            foreach (var item in results)
            {
                item.NasName = nases.FirstOrDefault(nas => nas.IP == item.NasName) != null ? nases.FirstOrDefault(nas => nas.IP == item.NasName).Name : item.NasName;
            }

            return View(results);
        }

        [AuthorizePermission(Permissions = "Clients")]
        [AjaxCall]
        public ActionResult UnstableClientCount()
        {
            ViewBag.UnstableClientCount = db.Subscriptions.FilterUnstableConnections().Count();
            return View();
        }

        [AuthorizePermission(Permissions = "Clients")]
        public ActionResult UnstableClients(int? page)
        {
            var dbSubscription = db.Subscriptions.OrderByDescending(c => c.ID).AsQueryable();
            dbSubscription = dbSubscription.FilterUnstableConnections();

            SetupPages(page, ref dbSubscription);

            var viewResults = dbSubscription.Select(s => new SubscriptionListDisplayViewModel()
            {
                Name = s.Customer.CorporateCustomerInfo != null ? s.Customer.CorporateCustomerInfo.Title : s.Customer.FirstName + " " + s.Customer.LastName,
                Username = s.Username,
                SubscriberNo = s.SubscriberNo,
                ID = s.ID,
                ContactPhoneNo = s.Customer.ContactPhoneNo,
                DSLNo = s.SubscriptionTelekomInfo != null ? s.SubscriptionTelekomInfo.SubscriptionNo : null,
                TariffName = s.Service.Name,
                State = s.State
            }).ToArray();

            return View(viewResults);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizePermission(Permissions = "Force Change Service")]
        //// POST: Client/ForceChangeService
        //public ActionResult ForceChangeService(long id)
        //{
        //    var dbSubscription = db.Subscriptions.Find(id);
        //    if (dbSubscription == null)
        //    {
        //        return RedirectToAction("Index", new { errorMessage = 4 });
        //    }
        //    if (!dbSubscription.ChangeServiceTypeTasks.Any())
        //    {
        //        return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
        //    }

        //    dbSubscription.ServiceID = dbSubscription.ChangeServiceTypeTasks.FirstOrDefault().NewServiceID;
        //    var tasks = dbSubscription.ChangeServiceTypeTasks.Select(task => task.SchedulerTask).ToArray();
        //    db.ChangeServiceTypeTasks.RemoveRange(dbSubscription.ChangeServiceTypeTasks);
        //    db.SchedulerTasks.RemoveRange(tasks);
        //    db.SystemLogs.Add(SystemLogProcessor.ForceChangeService(User.GiveUserId(), dbSubscription.ID));
        //    db.SaveChanges();

        //    return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
        //}

        [AuthorizePermission(Permissions = "Cancel Payment")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/CancelPayment
        public ActionResult CancelPayment(long id, string returnUrl)
        {
            var dbBill = db.Bills.Find(id);
            var uri = new UriBuilder(returnUrl);
            if (dbBill == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "8", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var results = db.CancelPayment(new[] { dbBill });
            if (results == BillPayment.ResponseType.HasCashierPays)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            db.SaveChanges();
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Client Files")]
        public ActionResult Files(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Details", new { errorMessage = 4 });
            }
            ViewBag.ClientName = dbSubscription.ValidDisplayName;
            ViewBag.ClientID = id;
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetClientAttachmentsList(id);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            
            var viewResults = result.Result.Select(file => new SavedFileViewModel()
            {
                FileName = file.ServerSideName,
                CreationDate = file.CreationDate,
                FileExtention = file.FileExtention,
                AttachmentType = (short)file.AttachmentType
            });

            ViewBag.AttachmentTypes = new SelectList(new LocalizedList<ClientAttachmentTypes, RadiusR.Localization.Lists.ClientAttachmentTypes>().GetList(), "Key", "Value");

            return View(viewResults.OrderBy(r => r.CreationDate).ToList());
        }

        [AuthorizePermission(Permissions = "Client Files")]
        public ActionResult DownloadFile(long id, string fileName)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetClientAttachment(id, fileName);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            return File(result.Result.Content, "application/octet-stream", result.Result.FileDetail.ServerSideName);
        }

        [AuthorizePermission(Permissions = "Client Files")]
        public ActionResult ViewFile(long id, string fileName)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetClientAttachment(id, fileName);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            return File(result.Result.Content, result.Result.FileDetail.MIMEType);
        }

        [AuthorizePermission(Permissions = "Edit Client Files")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UploadAttachment(long id, HttpPostedFileBase newAttachment, short? typeId)
        {
            if (!typeId.HasValue || !Enum.IsDefined(typeof(ClientAttachmentTypes), (int)typeId))
            {
                return RedirectToAction("Files", new { id = id, errorMessage = 9 });
            }
            var attachmentType = (ClientAttachmentTypes)typeId.Value;
            if (newAttachment == null)
                return RedirectToAction("Files", new { id = id, errorMessage = 9 });
            var fileType = newAttachment.FileName.Split('.').LastOrDefault();
            if (!IsValidAttachmentFileType(fileType))
                return RedirectToAction("Files", new { id = id, errorMessage = 9 });
            var fileManager = new MasterISSFileManager();
            var newFile = new FileManagerClientAttachmentWithContent(newAttachment.InputStream, new FileManagerClientAttachment(attachmentType, fileType));
            var result = fileManager.SaveClientAttachment(id, newFile);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            db.SystemLogs.Add(SystemLogProcessor.AddFile(newFile.FileDetail.ServerSideName, User.GiveUserId(), id, SystemLogInterface.MasterISS, null));
            db.SaveChanges();
            return RedirectToAction("Files", new { id = id, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Edit Client Files")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveAttachment(long id, string fileName)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.RemoveClientAttachment(id, fileName);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            db.SystemLogs.Add(SystemLogProcessor.RemoveFile(fileName, User.GiveUserId(), id, SystemLogInterface.MasterISS, null));
            db.SaveChanges();
            return RedirectToAction("Files", new { id = id, errorMessage = 0 });
        }

        // GET: Client/WorkOrders
        public ActionResult WorkOrders(long id, int? page)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._4);
            }

            var viewResults = db.CustomerSetupTasks.Where(task => task.SubscriptionID == dbSubscription.ID).OrderByDescending(task => task.TaskIssueDate).Select(task => new CustomerSetupServiceTaskViewModel()
            {
                ID = task.ID,
                CompletionDate = task.CompletionDate,
                Details = task.Details,
                IssueDate = task.TaskIssueDate,
                AllowanceState = task.AllowanceState,
                Status = task.TaskStatus,
                TaskType = task.TaskType,
                User = task.CustomerSetupUser.Name
            }).AsQueryable();

            SetupPages(page, ref viewResults);

            ViewBag.ErrorMessage = TempData.ContainsKey("ErrorMessage") ? TempData["ErrorMessage"] : null;
            ViewBag.SuccessMessage = TempData.ContainsKey("SuccessMessage") ? TempData["SuccessMessage"] : null;
            ViewBag.ClientID = dbSubscription.ID;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Create Setup Task")]
        [HttpGet]
        // GET: Client/AddWorkOrder
        public ActionResult AddWorkOrder(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }

            ViewBag.ClientID = dbSubscription.ID;
            ViewBag.SetupServiceOperators = new SelectList(db.CustomerSetupUsers.ActiveUsers().ValidPartnersForArea(dbSubscription.Address).OrderBy(user => user.Name).Select(user => new { ID = user.ID, Name = user.Name }), "ID", "Name");
            var task = new EditSetupServiceTaskViewModel()
            {
                ClientName = dbSubscription.ValidDisplayName,
                TaskType = (short)RadiusR.DB.Enums.CustomerSetup.TaskTypes.Fault
            };

            return View(task);
        }

        [AuthorizePermission(Permissions = "Create Setup Task")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/AddWorkOrder
        public ActionResult AddWorkOrder(long id, [Bind(Include = "TaskType,SetupUserID,HasModem,ModemName,XDSLType,TaskDescription")] EditSetupServiceTaskViewModel task)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                var setupUser = db.CustomerSetupUsers.Find(task.SetupUserID);
                if (setupUser == null)
                {
                    ModelState.AddModelError("SetupUserID", RadiusR.Localization.Validation.Common.InvalidInput);
                }
                else
                {
                    task.ModemName = task.HasModem ? task.ModemName : null;
                    var dbWorkOrder = new CustomerSetupTask()
                    {
                        SubscriptionID = dbSubscription.ID,
                        Details = task.TaskDescription,
                        HasModem = task.HasModem,
                        ModemName = task.ModemName,
                        SetupUserID = task.SetupUserID.Value,
                        TaskType = task.TaskType,
                        XDSLType = task.XDSLType,
                        TaskIssueDate = DateTime.Now,
                        TaskStatus = (short)RadiusR.DB.Enums.CustomerSetup.TaskStatuses.New,
                        Allowance = setupUser.Partners.Any() ? setupUser.Partners.First().SetupAllowance : (decimal?)null,
                        AllowanceState = (short)PartnerAllowanceState.OnHold
                    };
                    db.CustomerSetupTasks.Add(dbWorkOrder);

                    db.SaveChanges();
                    db.SystemLogs.Add(SystemLogProcessor.AddWorkOrder(dbWorkOrder.ID, User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null));
                    db.SaveChanges();
                    return Redirect(Url.Action("Details", "Client", new { id = id, errorMessage = 0 }) + "#faults");
                }
            }

            ViewBag.ClientID = dbSubscription.ID;
            ViewBag.SetupServiceOperators = new SelectList(db.CustomerSetupUsers.ActiveUsers().ValidPartnersForArea(dbSubscription.Address).OrderBy(user => user.Name).Select(user => new { ID = user.ID, Name = user.Name }), "ID", "Name", task.SetupUserID);
            task.ClientName = dbSubscription.ValidDisplayName;

            return View(task);
        }

        [AuthorizePermission(Permissions = "Create Setup Task")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/RemoveWorkOrder
        public ActionResult RemoveWorkOrder(long id)
        {
            var task = db.CustomerSetupTasks.Find(id);
            if (task == null || task.TaskStatus != (short)RadiusR.DB.Enums.CustomerSetup.TaskStatuses.New)
            {
                TempData.Add("ErrorMessage", RadiusR.Localization.Pages.ErrorMessages._9);
                return RedirectToAction("WorkOrders", "Client", new { id = task.SubscriptionID });
            }

            var subscriptionId = task.SubscriptionID;
            db.CustomerSetupTasks.Remove(task);
            db.SystemLogs.Add(SystemLogProcessor.RemoveWorkOrder(id, User.GiveUserId(), subscriptionId, SystemLogInterface.MasterISS, null));
            db.SaveChanges();
            TempData.Add("SuccessMessage", RadiusR.Localization.Pages.ErrorMessages._0);
            return RedirectToAction("WorkOrders", "Client", new { id = subscriptionId, errorMessage = 0 });
        }

        //[AuthorizePermission(Permissions = "Client Files")]
        //// GET: Client/DownloadContract
        //public ActionResult DownloadContract(long id)
        //{
        //    var subscription = db.Subscriptions.Find(id);
        //    if (subscription == null)
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        //    var createdPDF = RadiusR.PDFForms.PDFWriter.GetContractPDF(db, id);
        //    if (createdPDF.InternalException != null)
        //    {
        //        return Content(RadiusR.Localization.Pages.Common.FileManagerError);
        //    }
        //    return File(createdPDF.Result, "application/pdf", string.Format(RadiusR.Localization.Pages.Common.ContractFileName, subscription.SubscriberNo));
        //}

        [AuthorizePermission(Permissions = "Quota Sale")]
        [HttpGet]
        // GET: Client/AddQuota
        public ActionResult AddQuota(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!subscription.Service.CanHaveQuotaSale)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            ViewBag.CustomerName = subscription.ValidDisplayName;
            ViewBag.Packages = new SelectList(db.QuotaPackages.Select(package => new
            {
                ID = package.ID,
                Name = package.Name
            }), "ID", "Name");
            return View(new SellQuotaViewModel());
        }

        [AuthorizePermission(Permissions = "Quota Sale")]
        [HttpPost]
        // POST: Client/AddQuota
        public ActionResult AddQuota(long id, SellQuotaViewModel package)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!subscription.Service.CanHaveQuotaSale)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            if (ModelState.IsValid)
            {
                var dbPackage = db.QuotaPackages.Find(package.PacketID);
                if (dbPackage == null)
                {
                    return RedirectToAction("Details", new { id = id, errorMessage = 9 });
                }

                var quotaDescription = RateLimitFormatter.ToQuotaDescription(dbPackage.Amount, dbPackage.Name);

                subscription.SubscriptionQuotas.Add(new SubscriptionQuota()
                {
                    AddDate = DateTime.Now,
                    Amount = dbPackage.Amount
                });
                subscription.Fees.Add(new Fee()
                {
                    Cost = dbPackage.Price,
                    Date = DateTime.Now,
                    FeeTypeID = (short)FeeType.Quota,
                    InstallmentBillCount = 1,
                    Description = quotaDescription
                });

                db.SystemLogs.Add(SystemLogProcessor.AddSubscriptionQuota(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null, quotaDescription));
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id, errorMessage = 0 });
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            ViewBag.Packages = new SelectList(db.QuotaPackages.Select(p => new
            {
                ID = p.ID,
                Name = p.Name
            }), "ID", "Name");
            return View(new SellQuotaViewModel());
        }

        [AuthorizePermission(Permissions = "Clients")]
        // GET: Client/QuotaDetails
        public ActionResult QuotaDetails(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!subscription.Service.QuotaType.HasValue)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var currentPeriod = subscription.GetCurrentBillingPeriod();

            var viewResults = subscription.SubscriptionQuotas.Select(q => new QuotaDetailViewModel()
            {
                AddDate = q.AddDate,
                ItemName = RadiusR.Localization.Pages.Common.Quota,
                ItemValue = q.Amount
            }).ToList();
            viewResults.Insert(0, new QuotaDetailViewModel()
            {
                AddDate = currentPeriod.StartDate,
                ItemName = subscription.Service.Name,
                ItemValue = subscription.Service.BaseQuota ?? 0
            });
            viewResults.AddRange(new QuotaDetailViewModel[]
            {
                new QuotaDetailViewModel()
                {
                    ItemName = RadiusR.Localization.Pages.Common.Total,
                    ItemValue = (decimal)subscription.GetPeriodQuota(currentPeriod, db)
                },
                new QuotaDetailViewModel()
                {
                    ItemName = RadiusR.Localization.Pages.Common.Usage,
                    ItemValue = subscription.GetPeriodUsageInfo(currentPeriod, db).Total
                },
                new QuotaDetailViewModel()
                {
                    ItemName = RadiusR.Localization.Model.RadiusR.RemainingQuota,
                    ItemValue = (decimal)subscription.GetRemainingQuota(currentPeriod, db)
                }
            });

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewResults);
        }



        private string FixCasing(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            var capitalizeNamesRegex = new Regex(@"(^\w)|((?<=\s)\w)");
            return capitalizeNamesRegex.Replace(input.ToLower(), value => value.Value.ToUpper());
        }
    }
}