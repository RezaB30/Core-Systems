using RadiusR.DB;
using RadiusR.Files;
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
            ViewBag.MailBodyFiles = FileManager.GetContractMailBodies().ToArray();
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
                FileManager.SaveContractMailBody(htmlFile.InputStream, culture);
                return RedirectToAction("Forms", new { errorMessage = 0 });
            }
            return RedirectToAction("Forms", new { errorMessage = 9 });
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpGet]
        // GET: Email/DownloadFormFile
        public ActionResult DownloadFormFile(string fileName)
        {
            return File(FileManager.GetContractMailBodyByFileName(fileName), FileManager.GetMIMETypeFromFileName(fileName), fileName);
        }

        [AuthorizePermission(Permissions = "Email Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Email/RemoveFormFile
        public ActionResult RemoveFormFile(string fileName)
        {
            FileManager.DeleteContractMailBody(fileName);
            return RedirectToAction("Forms", new { errorMessage = 0 });
        }
    }
}