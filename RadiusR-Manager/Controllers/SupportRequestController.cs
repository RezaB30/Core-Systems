using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR_Manager.Models.ViewModels.Customer;
using System.Data.Entity;
using RezaB.Web.CustomAttributes;
using RezaB.Web;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Support Requests")]
    public class SupportRequestController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        // GET: SupportRequest
        public ActionResult Index(int? page, SupportRequestSearchViewModel search)
        {
            var viewResults = db.SubscriptionSupportRequests.OrderByDescending(request => request.Date).Select(request => new SupportRequestViewModel()
            {
                SubscriptionID = request.SubscriptionID,
                Date = request.Date,
                ID = request.ID,
                Message = request.Message,
                StateID = request.StateID,
                CustomerSetupTaskID = request.CustomerSetupTaskID,
                IssuerID = request.IssuerID,
                IssuerName = request.IssuerID.HasValue ? request.AppUser.Name : RadiusR.Localization.Model.RadiusR.Customer,
                Subscription = new SubscriptionListDisplayViewModel()
                {
                    ID = request.SubscriptionID,
                    Name = request.Subscription.Customer.CorporateCustomerInfo != null ? request.Subscription.Customer.CorporateCustomerInfo.Title : request.Subscription.Customer.FirstName + " " + request.Subscription.Customer.LastName
                }
            }).AsQueryable();

            if (search.DateStart.HasValue)
            {
                viewResults = viewResults.Where(sr => DbFunctions.TruncateTime(sr.Date) >= search.DateStart);
            }
            if (search.DateEnd.HasValue)
            {
                viewResults = viewResults.Where(sr => DbFunctions.TruncateTime(sr.Date) <= search.DateEnd);
            }
            if (search.Issuer.HasValue)
            {
                if (search.Issuer == (short)SupportRequestSearchViewModel.IssuerType.Customer)
                {
                    viewResults = viewResults.Where(sr => !sr.IssuerID.HasValue);
                }
                if (search.Issuer == (short)SupportRequestSearchViewModel.IssuerType.Operator)
                {
                    viewResults = viewResults.Where(sr => sr.IssuerID.HasValue);
                }
            }
            if (search.StateID.HasValue)
            {
                viewResults = viewResults.Where(sr => sr.StateID == search.StateID);
            }

            SetupPages(page, ref viewResults);

            ViewBag.Search = search;
            return View(viewResults.ToList());
        }

        [AuthorizePermission(Permissions = "Create Support Request")]
        [HttpGet]
        // GET: SupportRequest/Add
        public ActionResult Add(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }
            if (dbSubscription.SubscriptionSupportRequests.Any(r => r.StateID == (short)SubscriptionSupportRequestStateID.Sent || r.StateID == (short)SubscriptionSupportRequestStateID.Assigned))
            {
                return Redirect(Url.Action("Details", "Client", new { errorMessage = 36, id = id }) + "#faults");
            }
            var request = new SupportRequestViewModel()
            {
                SubscriptionID = dbSubscription.ID,
                Subscription = new SubscriptionListDisplayViewModel()
                {
                    ID = dbSubscription.ID,
                    Name = dbSubscription.Customer.CorporateCustomerInfo != null ? dbSubscription.Customer.CorporateCustomerInfo.Title : dbSubscription.Customer.FirstName + " " + dbSubscription.Customer.LastName
                }
            };
            return View(request);
        }

        [AuthorizePermission(Permissions = "Create Support Request")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/Add
        public ActionResult Add(long id, [Bind(Include = "Message")]SupportRequestViewModel request)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }
            if (dbSubscription.SubscriptionSupportRequests.Any(r => r.StateID == (short)SubscriptionSupportRequestStateID.Sent || r.StateID == (short)SubscriptionSupportRequestStateID.Assigned))
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 36 });
            }
            if (ModelState.IsValid)
            {
                dbSubscription.SubscriptionSupportRequests.Add(new SubscriptionSupportRequest()
                {
                    Date = DateTime.Now,
                    Message = request.Message,
                    IssuerID = User.GiveUserId(),
                    StateID = (short)SubscriptionSupportRequestStateID.Sent
                });

                db.SaveChanges();

                return RedirectToAction("Index", "SupportRequest", new { errorMessage = 0 });
            }

            request = new SupportRequestViewModel()
            {
                SubscriptionID = dbSubscription.ID,
                Subscription = new SubscriptionListDisplayViewModel()
                {
                    ID = dbSubscription.ID,
                    Name = dbSubscription.Customer.CorporateCustomerInfo != null ? dbSubscription.Customer.CorporateCustomerInfo.Title : dbSubscription.Customer.FirstName + " " + dbSubscription.Customer.LastName
                },
                Message = request.Message
            };
            return View(request);
        }

        // GET: SupportRequest/Details
        public ActionResult Details(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var dbSupportRequest = db.SubscriptionSupportRequests.FirstOrDefault(request => request.ID == id);
            if (dbSupportRequest == null)
            {
                return RedirectToAction("Index", new { errorMessage = 15 });
            }

            var supportRequest = new SupportRequestViewModel()
            {
                ID = dbSupportRequest.ID,
                SubscriptionID = dbSupportRequest.SubscriptionID,
                Date = dbSupportRequest.Date,
                Message = dbSupportRequest.Message,
                StateID = dbSupportRequest.StateID,
                CustomerSetupTaskID = dbSupportRequest.CustomerSetupTaskID,
                AdminMessage = dbSupportRequest.SupportResponse,
                IssuerID = dbSupportRequest.IssuerID,
                IssuerName = dbSupportRequest.IssuerID.HasValue ? dbSupportRequest.AppUser.Name : null,
                Subscription = new SubscriptionListDisplayViewModel()
                {
                    ID = dbSupportRequest.SubscriptionID,
                    Name = dbSupportRequest.Subscription.Customer.CorporateCustomerInfo != null ? dbSupportRequest.Subscription.Customer.CorporateCustomerInfo.Title : dbSupportRequest.Subscription.Customer.FirstName + " " + dbSupportRequest.Subscription.Customer.LastName,
                    InstallationAddress = new AddressViewModel(dbSupportRequest.Subscription.Address),
                    Username = dbSupportRequest.Subscription.Username,
                    State = dbSupportRequest.Subscription.State,
                    TariffName = dbSupportRequest.Subscription.Service.Name,
                    ContactPhoneNo = dbSupportRequest.Subscription.Customer.ContactPhoneNo
                },
                SetupServiceTask = (dbSupportRequest.CustomerSetupTaskID.HasValue) ? new CustomerSetupServiceTaskViewModel()
                {
                    Details = dbSupportRequest.CustomerSetupTask.Details,
                    ID = dbSupportRequest.ID,
                    TaskType = dbSupportRequest.CustomerSetupTask.TaskType,
                    IssueDate = dbSupportRequest.CustomerSetupTask.TaskIssueDate,
                    CompletionDate = dbSupportRequest.CustomerSetupTask.CompletionDate,
                    User = dbSupportRequest.CustomerSetupTask.CustomerSetupUser.Name,
                    Status = dbSupportRequest.CustomerSetupTask.TaskStatus
                } : null
            };

            return View(supportRequest);
        }

        [HttpGet]
        // GET: SupportRequest/FinishRequest
        public ActionResult FinishRequest(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var requestMessage = new SupportRequestMessageViewModel();
            requestMessage.RequestID = id;
            return View(requestMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/FinishRequest
        public ActionResult FinishRequest(long id, [Bind(Include = "RequestID,Message")]SupportRequestMessageViewModel requestMessage, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            if (ModelState.IsValid)
            {
                var dbSupportRequest = db.SubscriptionSupportRequests.Find(id);
                if (dbSupportRequest == null)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "15", Uri);
                    return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                    //return RedirectToAction("Index", new { errorMessage = 15 });
                }
                if (dbSupportRequest.StateID != (short)SubscriptionSupportRequestStateID.Sent)
                {
                    //UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                    //return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                    return RedirectToAction("Details", new { id = id, errorMessage = 9, returnUrl = returnUrl });
                }

                dbSupportRequest.StateID = (short)SubscriptionSupportRequestStateID.Done;
                dbSupportRequest.SupportResponse = requestMessage.Message;

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                //return RedirectToAction("Details", new { id = id, errorMessage = 0 });
            }

            return View(requestMessage);
        }

        [AuthorizePermission(Permissions = "Create Setup Task")]
        [HttpGet]
        // GET: SupportRequest/AddWorkOrder
        public ActionResult AddWorkOrder(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var supportRequest = db.SubscriptionSupportRequests.Find(id);
            if (supportRequest == null || supportRequest.StateID != (short)SubscriptionSupportRequestStateID.Sent)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                //return RedirectToAction("Index", "SupportRequest", new { errorMessage = 9 });
            }

            var task = new EditSetupServiceTaskViewModel()
            {
                ClientName = supportRequest.Subscription.ValidDisplayName,
                TaskType = (short)RadiusR.DB.Enums.CustomerSetup.TaskTypes.Fault
            };

            ViewBag.SetupServiceOperators = new SelectList(db.CustomerSetupUsers.Where(user => user.IsEnabled).OrderBy(user => user.Name).Select(user => new { ID = user.ID, Name = user.Name }), "ID", "Name");
            ViewBag.RequestID = id;
            return View(task);
        }

        [AuthorizePermission(Permissions = "Create Setup Task")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/AddWorkOrder
        public ActionResult AddWorkOrder(long id, [Bind(Include = "SetupUserID,XDSLType,HasModem,ModemName,TaskDescription")]EditSetupServiceTaskViewModel task, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var supportRequest = db.SubscriptionSupportRequests.Find(id);
            if (supportRequest == null || supportRequest.StateID != (short)SubscriptionSupportRequestStateID.Sent)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                //return RedirectToAction("Index", "SupportRequest", new { errorMessage = 9 });
            }

            task.ClientName = supportRequest.Subscription.ValidDisplayName;
            task.TaskType = (short)RadiusR.DB.Enums.CustomerSetup.TaskTypes.Fault;

            if (ModelState.IsValid)
            {
                supportRequest.CustomerSetupTask = new CustomerSetupTask()
                {
                    SubscriptionID = supportRequest.SubscriptionID,
                    Details = task.TaskDescription,
                    HasModem = task.HasModem,
                    IsCharged = false,
                    ModemName = task.ModemName,
                    SetupUserID = task.SetupUserID.Value,
                    TaskIssueDate = DateTime.Now,
                    TaskStatus = (short)RadiusR.DB.Enums.CustomerSetup.TaskStatuses.New,
                    TaskType = task.TaskType,
                    XDSLType = task.XDSLType
                };
                supportRequest.StateID = (short)SubscriptionSupportRequestStateID.Assigned;

                db.SaveChanges();
                return RedirectToAction("Details", "SupportRequest", new { id = id, errorMessage = 0, returnUrl = Uri.Uri.PathAndQuery + Uri.Fragment });
            }

            ViewBag.SetupServiceOperators = new SelectList(db.CustomerSetupUsers.Where(user => user.IsEnabled).OrderBy(user => user.Name).Select(user => new { ID = user.ID, Name = user.Name }), "ID", "Name", task.SetupUserID);
            ViewBag.RequestID = id;
            return View(task);
        }
    }
}