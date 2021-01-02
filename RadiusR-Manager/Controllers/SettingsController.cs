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
    [AuthorizePermission(Permissions = "System Settings")]
    public class SettingsController : BaseController
    {
        [HttpGet]
        // GET: Settings
        public ActionResult Index()
        {
            var results = new AppSettingsViewModel(true);
            return View(results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Settings
        public ActionResult Index(AppSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                AppSettings.Update(settings);
                return RedirectToAction(null, new { errorMessage = 0 });
            }

            return View(settings);
        }

        [HttpGet]
        // GET: Settings/Logo
        public ActionResult Logo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logo(HttpPostedFileBase logo)
        {
            if (logo != null && logo.ContentLength > 0)
            {
                var path = Server.MapPath("~/Content/Images/Logo/logo.svg");
                logo.SaveAs(path);
                return RedirectToAction("Index", "Settings");
            }
            return View();
        }
    }
}