using RadiusR.DB.DomainsCache;
using RadiusR.DB;
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
        RadiusREntities db = new RadiusREntities();

        [HttpGet]
        // GET: CustomerWebsiteSettings
        public ActionResult Index()
        {
            var results = new CustomerWebsiteSettingsViewModel(true);
            ViewBag.Domains = new SelectList(DomainsCache.GetTelekomDomains(), "ID", "Name", results.WebsiteServicesInfrastructureDomainID);
            ViewBag.Groups = new SelectList(db.Groups.Where(g => g.IsActive || g.ID == results.CustomerWebsiteRegistrationGroupID).Select(g => new { Name = g.Name, ID = g.ID }), "ID", "Name", results.CustomerWebsiteRegistrationGroupID);
            return View(results);
        }

        // POST: CustomerWebsiteSettings
        public ActionResult Index(CustomerWebsiteSettingsViewModel settings)
        {
            if (!settings.CustomerWebsiteUseGoogleRecaptcha)
            {
                settings.CustomerWebsiteRecaptchaClientKey = settings.CustomerWebsiteRecaptchaServerKey = "-";
                ModelState.Remove("CustomerWebsiteRecaptchaClientKey");
                ModelState.Remove("CustomerWebsiteRecaptchaServerKey");
            }

            if (ModelState.IsValid)
            {
                settings.WebsiteServicesInfrastructureDomainID = settings.WebsiteServicesInfrastructureDomainID ?? 0;
                settings.CustomerWebsiteRegistrationGroupID = settings.CustomerWebsiteRegistrationGroupID ?? 0;
                CustomerWebsiteSettings.Update(settings);
                return RedirectToAction(null, new { errorMessage = 0 });
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetTelekomDomains(), "ID", "Name", settings.WebsiteServicesInfrastructureDomainID);
            ViewBag.Groups = new SelectList(db.Groups.Where(g => g.IsActive || g.ID == settings.CustomerWebsiteRegistrationGroupID).Select(g => new { Name = g.Name, ID = g.ID }), "ID", "Name", settings.CustomerWebsiteRegistrationGroupID);
            return View(settings);
        }
    }
}