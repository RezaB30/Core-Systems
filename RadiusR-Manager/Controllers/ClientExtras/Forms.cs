using RadiusR_Manager.Models.ViewModels;
using RezaB.Data.Localization;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        [AuthorizePermission(Permissions = "Subscription Forms")]
        [HttpGet]
        // GET: Client/SubscriptionForms
        public ActionResult SubscriptionForms(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content("<span class='text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._9 + "</span>");
            }

            var viewResults = new SubscriptionFormsViewModel()
            {
                SubscriptionID = dbSubscription.ID,
                HasDSLInfo = dbSubscription.SubscriptionTelekomInfo != null,
                HasEmailAddress = !string.IsNullOrWhiteSpace(dbSubscription.Customer.Email)
            };
            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.AllForms = new LocalizedList<RadiusR_Manager.Models.Enums.PDFForms, RadiusR_Manager.Models.Enums.PDFFormNames>().GetList();
            ViewBag.Transfers = new SelectList(dbSubscription.SubscriptionTransferredFromHistories.Select(sth => new { Key = sth.ID, Name = sth.TransferredSubscription.SubscriberNo, Date = sth.Date }).Concat(dbSubscription.SubscriptionTransferredToHistories.Select(sth => new { Key = sth.ID, Name = sth.TransferrerSubscription.SubscriberNo, Date = sth.Date })).OrderByDescending(sth => sth.Date).ToArray(), "Key", "Name");
            return View(viewName: "Forms/Index", model: viewResults);
        }

        [AuthorizePermission(Permissions = "Subscription Forms")]
        // GET: Client/DownloadForm
        public ActionResult DownloadForm(long id, int formType, long? transferId = null)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            switch ((RadiusR_Manager.Models.Enums.PDFForms)formType)
            {
                case Models.Enums.PDFForms.ContractForm:
                    {
                        var createdPDF = RadiusR.PDFForms.PDFWriter.GetContractPDF(db, id);
                        if (createdPDF.InternalException != null)
                        {
                            return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                        }
                        return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR_Manager.Models.Enums.PDFFormNames.ContractForm}");
                    }
                case Models.Enums.PDFForms.TransitionForm:
                    {
                        var createdPDF = RadiusR.PDFForms.PDFWriter.GetTransitionPDF(db, id);
                        if (createdPDF.InternalException != null)
                        {
                            return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                        }
                        return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR_Manager.Models.Enums.PDFFormNames.TransitionForm}");
                    }
                case Models.Enums.PDFForms.PSTNtoNakedForm:
                    {
                        var createdPDF = RadiusR.PDFForms.PDFWriter.GetPSTNtoNakedPDF(db, id);
                        if (createdPDF.InternalException != null)
                        {
                            return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                        }
                        return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR_Manager.Models.Enums.PDFFormNames.PSTNtoNakedForm}");
                    }
                case Models.Enums.PDFForms.TransferForm:
                    {
                        if (!transferId.HasValue)
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                        // find transfer
                        var transfer = db.SubscriptionTransferHistories.Find(transferId);
                        if (transfer == null)
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

                        if (transfer.To == id || transfer.From == id)
                        {
                            var createdPDF = RadiusR.PDFForms.PDFWriter.GetTransferPDF(db, transfer.From, transfer.To);
                            if (createdPDF.InternalException != null)
                            {
                                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                            }
                            return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR_Manager.Models.Enums.PDFFormNames.TransferForm}");
                        }
                        else
                        {
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                        }
                    }
                default:
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}