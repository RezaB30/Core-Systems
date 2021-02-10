using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RezaB.Web.CustomAttributes;
using RadiusR_Manager.Models.ViewModels.JSON;
using RezaB.Web;

namespace RadiusR_Manager.Controllers
{
    public class ServiceController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Services")]
        // GET: Service
        public ActionResult Index(int? page, ServiceSearchViewModel search)
        {
            var temp = db.Services.ToArray();
            // search
            var results = db.Services.OrderByDescending(service => service.ID).AsQueryable();
            if (!string.IsNullOrEmpty(search.Name))
            {
                results = results.Where(service => service.Name.Contains(search.Name));
            }
            if (search.DomianID.HasValue)
            {
                results = results.Where(service => service.Domains.Any(d => d.ID == search.DomianID.Value));
            }
            if (search.BillingType.HasValue)
            {
                results = results.Where(service => service.BillingType == search.BillingType.Value);
            }
            if (search.QuotaType.HasValue)
            {
                results = results.Where(service => service.QuotaType == search.QuotaType.Value);
            }

            var viewResults = results.Select(service => new ServiceViewModel()
            {
                ID = service.ID,
                Name = service.Name,
                RateLimit = service.RateLimit,
                _price = service.Price,
                BillingType = service.BillingType,
                IsActive = service.IsActive,
                InfrastructureType = service.InfrastructureType,
                QuotaType = service.QuotaType,
                SoftQuotaRateLimit = service.SoftQuotaRateLimit,
                _maxSmartQuotaPrice = service.SmartQuotaMaxPrice,
                _baseQuota = service.BaseQuota,
                ServiceDomains = service.Domains.Select(d => new ServiceDomainViewModel()
                {
                    DomainID = d.ID,
                    DomainName = d.Name,
                    CanBeChanged = !d.Subscriptions.Any()
                }),
                BillingPeriods = service.ServiceBillingPeriods.Select(sbp => sbp.DayOfMonth),
                HasExternality = service.ExternalTariff != null
            });

            ViewBag.Search = search ?? new ServiceSearchViewModel();
            ViewBag.AllDomains = new SelectList(db.Domains.Select(d => new ServiceDomainViewModel()
            {
                DomainID = d.ID,
                DomainName = d.Name
            }), "DomainID", "DomainName", search.DomianID);

            SetupPages(page, ref viewResults);
            return View(viewResults.ToList());
        }

        [AuthorizePermission(Permissions = "Modify Services")]
        // GET: Service/Add
        public ActionResult Add()
        {
            ViewBag.AllDomains = db.Domains.Select(d => new ServiceDomainViewModel()
            {
                DomainID = d.ID,
                DomainName = d.Name
            });
            ViewBag.BillingPeriods = DaysOfMonth();

            return View(new ServiceViewModel()
            {
                ServiceDomains = new List<ServiceDomainViewModel>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Services")]
        // POST: Service/Add
        public ActionResult Add([Bind(Include = "Name,RateLimitView,Price,BillingType,InfrastructureType,MaxSmartQuotaPrice,BaseQuota,SoftQuotaRateLimitView,QuotaType,ServiceRateTimeTable,DomainIDs,BillingPeriods,PaymentTolerance,ExpirationTolerance,NoQueue")] ServiceViewModel service)
        {
            // prepare
            if (service.QuotaType == (short)QuotaType.SmartQuota && service.BillingType != (short)ServiceBillingType.Invoiced)
                ModelState.AddModelError("QuotaType", RadiusR.Localization.Validation.ModelSpecific.InvalidSmartQuotaForBillingType);
            PrepareServiceViewModel(service);
            PrepareServiceDomains(service);
            service.BillingPeriods = service.BillingPeriods != null ? service.BillingPeriods.Distinct().ToArray() : null;

            if (service.DomainIDs.Count() == 0)
                ModelState.AddModelError("ServiceDomains", RadiusR.Localization.Validation.Common.NoDomainsSelected);
            if (service.BillingType == (short)ServiceBillingType.Invoiced && service.BillingPeriods != null && service.BillingPeriods.Count() == 0)
                ModelState.AddModelError("BillingPeriods", RadiusR.Localization.Validation.Common.NoBillingPeriodSelected);

            if (ModelState.IsValid)
            {
                if (db.Services.FirstOrDefault(s => s.Name == service.Name) != null)
                {
                    ViewBag.ConflictError = RadiusR.Localization.Validation.Common.ConflictError;
                    return View(service);
                }
                if (service.HasConflictingTimeTable())
                {
                    ViewBag.TimeTableError = RadiusR.Localization.Validation.Common.ServiceTimeTableConflict;
                    return View(service);
                }
                if (service._maxSmartQuotaPrice.HasValue && service._price > service._maxSmartQuotaPrice)
                {
                    ViewBag.MaxSmartPriceError = RadiusR.Localization.Validation.Common.MaxSmartQuotaPriceError;
                    return View(service);
                }

                var addedDomains = service.DomainIDs.Select(dId => new Domain() { ID = dId }).ToArray();
                foreach (var addedDomain in addedDomains)
                {
                    db.Domains.Attach(addedDomain);
                }

                var dbService = new Service()
                {
                    Name = service.Name,
                    Price = service._price.Value,
                    BillingType = service.BillingType,
                    RateLimit = service.RateLimit,
                    IsActive = true,
                    InfrastructureType = service.InfrastructureType,
                    QuotaType = service.QuotaType,
                    BaseQuota = service._baseQuota,
                    SmartQuotaMaxPrice = service._maxSmartQuotaPrice,
                    SoftQuotaRateLimit = service.SoftQuotaRateLimit,
                    NoQueue = service.NoQueue,
                    ServiceRateTimeTables = service.ServiceRateTimeTable != null ? service.ServiceRateTimeTable.Select(timePartition => new ServiceRateTimeTable()
                    {
                        StartTime = timePartition._startTime.Value,
                        EndTime = timePartition._endTime.Value,
                        RateLimit = timePartition.RateLimit
                    }).ToList() : null,
                    Domains = addedDomains,
                    PaymentTolerance = service._paymentTolerance.Value,
                    ExpirationTolerance = service._expirationTolerance.Value
                };

                foreach (var dayOfMonth in service.BillingPeriods)
                {
                    dbService.ServiceBillingPeriods.Add(new ServiceBillingPeriod()
                    {
                        DayOfMonth = dayOfMonth
                    });
                }

                db.Services.Add(dbService);
                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.AllDomains = db.Domains.Select(d => new ServiceDomainViewModel()
            {
                DomainID = d.ID,
                DomainName = d.Name
            });
            ViewBag.BillingPeriods = DaysOfMonth(service.BillingPeriods);

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Services")]
        // POST: Service/Remove
        public ActionResult Remove(long id)
        {
            var service = db.Services.Find(id);
            if (service == null)
            {
                return RedirectToAction("Index", new { errorMessage = 2 });
            }
            if (service.Subscriptions.Any())
            {
                return RedirectToAction("Index", new { errorMessage = 1 });
            }

            db.ServiceRateTimeTables.RemoveRange(service.ServiceRateTimeTables);
            db.ServiceBillingPeriods.RemoveRange(service.ServiceBillingPeriods);
            if (service.ExternalTariff != null)
                db.ExternalTariffs.Remove(service.ExternalTariff);
            service.Domains.Clear();
            db.Services.Remove(service);
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Modify Services")]
        // GET: Service/Edit
        public ActionResult Edit(long id)
        {
            ViewBag.IsEdit = true;

            var dbService = PrepareService(db.Services.Include(s => s.ServiceBillingPeriods).Include(s => s.ServiceRateTimeTables).AsQueryable()).FirstOrDefault(s => s.DbService.ID == id);
            if (dbService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 2 });
            }
            var service = new ServiceViewModel()
            {
                Name = dbService.DbService.Name,
                RateLimit = dbService.DbService.RateLimit,
                _price = dbService.DbService.Price,
                BillingType = dbService.DbService.BillingType,
                InfrastructureType = dbService.DbService.InfrastructureType,
                QuotaType = dbService.DbService.QuotaType,
                SoftQuotaRateLimit = dbService.DbService.SoftQuotaRateLimit,
                _maxSmartQuotaPrice = dbService.DbService.SmartQuotaMaxPrice,
                _baseQuota = dbService.DbService.BaseQuota,
                NoQueue = dbService.DbService.NoQueue,
                ServiceRateTimeTable = dbService.DbService.ServiceRateTimeTables.Select(ratePartition => new ServiceRateTimePartitionViewModel()
                {
                    RateLimit = ratePartition.RateLimit,
                    _endTime = ratePartition.EndTime,
                    _startTime = ratePartition.StartTime
                }).ToList(),
                DomainIDs = dbService.Domains.Select(d => d.ID).ToArray(),
                BillingPeriods = dbService.DbService.ServiceBillingPeriods.Select(sbp => sbp.DayOfMonth).ToArray(),
                _paymentTolerance = dbService.DbService.PaymentTolerance,
                _expirationTolerance = dbService.DbService.ExpirationTolerance
            };

            PrepareServiceDomains(service, dbService);
            ViewBag.BillingPeriods = DaysOfMonth(service.BillingPeriods);
            return View(viewName: "Add", model: service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Services")]
        // POST: Service/Edit
        public ActionResult Edit(long id, [Bind(Include = "Name,RateLimitView,Price,BillingType,InfrastructureType,MaxSmartQuotaPrice,BaseQuota,SoftQuotaRateLimitView,QuotaType,ServiceRateTimeTable,DomainIDs,BillingPeriods,PaymentTolerance,ExpirationTolerance,NoQueue")] ServiceViewModel service)
        {
            var dbService = PrepareService(db.Services.Include(s => s.ServiceBillingPeriods).Include(s => s.ServiceRateTimeTables).AsQueryable()).FirstOrDefault(s => s.DbService.ID == id);
            if (dbService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 2 });
            }

            ViewBag.IsEdit = true;

            // prepare
            if (service.QuotaType == (short)QuotaType.SmartQuota && service.BillingType != (short)ServiceBillingType.Invoiced)
                ModelState.AddModelError("QuotaType", RadiusR.Localization.Validation.ModelSpecific.InvalidSmartQuotaForBillingType);
            PrepareServiceViewModel(service);
            PrepareServiceDomains(service, dbService);
            service.BillingPeriods = service.BillingPeriods != null ? service.BillingPeriods.Distinct().ToArray() : null;

            if (service.DomainIDs.Count() == 0)
                ModelState.AddModelError("ServiceDomains", RadiusR.Localization.Validation.Common.NoDomainsSelected);
            if (service.BillingType == (short)ServiceBillingType.Invoiced && service.BillingPeriods != null && service.BillingPeriods.Count() == 0)
                ModelState.AddModelError("BillingPeriods", RadiusR.Localization.Validation.Common.NoBillingPeriodSelected);

            if (ModelState.IsValid)
            {
                if (service.HasConflictingTimeTable())
                {
                    ViewBag.TimeTableError = RadiusR.Localization.Validation.Common.ServiceTimeTableConflict;
                    return View(viewName: "Add", model: service);
                }
                if (service._maxSmartQuotaPrice.HasValue && service._price > service._maxSmartQuotaPrice)
                {
                    ViewBag.MaxSmartPriceError = RadiusR.Localization.Validation.Common.MaxSmartQuotaPriceError;
                    return View(viewName: "Add", model: service);
                }

                dbService.DbService.RateLimit = service.RateLimit;
                dbService.DbService.Price = service._price.Value;
                dbService.DbService.InfrastructureType = service.InfrastructureType;
                dbService.DbService.QuotaType = service.QuotaType;
                dbService.DbService.BaseQuota = service._baseQuota;
                dbService.DbService.SmartQuotaMaxPrice = service._maxSmartQuotaPrice;
                dbService.DbService.SoftQuotaRateLimit = service.SoftQuotaRateLimit;
                dbService.DbService.PaymentTolerance = service._paymentTolerance.Value;
                dbService.DbService.ExpirationTolerance = service._expirationTolerance.Value;
                dbService.DbService.NoQueue = service.NoQueue;
                db.ServiceRateTimeTables.RemoveRange(dbService.DbService.ServiceRateTimeTables);
                dbService.DbService.ServiceRateTimeTables = service.ServiceRateTimeTable != null ? service.ServiceRateTimeTable.Select(timePartition => new ServiceRateTimeTable()
                {
                    StartTime = timePartition._startTime.Value,
                    EndTime = timePartition._endTime.Value,
                    RateLimit = timePartition.RateLimit
                }).ToList() : null;
                // check and edit domains
                var currentDomainIds = dbService.Domains.Select(d => d.ID).ToArray();
                foreach (var domainId in service.DomainIDs)
                {
                    if (!currentDomainIds.Contains(domainId))
                    {
                        var newDomain = new Domain() { ID = domainId };
                        db.Domains.Attach(newDomain);
                        dbService.DbService.Domains.Add(newDomain);
                    }
                }
                foreach (var domainId in currentDomainIds)
                {
                    if (!service.DomainIDs.Contains(domainId))
                    {
                        var newDomain = db.Domains.Find(domainId);
                        var externalTariffs = db.ExternalTariffs.Where(et => et.TariffID == dbService.DbService.ID && et.DomainID == domainId).ToArray();
                        db.ExternalTariffs.RemoveRange(externalTariffs);
                        dbService.DbService.Domains.Remove(newDomain);
                    }
                }
                // edit period lengths
                db.ServiceBillingPeriods.RemoveRange(dbService.DbService.ServiceBillingPeriods);
                foreach (var dayOfMonth in service.BillingPeriods)
                {
                    dbService.DbService.ServiceBillingPeriods.Add(new ServiceBillingPeriod()
                    {
                        DayOfMonth = dayOfMonth
                    });
                }
                // save
                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.BillingPeriods = DaysOfMonth(service.BillingPeriods);
            return View(viewName: "Add", model: service);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Modify Services")]
        // GET: Service/ChangeName
        public ActionResult ChangeName(string oldName)
        {
            var service = new RenameServiceViewModel()
            {
                OldName = oldName,
            };
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Services")]
        // POST: Service/ChangeName
        public ActionResult ChangeName([Bind(Include = "OldName,NewName")] RenameServiceViewModel service)
        {
            if (ModelState.IsValid)
            {
                var dbService = db.Services.FirstOrDefault(s => s.Name == service.OldName);
                if (dbService == null)
                {
                    return RedirectToAction("Edit", new { id = dbService.ID, errorMessage = 2 });
                }

                dbService.Name = service.NewName;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = dbService.ID, errorMessage = 0 });
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Services")]
        public ActionResult ChangeActiveState(int id)
        {
            var returnUrl = new UriBuilder(Request.UrlReferrer.ToString() ?? Url.Action("Index", null, null, Request.Url.Scheme));

            var dbService = db.Services.Find(id);
            if (dbService == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "2", returnUrl);
                return Redirect(returnUrl.Uri.PathAndQuery);
            }
            dbService.IsActive = !dbService.IsActive;
            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", returnUrl);
            return Redirect(returnUrl.Uri.PathAndQuery);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Externally Available Tariffs")]
        // GET: Service/AddExternality
        public ActionResult AddExternality(int id)
        {
            var dbTariff = db.Services.Find(id);
            if (dbTariff == null || dbTariff.ExternalTariff != null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            ViewBag.Domains = new SelectList(dbTariff.Domains.OrderBy(d => d.Name).Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name");
            ViewBag.TariffName = dbTariff.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Externally Available Tariffs")]
        // POST: Service/AddExternality
        public ActionResult AddExternality(int id, ExternalTariffViewModel externalTariff)
        {
            var dbTariff = db.Services.Find(id);
            if (dbTariff == null || dbTariff.ExternalTariff != null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                if (!(externalTariff.XDSL || externalTariff.Fiber))
                {
                    ModelState.AddModelError("XDSL", RadiusR.Localization.Validation.ModelSpecific.XDSLandFiberNotSelected);
                    ModelState.AddModelError("Fiber", RadiusR.Localization.Validation.ModelSpecific.XDSLandFiberNotSelected);
                }
                if (!dbTariff.Domains.Select(d => d.ID).Contains(externalTariff.DomainID))
                {
                    ModelState.AddModelError("DomainID", RadiusR.Localization.Validation.ModelSpecific.InvalidDomain);
                }
                if (ModelState.IsValid)
                {
                    dbTariff.ExternalTariff = new ExternalTariff()
                    {
                        DisplayName = externalTariff.DisplayName,
                        DomainID = externalTariff.DomainID,
                        HasFiber = externalTariff.Fiber,
                        HasXDSL = externalTariff.XDSL,
                    };

                    db.SaveChanges();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            ViewBag.Domains = new SelectList(dbTariff.Domains.OrderBy(d => d.Name).Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", externalTariff.DomainID);
            ViewBag.TariffName = dbTariff.Name;
            return View(externalTariff);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Externally Available Tariffs")]
        // GET: Service/EditExternality
        public ActionResult EditExternality(int id)
        {
            var dbTariff = db.Services.Find(id);
            if (dbTariff == null || dbTariff.ExternalTariff == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var externalTariff = new ExternalTariffViewModel()
            {
                DisplayName = dbTariff.ExternalTariff.DisplayName,
                DomainID = dbTariff.ExternalTariff.DomainID,
                Fiber = dbTariff.ExternalTariff.HasFiber,
                XDSL = dbTariff.ExternalTariff.HasXDSL,
                TariffID = dbTariff.ID
            };

            ViewBag.IsEdit = true;
            ViewBag.Domains = new SelectList(dbTariff.Domains.OrderBy(d => d.Name).Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", externalTariff.DomainID);
            ViewBag.TariffName = dbTariff.Name;
            return View(viewName: "AddExternality", model: externalTariff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Externally Available Tariffs")]
        // Post: Service/EditExternality
        public ActionResult EditExternality(int id, ExternalTariffViewModel externalTariff)
        {
            var dbTariff = db.Services.Find(id);
            if (dbTariff == null || dbTariff.ExternalTariff == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                if (!(externalTariff.XDSL || externalTariff.Fiber))
                {
                    ModelState.AddModelError("XDSL", RadiusR.Localization.Validation.ModelSpecific.XDSLandFiberNotSelected);
                    ModelState.AddModelError("Fiber", RadiusR.Localization.Validation.ModelSpecific.XDSLandFiberNotSelected);
                }
                if (!dbTariff.Domains.Select(d => d.ID).Contains(externalTariff.DomainID))
                {
                    ModelState.AddModelError("DomainID", RadiusR.Localization.Validation.ModelSpecific.InvalidDomain);
                }
                if (ModelState.IsValid)
                {
                    dbTariff.ExternalTariff.DomainID = externalTariff.DomainID;
                    dbTariff.ExternalTariff.DisplayName = externalTariff.DisplayName;
                    dbTariff.ExternalTariff.HasFiber = externalTariff.Fiber;
                    dbTariff.ExternalTariff.HasXDSL = externalTariff.XDSL;

                    db.SaveChanges();
                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            ViewBag.IsEdit = true;
            ViewBag.Domains = new SelectList(dbTariff.Domains.OrderBy(d => d.Name).Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", externalTariff.DomainID);
            ViewBag.TariffName = dbTariff.Name;
            return View(viewName: "AddExternality", model: externalTariff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Externally Available Tariffs")]
        // POST: Service/RemoveExternality
        public ActionResult RemoveExternality(int id)
        {
            var dbTariff = db.Services.Find(id);
            if (dbTariff == null || dbTariff.ExternalTariff == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            db.ExternalTariffs.Remove(dbTariff.ExternalTariff);
            db.SaveChanges();
            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Clients")]
        public JsonResult GetDomainServices(int id)
        {
            return Json(new SelectListJSON() { Items = db.Domains.Find(id).Services.Where(s => s.IsActive).Select(s => new SelectListJSON.SelectListItem() { Name = s.Name, Value = s.ID }) });
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Clients")]
        public JsonResult GetServiceBillingPeriods(int id)
        {
            var currentTariff = db.Services.Find(id);
            var items = currentTariff.ServiceBillingPeriods.OrderBy(sbp => sbp.DayOfMonth).Select(sbp => new SelectListJSON.SelectListItem() { Name = sbp.DayOfMonth.ToString(), Value = sbp.DayOfMonth });
            var selectedValue = currentTariff.GetBestBillingPeriod(DateTime.Now.Day);
            return Json(new SelectListJSON() { Items = items, selectedValue = selectedValue });
        }

        private void PrepareServiceViewModel(ServiceViewModel model)
        {
            // prepare quota type
            if (model.QuotaType == null)
            {
                model.BaseQuota = null;
                ModelState.Remove("BaseQuota");
            }
            if (model.QuotaType != (short)QuotaType.SoftQuota)
            {
                model.SoftQuotaRateLimit = null;
                ModelState.Remove("SoftQuotaRateLimit");
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("SoftQuotaRateLimitView.")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
                ModelState.Remove("SoftQuotaRateLimitView");
            }
            if (model.QuotaType != (short)QuotaType.SmartQuota)
            {
                model.MaxSmartQuotaPrice = null;
                ModelState.Remove("MaxSmartQuotaPrice");
            }
            // prepare billing type
            if (model.BillingType == (short)ServiceBillingType.PrePaid)
            {
                model.BillingPeriods = Enumerable.Empty<short>();
                model._paymentTolerance = 0;
                model._expirationTolerance = 0;
                ModelState.Remove("BillingPeriods");
                ModelState.Remove("PaymentTolerance");
                ModelState.Remove("ExpirationTolerance");
            }
        }

        private void PrepareServiceDomains(ServiceViewModel model, PreparedServiceWithDomains dbService = null)
        {
            var allDomains = db.Domains.Select(d => new ServiceDomainViewModel()
            {
                DomainID = d.ID,
                DomainName = d.Name
            }).ToArray();

            model.DomainIDs = model.DomainIDs != null ? model.DomainIDs.Distinct().ToArray() : new int[0];

            if (dbService == null)
            {
                model.ServiceDomains = model.DomainIDs.Select(dId => new ServiceDomainViewModel()
                {
                    DomainID = dId,
                    DomainName = allDomains.FirstOrDefault(ad => ad.DomainID == dId).DomainName,
                    CanBeChanged = true
                }).ToArray();
            }
            else
            {
                var constantDomains = dbService.Domains.Where(d => d.HasSubscribers).ToArray();
                model.DomainIDs = model.DomainIDs.Concat(constantDomains.Select(cd => cd.ID).ToArray()).Distinct().ToArray();
                model.ServiceDomains = model.DomainIDs.Select(dId => new ServiceDomainViewModel()
                {
                    DomainID = dId,
                    DomainName = allDomains.FirstOrDefault(ad => ad.DomainID == dId).DomainName,
                    CanBeChanged = !constantDomains.Select(cd => cd.ID).Contains(dId)
                }).ToArray();
                allDomains = allDomains.Where(d => !constantDomains.Select(cd => cd.ID).Contains(d.DomainID)).ToArray();
            }
            ViewBag.AllDomains = allDomains;
        }

        private MultiSelectList DaysOfMonth(IEnumerable<short> selectedValues = null)
        {
            List<object> values = new List<object>();
            for (short i = 1; i < 29; i++)
            {
                values.Add(new
                {
                    Name = i,
                    Value = i
                });
            }

            return new MultiSelectList(values, "Value", "Name", selectedValues);
        }

        private IQueryable<PreparedServiceWithDomains> PrepareService(IQueryable<Service> query)
        {
            return query.Select(s => new PreparedServiceWithDomains()
            {
                DbService = s,
                Domains = s.Domains.Select(d => new PreparedServiceWithDomains.DomainInfo()
                {
                    ID = d.ID,
                    HasSubscribers = d.Subscriptions.Any(sub => sub.ServiceID == s.ID)
                })
            });
        }

        private class PreparedServiceWithDomains
        {
            public Service DbService { get; set; }

            public IEnumerable<DomainInfo> Domains { get; set; }

            public class DomainInfo
            {
                public int ID { get; set; }

                public Domain Domain { get; set; }

                public bool HasSubscribers { get; set; }
            }
        }
    }
}