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
    public class FileManagerController : BaseController
    {
        [HttpGet]
        [AuthorizePermission(Permissions = "File Manager Settings")]
        // GET: FileManager
        public ActionResult Settings()
        {
            return View(new FileManagerSettingsViewModel(true));
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "File Manager Settings")]
        [ValidateAntiForgeryToken]
        // POST: FileManager/Index
        public ActionResult Settings(FileManagerSettingsViewModel settings)
        {
            if (settings.FileManagerType == (short)RadiusR.DB.Enums.FileManagerTypes.Local)
            {
                settings.FileManagerHost = settings.FileManagerUsername = settings.FileManagerPassword = "-";
                ModelState.Remove("FileManagerHost");
                ModelState.Remove("FileManagerUsername");
                ModelState.Remove("FileManagerPassword");
            }
            if (ModelState.IsValid)
            {
                FileManagerSettings.Update(settings);
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        }
    }
}