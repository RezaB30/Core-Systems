using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RezaB.NetInvoice.RadiusRDBAdapter;
using RadiusR.DB;
using System.Data.Entity;
using RezaB.NetInvoice.Wrapper;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "EBill")]
    public class EBillController : BaseController
    {
        private const int batchSize = 1000;

        // GET: EBill
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Batch EBill")]
        [HttpGet]
        // GET: EBill/Batch
        public ActionResult Batch()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Batch EBill")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: EBill/Batch
        public ActionResult Batch(HttpPostedFileBase BatchFile,BatchEBillViewModel batchSettings)
        {
            if (BatchFile != null && BatchFile.ContentLength > 0)
            {
                try
                {
                    StreamReader reader = new StreamReader(BatchFile.InputStream);
                    // skip titles
                    reader.ReadLine();
                    // create id list
                    var sentIds = new List<long>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var IdString = line.Split('\t').FirstOrDefault();
                        long parsed;
                        if (long.TryParse(IdString, out parsed))
                            sentIds.Add(parsed);
                    }
                    // try updating e-bill companies
                    try
                    {
                        Adapter.UpdateEBillCompanies();
                    }
                    catch { }
                    // send e-bills
                    var results = Adapter.SendBatch(sentIds, batchSettings.IssueDate);
                    TempData.Add("EBillBatchResults", results);
                    return RedirectToAction("BatchResults");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "";
                }
            }
            return RedirectToAction("Batch", new { errorMessage = 35 });
        }

        [AuthorizePermission(Permissions = "Batch EBill")]
        // GET: EBill/BatchResults
        public ActionResult BatchResults()
        {
            object results;
            if (TempData.TryGetValue("EBillBatchResults", out results))
            {
                var resultsData = results as EBillBatchResults;
                if(resultsData != null)
                {
                    var viewResults = new EBillBatchResultsViewModel()
                    {
                        ErrorCode = (short)resultsData.ErrorCode,
                        SuccessfulCount = resultsData.SuccessfulCount,
                        TotalCount = resultsData.TotalCount,
                        UnsuccessfulCount = resultsData.UnsuccessfulCount
                    };
                    return View(viewResults);
                }
            }

            return RedirectToAction("Batch");
        }
    }
}