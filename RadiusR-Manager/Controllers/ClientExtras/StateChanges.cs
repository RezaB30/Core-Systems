﻿using NLog;
using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;
using RadiusR_Manager.Models.ViewModels.ClientStates;
using RezaB.TurkTelekom.WebServices.Exceptions;
using RezaB.Web;
using RezaB.Web.Authentication;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        private static Logger stateChangesLogger = LogManager.GetLogger("client-state-changes");

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpGet]
        // GET: Client/FreezeSubscription
        public ActionResult FreezeSubscription(long id, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            ViewBag.BackUrl = new UriBuilder(redirectUrl);
            return View(viewName: "StateChanges/FreezeSubscription");
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/FreezeSubscription
        public ActionResult FreezeSubscription(long id, string redirectUrl, SubscriptionFreezeOptionsViewModel freezeOptions, bool force)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var domain = RadiusR.DB.DomainsCache.DomainsCache.GetDomainByID(dbSubscription.DomainID);

            if (!StateChangeUtilities.GetValidStateChanges((CustomerState)dbSubscription.State).Contains(CustomerState.Disabled))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                if (!StateChangeUtilities.ValidateFreezingDuration(dbSubscription, domain, freezeOptions.ReactivationDate.Value))
                {
                    ModelState.AddModelError("ReactivationDate", RadiusR.Localization.Validation.Common.ReactivationDateError);
                }
                if (ModelState.IsValid)
                {
                    //try
                    //{
                    var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new FreezeSubscriptionOptions()
                    {
                        AppUserID = User.GiveUserId(),
                        LogInterface = SystemLogInterface.MasterISS,
                        ForceThroughWebService = force && User.HasPermission("Force Freeze"),
                        ReleaseDate = freezeOptions.ReactivationDate.Value
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
                    else if (results.InternalException is TTWebServiceException ex && User.HasPermission("Force Freeze"))
                    {
                        //if (!User.HasPermission("Force Freeze"))
                        //{
                        //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", uri);
                        //    TempData["ErrorMessageDetails"] = ex.GetShortMessage();
                        //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        //}
                        ViewBag.WebServiceError = RadiusR.Localization.Pages.ErrorMessages._33 + " -> " + ex.GetShortMessage();
                        ViewBag.IsForce = true;
                    }
                    else
                    {
                        UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                        TempData["ErrorResults"] = results;
                        return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                    }
                    //}
                    //catch (TTWebServiceException ex)
                    //{
                    //    if (!User.HasPermission("Force Freeze"))
                    //    {
                    //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", uri);
                    //        TempData["ErrorMessageDetails"] = ex.GetShortMessage();
                    //        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                    //    }
                    //    ViewBag.WebServiceError = RadiusR.Localization.Pages.ErrorMessages._33 + " -> " + ex.GetShortMessage();
                    //    ViewBag.IsForce = true;
                    //}
                }
            }

            ViewBag.BackUrl = new UriBuilder(redirectUrl);
            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            return View(viewName: "StateChanges/FreezeSubscription", model: freezeOptions);
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpGet]
        // GET: Client/CancelSubscription
        public ActionResult CancelSubscription(long id, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery);
            }

            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            ViewBag.BackUrl = new UriBuilder(redirectUrl);
            return View(viewName: "StateChanges/CancelSubscription");
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/CancelSubscription
        public ActionResult CancelSubscription(long id, string redirectUrl, SubscriptionCancelOptionsViewModel cancelOptions, bool force)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (!StateChangeUtilities.GetValidStateChanges((CustomerState)dbSubscription.State).Contains(CustomerState.Cancelled))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                //try
                //{
                var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new CancelSubscriptionOptions()
                {
                    AppUserID = User.GiveUserId(),
                    LogInterface = SystemLogInterface.MasterISS,
                    CancellationReason = (CancellationReason)cancelOptions.ReasonID,
                    CancellationReasonDescription = cancelOptions.ReasonDescription,
                    ForceCancellation = force && User.HasPermission("Force Cancellation"),
                    DoNotCancelTelekomService = false
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
                else if (results.InternalException is TTWebServiceException ex && User.HasPermission("Force Cancellation"))
                {
                    //if (!User.HasPermission("Force Cancellation"))
                    //{
                    //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", uri);
                    //    TempData["ErrorMessageDetails"] = ex.GetShortMessage();
                    //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                    //}

                    ViewBag.WebServiceError = RadiusR.Localization.Pages.ErrorMessages._33 + " -> " + ex.GetShortMessage();
                    ViewBag.IsForce = true;
                }
                else
                {
                    UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                    TempData["ErrorResults"] = results;
                    return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                }
                //}
                //catch (TTWebServiceException ex)
                //{
                //    if (!User.HasPermission("Force Cancellation"))
                //    {
                //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "33", uri);
                //        TempData["ErrorMessageDetails"] = ex.GetShortMessage();
                //        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                //    }

                //    ViewBag.WebServiceError = RadiusR.Localization.Pages.ErrorMessages._33 + " -> " + ex.GetShortMessage();
                //    ViewBag.IsForce = true;
                //}
            }

            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            ViewBag.BackUrl = new UriBuilder(redirectUrl);
            return View(viewName: "StateChanges/CancelSubscription", model: cancelOptions);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Force Unfreeze")]
        // GET: Client/UnfreezeError
        public ActionResult UnfreezeError(long id, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.ErrorMessage = TempData["UnfreezeError"];
            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(viewName: "StateChanges/UnfreezeError");

        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Force Unfreeze")]
        [ActionName("UnfreezeError")]
        // GET: Client/UnfreezeError
        public ActionResult UnfreezeErrorConfirm(long id, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new ActivateSubscriptionOptions()
            {
                AppUserID = User.GiveUserId(),
                LogInterface = SystemLogInterface.MasterISS,
                ForceUnfreeze = true
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

        // GET: Client/StateChangeError
        public ActionResult StateChangeError(string returnUrl)
        {
            try
            {
                var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
            }
            catch
            {
                ViewBag.BackUrl = Url.Action("Index", "Client");
            }
            if (TempData["ErrorResults"] is StateChangeResult errorResult)
            {
                ViewBag.ErrorDetails = errorResult.ErrorMessage;
                // log errors
                stateChangesLogger.Warn(errorResult.InternalException, errorResult.ErrorMessage);
            }
            return View(viewName: "StateChanges/StateChangeError");
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Modify Clients")]
        [AjaxCall]
        // POST: Client/TransitionDocumentValidation
        public ActionResult TransitionDocumentValidation(long id)
        {
            var viewResult = new TransitionDocumentsValidationViewModel();
            var foundSub = db.Subscriptions.FirstOrDefault(s => s.ID == id);
            if (foundSub == null)
            {
                viewResult.ValidationMessage = RadiusR.Localization.Pages.ErrorMessages._4;
            }
            else
            {
                var validationResults = StateChangeUtilities.ValidateAttachmentsForTransition(foundSub);
                if (!validationResults.IsValid)
                {
                    if (validationResults.FileManagerException != null)
                    {
                        viewResult.ValidationMessage = RadiusR.Localization.Pages.Common.FileManagerError;
                    }
                    else
                    {
                        viewResult.ValidationMessage = RadiusR.Localization.Validation.ModelSpecific.MissingRequiredDocuments;
                        viewResult.Documents = validationResults.RequiredDocuments.Select(rd => new TransitionDocumentsValidationViewModel.DocumentValidation()
                        {
                            DocumentType = (short)rd.DocumentType,
                            IsValid = rd.IsAvailable
                        });
                    }
                }
                else
                {
                    viewResult.Documents = validationResults.RequiredDocuments.Select(rd => new TransitionDocumentsValidationViewModel.DocumentValidation()
                    {
                        DocumentType = (short)rd.DocumentType,
                        IsValid = rd.IsAvailable
                    });
                }
            }

            return View(viewName: "StateChanges/TransitionDocumentValidation", model: viewResult);
        }
    }
}