using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "BTK Settings")]
    public class BTKSettingsController : BaseController
    {
        [HttpGet]
        // GET: BTKSettings
        public ActionResult Index()
        {
            var result = new BTKSettingsViewModel()
            {
                BTKOperatorCode = BTKSettings.BTKOperatorCode,
                BTKOperatorDepartment = BTKSettings.BTKOperatorDepartment,
                BTKOperatorName = BTKSettings.BTKOperatorName,
                BTKOperatorType = BTKSettings.BTKOperatorType
            };

            ViewBag.OperatorTypes = new SelectList(Enum.GetNames(typeof(BTKOperatorTypes)).Select(name => new { Name = name, Value = name }), "Value", "Name", result.BTKOperatorType);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BTKSettings
        public ActionResult Index(BTKSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                BTKSettings.Update(settings);
                BTKSettings.ClearCache();
                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.OperatorTypes = new SelectList(Enum.GetNames(typeof(BTKOperatorTypes)).Select(name => new { Name = name, Value = name }), "Value", "Name", settings.BTKOperatorType);
            return View(settings);
        }
    }
}