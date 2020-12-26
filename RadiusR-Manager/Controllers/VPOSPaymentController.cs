using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Enums;
using RadiusR.SMS;
using RadiusR_Manager.Models;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.VPOS;
using RadiusR.SystemLogs;
using RadiusR.DB.ModelExtentions;
using RezaB.Web.CustomAttributes;
using RezaB.Web;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    public class VPOSPaymentController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [HttpGet]
        [AuthorizePermission(Permissions = "Payment")]
        // POST: VPOSPayment
        public ActionResult Index(int id)
        {
            var token = PaymentTokenManager.GetToken(id);
            if (token == null)
            {
                return RedirectToAction("Index", "Bill", new { errorMessage = 30 });
            }

            var VPOSModel = VPOSManager.GetVPOSModel(Url.Action("SuccessfulPay", null, new { id = id }, Request.Url.Scheme), Url.Action("FailedPay", null, new { id = id }, Request.Url.Scheme), token.Amount, token.Language, token.ClientName);
            //ViewBag.VPOSForm = VPOSModel.GetHtmlForm();
            return View(VPOSModel);

            //var uri = new UriBuilder(token.ReturnUrl);
            //ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;

            //return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AuthorizePermission(Permissions = "Clients")]
        //// POST: VPOSPayment
        //public ActionResult Index(int id, [Bind(Include = "CardNumber,CVV2,CardExpirationMonth,CardExpirationYear")] CardPaymentViewModel cardPayment)
        //{
        //    var token = PaymentTokenManager.GetToken(id);
        //    if (token == null)
        //    {
        //        return RedirectToAction("Index", "Bill", new { errorMessage = 30 });
        //    }

        //    //if (ModelState.IsValid)
        //    //{
        //    //    var posClient = new VirtualPOSClient();

        //    //    var results = posClient.SendPaymentRequest(new PaymentDetails()
        //    //    {
        //    //        Amount = token.Amount,
        //    //        PAN = cardPayment.CardNumber,
        //    //        CVV2 = cardPayment.CVV2,
        //    //        ExpirationMonth = cardPayment._expMonth.Value,
        //    //        ExpirationYear = cardPayment._expYear.Value,
        //    //        BillID = token is PaymentTokenManager.BillPaymentToken ? (token as PaymentTokenManager.BillPaymentToken).BillIds.First().ToString() : null,
        //    //        ClientAddress = token.ClientAddress,
        //    //        ClientName = token.ClientName,
        //    //        ClientTel = token.ClientTel,
        //    //        Currency = CurrencyCodes.TL,
        //    //        Language = token.Language,
        //    //        SubscriberNo = token.SubscriberNo,
        //    //        TariffName = token.ServiceName
        //    //    },
        //    //    Url.Action("SuccessfulPay", "VPOSPayment", new { id = id }, Request.Url.Scheme),
        //    //    Url.Action("FailedPay", "VPOSPayment", new { id = id }, Request.Url.Scheme));

        //    //    return Content(results);
        //    //}

        //    var uri = new UriBuilder(token.ReturnUrl);
        //    ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
        //    return View(cardPayment);
        //}

        // POST: VPOSPayment/SuccessfulPay
        public ActionResult SuccessfulPay(int id)
        {
            var token = PaymentTokenManager.GetToken(id);
            if (token == null)
            {
                return RedirectToAction("Index", "Bill", new { errorMessage = 30 });
            }

            //var PosClient = new VirtualPOSClient();

            //if (PosClient.GetAmount(Request.Form) != token.Amount)
            //{
            //    return RedirectToAction("FailedPay", new { id = id });
            //}

            var returnUrl = new UriBuilder(token.ReturnUrl);

            if (token is PaymentTokenManager.PacketExtentionToken)
            {
                var packetExtentionToken = (PaymentTokenManager.PacketExtentionToken)token;
                var dbClient = db.Subscriptions.Find(packetExtentionToken.ClientId);
                if(dbClient == null)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", returnUrl);
                    return Redirect(returnUrl.Uri.PathAndQuery + returnUrl.Fragment);
                }
                var result = db.ExtendClientPackage(dbClient, packetExtentionToken.ExtentionPeriodCount, PaymentType.VirtualPos, User.GiveAccountantType(), User.GiveUserId());
                if (result == BillPayment.ResponseType.NotEnoughCredit)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "12", returnUrl);
                    return Redirect(returnUrl.Uri.PathAndQuery + returnUrl.Fragment);
                }

                SMSServiceAsync SMSAsync = new SMSServiceAsync();
                db.SMSArchives.AddSafely(SMSAsync.SendSubscriberSMS(dbClient, SMSType.ExtendPackage, new Dictionary<string, object>()
                {
                    { SMSParamaterRepository.SMSParameterNameCollection.ExtendedMonths, "1" }
                }));

                db.SaveChanges();
            }
            else if (token is PaymentTokenManager.BillPaymentToken)
            {
                var billPaymentToken = (PaymentTokenManager.BillPaymentToken)token;
                var dbBills = db.Bills.Where(bill => billPaymentToken.BillIds.Contains(bill.ID)).ToList();
                var result = db.PayBills(dbBills, PaymentType.VirtualPos, User.GiveAccountantType(), User.GiveUserId());
                if (result == BillPayment.ResponseType.NotEnoughCredit)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "12", returnUrl);
                    return Redirect(returnUrl.Uri.PathAndQuery + returnUrl.Fragment);
                }

                // send payment sms
                var billGroups = dbBills.GroupBy(bill => bill.Subscription);
                SMSServiceAsync SMSAsync = new SMSServiceAsync();
                var sentSMSes = new List<SMSArchive>();
                foreach (var billGroup in billGroups)
                {
                    sentSMSes.Add(SMSAsync.SendSubscriberSMS(billGroup.Key, SMSType.PaymentDone, new Dictionary<string, object>()
                    {
                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, billGroup.Sum(bill => bill.GetPayableCost()) }
                    }));
                    db.SystemLogs.Add(SystemLogProcessor.BillPayment(billGroup.Select(b => b.ID), User.GiveUserId(), billGroup.Key.ID, SystemLogInterface.MasterISS, null, PaymentType.VirtualPos));
                }
                db.SMSArchives.AddRangeSafely(sentSMSes);

                db.SaveChanges();
            }
            else if (token is PaymentTokenManager.AdditionalFeePaymentToken)
            {
                var additionalFeePaymentToken = (PaymentTokenManager.AdditionalFeePaymentToken)token;
                var dbClient = db.Subscriptions.Find(additionalFeePaymentToken.ClientID);
                if (dbClient == null)
                {
                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", returnUrl);
                    return Redirect(returnUrl.Uri.PathAndQuery + returnUrl.Fragment);
                }
                var addedBill = db.AddPrepaidFeeAndPayTheBill(dbClient, additionalFeePaymentToken.AdditionalFees, PaymentType.VirtualPos, User.GiveUserId());

                SMSServiceAsync SMSAsync = new SMSServiceAsync();
                db.SMSArchives.AddSafely(SMSAsync.SendSubscriberSMS(dbClient, SMSType.PaymentDone, new Dictionary<string, object>()
                {
                    { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, addedBill.GetPayableCost() }
                }));

                db.SaveChanges();
            }

            PaymentTokenManager.RemoveToken(id);

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", returnUrl);
            return Redirect(returnUrl.Uri.PathAndQuery + returnUrl.Fragment);
        }

        // POST: VPOSPayment/FailedPay
        public ActionResult FailedPay(int id)
        {
            var token = PaymentTokenManager.GetToken(id);
            if (token == null)
            {
                return RedirectToAction("Index", "Bill", new { errorMessage = 30 });
            }

            var returnUrl = new UriBuilder(token.ReturnUrl);

            //var PosClient = new VirtualPOSClient();
            //ViewBag.ErrorMessage = PosClient.GetErrorMessage(Request.Form);

            ViewBag.ErrorMessage = Request.Form[VPOSManager.GetErrorMessageParameterName()];

            PaymentTokenManager.RemoveToken(id);

            ViewBag.BackUrl = returnUrl.Uri.PathAndQuery + returnUrl.Fragment;
            return View();
        }
    }
}