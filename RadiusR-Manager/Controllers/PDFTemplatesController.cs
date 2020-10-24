using RezaB.Web.CustomAttributes;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels.PDFTemplates;
using System.IO;
using RadiusR.Files;
using RezaB.Data.Localization;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "PDF Templates")]
    public class PDFTemplatesController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        protected const string pathBase = "~/LocalFiles/";

        // GET: PDFTemplates
        public ActionResult Index()
        {
            var list = new LocalizedList<PDFFormType, RadiusR.Localization.Lists.PDFFormType>();
            ViewBag.PDFFormsList = list.GetList();
            return View();
        }

        [HttpGet]
        // GET: PDFTemplates/EditTemplate
        public ActionResult EditTemplate(int id)
        {
            ViewBag.TemplateName = new LocalizedList<PDFFormType, RadiusR.Localization.Lists.PDFFormType>().GetDisplayText(id);
            ViewBag.HasBackground = RadiusR.Files.FileManager.PDFTemplateExists(id);
            return View(new PDFParametersViewModel(db, id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/EditTemplate
        public ActionResult EditTemplate(int id, PDFParametersViewModel parameters)
        {
            parameters.UpdateDatabase(db, id);
            db.SaveChanges();
            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/ChangeTemplateBackground
        public ActionResult ChangeTemplateBackground(int id, HttpPostedFileBase background)
        {
            if (background != null && background.ContentLength > 0)
            {
                RadiusR.Files.FileManager.SavePDFFormTemplate(background.InputStream, id, background.FileName.Split('.').LastOrDefault());
            }

            return RedirectToAction("EditTemplate", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/RemoveTemplateBackground
        public ActionResult RemoveTemplateBackground(int id)
        {
            RadiusR.Files.FileManager.DeletePDFTemplate(id);

            return RedirectToAction("EditTemplate", new { id = id });
        }

        [HttpGet]
        // GET: PDFTemplates/GetPDFBackground
        public ActionResult GetPDFBackground(int id)
        {
            var file = RadiusR.Files.FileManager.GetPDFTemplate(id);

            return File(file ?? new MemoryStream(), "image");
        }

        [HttpGet]
        // GET: PDFTemplates/ContractClausesPDF
        public ActionResult ContractClausesPDF()
        {
            ViewBag.FileExists = FileManager.ContractAppendixExists();
            return View();
        }

        [HttpGet]
        // GET: PDFTemplates/DownloadContractClauses
        public ActionResult DownloadContractClauses()
        {
            return File(FileManager.GetContractAppendix(), "application/pdf", RadiusR.Localization.Pages.Common.ContractAppendix + ".pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/RemoveContractClauses
        public ActionResult RemoveContractClauses()
        {
            FileManager.RemoveContractAppendix();
            return RedirectToAction("ContractClausesPDF", new { errorMessage = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/ContractClausesPDF
        public ActionResult ContractClausesPDF(HttpPostedFileBase AppendixFile)
        {
            FileManager.SaveContractAppendix(AppendixFile.InputStream);
            return RedirectToAction("ContractClausesPDF", new { errorMessage = 0 });
        }
    }
}