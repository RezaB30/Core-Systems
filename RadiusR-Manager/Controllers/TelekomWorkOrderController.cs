﻿using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using RadiusR_Manager.Models.RadiusViewModels;
using System.Collections.Concurrent;
using RadiusR.DB.DomainsCache;
using System.Threading.Tasks;
using RadiusR.SystemLogs;
using RezaB.TurkTelekom.WebServices.TTApplication;
using RadiusR.DB.TelekomOperations;
using RadiusR.DB.TelekomOperations.Caching;
using RezaB.Web;
using RezaB.Web.Authentication;
using RadiusR_Manager.Models.Extentions;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;
using NLog;

namespace RadiusR_Manager.Controllers
{
    public class TelekomWorkOrderController : BaseController
    {
        private static Logger stateChangesLogger = LogManager.GetLogger("client-state-changes");
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Telekom Work Orders")]
        [HttpGet]
        // GET: TelekomWorkOrder
        public ActionResult Index(int? page, long? subscriberId, TelekomWorkOrderSearchViewModel search)
        {
            search = search ?? new TelekomWorkOrderSearchViewModel();

            var baseQuery = db.TelekomWorkOrders.Where(two => two.Subscription.SubscriptionTelekomInfo != null).OrderByDescending(two => two.CreationDate).Include(two => two.Subscription.Customer).Include(two => two.Subscription.SubscriptionTelekomInfo).Include(two => two.AppUser);
            if (search.AppUserID.HasValue)
            {
                baseQuery = baseQuery.Where(two => two.AppUserID == search.AppUserID.Value);
            }
            if (search.EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(two => DbFunctions.TruncateTime(two.CreationDate) <= search.EndDate.Value);
            }
            if (search.OperationSubType.HasValue)
            {
                baseQuery = baseQuery.Where(two => two.OperationSubType == search.OperationSubType.Value);
            }
            if (search.OperationType.HasValue)
            {
                baseQuery = baseQuery.Where(two => two.OperationTypeID == search.OperationType.Value);
            }
            if (search.StartDate.HasValue)
            {
                baseQuery = baseQuery.Where(two => DbFunctions.TruncateTime(two.CreationDate) >= search.StartDate.Value);
            }
            if (!subscriberId.HasValue && search.ShowClosed != true)
            {
                baseQuery = baseQuery.Where(two => two.IsOpen);
            }
            if (subscriberId.HasValue)
            {
                baseQuery = baseQuery.Where(two => two.SubscriptionID == subscriberId);
            }
            if (search.Address != null && search.Address.ProvinceID != 0)
            {
                baseQuery = baseQuery.FilterBySetupAddress(search.Address);
            }
            if (search.GroupID.HasValue)
            {
                baseQuery = baseQuery.Where(two => two.Subscription.Groups.Select(g => g.ID).Contains(search.GroupID.Value));
            }
            // state from cache
            var cachedStateList = TelekomWorkOrderCache.GetCachedList();
            // search state
            if (search.State.HasValue)
            {
                var idList = cachedStateList.Where(csl => csl.State == search.State).Select(csl => csl.ID).ToArray();
                baseQuery = baseQuery.Where(two => idList.Contains(two.ID));
            }

            SetupPages(page, ref baseQuery);

            var results = baseQuery.ToArray().Select(two => new TelekomWorkOrderViewModel()
            {
                ID = two.ID,
                AppUserName = two.AppUserID.HasValue ? two.AppUser.Name : "-",
                ClosingDate = two.ClosingDate,
                CreationDate = two.CreationDate,
                OperationSubType = two.OperationSubType,
                OperationType = two.OperationTypeID,
                PhoneNo = two.Subscription.Customer.ContactPhoneNo,
                SubscriberID = two.SubscriptionID,
                SubscriberName = two.Subscription.ValidDisplayName,
                Username = two.Subscription.RadiusAuthorization.Username,
                XDSLNo = two.Subscription.SubscriptionTelekomInfo.SubscriptionNo,
                TelekomCustomerCode = two.Subscription.SubscriptionTelekomInfo.TTCustomerCode,
                DomainID = two.Subscription.DomainID,
                _managementCode = two.ManagementCode,
                _provinceCode = two.ProvinceCode,
                _queueNo = two.QueueNo,
                IsOpen = two.IsOpen
            }).ToArray();
            // setting states
            foreach (var item in results)
            {
                var stateItem = cachedStateList.FirstOrDefault(csl => csl.ID == item.ID);
                item.State = stateItem != null ? stateItem.State : (short?)null;
            }

            // clear search address validations
            {
                var names = ModelState.Where(s => s.Key.StartsWith("search.Address.")).Select(s => s.Key).ToArray();
                foreach (var name in names)
                {
                    ModelState.Remove(name);
                }
            }

            ViewBag.SearchModel = search;
            ViewBag.AppUsers = new SelectList(db.AppUsers.Select(user => new { Value = user.ID, Name = user.Name }).OrderBy(user => user.Name), "Value", "Name", search.AppUserID);
            ViewBag.SubscriptionGroups = new SelectList(db.Groups.OrderBy(g => g.Name).Select(g => new { Name = g.Name, Value = g.ID }).ToArray(), "Value", "Name", search.GroupID);
            return View(results);
        }

        [AuthorizePermission(Permissions = "Telekom Work Orders")]
        [HttpGet]
        // GET: TelekomWorkOrder/OutgoingIndex
        public ActionResult OutgoingIndex(int? page)
        {
            var cachedResults = TelekomWorkOrderCache.GetOutgoingList();
            var viewResults = cachedResults.OrderBy(cr => cr.CreationDate)
                .Select(cr => new OutgoingTransitionViewModel()
                {
                    DomainID = cr.DomainID,
                    DomainName = DomainsCache.GetDomainByID(cr.DomainID)?.Name ?? "-",
                    CounterpartOperator = cr.CounterpartOperator,
                    CreationDate = cr.CreationDate,
                    TransactionID = cr.TransactionID,
                    XDSLNo = cr.XDSLNo,
                    IndividualInfo = cr.IndividualInfo != null ? new OutgoingTransitionViewModel.IndividualCustomerInfo()
                    {
                        FirstName = cr.IndividualInfo.FirstName,
                        LastName = cr.IndividualInfo.LastName,
                        TCKNo = cr.IndividualInfo.TCKNo
                    } : null,
                    CorporateInfo = cr.CorporateInfo != null ? new OutgoingTransitionViewModel.CorporateCustomerInfo()
                    {
                        CompanyTitle = cr.CorporateInfo.CompanyTitle,
                        ExecutiveTCKNo = cr.CorporateInfo.TCKNo,
                        TaxNo = cr.CorporateInfo.TaxNo

                    } : null
                }).AsQueryable();

            SetupPages(page, ref viewResults);

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Telekom Work Orders")]
        // GET: TelekomWorkOrder/Details
        public ActionResult Details(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var workOrder = db.TelekomWorkOrders.Find(id);
            if (workOrder == null || workOrder.Subscription.SubscriptionTelekomInfo == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var result = new TelekomWorkOrderViewModel()
            {
                AppUserName = workOrder.AppUserID.HasValue ? workOrder.AppUser.Name : "-",
                ClosingDate = workOrder.ClosingDate,
                CreationDate = workOrder.CreationDate,
                DomainID = workOrder.Subscription.DomainID,
                ID = workOrder.ID,
                IsOpen = workOrder.IsOpen,
                LastRetryDate = workOrder.LastRetryDate,
                OperationSubType = workOrder.OperationSubType,
                OperationType = workOrder.OperationTypeID,
                PhoneNo = workOrder.Subscription.Customer.ContactPhoneNo,
                SubscriberID = workOrder.SubscriptionID,
                SubscriberName = workOrder.Subscription.ValidDisplayName,
                TelekomCustomerCode = workOrder.Subscription.SubscriptionTelekomInfo.TTCustomerCode,
                Username = workOrder.Subscription.RadiusAuthorization.Username,
                XDSLNo = workOrder.Subscription.SubscriptionTelekomInfo.SubscriptionNo,
                _managementCode = workOrder.ManagementCode,
                _provinceCode = workOrder.ProvinceCode,
                _queueNo = workOrder.QueueNo,
                _transactionID = workOrder.TransactionID
            };

            // state
            {
                var statusClient = new RadiusR.DB.TelekomOperations.Wrappers.TTWorkOrderClient();
                var status = statusClient.GetWorkOrderState(new RadiusR.DB.TelekomOperations.Wrappers.QueryReadyWorkOrder(workOrder));
                result.State = status.State;
                result.CancellationReason = status.CancellationReason;
            }

            return View(result);
        }

        [AuthorizePermission(Permissions = "Telekom Work Orders")]
        // GET: TelekomWorkOrder/OutgoingDetails
        public ActionResult OutgoingDetails(long id, string returnUrl = null)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("OutgoingIndex"));
            if (returnUrl != null)
            {
                uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            }
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var currentTransition = TelekomWorkOrderCache.GetOutgoingList().FirstOrDefault(transition => transition.TransactionID == id);
            if (currentTransition == null)
            {
                return RedirectToAction("OutgoingIndex", new { errorMessage = 9 });
            }
            var transitionViewModel = new OutgoingTransitionViewModel()
            {
                DomainID = currentTransition.DomainID,
                DomainName = DomainsCache.GetDomainByID(currentTransition.DomainID)?.Name ?? "-",
                CounterpartOperator = currentTransition.CounterpartOperator,
                CreationDate = currentTransition.CreationDate,
                TransactionID = currentTransition.TransactionID,
                XDSLNo = currentTransition.XDSLNo,
                IndividualInfo = currentTransition.IndividualInfo != null ? new OutgoingTransitionViewModel.IndividualCustomerInfo()
                {
                    FirstName = currentTransition.IndividualInfo.FirstName,
                    LastName = currentTransition.IndividualInfo.LastName,
                    TCKNo = currentTransition.IndividualInfo.TCKNo
                } : null,
                CorporateInfo = currentTransition.CorporateInfo != null ? new OutgoingTransitionViewModel.CorporateCustomerInfo()
                {
                    CompanyTitle = currentTransition.CorporateInfo.CompanyTitle,
                    ExecutiveTCKNo = currentTransition.CorporateInfo.TCKNo,
                    TaxNo = currentTransition.CorporateInfo.TaxNo

                } : null
            };

            var foundSubscription = db.Subscriptions.Where(s => s.State == (short)RadiusR.DB.Enums.CustomerState.Active || s.State == (short)RadiusR.DB.Enums.CustomerState.Reserved).FirstOrDefault(s => s.SubscriptionTelekomInfo.SubscriptionNo == currentTransition.XDSLNo);
            if (foundSubscription != null)
            {
                transitionViewModel.XDSLNoIsValid = true;
                if (transitionViewModel.IndividualInfo != null)
                {
                    transitionViewModel.IndividualInfo.DBFirstName = foundSubscription.Customer.FirstName;
                    transitionViewModel.IndividualInfo.DBLastName = foundSubscription.Customer.LastName;
                    transitionViewModel.IndividualInfo.DBTCKNo = foundSubscription.Customer.CustomerIDCard?.TCKNo ?? string.Empty;
                }
                if (transitionViewModel.CorporateInfo != null)
                {
                    transitionViewModel.CorporateInfo.DBCompanyTitle = foundSubscription.Customer.CorporateCustomerInfo?.Title ?? string.Empty;
                    transitionViewModel.CorporateInfo.DBExecutiveTCKNo = foundSubscription.Customer.CustomerIDCard?.TCKNo ?? string.Empty;
                    transitionViewModel.CorporateInfo.DBTaxNo = foundSubscription.Customer.CorporateCustomerInfo?.TaxNo ?? string.Empty;
                }
            }

            return View(transitionViewModel);
        }

        [AuthorizePermission(Permissions = "Telekom Work Orders")]
        [AjaxCall]
        // POST: TelekomWorkOrder/GetOutgoingDocuments
        public ActionResult GetOutgoingDocuments(long id)
        {
            var currentTransition = TelekomWorkOrderCache.GetOutgoingList().FirstOrDefault(transition => transition.TransactionID == id);
            if (currentTransition == null)
            {
                return Content($"<div class='text-danger centered'>{RadiusR.Localization.Validation.Common.InvalidInput}</div>");
            }
            var domain = DomainsCache.GetDomainByID(currentTransition.DomainID);
            if (domain == null)
            {
                return Content($"<div class='text-danger centered'>{RadiusR.Localization.Pages.Common.DomainNotFound}</div>");
            }

            var transitionFTPClient = CreateTransitionFTPClient(domain);
            var results = transitionFTPClient.GetOutgoingCustomerDocumentNames(currentTransition.XDSLNo, currentTransition.TransactionID);
            if (results.InternalException != null)
            {
                return Content($"<div class='text-danger centered'>{results.ErrorMessage}</div>");
            }

            ViewBag.DomainID = domain.ID;
            return View(results.Results);
        }

        [AuthorizePermission(Permissions = "Telekom Work Orders")]
        // GET: TelekomWorkOrder/DownloadOutgoingDocument
        public ActionResult DownloadOutgoingDocument(string fileName, string ownFolder, string counterpartFolder, int domainId)
        {
            var domain = DomainsCache.GetDomainByID(domainId);
            var transitionFTPClient = CreateTransitionFTPClient(domain);
            var result = transitionFTPClient.DownloadOutgoingCustomerDocument(new RezaB.TurkTelekom.FTPOperations.Inputs.OutgoingCustomerDocumentInfo()
            {
                FileName = fileName,
                CounterpartFolderName = counterpartFolder,
                OwnFolderName = ownFolder
            });
            if (result.InternalException != null)
            {
                return Content($"<div class='text-danger centered'>{result.ErrorMessage}</div>");
            }

            return File(result.Results, fileName.Contains('.') ? RadiusR.FileManagement.MIMEUtility.GetMIMETypeFromFileExtention(fileName.Substring(fileName.LastIndexOf('.') + 1)) : "Application/Unknown");
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [HttpGet]
        // GET: TelekomWorkOrder/RejectOutgoing
        public ActionResult RejectOutgoing(long id, string returnUrl = null)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("OutgoingIndex"));
            if (returnUrl != null)
            {
                uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            }
            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var currentTransition = TelekomWorkOrderCache.GetOutgoingList().FirstOrDefault(transition => transition.TransactionID == id);
            if (currentTransition == null)
            {
                return Content($"<div class='text-danger centered'>{RadiusR.Localization.Validation.Common.InvalidInput}</div>");
            }
            var domain = DomainsCache.GetDomainByID(currentTransition.DomainID);
            if (domain == null)
            {
                return Content($"<div class='text-danger centered'>{RadiusR.Localization.Pages.Common.DomainNotFound}</div>");
            }

            return View();
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: TelekomWorkOrder/RejectOutgoing
        public ActionResult RejectOutgoing(long id, string returnUrl, OutgoingTransitionRejectViewModel rejectModel)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("OutgoingIndex"));
            if (returnUrl != null)
            {
                uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            }
            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var currentTransition = TelekomWorkOrderCache.GetOutgoingList().FirstOrDefault(transition => transition.TransactionID == id);
            if (currentTransition == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Validation.Common.InvalidInput;
                return View(rejectModel);
            }
            var domain = DomainsCache.GetDomainByID(currentTransition.DomainID);
            if (domain == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.Common.DomainNotFound;
                return View(rejectModel);
            }

            if (ModelState.IsValid)
            {
                var transitionClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, domain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                var rejectResut = transitionClient.RejectOutgoingTransition(new RezaB.TurkTelekom.WebServices.TTChurnApplication.RejectOutgoingTransitionRequest()
                {
                    TransactionID = currentTransition.TransactionID,
                    XDSLNo = currentTransition.XDSLNo,
                    CancellationReason = (RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionCancellationReasons)rejectModel.RejectionReason,
                    CancellationDescription = rejectModel.RejectionDescription
                });
                if(rejectResut.InternalException != null)
                {
                    ViewBag.ErrorMessage = rejectResut.InternalException.GetShortMessage();
                }
                else
                {
                    TelekomWorkOrderCache.ClearOutgoingListCache();
                    return RedirectToAction("OutgoingIndex", new { errorMessage = 0 });
                }
            }

            return View(rejectModel);
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: TelekomWorkOrder/ApproveOutgoing
        public ActionResult ApproveOutgoing(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("OutgoingIndex"));
            if (returnUrl != null)
            {
                uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            }
            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var currentTransition = TelekomWorkOrderCache.GetOutgoingList().FirstOrDefault(transition => transition.TransactionID == id);
            if (currentTransition == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var domain = DomainsCache.GetDomainByID(currentTransition.DomainID);
            if (domain == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            
            var foundSubscription = db.Subscriptions.Where(s => s.State == (short)RadiusR.DB.Enums.CustomerState.Active || s.State == (short)RadiusR.DB.Enums.CustomerState.Reserved).FirstOrDefault(s => s.SubscriptionTelekomInfo.SubscriptionNo == currentTransition.XDSLNo);
            if (foundSubscription == null)
            {
                return Content($"<div class='text-danger centered'>{RadiusR.Localization.Validation.ModelSpecific.InvalidXDSLNo}</div>");
            }

            var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, domain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
            var approvalResult = serviceClient.ApproveOutgoingTransition(currentTransition.TransactionID);
            if (approvalResult.InternalException != null)
            {
                return Content($"<div class='text-danger centered'>{approvalResult.InternalException.GetShortMessage()}</div>");
            }
            else
            {
                TelekomWorkOrderCache.ClearOutgoingListCache();
            }
            var stateChangeResult = StateChangeUtilities.ChangeSubscriptionState(foundSubscription.ID, new CancelSubscriptionOptions()
            {
                AppUserID = User.GiveUserId(),
                CancellationReason = RadiusR.DB.Enums.CancellationReason.Transition,
                CancellationReasonDescription = string.Format(RadiusR.Localization.Pages.Common.TransitionToAnotherOperator, currentTransition.CounterpartOperator),
                DoNotCancelTelekomService = true,
                ForceCancellation = true,
                LogInterface = RadiusR.DB.Enums.SystemLogInterface.MasterISS,
                ScheduleSMSes = false
            });
            if (stateChangeResult.IsFatal)
            {
                throw stateChangeResult.InternalException;
            }
            else if (!stateChangeResult.IsSuccess)
            {
                stateChangesLogger.Warn(stateChangeResult.InternalException, stateChangeResult.ErrorMessage);
                return Content($"<div class='text-danger centered'>{stateChangeResult.ErrorMessage}</div>");
            }

            return RedirectToAction("OutgoingIndex", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [ActionName("Details")]
        [HttpPost]
        // POST: TelekomWorkOrder/Details
        public ActionResult FinishWorkOrder(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var workOrder = db.TelekomWorkOrders.Find(id);
            if (workOrder == null || workOrder.Subscription.SubscriptionTelekomInfo == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            // registration
            if (workOrder.OperationTypeID == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration)
            {
                if (workOrder.Subscription.State == (short)RadiusR.DB.Enums.CustomerState.Registered)
                {
                    return RedirectToAction("PostToReserveSubscriber", new { id = workOrder.SubscriptionID });
                }
            }
            // transition
            if (workOrder.OperationTypeID == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition)
            {
                var currentDomain = DomainsCache.GetDomainByID(workOrder.Subscription.DomainID);
                if (currentDomain == null)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                    return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                }

                if (currentDomain.TelekomCredential != null && workOrder.TransactionID.HasValue)
                {
                    var statusClient = new RadiusR.DB.TelekomOperations.Wrappers.TTWorkOrderClient();
                    var status = statusClient.GetWorkOrderState(new RadiusR.DB.TelekomOperations.Wrappers.QueryReadyWorkOrder(workOrder));
                    if (status.State == (short)RegistrationState.Done)
                    {
                        if (workOrder.Subscription.State == (short)RadiusR.DB.Enums.CustomerState.Registered)
                        {
                            return RedirectToAction("PostToReserveSubscriber", new { id = workOrder.SubscriptionID });
                        }
                        else
                        {
                            var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, workOrder.Subscription.SubscriptionTelekomInfo?.TTCustomerCode ?? currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                            var results = serviceClient.ApproveIncomingTransition(workOrder.TransactionID.Value);
                            if (results.InternalException != null)
                            {
                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", Uri);
                                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                            }
                            // set new XDSLNo after transition is complete
                            workOrder.Subscription.SubscriptionTelekomInfo.SubscriptionNo = results.Data.NewXDSLNo;
                        }
                    }
                    else if (status.State == (short)RegistrationState.Cancelled)
                    {
                        var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, workOrder.Subscription.SubscriptionTelekomInfo?.TTCustomerCode ?? currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                        var results = serviceClient.RejectIncomingTransition(new RezaB.TurkTelekom.WebServices.TTChurnApplication.RejectIncomingTransitionRequest()
                        {
                            TransactionID = workOrder.TransactionID.Value,
                            XDSLNo = workOrder.Subscription.SubscriptionTelekomInfo?.SubscriptionNo
                        });
                        if (results.InternalException != null)
                        {
                            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", Uri);
                            return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                        }
                    }
                    else if (status.State == (short)RegistrationState.InProgress)
                    {
                        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                        return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
                    }
                }
            }

            workOrder.ClosingDate = DateTime.Now;
            workOrder.IsOpen = false;

            db.SystemLogs.Add(SystemLogProcessor.CloseTelekomWorkOrder(User.GiveUserId(), workOrder.SubscriptionID, RadiusR.DB.Enums.SystemLogInterface.MasterISS, null, workOrder.ID, (RadiusR.DB.Enums.TelekomOperations.TelekomOperationType)workOrder.OperationTypeID, (RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType)workOrder.OperationSubType));

            db.SaveChanges();

            return RedirectToAction("Details", new { id = workOrder.ID, errorMessage = 0, returnUrl = Uri.Uri.PathAndQuery + Uri.Fragment });
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [HttpGet]
        // GET: TelekomWorkOrder/PostToReserveSubscriber
        public ActionResult PostToReserveSubscriber(long id)
        {
            ViewBag.SubscriberID = id;
            ViewBag.State = (int)RadiusR.DB.Enums.CustomerState.Reserved;
            ViewBag.RedirectUrl = Url.Action("Index", null, null, Request.Url.Scheme);
            return View();
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [HttpGet]
        public ActionResult Retry(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var workOrder = db.TelekomWorkOrders.Find(id);
            if (workOrder == null || workOrder.Subscription.SubscriptionTelekomInfo == null || !workOrder.IsOpen || workOrder.OperationTypeID == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            var currentDomain = DomainsCache.GetDomainByID(workOrder.Subscription.DomainID);
            if (currentDomain == null || currentDomain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            // will change as we add more services
            if (workOrder.OperationTypeID != (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }

            ViewBag.SelectedDomain = currentDomain;
            var selectedPacket = new
            {
                PacketCode = 0,
                TariffCode = 0
            };
            if (workOrder.Subscription.SubscriptionTelekomInfo != null)
            {
                selectedPacket = new
                {
                    PacketCode = workOrder.Subscription.SubscriptionTelekomInfo.PacketCode.Value,
                    TariffCode = workOrder.Subscription.SubscriptionTelekomInfo.TariffCode.Value
                };
            }
            var telekomTariff = TelekomTariffsCache.GetSpecificTariff(currentDomain, selectedPacket.PacketCode, selectedPacket.TariffCode);
            var viewResults = telekomTariff != null ? new TelekomTariffHelperViewModel(telekomTariff) : new TelekomTariffHelperViewModel();
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: TelekomWorkOrder/Retry
        public ActionResult Retry(long id, string returnUrl, TelekomTariffHelperViewModel telekomTariff)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var workOrder = db.TelekomWorkOrders.Find(id);
            if (workOrder == null || workOrder.Subscription.SubscriptionTelekomInfo == null || !workOrder.IsOpen)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            var currentDomain = DomainsCache.GetDomainByID(workOrder.Subscription.DomainID);
            if (currentDomain == null || currentDomain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            // will change as we add more services
            if (workOrder.OperationTypeID != (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                var cachedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(currentDomain, telekomTariff.PacketCode.Value, telekomTariff.TariffCode.Value);
                if (cachedTelekomTariff == null)
                {
                    ViewBag.ErrorMessage = RadiusR.Localization.Validation.Common.InvalidTelekomPacket;
                }
                else
                {
                    if (workOrder.Subscription.SubscriptionTelekomInfo == null)
                        workOrder.Subscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo();
                    workOrder.Subscription.SubscriptionTelekomInfo.SubscriptionNo = workOrder.Subscription.SubscriptionTelekomInfo.SubscriptionNo ?? " ";
                    workOrder.Subscription.SubscriptionTelekomInfo.TTCustomerCode = workOrder.Subscription.SubscriptionTelekomInfo.TTCustomerCode > 0 ? workOrder.Subscription.SubscriptionTelekomInfo.TTCustomerCode : currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt;
                    workOrder.Subscription.SubscriptionTelekomInfo.TariffCode = telekomTariff.TariffCode.Value;
                    workOrder.Subscription.SubscriptionTelekomInfo.PacketCode = telekomTariff.PacketCode.Value;
                    workOrder.Subscription.SubscriptionTelekomInfo.XDSLType = telekomTariff.XDSLType.Value;

                    // save TT packet change
                    db.SaveChanges();

                    // resend
                    if (workOrder.OperationTypeID == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration)
                    {
                        var serviceClient = new TTApplicationServiceClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, workOrder.Subscription.SubscriptionTelekomInfo.TTCustomerCode);
                        var registrationTicket = TelekomRegistrationTicketFactory.CreateRegistrationTicket(workOrder.Subscription.Customer, workOrder.Subscription);
                        var response = serviceClient.RegisterSubscriber(registrationTicket);
                        if (response.InternalException != null)
                        {
                            ViewBag.ErrorMessage = response.InternalException.GetShortMessage();
                        }
                        else
                        {
                            workOrder.QueueNo = response.Data.QueueNo;
                            workOrder.ManagementCode = response.Data.ManagementCode;
                            workOrder.ProvinceCode = response.Data.ProvinceCode;
                            workOrder.LastRetryDate = DateTime.Now;
                            workOrder.Subscription.SubscriptionTelekomInfo.SubscriptionNo = response.Data.SubscriptionNo;

                            db.SystemLogs.Add(SystemLogProcessor.RetryTelekomWorkOrder(User.GiveUserId(), workOrder.SubscriptionID, RadiusR.DB.Enums.SystemLogInterface.MasterISS, null, workOrder.ID, new RadiusR.SystemLogs.Parameters.TelekomWorkOrderDetails(workOrder)));

                            db.SaveChanges();

                            return RedirectToAction("Details", new { id = workOrder.ID, errorMessage = 0, returnUrl = Uri.Uri.PathAndQuery + Uri.Fragment });
                        }
                    }
                }
            }

            ViewBag.SelectedDomain = currentDomain;
            return View(telekomTariff);
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        // GET: TelekomWorkOrder/retryError
        public ActionResult RetryError()
        {
            if (TempData.ContainsKey("TelekomErrorMessage"))
                ViewBag.ErrorMessage = TempData["TelekomErrorMessage"];
            else
                ViewBag.ErrorMessage = "No Error Message Found.";
            return View();
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        // POST: TelekomWorkOrder/ReuploadTransitionFiles
        public ActionResult ReuploadTransitionFiles(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);

            var workOrder = db.TelekomWorkOrders.Find(id);
            if (workOrder == null || workOrder.OperationTypeID != (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            var statusClient = new RadiusR.DB.TelekomOperations.Wrappers.TTWorkOrderClient();
            var status = statusClient.GetWorkOrderState(new RadiusR.DB.TelekomOperations.Wrappers.QueryReadyWorkOrder(workOrder));
            if (status.State != (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.InProgress)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            var validDocuments = StateChangeUtilities.ValidateAttachmentsForTransition(workOrder.Subscription);
            if (!validDocuments.IsValid)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            var uploadResults = StateChangeUtilities.UploadTransitionFiles(workOrder.Subscription, workOrder.TransactionID.Value, validDocuments);
            if (uploadResults.IsSuccess)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
            else
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "28", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        // GET: TelekomWorkOrder/ManuallyAdd
        public ActionResult ManuallyAdd(long id)
        {
            var subscriber = db.Subscriptions.Find(id);
            if (subscriber == null || subscriber.SubscriptionTelekomInfo == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</span>");
            }

            var workOrder = new TelekomWorkOrderViewModel()
            {
                SubscriberID = subscriber.ID,
                SubscriberName = subscriber.ValidDisplayName,
                PhoneNo = subscriber.Customer.ContactPhoneNo,
            };

            return View(workOrder);
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: TelekomWorkOrder/ManuallyAdd
        public ActionResult ManuallyAdd(long id, TelekomWorkOrderViewModel workOrder)
        {
            var subscriber = db.Subscriptions.Find(id);
            if (subscriber == null || subscriber.SubscriptionTelekomInfo == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</span>");
            }

            // transition validation clearance
            if (workOrder.OperationType.HasValue)
            {
                if (workOrder.OperationType == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition)
                {
                    ModelState.Remove("ManagementCode");
                    ModelState.Remove("ProvinceCode");
                    ModelState.Remove("QueueNo");
                    ModelState.Remove("OperationSubType");
                    workOrder._managementCode = null;
                    workOrder._provinceCode = null;
                    workOrder._queueNo = null;
                    workOrder.OperationSubType = null;
                }
            }

            if (ModelState.IsValid)
            {
                // transition
                if (workOrder.OperationType == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition)
                {
                    // redirect to preparation
                    return RedirectToAction("PrepareTransitionForResend", "Client", new { id = subscriber.ID, returnUrl = $"{Url.Action("Details", "Client", new { id = subscriber.ID })}#faults" });
                }
                // non-transition
                var dbWorkOrder = new TelekomWorkOrder()
                {
                    AppUserID = User.GiveUserId(),
                    CreationDate = DateTime.Now,
                    IsOpen = true,
                    ManagementCode = workOrder._managementCode,
                    ProvinceCode = workOrder._provinceCode,
                    QueueNo = workOrder._queueNo,
                    OperationTypeID = workOrder.OperationType.Value,
                    OperationSubType = workOrder.OperationSubType.Value,
                    TransactionID = workOrder._transactionID,
                    SubscriptionID = subscriber.ID
                };

                db.TelekomWorkOrders.Add(dbWorkOrder);
                db.SaveChanges();
                // system log
                db.SystemLogs.Add(SystemLogProcessor.CreateTelekomWorkOrder(User.GiveUserId(), subscriber.ID, RadiusR.DB.Enums.SystemLogInterface.MasterISS, null, dbWorkOrder.ID, new RadiusR.SystemLogs.Parameters.TelekomWorkOrderDetails(dbWorkOrder)));
                db.SaveChanges();

                return Redirect(Url.Action("Details", "Client", new { id = subscriber.ID }) + "#faults");
            }

            workOrder.SubscriberID = subscriber.ID;
            workOrder.SubscriberName = subscriber.ValidDisplayName;
            workOrder.PhoneNo = subscriber.Customer.ContactPhoneNo;

            return View(workOrder);
        }

        [AuthorizePermission(Permissions = "Telekom Work Order Edits")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: TelekomWorkOrder/CancelWorkOrder
        public ActionResult CancelWorkOrder(long id, string returnUrl)
        {
            // return URL
            var Uri = new UriBuilder(Url.Action("Index", null, null, Request.Url.Scheme));
            if (!string.IsNullOrEmpty(returnUrl))
                Uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", Uri);
            ViewBag.ReturnUrl = Uri.Uri.PathAndQuery + Uri.Fragment;

            var workOrder = db.TelekomWorkOrders.Find(id);
            if (workOrder == null || workOrder.Subscription.SubscriptionTelekomInfo == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }

            var currentDomain = DomainsCache.GetDomainByID(workOrder.Subscription.DomainID);
            if (currentDomain == null || currentDomain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }

            var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, workOrder.Subscription.SubscriptionTelekomInfo?.TTCustomerCode ?? currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
            var results = serviceClient.RejectIncomingTransition(new RezaB.TurkTelekom.WebServices.TTChurnApplication.RejectIncomingTransitionRequest()
            {
                TransactionID = workOrder.TransactionID.Value,
                XDSLNo = workOrder.Subscription.SubscriptionTelekomInfo?.SubscriptionNo
            });
            if (results.InternalException != null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", Uri);
                return Redirect(Uri.Uri.PathAndQuery + Uri.Fragment);
            }

            workOrder.ClosingDate = DateTime.Now;
            workOrder.IsOpen = false;

            db.SystemLogs.Add(SystemLogProcessor.CloseTelekomWorkOrder(User.GiveUserId(), workOrder.SubscriptionID, RadiusR.DB.Enums.SystemLogInterface.MasterISS, null, workOrder.ID, (RadiusR.DB.Enums.TelekomOperations.TelekomOperationType)workOrder.OperationTypeID, (RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType)workOrder.OperationSubType));

            db.SaveChanges();

            return RedirectToAction("Details", new { id = workOrder.ID, errorMessage = 0, returnUrl = Uri.Uri.PathAndQuery + Uri.Fragment });
        }

        class WorkOrderState
        {
            public long WorkOrderID { get; set; }

            public short? State { get; set; }
        }

        private RezaB.TurkTelekom.FTPOperations.TransitionFTPClient CreateTransitionFTPClient(CachedDomain domain)
        {
            return new RezaB.TurkTelekom.FTPOperations.TransitionFTPClient(new RezaB.TurkTelekom.FTPOperations.TelekomServiceCredentials()
            {
                Username = domain.TelekomCredential.XDSLWebServiceUsernameInt,
                Password = domain.TelekomCredential.XDSLWebServicePassword,
                CustomerCode = domain.TelekomCredential.XDSLWebServiceCustomerCodeInt
            },
            new RezaB.TurkTelekom.FTPOperations.FTPCredentials()
            {
                Username = domain.TelekomCredential.TransitionFTPUsername,
                Password = domain.TelekomCredential.TransitionFTPPassword,
                FolderNames = domain.TelekomCredential.TransitionFolderName
            },
            TransitionOperatorsCache.GetAllOperators().Select(op => new RezaB.TurkTelekom.FTPOperations.TelekomOperator()
            {
                Username = op.Username,
                FolderNames = op.RemoteFolders
            }));
        }
    }
}