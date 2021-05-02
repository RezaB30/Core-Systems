using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.QueryExtentions;
using RadiusR.SystemLogs;
using RadiusR_Manager.Helpers;
using RadiusR_Manager.Models.CSVModels;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;
using RezaB.Data.Files;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RadiusR.DB.Utilities.Billing.BillExtentions;
using System.Data.SqlClient;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Reports")]
    public class ReportsController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [OutputCache(Duration = 5)]
        [AuthorizePermission(Permissions = "Online Clients")]
        public ActionResult GetOnlineClientCount()
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                return Content(Convert.ToString(db.OnlineSubscriptionCount));
            }
        }

        [AuthorizePermission(Permissions = "Services Client Count")]
        [HttpGet]
        // GET: Reports/ServicesClientCount
        public ActionResult ServicesClientCount(int? page)
        {
            var viewResults = db.Subscriptions.Where(client => client.State == (short)CustomerState.Active).GroupBy(client => client.Service).Select(cc => new ServiceClientCountViewModel()
            {
                ServiceID = cc.Key.ID,
                ServiceName = cc.Key.Name,
                _clientCount = cc.Count()
            }).OrderByDescending(cc => cc._clientCount).AsQueryable();

            SetupPages(page, ref viewResults);

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Services Client Count")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Reports/ServicesClientCount
        public ActionResult ServicesClientCount(int id)
        {
            var dbTariff = db.Services.Find(id);
            var results = db.Bills.Where(b => b.Subscription.State != (short)CustomerState.Cancelled && b.Subscription.ServiceID == id && b.Source == (short)BillSources.System && b.PeriodEnd > b.Subscription.SystemLogs.Where(sl => sl.LogType == (short)SystemLogTypes.ChangeService).Select(sl => sl.Date).DefaultIfEmpty(new DateTime(1980, 1, 1)).Max()).GroupBy(b => b.Subscription).Select(g => new
            {
                SubscriberNo = g.Key.SubscriberNo,
                BillingPeriod = g.Key.PaymentDay,
                BillCount = g.Count()
            }).ToArray();

            var csvResults = results.OrderByDescending(r => r.BillCount).Select(r => new TariffSubscriptionsBillDetails()
            {
                SubscriberNo = r.SubscriberNo,
                BillingPeriod = Convert.ToString(r.BillingPeriod),
                BillCount = Convert.ToString(r.BillCount)
            }).ToArray();

            return File(CSVGenerator.GetStream(csvResults, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.TariffSubscriptionBillsCount + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
        }

        [AuthorizePermission(Permissions = "Monthly Bill Report")]
        // GET: Reports/MonthlyBillReport
        public ActionResult MonthlyBillReport(int? page)
        {
            var rawResults = db.Bills.Where(bill => bill.BillStatusID != (short)BillState.Cancelled).Select(bill => new BillReport()
            {
                IssueDate = bill.IssueDate,
                State = bill.BillStatusID,
                Fees = bill.BillFees.Select(bf => bf.CurrentCost),
                Discounts = bill.BillFees.Select(bf => bf.DiscountID.HasValue ? bf.Discount.Amount : 0m)
            }).GroupBy(bill => bill.IssueDate).OrderByDescending(dailyGroup => dailyGroup.Key).Select(dailyGroup => new
            {
                groupingKey = new
                {
                    year = dailyGroup.Key.Year,
                    month = dailyGroup.Key.Month
                },
                bills = dailyGroup.Select(dg => dg)
            }).GroupBy(monthlyGroup => monthlyGroup.groupingKey)
            .Select(monthlyGroup => new
            {
                year = monthlyGroup.Key.year,
                month = monthlyGroup.Key.month,
                bills = monthlyGroup.SelectMany(mg => mg.bills)
            })
            .OrderByDescending(billReport => billReport.year).ThenByDescending(billReport => billReport.month).AsQueryable();

            SetupPages(page, ref rawResults);

            var viewResults = rawResults.ToList().Select(billReport => new MonthlyBillReport()
            {
                _year = billReport.year,
                _month = billReport.month,
                TotalBillCount = billReport.bills.Count(),
                PaidBillCount = billReport.bills.Where(bill => bill.State == (short)BillState.Paid).Count(),
                UnpaidBillCount = billReport.bills.Where(bill => bill.State == (short)BillState.Unpaid).Count(),
                _totalBillAmount = billReport.bills.Sum(bill => bill.GetTotal()),
                _paidBillAmount = billReport.bills.Where(bill => bill.State == (short)BillState.Paid).Sum(bill => bill.GetTotal()),
                _unpaidBillAmount = billReport.bills.Where(bill => bill.State == (short)BillState.Unpaid).Sum(bill => bill.GetTotal())
            });

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Cash Desk Report")]
        [HttpGet]
        // GET: Reports/CashDeskReport
        public ActionResult CashDeskReport(int? page, [Bind(Prefix = "search", Include = "FullName,PaymentType,StartDate,EndDate,AccountantID")] CashDeskSearchViewModel search)
        {
            ViewBag.AccountantList = new SelectList(db.AppUsers.Where(user => user.Role.Permissions.Select(permission => permission.Name).Contains("Clients")).Select(user => new { ID = user.ID, Name = user.Name }), "ID", "Name", search != null ? search.AccountantID : null);
            if (search == null)
            {
                search = new CashDeskSearchViewModel();
            }
            if (!search.StartDate.HasValue && !search.EndDate.HasValue)
            {
                search.StartDate = DateTime.Now.Date;
                search.EndDate = DateTime.Now.Date;
            }
            //if (search.EndDate.HasValue)
            //{
            //    search.EndDate = search.EndDate.Value.GetEndOfTheDay();
            //}

            // calculate bills
            var billsPart = db.Bills.RemoveCashierPayments().Where(bill => bill.BillStatusID == (short)BillState.Paid && bill.PayDate.HasValue && bill.PaymentTypeID == (short)PaymentType.Cash);

            // apply filter to bills
            if (!string.IsNullOrEmpty(search.FullName))
            {
                billsPart = billsPart.Where(bill => bill.Subscription.Customer.CorporateCustomerInfo != null ? bill.Subscription.Customer.CorporateCustomerInfo.Title.ToLower().Contains(search.FullName.ToLower()) : (bill.Subscription.Customer.FirstName + " " + bill.Subscription.Customer.LastName).ToLower().Contains(search.FullName.ToLower()));
            }
            if (search.PaymentType == (short)MoneyInputType.Credit)
            {
                billsPart = new List<Bill>().AsQueryable();
            }
            if (search.AccountantID.HasValue)
            {
                billsPart = billsPart.Where(bill => bill.AccountantID == search.AccountantID);
            }
            if (search.StartDate.HasValue)
            {
                billsPart = billsPart.Where(bill => DbFunctions.TruncateTime(bill.PayDate) >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                billsPart = billsPart.Where(bill => DbFunctions.TruncateTime(bill.PayDate) <= search.EndDate);
            }

            // calculate credit
            var creditPart = db.SubscriptionCredits.Where(credit => !credit.BillID.HasValue).AsQueryable();

            // apply filter to credit
            if (!string.IsNullOrEmpty(search.FullName))
            {
                creditPart = creditPart.Where(credit => credit.Subscription.Customer.CorporateCustomerInfo != null ? credit.Subscription.Customer.CorporateCustomerInfo.Title.ToLower().Contains(search.FullName.ToLower()) : (credit.Subscription.Customer.FirstName + " " + credit.Subscription.Customer.LastName).ToLower().Contains(search.FullName.ToLower()));
            }
            if (search.PaymentType == (short)MoneyInputType.Bill)
            {
                creditPart = new List<SubscriptionCredit>().AsQueryable();
            }
            if (search.AccountantID.HasValue)
            {
                creditPart = creditPart.Where(credit => credit.AccountantID == search.AccountantID);
            }
            if (search.StartDate.HasValue)
            {
                creditPart = creditPart.Where(credit => DbFunctions.TruncateTime(credit.Date) >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                creditPart = creditPart.Where(credit => DbFunctions.TruncateTime(credit.Date) <= search.EndDate);
            }

            var viewResults = billsPart.ToList().Select(bill => new CashDeskViewModel()
            {
                FullName = bill.Subscription.ValidDisplayName,
                PaymentType = (short)MoneyInputType.Bill,
                Date = bill.PayDate.Value,
                _total = bill.GetPayableCost() + bill.SubscriptionCredits.Sum(credit => credit.Amount),
                AccountantName = bill.AccountantID.HasValue ? bill.AppUser.Name : "-"
            }).Concat(creditPart.ToList().Select(credit => new CashDeskViewModel()
            {
                FullName = credit.Subscription.ValidDisplayName,
                PaymentType = (short)MoneyInputType.Credit,
                Date = credit.Date,
                _total = credit.Amount,
                AccountantName = credit.AccountantID.HasValue ? credit.AppUser.Name : "-"
            })).OrderByDescending(payment => payment.Date).AsQueryable();

            ViewBag.Total = viewResults.Sum(r => r._total).ToString("###,###,##0.00");

            SetupPages(page, ref viewResults);

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Risky Clients")]
        [HttpGet]
        // GET: Reports/RiskyClients
        public ActionResult RiskyClients(int? page, [Bind(Prefix = "search")] RiskyClientsSearchViewModel search)
        {
            search = search ?? new RiskyClientsSearchViewModel()
            {
                UnpaidBillCount = 2
            };

            var riskyClients = db.Subscriptions.Where(client => client.State == (short)CustomerState.Active).Where(client => client.Bills.Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Count() >= search.UnpaidBillCount).OrderByDescending(client => client.Bills.Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Count()).AsQueryable();

            SetupPages(page, ref riskyClients);

            var viewResults = riskyClients.ToList().Select(client => new RiskyClientViewModel()
            {
                ID = client.ID,
                FullName = client.ValidDisplayName,
                UnpaidBillCount = client.Bills.Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Count(),
                _unpaidAmount = client.Bills.Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Sum(bill => bill.GetPayableCost())
            });

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Client Count")]
        [HttpGet]
        // GET: Reports/ClientCount
        public ActionResult ClientCount()
        {
            var disconnectionTimeOfDay = TimeSpan.ParseExact(db.RadiusDefaults.FirstOrDefault(def => def.Attribute == "DailyDisconnectionTime").Value, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);
            var includedStates = new short[]
            {
                (short)CustomerState.Active,
                (short)CustomerState.Reserved,
                (short)CustomerState.Registered,
                (short)CustomerState.Disabled,
                (short)CustomerState.Cancelled,
            };
            var results = new ClientCountReportViewModel()
            {
                TotalCount = db.Subscriptions.Where(s => includedStates.Contains(s.State)).LongCount(),
                CancelledCount = db.Subscriptions.Where(client => client.State == (short)CustomerState.Cancelled).LongCount(),
                FreezedCount = db.Subscriptions.Where(client => client.State == (short)CustomerState.Disabled).LongCount(),
                PassiveCount = db.Subscriptions.Where(client => client.State == (short)CustomerState.Active || client.State == (short)CustomerState.Reserved && client.RadiusAuthorization.ExpirationDate < DbFunctions.AddSeconds(DateTime.Now, -1 * (int)disconnectionTimeOfDay.TotalSeconds)).LongCount()
            };

            // ------- Diagram Data -------
            // get current time
            var thisMoment = DateTime.Now;
            // query registered clients
            var registered = db.Subscriptions.GroupBy(client => DbFunctions.TruncateTime(client.MembershipDate)).Select(g => new
            {
                groupingKey = new
                {
                    year = g.Key.Value.Year,
                    month = g.Key.Value.Month
                },
                count = g.Count()
            }).GroupBy(g => g.groupingKey).OrderByDescending(g => g.Key.year).ThenByDescending(g => g.Key.month).Take(12).ToArray();
            // fill array for diagram with proper empty gaps
            var registeredClients = new List<decimal>();
            for (int i = 0; i < 12; i++)
            {
                var currentIteration = thisMoment.AddMonths(-1 * i);
                registeredClients.Add((decimal)registered.Where(g => g.Key.year == currentIteration.Year && g.Key.month == currentIteration.Month).Sum(g => g.Sum(sub => sub.count)));
            }
            // fix the order
            registeredClients.Reverse();
            //.Select(g => g.Sum(sub => sub.count)).Cast<decimal>().ToArray().Reverse();
            var cancelled = db.Subscriptions.Where(client => client.EndDate.HasValue).GroupBy(client => DbFunctions.TruncateTime(client.EndDate)).Select(g => new
            {
                groupingKey = new
                {
                    year = g.Key.Value.Year,
                    month = g.Key.Value.Month
                },
                count = g.Count()
            }).GroupBy(g => g.groupingKey).OrderByDescending(g => g.Key.year).ThenByDescending(g => g.Key.month).Take(12).ToArray();
            // fill array for diagram with proper empty gaps
            var cancelledClients = new List<decimal>();
            for (int i = 0; i < 12; i++)
            {
                var currentIteration = thisMoment.AddMonths(-1 * i);
                cancelledClients.Add((decimal)cancelled.Where(g => g.Key.year == currentIteration.Year && g.Key.month == currentIteration.Month).Sum(g => g.Sum(sub => sub.count)));
            }
            // fix the order
            cancelledClients.Reverse();
            //.Select(g => g.Sum(sub => sub.count)).Cast<decimal>().ToArray().Reverse();

            var growth = new List<decimal>();
            for (int i = 0; i < registeredClients.Count(); i++)
            {
                growth.Add(registeredClients[i] - cancelledClients[i]);
            }

            var diagramData = new List<LinearDiagramDataArray>();
            diagramData.Add(new LinearDiagramDataArray(RadiusR.Localization.Pages.Common.CancelledClients, cancelledClients.AsEnumerable(), data => data.ToString("###,###,##0")));
            diagramData.Add(new LinearDiagramDataArray(RadiusR.Localization.Pages.Common.NewClients, registeredClients.AsEnumerable(), data => data.ToString("###,###,##0")));
            diagramData.Add(new LinearDiagramDataArray(RadiusR.Localization.Pages.Common.Growth, growth.AsEnumerable(), data => data.ToString("###,###,##0")));

            results.DiagramData = diagramData.Select(data => (object)data).ToList();

            return View(results);
        }

        [AuthorizePermission(Permissions = "Online Clients")]
        [HttpGet]
        // GET: Reports/OnlineClients
        public ActionResult OnlineClients()
        {
            var NASNames = db.NAS.Select(nas => new { IP = nas.IP, Name = nas.Name }).ToDictionary(item => item.IP, item => item.Name);
            var onlineClients = db.Database.SqlQuery<string>("SELECT NASIP FROM RadiusAuthorization WHERE LastInterimUpdate IS NOT NULL AND (LastLogout IS NULL OR LastLogout < LastInterimUpdate);").ToArray().GroupBy(ra => ra).Select(group => new OnlineClientsReportViewModel()
            {
                ClientCount = group.LongCount(),
                IP = group.Key
            }).OrderByDescending(nasClients => nasClients.ClientCount).ToArray();
            foreach (var clientGroup in onlineClients)
            {
                clientGroup.Name = NASNames.ContainsKey(clientGroup.IP) ? NASNames[clientGroup.IP] : null;
            }

            return View(onlineClients);
        }

        [AuthorizePermission(Permissions = "Close NAS Connections")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Reports/CloseNASConnections
        public ActionResult CloseNASConnections(string IP)
        {
            var temp = new RadiusAuthorization()
            {
                LastInterimUpdate = DateTime.Now,
                LastLogout = DateTime.Now,
                NASIP = ""
            };
            var currentTime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(IP))
            {
                db.Database.ExecuteSqlCommand("UPDATE RadiusAuthorization SET LastLogout = @currentTime WHERE LastInterimUpdate IS NOT NULL AND (LastLogout IS NULL OR LastLogout < LastInterimUpdate) AND NASIP = @nasIP;", new[] { new SqlParameter("@currentTime", currentTime), new SqlParameter("@nasIP", IP) });
            }
            else
            {
                db.Database.ExecuteSqlCommand("UPDATE RadiusAuthorization SET LastLogout = @currentTime WHERE LastInterimUpdate IS NOT NULL AND (LastLogout IS NULL OR LastLogout < LastInterimUpdate);", new[] { new SqlParameter("@currentTime", currentTime) });
            }
            //var validRecordsQuery = db.RadiusAccountings.Where(ra => !ra.StopTime.HasValue);
            //if (!string.IsNullOrEmpty(IP))
            //{
            //    validRecordsQuery = validRecordsQuery.Where(ra => ra.NASIP == IP);
            //}
            //var validRecords = validRecordsQuery.ToArray();
            //var currentTime = DateTime.Now;
            //foreach (var record in validRecords)
            //{
            //    record.StopTime = currentTime;
            //}

            //db.SaveChanges();

            return RedirectToAction("OnlineClients");
        }

        [AuthorizePermission(Permissions = "Client Usage Report")]
        [HttpGet]
        // GET: Reports/ClientUsageReport
        public ActionResult ClientUsageReport(int? page, [Bind(Prefix = "search")] ClientUsageReportSearchViewModel search)
        {
            search = search ?? new ClientUsageReportSearchViewModel();
            search.StartDate = search.StartDate ?? DateTime.Now.Date.AddMonths(-1);
            search.EndDate = search.EndDate ?? DateTime.Now.Date;

            var results = db.RadiusDailyAccountings.Where(usage => usage.Date >= search.StartDate && usage.Date <= search.EndDate).GroupBy(usage => usage.SubscriptionID).Select(group => new
            {
                Client = group.Select(g => g.Subscription).FirstOrDefault(),
                Usage = group.Sum(g => (decimal)g.DownloadBytes + (decimal)g.UploadBytes)
            }).OrderByDescending(usage => usage.Usage).AsQueryable();

            SetupPages(page, ref results);

            var viewResults = results.ToList().Select(usage => new ClientUsageReportViewModel()
            {
                ClientID = usage.Client.ID,
                Name = usage.Client.ValidDisplayName,
                SubscriberNo = usage.Client.SubscriberNo,
                PhoneNo = usage.Client.Customer.ContactPhoneNo,
                Usage = usage.Usage
            });

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Cancelled Clients")]
        [HttpGet]
        // GET: Reports/CancelledClients
        public ActionResult CancelledClients(int? page)
        {
            var results = db.Subscriptions.Where(client => client.State == (short)CustomerState.Cancelled).OrderByDescending(client => client.EndDate).AsQueryable();

            SetupPages(page, ref results);

            var viewResults = results.ToList().Select(subscription => new SubscriptionListDisplayViewModel()
            {
                ID = subscription.ID,
                Name = subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.Title : subscription.Customer.FirstName + " " + subscription.Customer.LastName,
                CancellationDate = subscription.EndDate,
                SubscriberNo = subscription.SubscriberNo,
                ContactPhoneNo = subscription.Customer.ContactPhoneNo
            });

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Cancelled Clients Unpaid Bills")]
        [HttpGet]
        // GET: Reports/CancelledClientsUnpaidBills
        public ActionResult CancelledClientsUnpaidBills(int? page, [Bind(Prefix = "search")] CancelledUnpaidBillsSearchViewModel search)
        {
            search = search ?? new CancelledUnpaidBillsSearchViewModel();

            var queryBase = db.Subscriptions.Where(client => client.State == (short)CustomerState.Cancelled).Where(client => client.Bills.Any(bill => bill.BillStatusID == (short)BillState.Unpaid));
            if (search.StartDate.HasValue)
            {
                queryBase = queryBase.Where(client => DbFunctions.TruncateTime(client.EndDate) >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                var endDate = search.EndDate.Value.AddDays(1);
                queryBase = queryBase.Where(client => DbFunctions.TruncateTime(client.EndDate) < endDate);
            }

            var validClients = queryBase.Select(client => new
            {
                Client = client,
                UnpaidBillCount = client.Bills.Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Count(),
                TotalDebt = client.Bills.AsQueryable().Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Sum(bill => bill.BillFees.Select(billFee => billFee.CurrentCost).DefaultIfEmpty(0m).Sum() - bill.BillFees.Select(billFee => billFee.Discount != null ? billFee.Discount.Amount : 0m).Sum())
            }).OrderByDescending(client => client.TotalDebt).AsQueryable();

            SetupPages(page, ref validClients);

            var viewResults = validClients.ToList().Select(client => new CancelledClientUnpaidBillsViewModel()
            {
                ID = client.Client.ID,
                Name = client.Client.ValidDisplayName,
                CancellationDate = client.Client.EndDate,
                PhoneNo = client.Client.Customer.ContactPhoneNo,
                SubscriberNo = client.Client.SubscriberNo,
                UnpaidBillCount = client.UnpaidBillCount,
                _totalDebt = client.TotalDebt
            });

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Cancelled Clients Unpaid Bills")]
        [HttpPost]
        [ActionName("CancelledClientsUnpaidBills")]
        [ValidateAntiForgeryToken]
        // POST: Reports/CancelledClientsUnpaidBills
        public ActionResult CancelledClientsUnpaidBillsCSV(int? page, [Bind(Prefix = "search")] CancelledUnpaidBillsSearchViewModel search)
        {
            search = search ?? new CancelledUnpaidBillsSearchViewModel();

            var queryBase = db.Subscriptions.Where(client => client.State == (short)CustomerState.Cancelled).Where(client => client.Bills.Any(bill => bill.BillStatusID == (short)BillState.Unpaid));
            if (search.StartDate.HasValue)
            {
                queryBase = queryBase.Where(client => client.EndDate >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                var endDate = search.EndDate.Value.AddDays(1);
                queryBase = queryBase.Where(client => client.EndDate < endDate);
            }

            var validClients = queryBase.Select(client => new
            {
                Client = client,
                UnpaidBillCount = client.Bills.Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Count(),
                TotalDebt = client.Bills.AsQueryable().Where(bill => bill.BillStatusID == (short)BillState.Unpaid).Sum(bill => bill.BillFees.Select(billFee => billFee.CurrentCost).DefaultIfEmpty(0m).Sum() - bill.BillFees.Select(billFee => billFee.Discount != null ? billFee.Discount.Amount : 0m).Sum()),
                Bills = client.Bills.Where(b => b.BillStatusID == (short)BillState.Unpaid).Select(b => new
                {
                    Date = b.IssueDate,
                    Total = b.BillFees.Select(f => f.CurrentCost).DefaultIfEmpty(0m).Sum()
                })
            }).OrderByDescending(client => client.TotalDebt).AsQueryable();

            var viewResults = validClients.ToList().Select(client => new
            {
                Bill = new CancelledClientUnpaidBillsViewModel()
                {
                    ID = client.Client.ID,
                    Name = client.Client.ValidDisplayName,
                    CancellationDate = client.Client.EndDate,
                    PhoneNo = client.Client.Customer.ContactPhoneNo,
                    SubscriberNo = client.Client.SubscriberNo,
                    UnpaidBillCount = client.UnpaidBillCount,
                    _totalDebt = client.TotalDebt
                },
                BillDetails = string.Join("; ", client.Bills.Select(b => b.Date.ToString("yyyy-MM-dd") + " >> " + b.Total.ToString("###,##0.00")))
            }).ToArray().Select(item => new CancelledClientsUnpaidBillsCSVModel()
            {
                CustomerName = item.Bill.Name,
                PhoneNo = item.Bill.PhoneNo,
                SubscriberNo = item.Bill.SubscriberNo,
                UnpaidBillsCount = item.Bill.UnpaidBillCount.ToString(),
                CancellationDate = item.Bill.CancellationDate.HasValue ? item.Bill.CancellationDate.Value.ToShortDateString() : null,
                TotalDebt = item.Bill.TotalDebt,
                BillsDetail = item.BillDetails
            });

            var currentTime = DateTime.Now;
            return File(CSVGenerator.GetStream(viewResults, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.CancelledClientsUnpaidBills + "_" + currentTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
        }

        [AuthorizePermission(Permissions = "Total Download Upload")]
        [HttpGet]
        // GET: Reports/TotalDownloadUpload
        public ActionResult TotalDownloadUpload(int? page, [Bind(Prefix = "search")] TotalDownloadUploadSearchViewModel search)
        {
            search = search ?? new TotalDownloadUploadSearchViewModel();
            var baseQuery = db.RadiusDailyAccountings.AsQueryable();
            if (search.StartDate != null && search.StartDate.IsValid)
            {
                var compareDate = new DateTime(search.StartDate.Year.Value, search.StartDate.Month.Value, 1);
                baseQuery = baseQuery.Where(rda => rda.Date >= compareDate);
            }
            if (search.EndDate != null && search.EndDate.IsValid)
            {
                var compareDate = new DateTime(search.EndDate.Year.Value, search.EndDate.Month.Value, 1).AddMonths(1);
                baseQuery = baseQuery.Where(rda => rda.Date < compareDate);
            }
            var viewResults = baseQuery.GroupBy(daily => daily.Date).OrderByDescending(dailyGroup => dailyGroup.Key).Select(dailyGroup => new
            {
                groupingKey = new
                {
                    year = dailyGroup.Key.Year,
                    month = dailyGroup.Key.Month
                },
                download = dailyGroup.Select(daily => (decimal)daily.DownloadBytes).Sum(),
                upload = dailyGroup.Select(daily => (decimal)daily.UploadBytes).Sum()
            }).GroupBy(monthlyGroup => monthlyGroup.groupingKey).Select(monthlyGroup => new UsageInfoViewModel()
            {
                _year = monthlyGroup.Key.year,
                _month = monthlyGroup.Key.month,
                Download = monthlyGroup.Sum(dailyGroup => dailyGroup.download),
                Upload = monthlyGroup.Sum(dailyGroup => dailyGroup.upload)
            }).OrderByDescending(usage => usage._year).ThenByDescending(usage => usage._month).AsQueryable();

            SetupPages(page, ref viewResults);

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Static IP Report")]
        [HttpGet]
        // GET: Reports/StaticIPReport
        public ActionResult StaticIPReport(int? page, [Bind(Prefix = "search")] StaticIPReportSearchViewModel search)
        {
            search = search ?? new StaticIPReportSearchViewModel();
            var baseQuery = db.Subscriptions.Where(client => !string.IsNullOrEmpty(client.RadiusAuthorization.StaticIP));
            if (!string.IsNullOrEmpty(search.StaticIP))
            {
                baseQuery = baseQuery.Where(client => client.RadiusAuthorization.StaticIP == search.StaticIP);
            }
            baseQuery = baseQuery.OrderBy(client => client.ID).AsQueryable();

            SetupPages(page, ref baseQuery);

            var viewResults = baseQuery.AsEnumerable().Select(client => new StaticIPReportViewModel()
            {
                ClientID = client.ID,
                ClientName = client.ValidDisplayName,
                Username = client.RadiusAuthorization.Username,
                StaticIP = client.RadiusAuthorization.StaticIP
            }).ToList();

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "System Logs Report")]
        [HttpGet]
        // GET: Reports/SystemLogsReport
        public ActionResult SystemLogsReport(int? page, SystemLogsReportSearchViewModel search)
        {
            var query = db.SystemLogs.OrderByDescending(log => log.Date).AsQueryable();
            if (search.LogType.HasValue)
            {
                query = query.Where(log => log.LogType == search.LogType);
            }
            if (search.UserID.HasValue)
            {
                query = query.Where(log => log.AppUserID == search.UserID);
            }
            if (search.StartDate.HasValue)
            {
                query = query.Where(log => DbFunctions.TruncateTime(log.Date) >= search.StartDate.Value);
            }
            if (search.EndDate.HasValue)
            {
                query = query.Where(log => DbFunctions.TruncateTime(log.Date) <= search.EndDate.Value);
            }

            SetupPages(page, ref query);

            var processor = new SystemLogProcessor(Url);
            var results = query.Select(log => new
            {
                ID = log.ID,
                Date = log.Date,
                LogType = log.LogType,
                Interface = log.Interface,
                InterfaceUsername = log.InterfaceUsername,
                CustomerID = log.CustomerID,
                SubscriptionID = log.SubscriptionID,
                UserName = log.AppUser != null ? log.AppUser.Name : "-",
                Parameters = log.Parameters,
                SubscriptionName = log.Subscription != null ? (log.Subscription.Customer.CorporateCustomerInfo != null ? log.Subscription.Customer.CorporateCustomerInfo.Title : log.Subscription.Customer.FirstName + " " + log.Subscription.Customer.LastName) : null,
                CustomerName = log.Customer != null ? (log.Customer.CorporateCustomerInfo != null ? log.Customer.CorporateCustomerInfo.Title : log.Customer.FirstName + " " + log.Customer.LastName) : null,
                S1 = log.Subscription != null ? new { ID = log.Subscription.ID, No = log.Subscription.SubscriberNo } : null,
                S2 = log.Customer.Subscriptions.Select(sub => new { ID = sub.ID, No = sub.SubscriberNo })
            }).ToArray().Select(log => new SystemLogsReportViewModel()
            {
                ID = log.ID,
                Date = log.Date,
                LogType = log.LogType,
                LogInterfaceType = log.Interface,
                LogInterfaceUsername = log.InterfaceUsername ?? "-",
                CustomerID = log.CustomerID,
                SubscriptionID = log.SubscriptionID,
                UserName = log.UserName,
                CustomerName = log.CustomerName ?? log.SubscriptionName,
                Subscribers = log.S2 != null && log.S2.Count() > 0 ? log.S2.Select(sub => new SystemLogsReportViewModel.Subscriber()
                {
                    ID = sub.ID,
                    SubscriberNo = sub.No
                }) : log.S1 != null ? new[]
                {
                    new SystemLogsReportViewModel.Subscriber()
                    {
                        ID = log.S1.ID,
                        SubscriberNo = log.S1.No
                    }
                } : null,
                ProcessedLog = processor.TranslateLog((SystemLogTypes)log.LogType, log.Parameters)
            });

            ViewBag.Search = search;
            ViewBag.Users = new SelectList(db.AppUsers.Select(user => new { Name = user.Name, Value = user.ID }).OrderBy(user => user.Name), "Value", "Name", search.UserID);
            return View(results);
        }

        [AuthorizePermission(Permissions = "Commitment Report")]
        [HttpGet]
        // GET: Reports/CommitmentReport
        public ActionResult CommitmentReport(int? page, CommitmentReportSearchViewModel search)
        {
            IEnumerable<CommitmentReportViewModel> results;
            if (search.NoCommitment)
            {
                var query = db.Subscriptions.Where(subscription => subscription.SubscriptionCommitment == null).OrderBy(subscription => subscription.ID).Select(subscription => new
                {
                    ID = subscription.ID,
                    SubscriberNo = subscription.SubscriberNo,
                    Name = subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.Title : subscription.Customer.FirstName + " " + subscription.Customer.LastName
                });

                SetupPages(page, ref query);

                results = query.ToArray().Select(subscription => new CommitmentReportViewModel()
                {
                    SubscriptionID = subscription.ID,
                    SubscriberNo = subscription.SubscriberNo,
                    SubscriberName = subscription.Name
                });
            }
            else
            {
                var query = db.SubscriptionCommitments.OrderByDescending(commitment => commitment.CommitmentExpirationDate).AsQueryable();
                if (search.StartDate.HasValue)
                {
                    query = query.Where(commitment => commitment.CommitmentExpirationDate >= search.StartDate);
                }
                if (search.EndDate.HasValue)
                {
                    query = query.Where(commitment => commitment.CommitmentExpirationDate < DbFunctions.AddDays(search.EndDate, 1));
                }

                SetupPages(page, ref query);

                results = query.Select(commitment => new
                {
                    Commitment = commitment,
                    Name = commitment.Subscription.Customer.CorporateCustomerInfo != null ? commitment.Subscription.Customer.CorporateCustomerInfo.Title : commitment.Subscription.Customer.FirstName + " " + commitment.Subscription.Customer.LastName,
                    SubscriberNo = commitment.Subscription.SubscriberNo
                }).ToArray().Select(c => new CommitmentReportViewModel()
                {
                    SubscriptionID = c.Commitment.SubscriptionID,
                    SubscriberName = c.Name,
                    SubscriberNo = c.SubscriberNo,
                    CommitmentLength = c.Commitment.CommitmentLength,
                    ExpirationDate = c.Commitment.CommitmentExpirationDate
                });
            }


            ViewBag.Search = search;
            return View(results);
        }

        [AuthorizePermission(Permissions = "Automatic Payment Report")]
        [HttpGet]
        // GET: Reports/AutomaticPayments
        public ActionResult AutomaticPayments(int? page, AutomaticPaymentReportSearchViewModel search)
        {
            var results = db.MobilExpressAutoPayments.Select(ap => new AutomaticPaymentReportViewModel()
            {
                SubscriptionID = ap.SubscriptionID,
                SubscriberNo = ap.Subscription.SubscriberNo,
                Gateway = "MobilExpress"
            }).Concat(db.RecurringPaymentSubscriptions.Select(rps => new AutomaticPaymentReportViewModel()
            {
                SubscriptionID = rps.SubscriptionID,
                SubscriberNo = rps.Subscription.SubscriberNo,
                Gateway = rps.RadiusRBillingService.Name
            })).OrderBy(apr => apr.SubscriptionID).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.Gateway))
                results = results.Where(apr => apr.Gateway == search.Gateway);

            SetupPages(page, ref results);

            ViewBag.Search = search;
            var gateways = db.RadiusRBillingServices.Select(rbs => new { Name = rbs.Name, Value = rbs.Name }).ToList();
            gateways.Add(new { Name = "MobilExpress", Value = "MobilExpress" });
            ViewBag.Gateways = new SelectList(gateways, "Value", "Name", search != null ? search.Gateway : "");
            return View(results.ToArray());
        }

        [AuthorizePermission(Permissions = "IPDR Report")]
        [HttpGet]
        // GET: Reports/IPDR
        public ActionResult IPDR(int? page, IPDRSearchViewModel search)
        {
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Database.CommandTimeout = 300;
            var baseQuery = db.RadiusAccountings.OrderByDescending(ra => ra.ID).Include(ra => ra.RadiusAccountingIPInfo);

            search = search ?? new IPDRSearchViewModel();
            if (search.StartDate.HasValue)
            {
                baseQuery = baseQuery.Where(ra => ra.StopTime >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(ra => ra.StartTime <= search.EndDate);
            }
            if (!string.IsNullOrWhiteSpace(search.LocalIP))
            {
                baseQuery = baseQuery.Where(ra => (ra.RadiusAccountingIPInfo != null && ra.RadiusAccountingIPInfo.LocalIP == search.LocalIP) || ra.FramedIPAddress == search.LocalIP);
            }
            if (!string.IsNullOrWhiteSpace(search.RealIP))
            {
                baseQuery = baseQuery.Where(ra => ra.RadiusAccountingIPInfo.RealIP == search.RealIP);
            }

            var viewResults = baseQuery.Select(ra => new IPDRReportViewModel()
            {
                CallingStation = ra.CallingStationID,
                LocalIP = ra.RadiusAccountingIPInfo != null ? ra.RadiusAccountingIPInfo.LocalIP : ra.FramedIPAddress,
                RealIP = ra.RadiusAccountingIPInfo != null ? ra.RadiusAccountingIPInfo.RealIP : "-",
                NASName = ra.NASIP,
                PortRange = ra.RadiusAccountingIPInfo != null ? ra.RadiusAccountingIPInfo.PortRange : "-",
                SessionID = ra.SessionID,
                SubscriptionID = ra.SubscriptionID,
                StartDate = ra.StartTime,
                Username = ra.Username,
                _endDate = ra.StopTime,
                _updateDate = ra.UpdateTime
            });

            SetupPages(page, ref viewResults);

            var nases = db.NAS.Select(nas => new { IP = nas.IP, Name = nas.Name }).ToArray();
            var results = viewResults.ToList();

            foreach (var item in results)
            {
                item.NASName = nases.FirstOrDefault(nas => nas.IP == item.NASName) != null ? nases.FirstOrDefault(nas => nas.IP == item.NASName).Name : item.NASName;
            }

            ViewBag.Search = search;
            return View(results);
        }

        [AuthorizePermission(Permissions = "Discount Report")]
        [HttpGet]
        // GET: Reports/Discounts
        public ActionResult Discounts(int? page, [Bind(Prefix = "search")] DiscountReportSearchViewModel search)
        {
            search = search ?? new DiscountReportSearchViewModel();
            if (search.StartDate == null || search.EndDate == null || search.StartDate > search.EndDate || (search.EndDate.Value - search.StartDate.Value).Days > 60)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Validation.ModelSpecific.MaxDateRange60Days;
                return View();
            }

            var baseQuery = db.Bills.OrderByDescending(b => b.IssueDate).Where(b => b.BillStatusID == (short)BillState.Cancelled || b.BillFees.Any(bf => bf.DiscountID != null));
            if (search.StartDate.HasValue)
            {
                baseQuery = baseQuery.Where(b => DbFunctions.TruncateTime(b.IssueDate) >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(b => DbFunctions.TruncateTime(b.IssueDate) <= search.EndDate);
            }

            var finalQuery = baseQuery.Include(b => b.BillFees.Select(bf => bf.Discount));

            SetupPages(page, ref finalQuery);

            var viewResults = new DiscountReportViewModel()
            {
                Rows = finalQuery.ToArray().Select(b => new DiscountReportViewModel.BillRow()
                {
                    SubscriberId = b.SubscriptionID,
                    SubscriberNo = b.Subscription.SubscriberNo,
                    IssueDate = b.IssueDate,
                    IsCancelled = b.BillStatusID == (short)BillState.Cancelled,
                    _total = b.GetTotalCost(),
                    _discountTotal = b.BillStatusID == (short)BillState.Cancelled ? b.GetTotalCost() : b.GetTotalDiscount(),
                    _paymentTotal = b.BillStatusID == (short)BillState.Cancelled ? 0m : b.GetPayableCost()
                })
            };

            viewResults._total = baseQuery.GetTotalAmount();
            viewResults._discountTotal = baseQuery.GetTotalDiscountAmount();
            viewResults._paymentTotal = baseQuery.GetTotalPayableAmount();

            ViewBag.Search = search;
            return View(viewResults);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Reports/Discounts
        public ActionResult Discounts(DiscountReportSearchViewModel search)
        {
            search = search ?? new DiscountReportSearchViewModel();
            if (search.StartDate == null || search.EndDate == null || search.StartDate > search.EndDate || (search.EndDate.Value - search.StartDate.Value).Days > 60)
            {
                return Content("<div class='centered text-danger'>" + RadiusR.Localization.Validation.ModelSpecific.MaxDateRange60Days + "</div>");
            }

            var baseQuery = db.Bills.OrderByDescending(b => b.IssueDate).Where(b => b.BillStatusID == (short)BillState.Cancelled || b.BillFees.Any(bf => bf.DiscountID != null));
            if (search.StartDate.HasValue)
            {
                baseQuery = baseQuery.Where(b => DbFunctions.TruncateTime(b.IssueDate) >= search.StartDate);
            }
            if (search.EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(b => DbFunctions.TruncateTime(b.IssueDate) <= search.EndDate);
            }

            var processedList = baseQuery.Select(b => new
            {
                SubscriberNo = b.Subscription.SubscriberNo,
                IssueDate = b.IssueDate,
                Total = b.BillFees.Select(fee => fee.CurrentCost).DefaultIfEmpty(0m).Sum(),
                DiscountTotal = b.BillStatusID == (short)BillState.Cancelled ? b.BillFees.Select(fee => fee.CurrentCost).DefaultIfEmpty(0m).Sum() : b.BillFees.Sum(fee => fee.DiscountID.HasValue ? fee.Discount.Amount : 0m)
            }).ToArray();

            var csvResults = processedList.Select(b => new DiscountReportCSVViewModel()
            {
                SubscriberNo = b.SubscriberNo,
                IssueDate = b.IssueDate.ToString("dd-MM-yyyy"),
                Total = b.Total.ToString("###,##0.00"),
                DiscountTotal = b.DiscountTotal.ToString("###,##0.00"),
                PaymentTotal = Math.Max(0, b.Total - b.DiscountTotal).ToString("###,##0.00")
            }).ToList();

            csvResults.Add(new DiscountReportCSVViewModel()
            {
                SubscriberNo = RadiusR.Localization.Pages.Common.Total,
                IssueDate = string.Empty,
                Total = processedList.Select(pd => pd.Total).DefaultIfEmpty(0m).Sum().ToString("###,##0.00"),
                DiscountTotal = processedList.Select(pd => pd.DiscountTotal).DefaultIfEmpty(0m).Sum().ToString("###,##0.00"),
                PaymentTotal = processedList.Select(pd => Math.Max(0m, pd.Total - pd.DiscountTotal)).DefaultIfEmpty(0m).Sum().ToString("###,##0.00"),
            });

            var currentTime = DateTime.Now;
            return File(CSVGenerator.GetStream(csvResults, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.DiscountReport + "_" + currentTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
        }

        [AuthorizePermission(Permissions = "Referral Discount Report")]
        [HttpGet]
        // GET: Reports/ReferralDiscounts
        public ActionResult ReferralDiscounts(int? page, DiscountReportSearchViewModel search)
        {
            search = search ?? new DiscountReportSearchViewModel();
            var baseQuery = db.RecurringDiscounts.OrderByDescending(rd => rd.CreationTime).ThenByDescending(rd => rd.ID).Where(rd => rd.ReferrerRecurringDiscountID.HasValue);
            if (search.StartDate.HasValue)
            {
                baseQuery = baseQuery.Where(rd => DbFunctions.TruncateTime(rd.CreationTime) >= search.StartDate.Value);
            }
            if (search.EndDate.HasValue)
            {
                baseQuery = baseQuery.Where(rd => DbFunctions.TruncateTime(rd.CreationTime) <= search.EndDate.Value);
            }

            var viewResults = baseQuery.Select(rd => new ReferralDiscountReportViewModel()
            {
                ReferredSubscriptionID = rd.SubscriptionID,
                ReferredSubscriptionNo = rd.Subscription.SubscriberNo,
                ReferrerSubscriptionID = rd.ReferrerRecurringDiscount.SubscriptionID,
                ReferrerSubscriptionNo = rd.ReferrerRecurringDiscount.Subscription.SubscriberNo,
                RecurringDiscount = new RecurringDiscountViewModel()
                {
                    _amount = rd.Amount,
                    ApplicationTimes = rd.ApplicationTimes,
                    ApplicationType = rd.ApplicationType,
                    CancellationCause = rd.CancellationCause,
                    CancellationDate = rd.CancellationDate,
                    CreationTime = rd.CreationTime,
                    Description = rd.Description,
                    DiscountType = rd.DiscountType,
                    FeeTypeID = rd.FeeTypeID,
                    IsDisabled = rd.IsDisabled,
                    OnlyFullInvoice = rd.OnlyFullInvoice,
                    TimesApplied = rd.AppliedRecurringDiscounts.Count()
                }
            });

            SetupPages(page, ref viewResults);

            ViewBag.Search = search;
            return View(viewResults);
        }
    }
}