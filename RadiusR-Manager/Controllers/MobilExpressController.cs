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
    public class MobilExpressController : BaseController
    {
        [AuthorizePermission(Permissions = "MobilExpress Settings")]
        [HttpGet]
        // GET: MobilExpress/Settings
        public ActionResult Settings()
        {
            var settings = new MobilExpressSettingsViewModel()
            {
                MobilExpressMerchantKey = MobilExpressSettings.MobilExpressMerchantKey,
                MobilExpressAPIPassword = MobilExpressSettings.MobilExpressAPIPassword,
                MobilExpressPOSID = MobilExpressSettings.MobilExpressPOSID.ToString(),
                MobilExpressIsActive = MobilExpressSettings.MobilExpressIsActive
            };
            return View(settings);
        }

        [AuthorizePermission(Permissions = "MobilExpress Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: MobilExpress/Settings
        public ActionResult Settings(MobilExpressSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                MobilExpressSettings.Update(settings);
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        } 
    }
}