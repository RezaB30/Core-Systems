using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using RadiusR.DB.Enums;
using RadiusR.DB.Enums.CustomerSetup;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;
using RezaB.Web;
using RadiusR.SystemLogs;
using RadiusR.DB.Utilities.Extentions;

namespace RadiusR_Manager.Controllers
{
    public class CustomerSetupServiceController : BaseController
    {
        private RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Customer Setup Service")]
        // GET: CustomerSetupService
        public ActionResult Index(int? page, [Bind(Prefix = "search")] CustomerSetupTaskSearchViewModel search)
        {
            search = search ?? new CustomerSetupTaskSearchViewModel();
            var setupTasks = db.CustomerSetupTasks.OrderByDescending(task => task.ID).Include(task => task.CustomerSetupUser).Include(task => task.Subscription).Include(task => task.CustomerSetupStatusUpdates);

            if (search.OperatorID.HasValue)
            {
                setupTasks = setupTasks.Where(task => task.SetupUserID == search.OperatorID);
            }
            if (search.TaskType > 0)
            {
                setupTasks = setupTasks.Where(task => task.TaskType == search.TaskType);
            }
            if (search.TaskState > 0)
            {
                setupTasks = setupTasks.Where(task => task.TaskStatus == search.TaskState);
            }
            if (search.StartDate.HasValue)
            {
                setupTasks = setupTasks.Where(task => DbFunctions.TruncateTime(task.TaskIssueDate) >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                setupTasks = setupTasks.Where(task => DbFunctions.TruncateTime(task.TaskIssueDate) <= search.EndDate);
            }

            SetupPages(page, ref setupTasks);
            var viewResults = setupTasks.ToList().Select(task => new CustomerSetupServiceTaskViewModel()
            {
                ID = task.ID,
                CompletionDate = task.CompletionDate,
                Details = task.Details,
                HasModem = task.HasModem,
                IssueDate = task.TaskIssueDate,
                ModemName = task.ModemName,
                Status = task.TaskStatus,
                TaskType = task.TaskType,
                XDSLType = task.XDSLType,
                AllowanceState = task.AllowanceState,
                ReservationDate = task.CustomerSetupStatusUpdates.Any() ? task.CustomerSetupStatusUpdates.LastOrDefault().ReservationDate : null,
                User = task.CustomerSetupUser.Name,
                Client = new SubscriptionListDisplayViewModel()
                {
                    ID = task.SubscriptionID,
                    Name = task.Subscription.Customer.CorporateCustomerInfo != null ? task.Subscription.Customer.CorporateCustomerInfo.Title : task.Subscription.Customer.FirstName + " " + task.Subscription.Customer.LastName,
                    SubscriberNo = task.Subscription.SubscriberNo,
                    State = task.Subscription.State,
                    Username = task.Subscription.RadiusAuthorization.Username
                }
            });
            ViewBag.Search = search;
            ViewBag.Operators = new SelectList(db.CustomerSetupUsers.Select(u => new { ID = u.ID, Name = u.Name }), "ID", "Name", search.OperatorID);
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        // GET: CustomerSetupService/Users
        public ActionResult Users(int? page)
        {
            var viewResults = db.CustomerSetupUsers.OrderByDescending(user => user.ID).Select(user => new CustomerSetupUserViewModel()
            {
                ID = user.ID,
                Name = user.Name,
                Username = user.Username,
                IsEnabled = user.IsEnabled,
                HasAssignedTasks = user.CustomerSetupTasks.Any(),
                IsAgentUser = user.Agents.Any()
            });

            SetupPages(page, ref viewResults);

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CustomerSetupService/ChangeState
        public ActionResult ChangeState(int id)
        {
            var user = db.CustomerSetupUsers.Find(id);
            if (user == null)
            {
                return RedirectToAction("Users", new { errorMessage = 9 });
            }

            user.IsEnabled = !user.IsEnabled;

            db.SaveChanges();

            return RedirectToAction("Users", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CustomerSetupService/Remove
        public ActionResult Remove(int id)
        {
            var user = db.CustomerSetupUsers.Find(id);
            if (user == null || user.CustomerSetupTasks.Any() || user.Agents.Any())
            {
                return RedirectToAction("Users", new { errorMessage = 9 });
            }

            db.CustomerSetupUsers.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Users", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpGet]
        // GET: CustomerSetupService/Edit
        public ActionResult EditUser(int id)
        {
            var user = db.CustomerSetupUsers.Find(id);
            if (user == null)
            {
                return RedirectToAction("Users", new { errorMessage = 9 });
            }

            var results = new CustomerSetupUserViewModel()
            {
                ID = user.ID,
                Name = user.Name,
                Username = user.Username
            };

            return View(results);
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: CustomerSetupService/Edit
        public ActionResult EditUser(int id, [Bind(Include = "Name,Username")] CustomerSetupUserViewModel user)
        {
            ModelState.Remove("Password");
            var dbUser = db.CustomerSetupUsers.Find(id);
            if (dbUser == null)
            {
                return RedirectToAction("Users", new { errorMessage = 9 });
            }
            // prevent duplicate username
            if (db.CustomerSetupUsers.Where(u => u.ID != id).Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", RadiusR.Localization.Validation.Common.UsernameExists);
            }

            if (ModelState.IsValid)
            {
                dbUser.Name = user.Name;
                dbUser.Username = user.Username;

                db.SaveChanges();

                return RedirectToAction("Users", new { errorMessage = 0 });
            }

            user.ID = id;
            return View(user);
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpGet]
        // GET: CustomerSetupService/ChangePassword
        public ActionResult ChangePassword(int id)
        {
            var user = db.CustomerSetupUsers.Find(id);
            if (user == null)
            {
                return RedirectToAction("Users", new { errorMessage = 9 });
            }

            var results = new CustomerSetupUserViewModel()
            {
                ID = user.ID,
                Name = user.Name,
                Username = user.Username
            };

            return View(results);
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CustomerSetupService/ChangePassword
        public ActionResult ChangePassword(int id, [Bind(Include = "Name,Username,Password")]CustomerSetupUserViewModel user)
        {
            var dbUser = db.CustomerSetupUsers.Find(id);
            if (dbUser == null)
            {
                return RedirectToAction("Users", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbUser.Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(user.Password).ToLower();

                db.SaveChanges();
                return RedirectToAction("Users", new { errorMessage = 0 });
            }

            user.ID = dbUser.ID;
            return View(user);
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpGet]
        // GET: CustomerSetupService/AddUser
        public ActionResult AddUser()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Customer Setup Users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CustomerSetupService/AddUser
        public ActionResult AddUser([Bind(Include = "Name,Username,Password")] CustomerSetupUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (db.CustomerSetupUsers.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", RadiusR.Localization.Validation.Common.UsernameExists);
                    return View(user);
                }

                db.CustomerSetupUsers.Add(new CustomerSetupUser()
                {
                    IsEnabled = true,
                    Name = user.Name,
                    Username = user.Username,
                    Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(user.Password).ToLower()
                });

                db.SaveChanges();

                return RedirectToAction("Users", new { errorMessage = 0 });
            }

            return View(user);
        }

        //[AuthorizePermission(Permissions = "Setup Task Service Fee")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //// POST: CustomerSetupService/AddServiceFee
        //public ActionResult AddServiceFee(long id, string page)
        //{
        //    page = page ?? "0";
        //    var task = db.CustomerSetupTasks.Find(id);
        //    if (task == null || task.TaskStatus != (short)TaskStatuses.Completed || task.IsCharged)
        //    {
        //        return RedirectToAction("Index", new { errorMessage = 9 });
        //    }
        //    //add fee
        //    var serviceFee = db.FeeTypeCosts.Find((short)FeeType.Service);
        //    task.Subscription.Fees.Add(new Fee()
        //    {
        //        FeeTypeID = serviceFee.FeeTypeID,
        //        Cost = serviceFee.Cost,
        //        InstallmentBillCount = 1,
        //        Date = DateTime.Now
        //    });
        //    task.IsCharged = true;

        //    db.SaveChanges();

        //    return RedirectToAction("Index", new { errorMessage = 0, page = page });
        //}

        [HttpGet]
        // GET: CustomerSetupService/AddNewTask
        public ActionResult AddNewTask(long id)
        {
            var dbClient = db.Subscriptions.Find(id);
            if (dbClient == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }

            var task = new EditSetupServiceTaskViewModel()
            {
                ClientName = dbClient.ValidDisplayName
            };
            return View();
        }

        [AuthorizePermission(Permissions = "Customer Setup Service,Clients")]
        // GET: CustomerSetupService/Details
        public ActionResult Details(long id, string redirectUrl)
        {
            var uri = redirectUrl != null ? new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl) : new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));

            var task = db.CustomerSetupTasks.Find(id);
            if (task == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var viewResults = new SetupServiceTaskDetailsViewModel()
            {
                ID = task.ID,
                ClientName = task.Subscription.ValidDisplayName,
                HasModem = task.HasModem,
                IssueDate = task.TaskIssueDate,
                AllowanceState = task.AllowanceState,
                ModemName = task.ModemName,
                User = task.CustomerSetupUser.Name,
                Details = task.Details,
                TaskType = task.TaskType,
                XDSLType = task.XDSLType,
                Status = task.TaskStatus,
                Stages = task.CustomerSetupStatusUpdates.Select(update => new SetupServiceTaskDetailsViewModel.Stage()
                {
                    Date = update.Date,
                    Details = update.Description,
                    Status = update.FaultCode,
                    ReservationDate = update.ReservationDate
                })
            };

            ViewBag.BackLink = uri.Uri.PathAndQuery + uri.Fragment;

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Close Setup Task")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CustomerSetupService/CompleteTask
        public ActionResult CompleteTask(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            var task = db.CustomerSetupTasks.Find(id);
            if (task == null || !task.IsActive)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            task.CompleteCustomerSetupTask();
            db.SystemLogs.Add(SystemLogProcessor.CloseWorkOrder(task.ID, User.GiveUserId(), task.SubscriptionID, SystemLogInterface.MasterISS, null));
            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Close Setup Task")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CustomerSetupService/CancelTask
        public ActionResult CancelTask(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            var task = db.CustomerSetupTasks.Find(id);
            if (task == null || !task.IsActive)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            task.CancelCustomerSetupTask();
            db.SystemLogs.Add(SystemLogProcessor.CloseWorkOrder(task.ID, User.GiveUserId(), task.SubscriptionID, SystemLogInterface.MasterISS, null));
            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }
    }
}