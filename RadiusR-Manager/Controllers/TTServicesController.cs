using RadiusR.DB;
using RadiusR.DB.DomainsCache;
using RadiusR.DB.Enums;
using RadiusR.SystemLogs;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RezaB.TurkTelekom.WebServices.Address;
using RezaB.TurkTelekom.WebServices.Availability;
using RezaB.TurkTelekom.WebServices.OLO;
using RezaB.TurkTelekom.WebServices.TTApplication;
using RezaB.TurkTelekom.WebServices.TTOYS;
using RezaB.Web.Authentication;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public class TTServicesController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Infrastructure Service")]
        // GET: TTServices/InfrastructureCheck
        public ActionResult InfrastructureCheck()
        {
            if (!DomainsCache.HasAnyTelekomDomains)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [AuthorizePermission(Permissions = "Infrastructure Service")]
        // GET: TTServices/InfrastructureCheckByPSTN
        public ActionResult InfrastructureCheckByPSTN()
        {
            if (!DomainsCache.HasAnyTelekomDomains)
                return RedirectToAction("Index", "Home");

            ViewBag.TelekomDomains = new SelectList(DomainsCache.GetTelekomDomains(), "ID", "Name", DomainsCache.GetTelekomDomains().FirstOrDefault().ID);
            return View();
        }

        [AuthorizePermission(Permissions = "Infrastructure Service")]
        // GET: TTServices/InfrastructureCheckByBBK
        public ActionResult InfrastructureCheckByBBK(long? id = null)
        {
            if (!DomainsCache.HasAnyTelekomDomains)
                return RedirectToAction("Index", "Home");

            ViewBag.TelekomDomains = new SelectList(DomainsCache.GetTelekomDomains(), "ID", "Name", DomainsCache.GetTelekomDomains().FirstOrDefault().ID);
            return View(new AddressViewModel());
        }

        [AuthorizePermission(Permissions = "Infrastructure Service")]
        [HttpPost]
        [AjaxCall]
        // POST: TTServices/GetAvailability
        public ActionResult GetAvailability(int domainId = 0, string BBK = null, string PSTN = null)
        {
            if (string.IsNullOrWhiteSpace(BBK) && string.IsNullOrWhiteSpace(PSTN))
            {
                return Content("<div class='text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._32 + "</div>");
            }
            var cachedDomain = DomainsCache.GetDomainByID(domainId);
            if (cachedDomain == null || cachedDomain.TelekomCredential == null)
            {
                return Content("<div class='text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._37 + "</div>");
            }
            var serviceClient = new AvailabilityServiceClient(cachedDomain.TelekomCredential.XDSLWebServiceUsernameInt, cachedDomain.TelekomCredential.XDSLWebServicePassword);
            RezaB.TurkTelekom.WebServices.ServiceResponse<AvailabilityServiceClient.AvailabilityDescription> serviceResultsADSL;
            RezaB.TurkTelekom.WebServices.ServiceResponse<AvailabilityServiceClient.AvailabilityDescription> serviceResultsVDSL;
            RezaB.TurkTelekom.WebServices.ServiceResponse<AvailabilityServiceClient.AvailabilityDescription> serviceResultsFiber;
            if (!string.IsNullOrWhiteSpace(PSTN))
            {
                serviceResultsADSL = serviceClient.Check(AvailabilityServiceClient.XDSLType.ADSL, AvailabilityServiceClient.QueryType.PSTN, PSTN);
                serviceResultsVDSL = serviceClient.Check(AvailabilityServiceClient.XDSLType.VDSL, AvailabilityServiceClient.QueryType.PSTN, PSTN);
                serviceResultsFiber = serviceClient.Check(AvailabilityServiceClient.XDSLType.Fiber, AvailabilityServiceClient.QueryType.PSTN, PSTN);
            }
            else
            {
                serviceResultsADSL = serviceClient.Check(AvailabilityServiceClient.XDSLType.ADSL, AvailabilityServiceClient.QueryType.BBK, BBK);
                serviceResultsVDSL = serviceClient.Check(AvailabilityServiceClient.XDSLType.VDSL, AvailabilityServiceClient.QueryType.BBK, BBK);
                serviceResultsFiber = serviceClient.Check(AvailabilityServiceClient.XDSLType.Fiber, AvailabilityServiceClient.QueryType.BBK, BBK);
            }

            if (serviceResultsADSL.InternalException != null ||
                serviceResultsVDSL.InternalException != null ||
                serviceResultsFiber.InternalException != null)
            {
                return Content("<div class='text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._33 + "</div>");
            }

            var queryType = !string.IsNullOrEmpty(BBK) ? BBKOrPSTN.BBK : BBKOrPSTN.PSTN;
            var serviceResultsCancellationHistory = serviceClient.DaysSinceLastClosure(queryType, queryType == BBKOrPSTN.BBK ? BBK : PSTN);

            var results = new AvailabilityResultsViewModel()
            {
                ADSL = new AvailabilityResultsViewModel.AvailabilityResult()
                {
                    SVUID = serviceResultsADSL.Data.Description.SVUID,
                    XDSLType = (short)serviceResultsADSL.Data.Description.Type,
                    PortState = (short)serviceResultsADSL.Data.Description.PortState,
                    InfrastructureType = (short)serviceResultsADSL.Data.Description.InfrastructureType,
                    DSLMaxSpeed = serviceResultsADSL.Data.Description.DSLMaxSpeed.HasValue ? Convert.ToString(serviceResultsADSL.Data.Description.DSLMaxSpeed * 1024) : null,
                    Distance = serviceResultsADSL.Data.Description.Distance,
                    DistanceIsValid = serviceResultsADSL.Data.Description.DistanceIsValid,
                    HasInfrastructure = serviceResultsADSL.Data.Description.HasInfrastructure,
                    HasOpenRequest = serviceResultsADSL.Data.Description.HasOpenRequest,
                    ErrorMessage = serviceResultsADSL.Data.Description.ErrorMessage,
                    Result = (short)serviceResultsADSL.Data.Result
                },
                VDSL = new AvailabilityResultsViewModel.AvailabilityResult()
                {
                    SVUID = serviceResultsVDSL.Data.Description.SVUID,
                    XDSLType = (short)serviceResultsVDSL.Data.Description.Type,
                    PortState = (short)serviceResultsVDSL.Data.Description.PortState,
                    InfrastructureType = (short)serviceResultsVDSL.Data.Description.InfrastructureType,
                    DSLMaxSpeed = serviceResultsVDSL.Data.Description.DSLMaxSpeed.HasValue ? Convert.ToString(serviceResultsVDSL.Data.Description.DSLMaxSpeed * 1024) : null,
                    Distance = serviceResultsVDSL.Data.Description.Distance,
                    DistanceIsValid = serviceResultsVDSL.Data.Description.DistanceIsValid,
                    HasInfrastructure = serviceResultsVDSL.Data.Description.HasInfrastructure,
                    HasOpenRequest = serviceResultsVDSL.Data.Description.HasOpenRequest,
                    ErrorMessage = serviceResultsVDSL.Data.Description.ErrorMessage,
                    Result = (short)serviceResultsVDSL.Data.Result
                },
                Fiber = new AvailabilityResultsViewModel.AvailabilityResult()
                {
                    SVUID = serviceResultsFiber.Data.Description.SVUID,
                    XDSLType = (short)serviceResultsFiber.Data.Description.Type,
                    PortState = (short)serviceResultsFiber.Data.Description.PortState,
                    InfrastructureType = (short)serviceResultsFiber.Data.Description.InfrastructureType,
                    DSLMaxSpeed = serviceResultsFiber.Data.Description.DSLMaxSpeed.HasValue ? Convert.ToString(serviceResultsFiber.Data.Description.DSLMaxSpeed * 1024) : null,
                    Distance = serviceResultsFiber.Data.Description.Distance,
                    DistanceIsValid = serviceResultsFiber.Data.Description.DistanceIsValid,
                    HasInfrastructure = serviceResultsFiber.Data.Description.HasInfrastructure,
                    HasOpenRequest = serviceResultsFiber.Data.Description.HasOpenRequest,
                    ErrorMessage = serviceResultsFiber.Data.Description.ErrorMessage,
                    Result = (short)serviceResultsFiber.Data.Result
                },
                CancellationHistory = new AvailabilityResultsViewModel.CancellationHistoryResult()
                {
                    Days = serviceResultsCancellationHistory.InternalException == null ? serviceResultsCancellationHistory.Data.ToString() : "-",
                    ErrorMessage = serviceResultsCancellationHistory.InternalException != null ? serviceResultsCancellationHistory.InternalException.GetShortMessage() : string.Empty
                }
            };

            return View(results);
        }

        [AuthorizePermission(Permissions = "Line Quality Service")]
        [HttpPost]
        [AjaxCall]
        public ActionResult LineQualityCheck(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return Content("<div class='centered text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._4 + "</div>");
            }
            var domain = DomainsCache.GetDomainByID(subscription.DomainID);
            if (domain == null || domain.TelekomCredential == null || subscription.SubscriptionTelekomInfo == null)
            {
                return Content("<div class='centered text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</div>");
            }

            var serviceClient = new TTOYSServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
            var response = serviceClient.Check(subscription.SubscriptionTelekomInfo.SubscriptionNo);
            if (response.InternalException != null)
            {
                return Content("<div class='centered text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._33 + "</div>");
            }
            var results = new LineQualityViewModel()
            {
                CRMSpeedProfile = response.Data.LowerSpeedBase,
                CurrentDownloadSpeed = response.Data.CurrentDown,
                CurrentUploadSpeed = response.Data.CurrentUp,
                DownloadAttn = response.Data.DownloadAttenuation,
                UploadAttn = response.Data.UploadAttenuation,
                DownloadNoiseMargin = response.Data.NoiseRateDown,
                UploadNoiseMargin = response.Data.NoiseRateUp,
                DownloadOutputPower = response.Data.DownOutputPower,
                UploadOutputPower = response.Data.UpOutputPower,
                DownloadSpeedCapacity = response.Data.MaxDown,
                UploadSpeedCapacity = response.Data.MaxUp,
                DSLAM = Convert.ToString(response.Data.DSLAMNo),
                LineState = response.Data.OperationStatus == TTOYSServiceClient.OperationStatus.Open ? (short)TTLineState.Open : (short)TTLineState.Closed,
                NMSSpeedProfile = response.Data.NMSSpeedValue,
                ShelfCardPort = "R" + response.Data.DSLAMNo + ".S" + response.Data.DSLAMNo + ".LT" + response.Data.CardNo + "." + response.Data.PortNo
            };

            db.SystemLogs.Add(SystemLogProcessor.LineQualityCheck(User.GiveUserId(), id, SystemLogInterface.MasterISS, null, results));
            db.SaveChanges();
            ViewBag.SubscriberID = subscription.ID;
            return View(results);
        }

        [AuthorizePermission(Permissions = "Telekom Services,Modify Client")]
        [HttpPost]
        [AjaxCall]
        public ActionResult TTpacketListForDomain(int domainId)
        {
            var cachedDomain = DomainsCache.GetDomainByID(domainId);
            if (cachedDomain == null || cachedDomain.TelekomCredential == null)
                return Json(new { ErrorMessage = RadiusR.Localization.Pages.ErrorMessages._37, Data = Enumerable.Empty<TelekomTariffHelperViewModel>() });
            var cachesTelekomTariffs = TelekomTariffsCache.GetAllTariffs(cachedDomain);
            return Json(new { Data = cachesTelekomTariffs.Select(ctt => new TelekomTariffHelperViewModel(ctt)).ToArray() });
        }

        [AuthorizePermission(Permissions = "General Fault Query")]
        [HttpPost]
        //[AjaxCall]
        public ActionResult GeneralFaults(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._4 + "</div>");
            }
            var domain = DomainsCache.GetDomainByID(subscription.DomainID);
            if (subscription.SubscriptionTelekomInfo == null || domain == null || domain.TelekomCredential == null)
            {
                return Content("<div class='text-danger centered'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</div>");
            }
            OLOServiceClient client = new OLOServiceClient(domain.TelekomCredential.OLOPortalUsername, domain.TelekomCredential.OLOPortalPassword);
            var response = client.ListGeneralFaultsBySubscriptionNo(subscription.SubscriptionTelekomInfo.SubscriptionNo);

            var results = new GeneralFaultViewModel()
            {
                ErrorMessage = response.InternalException == null ? null : response.InternalException.GetShortMessage(),
                Faults = response.Data != null ? response.Data.Select(fault => new GeneralFaultViewModel.FaultViewModel()
                {
                    StartDate = fault.StartDate,
                    EndDate = fault.EndDate,
                    Topic = fault.WorkingTopic
                }) : Enumerable.Empty<GeneralFaultViewModel.FaultViewModel>()
            };

            return View(results);
        }

        [AuthorizePermission(Permissions = "Telekom Tariffs")]
        [HttpGet]
        // GET: TTServices/TelekomTariffs
        public ActionResult TelekomTariffs(int? page)
        {
            var results = db.TelekomTariffs.OrderBy(tt => tt.SpeedCode).Select(tt => new TelekomTariffViewModel()
            {
                _SpeedCode = tt.SpeedCode,
                Name = tt.Name
            });

            SetupPages(page, ref results);

            return View(results.ToList());
        }

        [AuthorizePermission(Permissions = "Telekom Tariffs")]
        [HttpGet]
        // GET: TTServices/AddTelekomTariff
        public ActionResult AddTelekomTariff()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Telekom Tariffs")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: TTServices/AddTelekomTariff
        public ActionResult AddTelekomTariff(TelekomTariffViewModel tariff)
        {
            if (ModelState.IsValid)
            {
                var sameCodeTariff = db.TelekomTariffs.Find(tariff._SpeedCode);
                if (sameCodeTariff != null)
                {
                    ModelState.AddModelError("SpeedCode", RadiusR.Localization.Validation.Common.SpeedCodeExists);
                    return View(tariff);
                }

                db.TelekomTariffs.Add(new TelekomTariff()
                {
                    SpeedCode = tariff._SpeedCode.Value,
                    Name = tariff.Name
                });

                db.SaveChanges();

                return RedirectToAction("TelekomTariffs", new { errorMessage = 0 });
            }

            return View(tariff);
        }

        [AuthorizePermission(Permissions = "Telekom Tariffs")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: TTServices/RemoveTelekomTariff
        public ActionResult RemoveTelekomTariff(int id)
        {
            var tariff = db.TelekomTariffs.Find(id);
            if (tariff == null)
                return RedirectToAction("TelekomTariffs", new { errorMessage = 9 });

            db.TelekomTariffs.Remove(tariff);

            db.SaveChanges();

            return RedirectToAction("TelekomTariffs", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Telekom Tariffs")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: TTServices/ClearTelekomTariffsCache
        public ActionResult ClearTelekomTariffsCache()
        {
            TelekomTariffsCache.ClearCache();
            return RedirectToAction("TelekomTariffs", new { errorMessage = 0 });
        }

        [AjaxCall]
        // POST: TTServices/GetTelekomPacketSelector
        public ActionResult GetTelekomPacketSelector(int? domainId, string prefix = null)
        {
            if (!domainId.HasValue)
                return Content(RadiusR.Localization.Pages.Common.AjaxLoadingError);

            var domain = DomainsCache.GetDomainByID(domainId.Value);
            if (domain == null)
                return Content(RadiusR.Localization.Pages.Common.AjaxLoadingError);

            ViewBag.CurrentDomain = domain;
            ViewData.TemplateInfo.HtmlFieldPrefix = prefix;
            return View();
        }
    }
}