﻿using RadiusR.DB.DomainsCache;
using RadiusR.DB.Settings;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Customer Website Settings")]
    public class CustomerWebsiteSettingsController : BaseController
    {
        [HttpGet]
        // GET: CustomerWebsiteSettings
        public ActionResult Index()
        {
            var results = new CustomerWebsiteSettingsViewModel(true);
            ViewBag.Domains = new SelectList(DomainsCache.GetTelekomDomains(), "ID", "Name", results.WebsiteServicesInfrastructureDomainID);
            return View(results);
        }

        // POST: CustomerWebsiteSettings
        public ActionResult Index(CustomerWebsiteSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                settings.WebsiteServicesInfrastructureDomainID = settings.WebsiteServicesInfrastructureDomainID ?? 0;
                CustomerWebsiteSettings.Update(settings);
                return RedirectToAction(null, new { errorMessage = 0 });
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetTelekomDomains(), "ID", "Name", settings.WebsiteServicesInfrastructureDomainID);
            return View(settings);
        }
    }
}