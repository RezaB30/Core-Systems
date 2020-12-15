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
            return View(results);
        }

        // POST: CustomerWebsiteSettings
        public ActionResult Index(CustomerWebsiteSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                CustomerWebsiteSettings.Update(settings);
                return RedirectToAction(null, new { errorMessage = 0 });
            }

            return View(settings);
        }
    }
}