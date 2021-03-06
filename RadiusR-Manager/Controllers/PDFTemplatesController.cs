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
using RezaB.Data.Localization;
using RadiusR.FileManagement;

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
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetPDFForm((PDFFormType)id);
            if (result.InternalException != null)
            {
                ViewBag.FileErrorMessage = RadiusR.Localization.Pages.Common.FileManagerError;
                ViewBag.HasBackground = false;
            }
            else
            {
                ViewBag.HasBackground = true;
            }
            ViewBag.TemplateName = new LocalizedList<PDFFormType, RadiusR.Localization.Lists.PDFFormType>().GetDisplayText(id);

            var list = new LocalizedList<PDFItemIDs, RadiusR.Localization.Lists.PDFItemIDs>();
            var viewResults = db.PDFFormItemPlacements.Where(fi => fi.FormType == id).ToArray()
                .Select(fi => new PDFItemPlacementViewModel()
                {
                    ID = fi.ItemID,
                    Name = list.GetDisplayText(fi.ItemID),
                    Placement = new Coordinates(fi.CoordsX, fi.CoordsY)
                }).ToArray();
            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/EditTemplate
        public ActionResult EditTemplate(int id, PDFItemPlacementViewModel[] parameters)
        {
            var toRemove = db.PDFFormItemPlacements.Where(fi => fi.FormType == id).ToArray();
            db.PDFFormItemPlacements.RemoveRange(toRemove);
            var toAdd = parameters.Select(p => new PDFFormItemPlacement()
            {
                FormType = id,
                ItemID = p.ID,
                CoordsX = p.Placement.X.Value,
                CoordsY = p.Placement.Y.Value
            }).ToArray();
            db.PDFFormItemPlacements.AddRange(toAdd);
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
                var fileManager = new MasterISSFileManager();
                var result = fileManager.SavePDFForm((PDFFormType)id, new FileManagerBasicFile(background.FileName, background.InputStream));
                if (result.InternalException != null)
                {
                    return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                }
            }

            return RedirectToAction("EditTemplate", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/RemoveTemplateBackground
        public ActionResult RemoveTemplateBackground(int id)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.RemovePDFForm((PDFFormType)id);
            if(result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }

            return RedirectToAction("EditTemplate", new { id = id });
        }

        [HttpGet]
        // GET: PDFTemplates/GetPDFBackground
        public ActionResult GetPDFBackground(int id)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetPDFForm((PDFFormType)id);
            if (result.InternalException != null)
            {
                return new HttpStatusCodeResult(500);
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }

            return File(result.Result.Content, result.Result.MIMEType);
        }

        [HttpGet]
        // GET: PDFTemplates/ContractClausesPDF
        public ActionResult ContractClausesPDF()
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.ContractAppendixExists();
            ViewBag.FileExists = result.Result;
            return View();
        }

        [HttpGet]
        // GET: PDFTemplates/DownloadContractClauses
        public ActionResult DownloadContractClauses()
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetContractAppendix();
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }

            return File(result.Result.Content, result.Result.MIMEType, RadiusR.Localization.Pages.Common.ContractAppendix + ".pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/RemoveContractClauses
        public ActionResult RemoveContractClauses()
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.RemoveContractAppendix();
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }

            return RedirectToAction("ContractClausesPDF", new { errorMessage = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PDFTemplates/ContractClausesPDF
        public ActionResult ContractClausesPDF(HttpPostedFileBase AppendixFile)
        {
            var fileManager = new MasterISSFileManager();
            var result = fileManager.SaveContractAppendix(new FileManagerBasicFile(AppendixFile.FileName, AppendixFile.InputStream));
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }

            return RedirectToAction("ContractClausesPDF", new { errorMessage = 0 });
        }
    }
}