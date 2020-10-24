using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB.Utilities.Billing;
using RadiusR.SMS;
using System.IO;
using System.Text;
using RadiusR_Manager.Models.CSVModels;
using RezaB.Data.Files;
using RadiusR_Manager.Models;
using static RadiusR.DB.Utilities.Billing.BillExtentions;
using RadiusR.SystemLogs;
using RadiusR.DB.ModelExtentions;
using RezaB.Web.CustomAttributes;
using RadiusR.DB.QueryExtentions;
using RezaB.Web;
using RezaB.Data.Localization;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    public class BillController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Bills")]
        [HttpGet]
        // GET: Bill
        public ActionResult Index(int? page, [Bind(Include = "CustomerName,AccountantID,IssueDateStart,IssueDateEnd,DueDateStart,DueDateEnd,PaymentDateStart,PaymentDateEnd,State,PaymentTypeID,PaymentGateway,BillingPeriod,EBillCode")]BillSearchViewModel search)
        {
            var viewResults = db.Bills.OrderByDescending(bill => bill.ID).AsQueryable();
            // apply search variables
            viewResults = FilterSearch(viewResults, search);

            // set pagings
            SetupPages(page, ref viewResults);
            // fill viewbag
            ViewBag.Accountants = new SelectList(db.AppUsers.ToArray(), "ID", "Name", search.AccountantID);
            ViewBag.PaymentGateways = new SelectList(db.RadiusRBillingServices.Select(gateway => new { Id = gateway.ID, Name = gateway.Name }).ToArray(), "Id", "Name", search.PaymentGateway);
            ViewBag.Search = search;
            return View(viewResults.GetViewModels());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [AuthorizePermission(Permissions = "Download Bills Tax Details")]
        // POST: Bill/BillTaxCSV
        public ActionResult BillTaxCSV([Bind(Include = "CustomerName,AccountantID,IssueDateStart,IssueDateEnd,DueDateStart,DueDateEnd,PaymentDateStart,PaymentDateEnd,State,PaymentTypeID,BillingPeriod,EBillCode")]BillSearchViewModel search)
        {
            var viewResults = db.Bills.Where(bill => bill.EBill != null && bill.BillStatusID != (short)BillState.Cancelled).AsQueryable();
            // apply search variables
            viewResults = FilterSearch(viewResults, search);

            //viewResults = viewResults.Where(bill => bill.PartialPercentage > 0m && bill.AllTimeFees.Any(fee => fee.FeeTypeID == (short)FeeType.Tariff));
            var results = viewResults
                .Include(bill => bill.Subscription)
                .Include(bill => bill.EBill)
                //.Include(bill => bill.Service)
                .Include(bill => bill.BillFees.Select(bf => bf.FeeTypeCost.TaxRates)).Include(bill => bill.BillFees.Select(bf => bf.Discount)).Include(bill => bill.BillFees.Select(bf => bf.Fee.FeeTypeCost.TaxRates))
                .ToList().OrderBy(bill => bill.IssueDate).ThenBy(bill => bill.Subscription.ValidDisplayName).Select(bill => new BillTaxDetailsCSVModel()
                {
                    IssueDate = bill.EBill.EBillIssueDate.ToString("dd-MM-yyyy"),
                    EBillIssueDate = bill.EBill.EBillIssueDate.ToString("dd-MM-yyyy"),
                    EBillType = new LocalizedList<EBillType, RadiusR.Localization.Lists.EBillType>().GetDisplayText(bill.EBill.EBillType),
                    BillCode = bill.EBill.BillCode,
                    TaxRegion = bill.Subscription.Customer.CorporateCustomerInfo != null ? bill.Subscription.Customer.CorporateCustomerInfo.TaxOffice : null,
                    TaxNo = bill.Subscription.Customer.CorporateCustomerInfo != null ? bill.Subscription.Customer.CorporateCustomerInfo.TaxNo : null,
                    TCKNo = bill.Subscription.Customer.CustomerIDCard.TCKNo,
                    ServiceName = bill.BillFees.Any(bf => bf.FeeTypeID == (short)FeeType.Tariff) ? bill.BillFees.FirstOrDefault(bf => bf.FeeTypeID == (short)FeeType.Tariff).Description : null,
                    SubscriberNo = bill.Subscription.SubscriberNo,
                    ClientValidName = bill.Subscription.ValidDisplayName,
                    Total = bill.GetPayableCost().ToString("###,###,##0.00"),
                    TaxBase = bill.GetTaxBase().ToString("###,###,##0.00"),
                    VAT = bill.GetTaxAmount(TaxTypeID.VAT).ToString("###,###,##0.00"),
                    SCT = bill.GetTaxAmount(TaxTypeID.SCT).ToString("###,###,##0.00"),
                    Discount = bill.GetTotalDiscount().ToString("###,###,##0.00")
                });

            var currentTime = DateTime.Now;
            return File(CSVGenerator.GetStream(results, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.GetTaxDetails + "_" + currentTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
        }

        [AuthorizePermission(Permissions = "Batch EBill")]
        [HttpPost]
        // POST: Bill/EBillCSV
        public ActionResult EBillCSV(BillSearchViewModel search)
        {
            var viewResults = db.Bills.Where(bill => bill.BillStatusID != (short)BillState.Cancelled).AsQueryable();
            // apply search variables
            viewResults = FilterSearch(viewResults, search);

            var results = viewResults
                .Include(bill => bill.Subscription)
                //.Include(bill => bill.Service)
                .Include(bill => bill.ExternalPayment.RadiusRBillingService).Include(bill => bill.ExternalPayment.OfflinePaymentGateway)
                .Include(bill => bill.BillFees.Select(bf => bf.FeeTypeCost.TaxRates)).Include(bill => bill.BillFees.Select(bf => bf.Discount)).Include(bill => bill.BillFees.Select(bf => bf.Fee.FeeTypeCost.TaxRates))
                .ToList().OrderBy(bill => bill.IssueDate).ThenBy(bill => bill.Subscription.ValidDisplayName).Select(bill => new BatchEBillCSVModel()
                {
                    BillId = bill.ID.ToString("000000000000000"),
                    SubscriberNo = bill.Subscription.SubscriberNo,
                    ClientValidName = bill.Subscription.ValidDisplayName,
                    ClientPhoneNo = bill.Subscription.Customer.ContactPhoneNo,
                    IssueDate = bill.IssueDate.ToString("yyyyMMdd"),
                    DueDate = bill.DueDate.ToString("yyyyMMdd"),
                    PayDate = bill.PayDate.HasValue ? bill.PayDate.Value.ToString("yyyyMMdd") : null,
                    Total = bill.GetPayableCost().ToString("###,###,##0.00"),
                    PaymentType = new LocalizedList<PaymentType, RadiusR.Localization.Lists.PaymentType>().GetDisplayText(bill.PaymentTypeID) + (bill.ExternalPayment != null ? "(" + (bill.ExternalPayment.RadiusRBillingService != null ? bill.ExternalPayment.RadiusRBillingService.Name : bill.ExternalPayment.OfflinePaymentGateway != null ? bill.ExternalPayment.OfflinePaymentGateway.Name : "<INVALID>") + ")" : null),
                    URL = Url.Action("Details", "Bill", new { id = bill.ID }, Request.Url.Scheme)
                });

            var currentTime = DateTime.Now;
            return File(CSVGenerator.GetStream(results, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.Bills + "_" + currentTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
        }

        [AuthorizePermission(Permissions = "Bills,Clients")]
        // GET: Bill/Details/id
        public ActionResult Details(int id, string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = Request.Url.Host + Url.Action("Index", "Bill");
            }
            var uri = new UriBuilder(redirectUrl);

            var dbBill = db.Bills.Find(id);
            if (dbBill == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "8", uri);
                return Redirect(uri.Uri.PathAndQuery);
            }

            var bill = dbBill.GetViewModel();

            ViewBag.RedirectUrl = redirectUrl;
            var credits = dbBill.Subscription.SubscriptionCredits.Sum(credit => credit.Amount);
            ViewBag.Credits = credits;
            var payingAmount = bill._totalPayableAmount - credits;
            payingAmount = payingAmount < 0m ? 0m : payingAmount;
            ViewBag.PayingAmount = payingAmount;
            return View(bill);
        }

        [AuthorizePermission(Permissions = "Clients")]
        // GET: Bill/ClientBills
        public ActionResult ClientBills(long id, int? page)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var clientBills = db.Bills.OrderByDescending(bill => bill.IssueDate).OrderByDescending(bill => bill.ID).Where(bill => bill.SubscriptionID == dbSubscription.ID);

            SetupPages(page, ref clientBills);

            ViewBag.ClientName = dbSubscription.ValidDisplayName;
            ViewBag.ClientID = dbSubscription.ID;
            ViewBag.IsValidForPacketExtention = dbSubscription.IsActive && !dbSubscription.HasBilling;
            ViewBag.HasBilling = dbSubscription.HasBilling;
            return View(clientBills.GetViewModels());
        }

        [AuthorizePermission(Permissions = "Payment", Roles = "cashier")]
        [HttpGet]
        // GET: Bill/ClientPayment
        public ActionResult ClientPayment(long id)
        {
            var dbClient = db.Subscriptions.Find(id);
            if (dbClient == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!dbClient.HasBilling)
            {
                return RedirectToAction("Details", "Client", new { errorMessage = 9, id = id });
            }

            var clientBills = new ClientBillsViewModel(dbClient);

            clientBills.BillSelections = clientBills.BillSelections.Where(selection => selection.Bill.StateID == (short)BillState.Unpaid).OrderBy(selection => selection.Bill.IssueDate);

            return View(clientBills);
        }

        [AuthorizePermission(Permissions = "Payment", Roles = "cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Bill/ClientPayment
        public ActionResult ClientPayment([Bind(Include = "BillSelections,ClientID")]ClientBillsViewModel clientBills)
        {
            var dbClient = db.Subscriptions.Find(clientBills.ClientID);
            if (dbClient == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!dbClient.HasBilling)
            {
                return RedirectToAction("Details", "Client", new { errorMessage = 9, id = clientBills.ClientID });
            }

            var dbClientBills = new ClientBillsViewModel(dbClient);
            dbClientBills.BillSelections = dbClientBills.BillSelections.Where(selection => selection.Bill.StateID == (short)BillState.Unpaid).OrderBy(selection => selection.Bill.IssueDate).ToList();

            foreach (var item in dbClientBills.BillSelections)
            {
                item.IsSelected = clientBills.BillSelections.FirstOrDefault(selection => selection.BillID == item.BillID).IsSelected;
            }

            if (clientBills.BillSelections != null && clientBills.BillSelections.Any(selection => selection.IsSelected))
            {
                dbClientBills.BillSelections = dbClientBills.BillSelections.Where(selection => selection.IsSelected).ToList();

                var credits = dbClient.SubscriptionCredits.Sum(credit => credit.Amount);
                ViewBag.Credits = credits;
                var payingAmount = dbClientBills._totalCost - credits;
                payingAmount = payingAmount < 0m ? 0 : payingAmount;
                ViewBag.PayingAmount = payingAmount;

                return View("ClientPaymentConfirm", dbClientBills);
            }

            return View(dbClientBills);
        }

        [AuthorizePermission(Permissions = "Payment", Roles = "cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Bill/ClientPaymentConfirm
        public ActionResult ClientPaymentConfirm(long[] billIds, string redirectUrl, long? clientId, PaymentType paymentType, bool HasPrintRequested = false)
        {
            UriBuilder uri = null;
            if (User.IsInRole("cashier"))
            {
                uri = new UriBuilder(Url.Action("ClientPayment", "Bill", new { id = clientId }, Request.Url.Scheme));
            }
            else
            {
                if (clientId.HasValue)
                {
                    uri = new UriBuilder(Url.Action("Details", "Client", null, Request.Url.Scheme) + "#bills");
                }
                else
                {
                    uri = new UriBuilder(redirectUrl);
                }
            }

            if (paymentType == PaymentType.None || paymentType == PaymentType.OnlineBanking)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var dbBills = db.Bills.Where(bill => billIds.Contains(bill.ID)).Where(bill => bill.BillStatusID == (short)BillState.Unpaid).ToList();

            if (clientId.HasValue && !User.IsInRole("cashier"))
            {
                uri = new UriBuilder(uri.Host + Url.Action("Details", "Client", new { id = dbBills.FirstOrDefault().Subscription.ID }) + "#bills");
            }

            if (paymentType == PaymentType.VirtualPos)
            {
                // send to vpos payment
                var tokenCode = PaymentTokenManager.AddToken(new PaymentTokenManager.BillPaymentToken()
                {
                    BillIds = dbBills.Select(bill => bill.ID),
                    ReturnUrl = uri.Uri.AbsoluteUri,
                    Amount = dbBills.Sum(bill => bill.GetPayableCost()),
                    ClientName = dbBills.FirstOrDefault().Subscription.ValidDisplayName,
                    ClientAddress = dbBills.FirstOrDefault().Subscription.Address.AddressText,
                    ClientTel = dbBills.FirstOrDefault().Subscription.Customer.ContactPhoneNo,
                    Language = dbBills.FirstOrDefault().Subscription.Customer.Culture.Split('-')[0],
                    SubscriberNo = dbBills.FirstOrDefault().Subscription.SubscriberNo,
                    ServiceName = dbBills.FirstOrDefault().BillFees.Any(bf => bf.FeeTypeID == (short)FeeType.Tariff) ? dbBills.FirstOrDefault().BillFees.FirstOrDefault(bf => bf.FeeTypeID == (short)FeeType.Tariff).Description : null
                });

                return RedirectToAction("Index", "VPOSPayment", new { id = tokenCode });
            }

            var results = db.PayBills(dbBills, paymentType, User.GiveAccountantType(), User.GiveUserId());

            if (results == BillPayment.ResponseType.NotEnoughCredit)
                return RedirectToAction("ClientPayment", "Bill", new { id = clientId, errorMessage = 12 });

            // send payment sms
            var billGroups = dbBills.GroupBy(bill => bill.Subscription);
            var SMSAsync = new SMSServiceAsync();
            var sentSMSes = new List<SMSArchive>();
            foreach (var billGroup in billGroups)
            {
                sentSMSes.Add(SMSAsync.SendSubscriberSMS(billGroup.Key, SMSType.PaymentDone, new Dictionary<string, object>()
                {
                    { SMSParamaterRepository.SMSParameterNameCollection.BillTotal,  billGroup.Sum(bill => bill.GetPayableCost()) }
                }));
                db.SystemLogs.Add(SystemLogProcessor.BillPayment(billGroup.Select(b => b.ID), User.GiveUserId(), billGroup.Key.ID, SystemLogInterface.MasterISS, null, paymentType));
            }
            db.SMSArchives.AddRangeSafely(sentSMSes);

            db.SaveChanges();

            if (HasPrintRequested)
            {
                if (!dbBills.Any())
                {
                    dbBills = db.Bills.Where(bill => billIds.Contains(bill.ID)).ToList();
                }
                ReceiptViewModel receipt = new ReceiptViewModel()
                {
                    ClientName = dbBills.FirstOrDefault().Subscription.ValidDisplayName,
                    Total = dbBills.Sum(bill => bill.GetPayableCost()).ToString("###,###,##0.00"),
                    Date = DateTime.Now,
                    IssueDates = dbBills.OrderBy(bill => bill.IssueDate).Select(bill => bill.IssueDate),
                    RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment
                };

                return View("Receipt", receipt);
            }

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery);
        }

        [AuthorizePermission(Permissions = "Clients")]
        [HttpGet]
        // GET: Bill/PrepaidPayment
        public ActionResult PrepaidPayment(long id, string redirectUrl)
        {
            var uri = new UriBuilder(redirectUrl);

            var dbClient = db.Subscriptions.Find(id);
            if (dbClient == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery);
            }

            var dbClientBills = new ClientBillsViewModel(dbClient);
            dbClientBills.BillSelections.AsParallel().ForAll(selection => selection.IsSelected = true);
            dbClientBills.BillSelections = dbClientBills.BillSelections.Where(selection => selection.Bill.StateID == (short)BillState.Unpaid).OrderBy(selection => selection.Bill.IssueDate).ToList();
            if (dbClientBills.BillSelections.Count() == 0)
            {
                db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(User.GiveUserId(), dbClient.ID, SystemLogInterface.MasterISS, null, (CustomerState)dbClient.State, CustomerState.Active));
                dbClient.State = (short)CustomerState.Active;
                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery);
            }

            ViewBag.FullName = dbClient.ValidDisplayName;
            ViewBag.Total = dbClientBills.TotalCost;
            ViewBag.BackUrl = uri.Uri.PathAndQuery;
            return View(dbClientBills.BillSelections.Select(selection => selection.Bill));
        }

        //[AuthorizePermission(Permissions = "Clients")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult PrepaidPayment(long id, string redirectUrl, long[] billIds)
        //{
        //    var uri = new UriBuilder(redirectUrl);

        //    var dbClient = db.Subscriptions.Find(id);
        //    if (dbClient == null)
        //    {
        //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
        //        return Redirect(uri.Uri.PathAndQuery);
        //    }
        //    var dbBills = db.Bills.Where(bill => billIds.Contains(bill.ID)).ToArray();
        //    if (dbBills.Any(bill => bill.SubscriptionID != dbClient.ID))
        //    {
        //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
        //        return Redirect(uri.Uri.PathAndQuery);
        //    }
        //    // pay the bills
        //    foreach (var dbBill in dbBills)
        //    {
        //        dbBill.BillStatusID = (short)BillState.Paid;
        //        dbBill.PaymentTypeID = (short)PaymentType.Cash;
        //        dbBill.PayDate = DateTime.Now;
        //        dbBill.AccountantID = User.GiveUserId();
        //    }
        //    // activate client
        //    db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(User.GiveUserId(), dbClient.ID, (CustomerState)dbClient.State, CustomerState.Active));
        //    dbClient.State = (short)CustomerState.Active;
        //    dbClient.LastAllowedDate = DateTime.Now.AddDays(dbClient.Service.PeriodLength.Value * (int)Math.Floor(dbBills.OrderByDescending(bill => bill.IssueDate).FirstOrDefault().PartialPercentage));

        //    // send payment sms
        //    SMSServiceAsync SMSAsync = new SMSServiceAsync();
        //    db.SMSArchives.Add(SMSAsync.SendSubscriberSMS(dbClient, SMSType.PaymentDone, new Dictionary<string, object>()
        //    {
        //        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, dbBills.Sum(bill => bill.GetPayableCost()) }
        //    }));

        //    db.SaveChanges();

        //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
        //    return Redirect(uri.Uri.PathAndQuery);
        //}

        [AuthorizePermission(Permissions = "Cancel Payment")]
        [HttpPost]
        // POST: Bill/CancelPayment
        public ActionResult CancelPayment(string redirectUrl, long[] billIds)
        {
            var uri = new UriBuilder(redirectUrl);
            if (billIds == null || !billIds.Any())
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var bills = db.Bills.Where(bill => billIds.Contains(bill.ID));
            if (bills.Any(bill => bill.PaymentTypeID == (short)PaymentType.OnlineBanking))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            db.CancelPayment(bills);
            var billGroups = bills.GroupBy(b => b.SubscriptionID);
            foreach (var billGroup in billGroups)
            {
                db.SystemLogs.Add(SystemLogProcessor.CancelPayment(billGroup.Select(b => b.ID), User.GiveUserId(), billGroup.Key, SystemLogInterface.MasterISS, null));
            }
            db.SaveChanges();
            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Cancel Bill")]
        [HttpPost]
        // POST: Bill/CancelPayment
        public ActionResult CancelBill(string redirectUrl, long[] billIds)
        {
            var uri = new UriBuilder(redirectUrl);
            if (billIds == null || !billIds.Any())
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var bills = db.Bills.Where(bill => billIds.Contains(bill.ID));
            if (bills.Any(bill => bill.BillStatusID != (short)BillState.Unpaid))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            db.CancelBills(bills);
            var billGroups = bills.GroupBy(b => b.SubscriptionID);
            foreach (var billGroup in billGroups)
            {
                db.SystemLogs.Add(SystemLogProcessor.CancelBill(billGroup.Select(b => b.ID), User.GiveUserId(), billGroup.Key, SystemLogInterface.MasterISS, null));
            }
            db.SaveChanges();
            // cancel e-bill
            var resultsList = new List<RezaB.NetInvoice.RadiusRDBAdapter.CancellationResult>();
            foreach (var bill in bills)
            {
                if (bill.EBill != null)
                {
                    var results = RezaB.NetInvoice.RadiusRDBAdapter.Adapter.CancelEBill(bill.EBill);
                    if (results.ResultType != RezaB.NetInvoice.RadiusRDBAdapter.CancellationResultType.Success)
                        resultsList.Add(results);
                }
            }
            if (resultsList.Any())
            {
                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                if (TempData.ContainsKey("EBillCancellationErrors"))
                    TempData.Remove("EBillCancellationErrors");
                TempData.Add("EBillCancellationErrors", resultsList);
                return RedirectToAction("EBillCancellationError", new { redirectUrl = (uri.Uri.PathAndQuery + uri.Fragment) });
            }
            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Cancel Bill")]
        // GET: Bill/EBillCancellationError
        public ActionResult EBillCancellationError(string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            ViewBag.ErrorList = TempData.ContainsKey("EBillCancellationErrors") ? TempData["EBillCancellationErrors"] as IEnumerable<RezaB.NetInvoice.RadiusRDBAdapter.CancellationResult> : Enumerable.Empty<RezaB.NetInvoice.RadiusRDBAdapter.CancellationResult>();
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View();
        }

        [AuthorizePermission(Permissions = "Bills")]
        [AjaxCall]
        // GET: Bill/TodayPayments
        public ActionResult TodayPayments()
        {
            var today = DateTime.Today.Date;
            var endOfDay = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
            var viewResults = db.Bills
                .Where(bill => bill.BillStatusID == (short)BillState.Paid && bill.PayDate >= today && bill.PayDate <= endOfDay)
                .AsEnumerable()
                .GetViewModels()
                .OrderByDescending(bill => bill.PayDate).ToList();

            var total = viewResults.Sum(bill => bill._totalCost);
            var totalCredit = viewResults.Where(bill => bill.CreditPay != null).Sum(bill => bill.CreditPay._amount);
            var totalCash = total + totalCredit;

            ViewBag.BillsTotal = total.ToString("###,###,##0.00");
            ViewBag.CreditsTotal = totalCredit.ToString("###,###,##0.00");
            ViewBag.TotalPayments = totalCash.ToString("###,###,##0.00");

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Clients,Bills")]
        [HttpGet]
        // GET: Bill/Receipt
        public ActionResult Receipt(long billId, string redirectUrl)
        {
            UriBuilder uri = new UriBuilder(redirectUrl);
            var backUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.RedirectUrl = backUrl;

            var dbBill = db.Bills.Find(billId);
            if (dbBill == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "8", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ReceiptViewModel receipt = new ReceiptViewModel()
            {
                ClientName = dbBill.Subscription.ValidDisplayName,
                Total = dbBill.GetPayableCost().ToString("###,###,##0.00"),
                Date = DateTime.Now,
                IssueDates = new DateTime[] { dbBill.IssueDate },
                RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment
            };

            return View(receipt);
        }

        [AuthorizePermission(Permissions = "Bills")]
        [AjaxCall]
        [HttpPost]
        // POST: Bill/CalculateTotal
        public ActionResult CalculateTotal([Bind(Include = "CustomerName,AccountantID,IssueDateStart,IssueDateEnd,PaymentDateStart,PaymentDateEnd,State,PaymentTypeID,PaymentGateway,BillingPeriod,EBillCode")]BillSearchViewModel search)
        {
            var viewResults = db.Bills.AsQueryable();
            // apply search variables
            viewResults = FilterSearch(viewResults, search);

            var fetchedResults = viewResults.GetTotalPayableAmount();

            return View(model: fetchedResults.ToString("###,###,##0.00"));
        }

        [AuthorizePermission(Permissions = "Discount")]
        [HttpGet]
        // GET: Bill/Discount
        public ActionResult Discount(long id, string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = Request.Url.Host + Url.Action("Index", "Bill");
            }

            var uri = new UriBuilder(redirectUrl);
            var dbBill = db.Bills.Find(id);
            if (dbBill == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "8", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var bill = dbBill.GetViewModel();
            if (bill.EBillIsSent || bill.StateID != (short)BillState.Unpaid)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var discounts = bill.BillFees.Select(bf => new BillFeeDiscountViewModel()
            {
                BillFeeID = bf.ID,
                FeeTitle = bf.DisplayName,
                _feeCost = bf._currentCost.Value,
                _discountAmount = bf._discountAmount
            });

            ViewBag.RedirectUrl = redirectUrl;
            return View(discounts.ToList());
        }

        [AuthorizePermission(Permissions = "Discount")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Bill/Discount
        public ActionResult Discount(long id, string redirectUrl, [Bind(Prefix = "items", Include = "DiscountAmount,BillFeeID")]BillFeeDiscountViewModel[] discounts)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = Request.Url.Host + Url.Action("Index", "Bill");
            }

            var uri = new UriBuilder(redirectUrl);
            var dbBill = db.Bills.Find(id);
            if (dbBill == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "8", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var bill = dbBill.GetViewModel();
            if (bill.EBillIsSent || bill.StateID != (short)BillState.Unpaid)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var results = discounts.ToList();
            foreach (var discount in results)
            {
                var current = bill.BillFees.FirstOrDefault(bf => bf.ID == discount.BillFeeID);
                discount.FeeTitle = current.DisplayName;
                discount._feeCost = current._currentCost;
            }
            if (ModelState.IsValid)
            {
                int index = 0;
                foreach (var billFee in dbBill.BillFees)
                {
                    var current = results.FirstOrDefault(discount => discount.BillFeeID == billFee.ID);
                    if (!current._discountAmount.HasValue && billFee.DiscountID.HasValue)
                    {
                        billFee.DiscountID = null;
                        db.Discounts.Remove(billFee.Discount);
                    }
                    if (current._discountAmount <= 0m || current._discountAmount > current._feeCost)
                    {
                        ModelState.AddModelError("items[" + index + "].DiscountAmount", RadiusR.Localization.Validation.Common.DiscountExceedsFeeCost);
                    }
                    else if (current._discountAmount.HasValue)
                    {
                        if (billFee.DiscountID.HasValue)
                        {
                            billFee.Discount.Amount = current._discountAmount.Value;
                        }
                        else
                        {
                            billFee.Discount = new Discount()
                            {
                                Amount = current._discountAmount.Value,
                                DiscountType = (short)DiscountType.Manual
                            };
                        }
                    }
                    index++;
                }
                if (ModelState.IsValid)
                {
                    db.SystemLogs.Add(SystemLogProcessor.ChangeDiscount(User.GiveUserId(), bill.SubscriptionID, SystemLogInterface.MasterISS, null, bill.ID));
                    db.SaveChanges();

                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                }
            }

            ViewBag.RedirectUrl = redirectUrl;
            return View(results);
        }

        private IQueryable<Bill> FilterSearch(IQueryable<Bill> query, BillSearchViewModel search)
        {
            if (!string.IsNullOrWhiteSpace(search.CustomerName))
            {
                query = query.Where(r => r.Subscription.Customer.CorporateCustomerInfo != null ? r.Subscription.Customer.CorporateCustomerInfo.Title.Contains(search.CustomerName.ToLower()) : (r.Subscription.Customer.FirstName.ToLower() + " " + r.Subscription.Customer.LastName.ToLower()).Contains(search.CustomerName.ToLower()));
            }
            if (search.AccountantID.HasValue)
            {
                query = query.Where(r => r.AccountantID == search.AccountantID);
            }
            if (search.IssueDateStart.HasValue)
            {
                query = query.Where(r => r.IssueDate >= search.IssueDateStart);
            }
            if (search.IssueDateEnd.HasValue)
            {
                query = query.Where(r => r.IssueDate <= search.IssueDateEnd);
            }
            if (search.DueDateStart.HasValue)
            {
                query = query.Where(r => r.DueDate >= search.DueDateStart);
            }
            if (search.DueDateEnd.HasValue)
            {
                query = query.Where(r => r.DueDate <= search.DueDateEnd);
            }
            if (search.PaymentDateStart.HasValue)
            {
                query = query.Where(r => r.PayDate >= search.PaymentDateStart);
            }
            if (search.PaymentDateEnd.HasValue)
            {
                //search.PaymentDateEnd = search.PaymentDateEnd.Value.GetEndOfTheDay();
                query = query.Where(r => DbFunctions.TruncateTime(r.PayDate) <= search.PaymentDateEnd);
            }
            if (search.State > 0)
            {
                query = query.Where(r => r.BillStatusID == search.State);
            }
            if (search.PaymentTypeID > 0)
            {
                query = query.Where(r => r.PaymentTypeID == search.PaymentTypeID);
            }
            if (search.PaymentGateway.HasValue)
            {
                query = query.Where(r => r.ExternalPayment != null && r.ExternalPayment.ExternalUserID != null && r.ExternalPayment.ExternalUserID == search.PaymentGateway);
            }
            if (search.BillingPeriod.HasValue)
            {
                query = query.Where(r => r.Subscription.PaymentDay == search.BillingPeriod);
            }
            if (!string.IsNullOrWhiteSpace(search.EBillCode))
            {
                query = query.Where(r => r.EBill.BillCode.Contains(search.EBillCode));
            }

            return query;
        }

        //private bool IsValidForDealer(Bill bill)
        //{
        //    var dealerId = User.GiveUserDealerId();
        //    if (!dealerId.HasValue)
        //        return true;
        //    if (User.IsInRole("cashier") && !bill.Subscription.DealerID.HasValue)
        //        return true;
        //    if (bill.Subscription.DealerID == dealerId)
        //        return true;

        //    return false;
        //}
    }
}