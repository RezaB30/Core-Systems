using RadiusR.DB;
using RadiusR.DB.DomainsCache;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Domain Settings")]
    public class DomainSettingsController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        // GET: DomainSettings
        public ActionResult Index(int? page)
        {
            var results = db.Domains.OrderBy(domain => domain.ID).Select(domain => new DomainViewModel()
            {
                ID = domain.ID,
                Name = domain.Name,
                UsernamePrefix = domain.UsernamePrefix,
                SubscriberNoPrefix = domain.SubscriberNoPrefix,
                AccessMethod = domain.AccessMethod,
                //TelekomAccessCredential = domain.AccessMethod.HasValue ? new TelekomAccessCredentialViewModel()
                //{
                //    OLOPortalCustomerCode = domain.TelekomAccessCredential.OLOPortalCustomerCode,
                //    OLOPortalPassword = domain.TelekomAccessCredential.OLOPortalPassword,
                //    OLOPortalUsername = domain.TelekomAccessCredential.OLOPortalUsername,
                //    XDSLWebServiceCustomerCode = domain.TelekomAccessCredential.XDSLWebServiceCustomerCode,
                //    XDSLWebServicePassword = domain.TelekomAccessCredential.XDSLWebServicePassword,
                //    XDSLWebServiceUsername = domain.TelekomAccessCredential.XDSLWebServiceUsername,
                //    TransitionFTPUsername = domain.TelekomAccessCredential.TransitionFTPUsername,
                //    TransitionFTPPassword = domain.TelekomAccessCredential.TransitionFTPPassword,
                //    TransitionOperatorID = domain.TelekomAccessCredential.TransitionOperatorID
                //} : null,
                CanBeDeleted = !domain.Subscriptions.Any() && !domain.Services.Any()
            });

            SetupPages(page, ref results);

            return View(results.ToList());
        }

        [HttpGet]
        // GET: DomainSettings/Add
        public ActionResult Add()
        {
            return View(new DomainViewModel() { TelekomAccessCredential = new TelekomAccessCredentialViewModel() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: DomainSettings/Add
        public ActionResult Add(DomainViewModel addedDomain)
        {
            if (!addedDomain.AccessMethod.HasValue)
            {
                var names = ModelState.Where(ms => ms.Key.Contains("TelekomAccessCredential.")).Select(ms => ms.Key).ToArray();
                foreach (var name in names)
                {
                    ModelState.Remove(name);
                }
            }

            if (ModelState.IsValid)
            {
                addedDomain.Name = addedDomain.Name.ToLower();
                addedDomain.UsernamePrefix = addedDomain.UsernamePrefix.ToLower();
                if (db.Domains.Any(d => d.Name == addedDomain.Name))
                {
                    ModelState.AddModelError("Name", string.Format(RadiusR.Localization.Validation.Common.Unique, typeof(DomainViewModel).GetProperty("Name").GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().Single().GetName()));
                }
                if (db.Domains.Any(d => d.UsernamePrefix == addedDomain.UsernamePrefix))
                {
                    ModelState.AddModelError("UsernamePrefix", string.Format(RadiusR.Localization.Validation.Common.Unique, typeof(DomainViewModel).GetProperty("UsernamePrefix").GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().Single().GetName()));
                }
                if (ModelState.IsValid)
                {
                    var dbDomain = new Domain()
                    {
                        Name = addedDomain.Name,
                        UsernamePrefix = addedDomain.UsernamePrefix,
                        SubscriberNoPrefix = addedDomain.SubscriberNoPrefix,
                        AccessMethod = addedDomain.AccessMethod,
                        MaxFreezeDuration = addedDomain.MaxFreezeDuration.Value,
                        MaxFreezesPerYear = addedDomain.MaxFreezesPerYear.Value
                    };
                    if (addedDomain.AccessMethod.HasValue)
                    {
                        dbDomain.TelekomAccessCredential = new TelekomAccessCredential()
                        {
                            OLOPortalCustomerCode = addedDomain.TelekomAccessCredential.OLOPortalCustomerCode,
                            OLOPortalPassword = addedDomain.TelekomAccessCredential.OLOPortalPassword,
                            OLOPortalUsername = addedDomain.TelekomAccessCredential.OLOPortalUsername,
                            XDSLWebServiceCustomerCode = addedDomain.TelekomAccessCredential.XDSLWebServiceCustomerCode,
                            XDSLWebServicePassword = addedDomain.TelekomAccessCredential.XDSLWebServicePassword,
                            XDSLWebServiceUsername = addedDomain.TelekomAccessCredential.XDSLWebServiceUsername,
                            TransitionFTPUsername = addedDomain.TelekomAccessCredential.TransitionFTPUsername,
                            TransitionFTPPassword = addedDomain.TelekomAccessCredential.TransitionFTPPassword,
                            TransitionOperatorID = addedDomain.TelekomAccessCredential.TransitionOperatorID.Value
                        };
                    }

                    db.Domains.Add(dbDomain);
                    db.SaveChanges();

                    DomainsCache.UpdateCache();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            return View(addedDomain);
        }

        [HttpGet]
        // GET: DomainSettings/Edit
        public ActionResult Edit(int id)
        {
            var dbDomain = db.Domains.Find(id);
            if (dbDomain == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            ViewBag.CurrentDomain = dbDomain.Name;

            return View(viewName: "Add", model: new DomainViewModel()
            {
                Name = dbDomain.Name,
                UsernamePrefix = dbDomain.UsernamePrefix,
                SubscriberNoPrefix = dbDomain.SubscriberNoPrefix,
                AccessMethod = dbDomain.AccessMethod,
                MaxFreezeDuration = dbDomain.MaxFreezeDuration,
                MaxFreezesPerYear = dbDomain.MaxFreezesPerYear,
                TelekomAccessCredential = dbDomain.TelekomAccessCredential != null ?
                new TelekomAccessCredentialViewModel()
                {
                    OLOPortalCustomerCode = dbDomain.TelekomAccessCredential.OLOPortalCustomerCode,
                    OLOPortalPassword = dbDomain.TelekomAccessCredential.OLOPortalPassword,
                    OLOPortalUsername = dbDomain.TelekomAccessCredential.OLOPortalUsername,
                    XDSLWebServiceCustomerCode = dbDomain.TelekomAccessCredential.XDSLWebServiceCustomerCode,
                    XDSLWebServicePassword = dbDomain.TelekomAccessCredential.XDSLWebServicePassword,
                    XDSLWebServiceUsername = dbDomain.TelekomAccessCredential.XDSLWebServiceUsername,
                    TransitionFTPUsername = dbDomain.TelekomAccessCredential.TransitionFTPUsername,
                    TransitionFTPPassword = dbDomain.TelekomAccessCredential.TransitionFTPPassword,
                    TransitionOperatorID = dbDomain.TelekomAccessCredential.TransitionOperatorID
                }
                : new TelekomAccessCredentialViewModel()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: DomainSettings/Edit
        public ActionResult Edit(int id, DomainViewModel editedDomain)
        {
            var dbDomain = db.Domains.Find(id);
            if (dbDomain == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            if (!editedDomain.AccessMethod.HasValue)
            {
                var names = ModelState.Where(ms => ms.Key.Contains("TelekomAccessCredential.")).Select(ms => ms.Key).ToArray();
                foreach (var name in names)
                {
                    ModelState.Remove(name);
                }
            }

            if (ModelState.IsValid)
            {
                editedDomain.Name = editedDomain.Name.ToLower();
                editedDomain.UsernamePrefix = editedDomain.UsernamePrefix.ToLower();
                if (db.Domains.Any(d => d.Name == editedDomain.Name && d.ID != dbDomain.ID))
                {
                    ModelState.AddModelError("Name", string.Format(RadiusR.Localization.Validation.Common.Unique, typeof(DomainViewModel).GetProperty("Name").GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().Single().GetName()));
                }
                if (db.Domains.Any(d => d.UsernamePrefix == editedDomain.UsernamePrefix && d.ID != dbDomain.ID))
                {
                    ModelState.AddModelError("UsernamePrefix", string.Format(RadiusR.Localization.Validation.Common.Unique, typeof(DomainViewModel).GetProperty("UsernamePrefix").GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().Single().GetName()));
                }

                if (ModelState.IsValid)
                {

                    dbDomain.Name = editedDomain.Name;
                    dbDomain.UsernamePrefix = editedDomain.UsernamePrefix;
                    dbDomain.SubscriberNoPrefix = editedDomain.SubscriberNoPrefix;
                    dbDomain.AccessMethod = editedDomain.AccessMethod;
                    dbDomain.MaxFreezeDuration = editedDomain.MaxFreezeDuration.Value;
                    dbDomain.MaxFreezesPerYear = editedDomain.MaxFreezesPerYear.Value;

                    if (editedDomain.AccessMethod.HasValue)
                    {
                        if (dbDomain.TelekomAccessCredential == null)
                        {
                            dbDomain.TelekomAccessCredential = new TelekomAccessCredential()
                            {
                                OLOPortalCustomerCode = editedDomain.TelekomAccessCredential.OLOPortalCustomerCode,
                                OLOPortalPassword = editedDomain.TelekomAccessCredential.OLOPortalPassword,
                                OLOPortalUsername = editedDomain.TelekomAccessCredential.OLOPortalUsername,
                                XDSLWebServiceCustomerCode = editedDomain.TelekomAccessCredential.XDSLWebServiceCustomerCode,
                                XDSLWebServicePassword = editedDomain.TelekomAccessCredential.XDSLWebServicePassword,
                                XDSLWebServiceUsername = editedDomain.TelekomAccessCredential.XDSLWebServiceUsername,
                                TransitionFTPUsername = editedDomain.TelekomAccessCredential.TransitionFTPUsername,
                                TransitionFTPPassword = editedDomain.TelekomAccessCredential.TransitionFTPPassword,
                                TransitionOperatorID = editedDomain.TelekomAccessCredential.TransitionOperatorID.Value
                            };
                        }
                        else
                        {
                            dbDomain.TelekomAccessCredential.OLOPortalCustomerCode = editedDomain.TelekomAccessCredential.OLOPortalCustomerCode;
                            dbDomain.TelekomAccessCredential.OLOPortalPassword = editedDomain.TelekomAccessCredential.OLOPortalPassword;
                            dbDomain.TelekomAccessCredential.OLOPortalUsername = editedDomain.TelekomAccessCredential.OLOPortalUsername;
                            dbDomain.TelekomAccessCredential.XDSLWebServiceCustomerCode = editedDomain.TelekomAccessCredential.XDSLWebServiceCustomerCode;
                            dbDomain.TelekomAccessCredential.XDSLWebServicePassword = editedDomain.TelekomAccessCredential.XDSLWebServicePassword;
                            dbDomain.TelekomAccessCredential.XDSLWebServiceUsername = editedDomain.TelekomAccessCredential.XDSLWebServiceUsername;
                            dbDomain.TelekomAccessCredential.TransitionFTPUsername = editedDomain.TelekomAccessCredential.TransitionFTPUsername;
                            dbDomain.TelekomAccessCredential.TransitionFTPPassword = editedDomain.TelekomAccessCredential.TransitionFTPPassword;
                            dbDomain.TelekomAccessCredential.TransitionOperatorID = editedDomain.TelekomAccessCredential.TransitionOperatorID.Value;
                        }
                    }
                    else
                    {
                        if(dbDomain.TelekomAccessCredential != null)
                        {
                            db.TelekomAccessCredentials.Remove(dbDomain.TelekomAccessCredential);
                        }
                    }

                    db.SaveChanges();

                    DomainsCache.UpdateCache();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            ViewBag.CurrentDomain = dbDomain.Name;

            return View(viewName: "Add", model: editedDomain);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: DomainSettings
        public ActionResult Remove(int id)
        {
            var dbDomain = db.Domains.Find(id);
            if (dbDomain == null || dbDomain.Subscriptions.Any() || dbDomain.Services.Any())
                return RedirectToAction("Index", new { errorMessage = 9 });

            if (dbDomain.TelekomAccessCredential != null)
                db.TelekomAccessCredentials.Remove(dbDomain.TelekomAccessCredential);
            db.Domains.Remove(dbDomain);
            db.SaveChanges();

            DomainsCache.UpdateCache();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }
    }
}