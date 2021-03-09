using RadiusR.DB;
using RadiusR.FileManagement;
using RadiusR.SystemLogs;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Data.Localization;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var viewResults = new SubscriptionFormsViewModel()
            {
                SubscriptionID = dbSubscription.ID,
                HasDSLInfo = dbSubscription.SubscriptionTelekomInfo != null,
                HasEmailAddress = !string.IsNullOrWhiteSpace(dbSubscription.Customer.Email)
            };
            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.AllForms = new LocalizedList<RadiusR.DB.Enums.GeneralPDFFormTypes, RadiusR.Localization.Lists.GeneralPDFFormTypes>().GetList();
            ViewBag.Transfers = new SelectList(dbSubscription.SubscriptionTransferredFromHistories.Select(sth => new { Key = sth.ID, Name = sth.TransferredSubscription.SubscriberNo, Date = sth.Date }).Concat(dbSubscription.SubscriptionTransferredToHistories.Select(sth => new { Key = sth.ID, Name = sth.TransferrerSubscription.SubscriberNo, Date = sth.Date })).OrderByDescending(sth => sth.Date).ToArray(), "Key", "Name");
            ViewBag.SendEmailError = TempData["SendEmailError"];
            return View(viewName: "Forms/Index", model: viewResults);
        }

        [AuthorizePermission(Permissions = "Subscription Forms,Send Email To Client")]
        [HttpPost]
        // POST: Client/SubscriptionForms
        public ActionResult SubscriptionForms(long id, SubscriptionFormEmailListViewModel[] selectedForms, long? transferId = null)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null || string.IsNullOrWhiteSpace(dbSubscription.Customer.Email))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            if (!selectedForms.Any(sf => sf.IsSelected))
            {
                TempData["SendEmailError"] = RadiusR.Localization.Validation.ModelSpecific.NoFormsSelected;
                return RedirectToAction("SubscriptionForms", new { id = id, errorMessage = 9 });
            }

            // localized strings
            var rm = RadiusR.Localization.MasterResourceManager.GetResourceManager("RadiusR.Localization.Pages.Common");
            //var attachmentName = $"{rm.GetString("CotractMailAttachmentName", System.Globalization.CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture))}.pdf";
            var subject = string.Format(rm.GetString("ContractMailSubject", System.Globalization.CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture)), AppSettings.CompanyName);
            // get body
            string bodyText = string.Empty;
            var fileManager = new MasterISSFileManager();
            using (var bodyContent = fileManager.GetContractMailBody(dbSubscription.Customer.Culture))
            {
                if (bodyContent.InternalException != null)
                {
                    return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                }
                using (var reader = new StreamReader(bodyContent.Result.Content))
                {
                    bodyText = reader.ReadToEnd();
                    bodyText = bodyText.Replace("([fullName])", dbSubscription.ValidDisplayName);
                }
            }
            // prepare attachments
            var fileNameRM = RadiusR.Localization.Lists.GeneralPDFFormTypes.ResourceManager;
            var attachments = new List<RezaB.Mailing.MailFileAttachment>();
            foreach (var selectedForm in selectedForms.Where(sf => sf.IsSelected))
            {
                switch ((RadiusR.DB.Enums.GeneralPDFFormTypes)selectedForm.FormType)
                {
                    case RadiusR.DB.Enums.GeneralPDFFormTypes.ContractForm:
                        {
                            var form = RadiusR.PDFForms.PDFWriter.GetContractPDF(db, dbSubscription.ID);
                            if (form.InternalException != null)
                            {
                                return Content($"<div class='centered text-danger'>{RadiusR.Localization.Pages.Common.FileManagerError}</div>");
                            }
                            attachments.Add(new RezaB.Mailing.MailFileAttachment()
                            {
                                Content = form.Result,
                                FileName = $"{dbSubscription.SubscriberNo}-{fileNameRM.GetString("ContractForm", CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture))}.pdf"
                            });
                        }
                        break;
                    case RadiusR.DB.Enums.GeneralPDFFormTypes.TransitionForm:
                        {
                            var form = RadiusR.PDFForms.PDFWriter.GetTransitionPDF(db, dbSubscription.ID);
                            if (form.InternalException != null)
                            {
                                return Content($"<div class='centered text-danger'>{RadiusR.Localization.Pages.Common.FileManagerError}</div>");
                            }
                            attachments.Add(new RezaB.Mailing.MailFileAttachment()
                            {
                                Content = form.Result,
                                FileName = $"{dbSubscription.SubscriberNo}-{fileNameRM.GetString("TransitionForm", CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture))}.pdf"
                            });
                        }
                        break;
                    case RadiusR.DB.Enums.GeneralPDFFormTypes.PSTNtoNakedForm:
                        {
                            var form = RadiusR.PDFForms.PDFWriter.GetPSTNtoNakedPDF(db, dbSubscription.ID);
                            if (form.InternalException != null)
                            {
                                return Content($"<div class='centered text-danger'>{fileNameRM.GetString("FileManagerError", CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture))}</div>");
                            }
                            attachments.Add(new RezaB.Mailing.MailFileAttachment()
                            {
                                Content = form.Result,
                                FileName = $"{dbSubscription.SubscriberNo}-{fileNameRM.GetString("PSTNtoNakedForm", CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture))}.pdf"
                            });
                        }
                        break;
                    case RadiusR.DB.Enums.GeneralPDFFormTypes.TransferForm:
                        {
                            if (transferId.HasValue)
                            {
                                // find transfer
                                var transfer = db.SubscriptionTransferHistories.Find(transferId);
                                if (transfer == null)
                                {
                                    return RedirectToAction("SubscriptionForms", new { id = id, errorMessage = 9 });
                                }
                                    

                                if (transfer.To == id || transfer.From == id)
                                {
                                    var form = RadiusR.PDFForms.PDFWriter.GetTransferPDF(db, transfer.From, transfer.To);
                                    if (form.InternalException != null)
                                    {
                                        return Content($"<div class='centered text-danger'>{RadiusR.Localization.Pages.Common.FileManagerError}</div>");
                                    }
                                    attachments.Add(new RezaB.Mailing.MailFileAttachment()
                                    {
                                        Content = form.Result,
                                        FileName = $"{dbSubscription.SubscriberNo}-{fileNameRM.GetString("TransferForm", CultureInfo.CreateSpecificCulture(dbSubscription.Customer.Culture))}.pdf"
                                    });
                                }
                                else
                                {
                                    return RedirectToAction("SubscriptionForms", new { id = id, errorMessage = 9 });
                                }
                            }
                            else
                            {
                                TempData["SendEmailError"] = RadiusR.Localization.Validation.ModelSpecific.NoTransferSelected;
                                return RedirectToAction("SubscriptionForms", new { id = id, errorMessage = 9 });
                            }
                        }
                        break;
                    default:
                        return RedirectToAction("SubscriptionForms", new { id = id, errorMessage = 9 });
                }
            }
            // send mail
            RezaB.Mailing.IMailClient mailClient = new RezaB.Mailing.Client.MailClient(EmailSettings.SMTPEmailHost, EmailSettings.SMTPEMailPort, false, EmailSettings.SMTPEmailAddress, EmailSettings.SMTPEmailPassword);
            mailClient.SendMail(new RezaB.Mailing.StandardMailMessage(
                new System.Net.Mail.MailAddress(EmailSettings.SMTPEmailDisplayEmail, EmailSettings.SMTPEmailDisplayName),
                new string[] { dbSubscription.Customer.Email },
                null,
                null,
                subject,
                bodyText,
                RezaB.Mailing.MailBodyType.HTML,
                attachments
            ));

            db.SystemLogs.Add(SystemLogProcessor.SentFormViaEmail(User.GiveUserId(), dbSubscription.ID, selectedForms.Where(sf => sf.IsSelected).Select(sf => (RadiusR.DB.Enums.GeneralPDFFormTypes)sf.FormType).ToArray(), RadiusR.DB.Enums.SystemLogInterface.MasterISS, null));
            db.SaveChanges();

            return RedirectToAction("SubscriptionForms", new { id = id, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Subscription Forms")]
        // GET: Client/DownloadForm
        public ActionResult DownloadForm(long id, int formType, long? transferId = null)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            switch ((RadiusR.DB.Enums.GeneralPDFFormTypes)formType)
            {
                case RadiusR.DB.Enums.GeneralPDFFormTypes.ContractForm:
                    {
                        var createdPDF = RadiusR.PDFForms.PDFWriter.GetContractPDF(db, id);
                        if (createdPDF.InternalException != null)
                        {
                            return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                        }
                        return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR.Localization.Lists.GeneralPDFFormTypes.ContractForm}.pdf");
                    }
                case RadiusR.DB.Enums.GeneralPDFFormTypes.TransitionForm:
                    {
                        var createdPDF = RadiusR.PDFForms.PDFWriter.GetTransitionPDF(db, id);
                        if (createdPDF.InternalException != null)
                        {
                            return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                        }
                        return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR.Localization.Lists.GeneralPDFFormTypes.TransitionForm}.pdf");
                    }
                case RadiusR.DB.Enums.GeneralPDFFormTypes.PSTNtoNakedForm:
                    {
                        var createdPDF = RadiusR.PDFForms.PDFWriter.GetPSTNtoNakedPDF(db, id);
                        if (createdPDF.InternalException != null)
                        {
                            return Content(RadiusR.Localization.Pages.Common.FileManagerError);
                        }
                        return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR.Localization.Lists.GeneralPDFFormTypes.PSTNtoNakedForm}.pdf");
                    }
                case RadiusR.DB.Enums.GeneralPDFFormTypes.TransferForm:
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
                            return File(createdPDF.Result, "application/pdf", $"{subscription.SubscriberNo}-{RadiusR.Localization.Lists.GeneralPDFFormTypes.TransferForm}.pdf");
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