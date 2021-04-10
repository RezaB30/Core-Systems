using RadiusR.DB.DomainsCache;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Mikrotik.Extentions;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RezaB.TurkTelekom.WebServices.TTApplication;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using RezaB.TurkTelekom.WebServices.InfrastructureInfo;
using RadiusR.SystemLogs;
using RezaB.TurkTelekom.WebServices.TTOYS;
using RezaB.Web.Authentication;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        [AuthorizePermission(Permissions = "Disconnect User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/DisconnectUser
        public ActionResult DisconnectUser(long id)
        {
            var accountingRecord = db.RadiusAccountings.Find(id);
            if (accountingRecord == null)
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</div>");
            var nas = db.NAS.FirstOrDefault(n => n.IP == accountingRecord.NASIP);
            if (nas == null)
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</div>");

            accountingRecord.StopTime = DateTime.Now;
            accountingRecord.TerminateCause = 6;
            db.SaveChanges();

            MikrotikRouter router = new MikrotikRouter(new MikrotikApiCredentials(nas.IP, nas.ApiPort, nas.ApiUsername, nas.ApiPassword));
            if (router.DisconnectUser(new[] { accountingRecord.Username }))
            {
                return RedirectToAction("ConnectionHistory", new { id = accountingRecord.SubscriptionID });
            }

            return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._19 + "</div>");
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        // POST: Client/CheckCustomerIdentity
        public ActionResult CheckCustomerIdentity([Bind(Prefix = "IDCard")]IDCardViewModel IDCard)
        {
            if (ModelState.IsValid)
            {
                var validationResults = ValidateTCK(IDCard);

                // check available customer
                if (!string.IsNullOrWhiteSpace(IDCard.TCKNo))
                {
                    var dbCustomer = db.CustomerIDCards.FirstOrDefault(cid => cid.TCKNo == IDCard.TCKNo);
                    if (dbCustomer != null)
                    {
                        validationResults.ResultContent += "<div class='text-danger'>" + RadiusR.Localization.Pages.Common.FoundExistingCustomer + "<a class='details-link' target='_blank' href='" + Url.Action("Index", new { TCKNo = IDCard.TCKNo }) + "'>" + RadiusR.Localization.Pages.Common.Show + "</a></div>";
                    }
                }

                return Content(validationResults.ResultContent);
            }

            return Content("<div class='text-danger'>" + RadiusR.Localization.Validation.Common.TCKValidationStep1 + "</div>");

        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Modify Clients")]
        // POST: Client/HasDomainCredentials
        public ActionResult HasDomainCredentials(int id)
        {
            var cachedDomain = DomainsCache.GetDomainByID(id);
            if (cachedDomain == null || cachedDomain.TelekomCredential == null)
                return Json(new { FoundCredentials = false });
            return Json(new { FoundCredentials = true });
        }

        [AuthorizePermission(Permissions = "Change Speed Profile")]
        [HttpPost]
        public ActionResult ChangeSpeedProfile(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._4 + "</div>");
            }
            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null || domain.TelekomCredential == null || dbSubscription.SubscriptionTelekomInfo == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</div>");
            }

            CachedTelekomTariff cachedTariff = TelekomTariffsCache.GetSpecificTariff(domain, dbSubscription.SubscriptionTelekomInfo.PacketCode.Value, dbSubscription.SubscriptionTelekomInfo.TariffCode.Value);

            var telekomTariff = cachedTariff != null ? new TelekomTariffHelperViewModel(cachedTariff) : null;

            ViewBag.Domain = domain;

            return View(viewName: "AjaxActions/ChangeSpeedProfile", model: telekomTariff);
        }

        [AuthorizePermission(Permissions = "Change Speed Profile")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangeSpeedProfileConfirm(long id, TelekomTariffHelperViewModel telekomTariff)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._4 + "</div>");
            }
            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null || domain.TelekomCredential == null || dbSubscription.SubscriptionTelekomInfo == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</div>");
            }

            CachedTelekomTariff cachedTariff = TelekomTariffsCache.GetSpecificTariff(domain, dbSubscription.SubscriptionTelekomInfo.PacketCode.Value, dbSubscription.SubscriptionTelekomInfo.TariffCode.Value);

            // 

            if (ModelState.IsValid)
            {
                var serviceClient = new TTApplicationServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, dbSubscription.SubscriptionTelekomInfo.TTCustomerCode);
                var response = serviceClient.ChangeSpeedProfile(dbSubscription.SubscriptionTelekomInfo.SubscriptionNo, telekomTariff.TariffCode.Value, telekomTariff.PacketCode.Value, telekomTariff.SpeedCode.Value);
                if (response.InternalException != null)
                {
                    return Content("<div class='text-danger centered'>" + response.InternalException.GetShortMessage() + "</div>");
                }

                dbSubscription.SubscriptionTelekomInfo.TariffCode = telekomTariff.TariffCode.Value;
                dbSubscription.SubscriptionTelekomInfo.PacketCode = telekomTariff.PacketCode.Value;
                dbSubscription.SubscriptionTelekomInfo.IsPaperWorkNeeded = telekomTariff.IsPaperworkNeeded;
                dbSubscription.SubscriptionTelekomInfo.XDSLType = telekomTariff.XDSLType.Value;

                db.SaveChanges();

                return Content("<div class='text-success centered'>" + RadiusR.Localization.Pages.ErrorMessages._0 + "</div>");
            }

            return View(viewName: "AjaxActions/ChangeSpeedProfile", model: telekomTariff);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SyncronizeTelekomInfo(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null || domain.TelekomCredential == null || dbSubscription.SubscriptionTelekomInfo == null)
            {
                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            var results = db.UpdateSubscriberTelekomInfoFromWebService(new TelekomSynchronizationOptions()
            {
                AppUserID = User.GiveUserId(),
                LogInterface = SystemLogInterface.MasterISS,
                DBSubscription = dbSubscription,
                DSLNo = dbSubscription.SubscriptionTelekomInfo.SubscriptionNo
            });

            if (results.ResultCode != TelekomSynchronizationResultCodes.Success)
            {
                TempData["tt-sync-error"] = results.ResultCode == TelekomSynchronizationResultCodes.TelekomError ? string.Join(Environment.NewLine, results.TelekomExceptions.Select(tte => tte.GetShortMessage())) : GetSynchronizationErrorMessage(results.ResultCode) ;
                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 33 });
            }

            db.SaveChanges();
            return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [AjaxCall]
        // GET: Client/GetNewDSLNo
        public ActionResult GetNewDSLNo(int domainId, string oldDSLNo)
        {
            var domain = DomainsCache.GetDomainByID(domainId);
            if (domain == null)
            {
                return Json(new { Error = RadiusR.Localization.Pages.Common.DomainNotFound });
            }

            var serviceClient = new TelekomSubscriberInfoServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
            var response = serviceClient.GetNewSubscriberNoByOldOne(oldDSLNo);
            if (response.InternalException != null)
            {
                return Json(new { Error = response.InternalException.GetShortMessage() });
            }

            return Json(new { Data = response.Data });
        }

        [AuthorizePermission(Permissions = "Clients")]
        [AjaxCall]
        // GET: Client/LastRegisters
        public ActionResult LastRegisters(int? page)
        {
            var viewResults = db.Subscriptions.OrderByDescending(subscription => subscription.MembershipDate).Select(subscription => new SubscriptionListDisplayViewModel()
            {
                ID = subscription.ID,
                Name = subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.Title : subscription.Customer.FirstName + " " + subscription.Customer.LastName,
                RegistrationDate = subscription.MembershipDate,
                Username = subscription.RadiusAuthorization.Username
            }).Take(30);

            return View(viewName: "AjaxActions/LastRegisters", model: viewResults);
        }

        [AuthorizePermission(Permissions = "Change Speed Profile")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult StepUpSpeedProfile(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.ErrorMessages._4;
                return View("StepSpeedProfile");
            }
            var domain = DomainsCache.GetDomainByID(subscription.DomainID);
            if (domain == null || domain.TelekomCredential == null || subscription.SubscriptionTelekomInfo == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.ErrorMessages._9;
                return View("StepSpeedProfile");
            }

            var serviceClient = new TTOYSServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
            var response = serviceClient.StepUpSpeedProfile(subscription.SubscriptionTelekomInfo.SubscriptionNo);
            if (response.InternalException != null)
            {
                ViewBag.ErrorMessage = response.InternalException.GetShortMessage();
            }
            else
            {
                ViewBag.SuccessMessage = RadiusR.Localization.Pages.ErrorMessages._0;
                db.SystemLogs.Add(SystemLogProcessor.StepUpSubscriptionSpeedProfile(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null));
                db.SaveChanges();
            }

            return View("StepSpeedProfile");
        }

        [AuthorizePermission(Permissions = "Change Speed Profile")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult StepDownSpeedProfile(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.ErrorMessages._4;
                return View("StepSpeedProfile");
            }
            var domain = DomainsCache.GetDomainByID(subscription.DomainID);
            if (domain == null || domain.TelekomCredential == null || subscription.SubscriptionTelekomInfo == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.ErrorMessages._9;
                return View("StepSpeedProfile");
            }

            var serviceClient = new TTOYSServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
            var response = serviceClient.StepDownSpeedProfile(subscription.SubscriptionTelekomInfo.SubscriptionNo);
            if (response.InternalException != null)
            {
                ViewBag.ErrorMessage = response.InternalException.GetShortMessage();
            }
            else
            {
                ViewBag.SuccessMessage = RadiusR.Localization.Pages.ErrorMessages._0;
                db.SystemLogs.Add(SystemLogProcessor.StepDownSubscriptionSpeedProfile(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null));
                db.SaveChanges();
            }

            return View("StepSpeedProfile");
        }

        private bool IsValidAttachmentFileType(string fileType)
        {
            var validTypes = new[]
            {
                "jpeg",
                "jpg",
                "png",
                "pdf"
            };

            return validTypes.Contains(fileType.ToLower());
        }
    }
}