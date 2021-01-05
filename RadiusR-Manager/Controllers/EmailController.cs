using RadiusR.DB;
using RadiusR.FileManagement;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public class EmailController : BaseController
    {
        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpGet]
        // GET: Email/Settings
        public ActionResult Settings()
        {
            var settings = new EmailSettingsViewModel(true);
            return View(settings);
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Email/Settings
        public ActionResult Settings(EmailSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                EmailSettings.Update(settings);
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpGet]
        // GET: Email/Forms
        public ActionResult Forms()
        {
            var fileManager = new MasterISSFileManager();
            var results = fileManager.ListContractMailBodies();
            if (results.InternalException != null)
            {
                ViewBag.FileErrorMessage = RadiusR.Localization.Pages.Common.FileManagerError;
                ViewBag.MailBodyFiles = new string[0];
            }
            else
            {
                ViewBag.MailBodyFiles = results.Result.ToArray();
            }
            return View();
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Email/Forms
        public ActionResult Forms(HttpPostedFileBase htmlFile, string culture)
        {
            if (htmlFile != null && htmlFile.ContentLength > 0)
            {
                if (htmlFile.ContentLength > 10 * Math.Pow(1024, 2))
                {
                    return RedirectToAction("Forms", new { errorMessage = 38 });
                }
                var fileManager = new MasterISSFileManager();
                var result = fileManager.SaveContractMailBody(new FileManagerBasicFile(htmlFile.FileName, htmlFile.InputStream), culture);
                if (result.InternalException != null)
                {
                    return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                }
                else
                {
                    return RedirectToAction("Forms", new { errorMessage = 0 });
                }
            }
            return RedirectToAction("Forms", new { errorMessage = 9 });
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpGet]
        // GET: Email/DownloadFormFile
        public ActionResult DownloadFormFile(string fileName)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetContractMailBodyByName(fileName);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            return File(result.Result.Content, result.Result.MIMEType, result.Result.FileName);
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Email/RemoveFormFile
        public ActionResult RemoveFormFile(string fileName)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.RemoveContractMailBodyByName(fileName);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }
            return RedirectToAction("Forms", new { errorMessage = 0 });
        }
    }
}