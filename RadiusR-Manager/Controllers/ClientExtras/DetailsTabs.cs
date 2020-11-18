using RadiusR.DB.Enums;
using RadiusR.SystemLogs;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using RadiusR_Manager.Models.ViewModels;
using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Data.Localization;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        [AuthorizePermission(Permissions = "Clients")]
        [AjaxCall]
        // GET: Client/SusbscriptionFees
        public ActionResult SubscriptionFees(long subscriptionId, int? page = 0)
        {
            var dbSubscription = db.Subscriptions.Find(subscriptionId);
            if (dbSubscription == null)
            {
                return Content("<span class='text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._4 + "</span>");
            }

            ViewBag.IsCancelled = dbSubscription.IsCancelled;
            ViewBag.SubscriptionID = dbSubscription.ID;

            var viewResults = dbSubscription.Fees.Where(fee => fee.FeeTypeID != (short)FeeType.Tariff).Select(fee => new FeeViewModel()
            {
                Date = fee.Date,
                FeeTypeID = fee.FeeTypeID,
                InstallmentBillCount = fee.InstallmentBillCount,
                SubscriptionID = fee.SubscriptionID,
                ID = fee.ID,
                FeeType = new AdditionalFeeViewModel()
                {
                    FeeTypeID = fee.FeeTypeCost.FeeTypeID,
                    TaxTypes = fee.FeeTypeCost.TaxRates.Select(rate => new TaxRateViewModel()
                    {
                        ID = rate.ID,
                        _rate = rate.Rate
                    }),
                    IsAllTime = fee.FeeTypeCost.IsAllTime,
                    _price = (fee.Cost.HasValue) ? fee.Cost.Value : fee.FeeTypeCost.Cost.Value
                },
                Cost = fee.Cost,
                FeeTypeVariant = (fee.FeeTypeVariant == null) ? null : new FeeTypeVariantViewModel()
                {
                    Title = fee.FeeTypeVariant.Title,
                    FeeTypeID = fee.FeeTypeVariant.FeeTypeID,
                    ID = fee.FeeTypeVariant.ID,
                    _price = fee.FeeTypeVariant.Price
                },
                CanBeCancelled = fee.CanBeCancelled,
                IsCancelled = fee.IsCancelled,
                Description = fee.Description,
                StartDate = fee.StartDate,
                EndDate = fee.EndDate
            }).OrderBy(f => f.Date).AsQueryable();

            SetupPages(page, ref viewResults);

            return View(viewName: "DetailsTabs/SubscriptionFees", model: viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Clients")]
        // POST: Client/RemoveAdditionalFee
        public ActionResult CancelAdditionalFee(long feeId, long subscriptionId)
        {
            var fee = db.Fees.Find(feeId);
            if (fee == null)
            {
                return Redirect(Url.Action("SubscriptionFees", new { subscriptionId = subscriptionId, errorMessage = 6 }));
            }
            if (fee.Subscription.IsCancelled || fee.IsCancelled)
            {
                return Redirect(Url.Action("SubscriptionFees", new { subscriptionId = fee.Subscription.ID, errorMessage = 9 }));
            }

            if (!fee.CanBeCancelled)
            {
                return Redirect(Url.Action("SubscriptionFees", new { subscriptionId = subscriptionId, errorMessage = 7 }));
            }

            // remove all-time fees
            if (fee.FeeTypeCost.IsAllTime)
                db.Fees.Remove(fee);
            // cancel the rest
            else
                fee.IsCancelled = true;

            db.SystemLogs.Add(SystemLogProcessor.CancelAdditionalFee(feeId, User.GiveUserId(), subscriptionId, SystemLogInterface.MasterISS, null));
            db.SaveChanges();

            return Redirect(Url.Action("SubscriptionFees", new { subscriptionId = subscriptionId, errorMessage = 0 }));
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        // GET: Client/AddFee
        public ActionResult AddFee(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;

            ViewBag.AvailableFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray().Where(f => f.CanBeAdditional).Select(f => new SubscriberFeesAddViewModel()
            {
                FeeTypeID = f.FeeTypeID,
                FeeTypeName = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(f.FeeTypeID),
                IsAllTime = f.IsAllTime,
                Variants = f.FeeTypeVariants.Any() ? f.FeeTypeVariants.Select(fv => new SubscriberFeesAddViewModel.FeeVariant()
                {
                    ID = fv.ID,
                    Name = fv.Title
                }) : null,
                CustomFees = !f.Cost.HasValue && !f.FeeTypeVariants.Any() ? Enumerable.Empty<SubscriberFeesAddViewModel.CustomFee>() : null
            }).ToArray();

            return View(viewName: "DetailsTabs/AddFee", model: new List<SubscriberFeesAddViewModel>());
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        // POST: Client/AddFee
        public ActionResult AddFee(long id, SubscriberFeesAddViewModel[] fees)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var availableFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray().Where(f => f.CanBeAdditional).Select(f => new SubscriberFeesAddViewModel()
            {
                FeeTypeID = f.FeeTypeID,
                FeeTypeName = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(f.FeeTypeID),
                IsAllTime = f.IsAllTime,
                Variants = f.FeeTypeVariants.Any() ? f.FeeTypeVariants.Select(fv => new SubscriberFeesAddViewModel.FeeVariant()
                {
                    ID = fv.ID,
                    Name = fv.Title
                }) : null,
                CustomFees = !f.Cost.HasValue && !f.FeeTypeVariants.Any() ? Enumerable.Empty<SubscriberFeesAddViewModel.CustomFee>() : null
            }).ToArray();

            FixSubscriptionAddedFeesModelState("fees", fees, availableFees);

            if (ModelState.IsValid)
            {
                var dbFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray();
                var addedFees = fees.Select(af => af.GetDBObject(dbFees)).SelectMany(f => f).Where(f => f != null).ToList();

                // no installment for pre-paid
                if (!dbSubscription.HasBilling)
                    addedFees.ForEach(f => f.InstallmentBillCount = 1);

                foreach (var fee in addedFees)
                {
                    dbSubscription.Fees.Add(fee);
                }

                // pre-paid subscriber bill
                Bill addedBill = null;
                if (!dbSubscription.HasBilling)
                {
                    // prepare and remove all-time fees
                    var preparedFees = addedFees.Select(f => new
                    {
                        Fee = f,
                        dbReference = dbFees.FirstOrDefault(dbf => dbf.FeeTypeID == f.FeeTypeID),
                        dbVariant = dbFees.FirstOrDefault(dbf => dbf.FeeTypeID == f.FeeTypeID).FeeTypeVariants.FirstOrDefault(v => v.ID == f.FeeTypeVariantID)
                    })
                    .Where(f => !f.dbReference.IsAllTime)
                    .ToList();
                    // if any other fees remain create a bill
                    if (preparedFees.Any())
                    {
                        addedBill = new Bill()
                        {
                            BillStatusID = (short)BillState.Unpaid,
                            IssueDate = DateTime.Now,
                            DueDate = DateTime.Now,
                            PaymentTypeID = (short)PaymentType.None,
                            BillFees = preparedFees.Select(f => new BillFee()
                            {
                                InstallmentCount = 1,
                                Fee = f.Fee,
                                CurrentCost = f.Fee.Cost ?? f.dbReference.Cost ?? f.dbVariant.Price
                            }).ToList(),
                            Source = (short)BillSources.Manual
                        };
                        dbSubscription.Bills.Add(addedBill);
                    }
                }

                db.SaveChanges();
                // system log
                db.SystemLogs.Add(SystemLogProcessor.AddAdditionalFees(addedFees.Select(f => f.ID).ToArray(), User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null));
                db.SaveChanges();

                // go to bill details for pre-paid if available
                if (!dbSubscription.HasBilling && addedBill != null)
                {
                    return RedirectToAction("Details", "Bill", new { id = addedBill.ID, redirectUrl = Url.Action("Details", null, new { id = dbSubscription.ID }, Request.Url.Scheme) + "#bills" });
                }

                return Redirect(Url.Action("Details", new { id = dbSubscription.ID }) + "#additional-fees");
            }

            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            ViewBag.AvailableFees = availableFees;

            return View(viewName: "DetailsTabs/AddFee", model: fees);
        }

        [AuthorizePermission(Permissions = "Modify Clients,Create Bill")]
        // GET: Client/CreateBillForFees
        public ActionResult CreateBillForFees(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }
            if (!dbSubscription.HasBilling)
            {
                return RedirectToAction("Details", "Client", new { errorMessage = 9 });
            }
            var validFees = dbSubscription.Fees.Where(fee => fee.CanBeCancelled && fee.InstallmentBillCount == 1 && !fee.FeeTypeCost.IsAllTime).ToList();
            if (validFees.Count() <= 0)
            {
                return RedirectToAction("Details", "Client", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            var viewResults = new CreateBillViewModel()
            {
                ClientID = dbSubscription.ID,
                ClientName = dbSubscription.ValidDisplayName,
                Fees = validFees.Select(fee => new CreateBillViewModel.AddedFeeViewModel()
                {
                    IsSelected = false,
                    ID = fee.ID,
                    FeeTypeID = fee.FeeTypeID,
                    Description = fee.Description
                })
            };

            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Clients,Create Bill")]
        // POST: Client/CreateBillForFees
        public ActionResult CreateBillForFees(long id, long[] fees)
        {
            fees = fees ?? new long[0];
            fees = fees.Distinct().ToArray();
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", "Client", new { errorMessage = 4 });
            }
            if (!dbSubscription.HasBilling)
            {
                return RedirectToAction("Details", "Client", new { errorMessage = 9 });
            }
            var validFees = dbSubscription.Fees.Where(fee => fee.CanBeCancelled && fee.InstallmentBillCount == 1 && !fee.FeeTypeCost.IsAllTime).ToList();
            if (validFees.Count() <= 0)
            {
                return RedirectToAction("Details", "Client", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            if (fees.Count() > 0)
            {
                if (fees.Except(validFees.Select(fee => fee.ID)).Any())
                {
                    return RedirectToAction("Details", "Client", new { id = dbSubscription.ID, errorMessage = 9 });
                }

                dbSubscription.CreateManualBill(fees);

                db.SaveChanges();

                return Redirect(Url.Action("Details", new { id = dbSubscription.ID, errorMessage = 0 }) + "#bills");
            }

            var viewResults = new CreateBillViewModel()
            {
                ClientID = dbSubscription.ID,
                ClientName = dbSubscription.ValidDisplayName,
                Fees = validFees.Select(fee => new CreateBillViewModel.AddedFeeViewModel()
                {
                    IsSelected = fees.Contains(fee.ID),
                    ID = fee.ID,
                    FeeTypeID = fee.FeeTypeID,
                    Description = fee.Description
                })
            };

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Clients")]
        [AjaxCall]
        // GET: Client/RecurringDiscounts
        public ActionResult RecurringDiscounts(long id, int? page = 0)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content("<span class='text-danger'>" + RadiusR.Localization.Pages.ErrorMessages._4 + "</span>");
            }

            var viewResults = db.RecurringDiscounts.OrderByDescending(rd => rd.CreationTime).Where(rd => rd.SubscriptionID == dbSubscription.ID).Select(rd => new RecurringDiscountViewModel()
            {
                ID = rd.ID,
                CreationTime = rd.CreationTime,
                DiscountType = rd.DiscountType,
                ApplicationType = rd.ApplicationType,
                _amount = rd.Amount,
                FeeTypeID = rd.FeeTypeID,
                ApplicationTimes = rd.ApplicationTimes,
                TimesApplied = rd.AppliedRecurringDiscounts.Count(),
                IsDisabled = rd.IsDisabled,
                OnlyFullInvoice = rd.OnlyFullInvoice,
                Description = rd.Description,
                CancellationDate = rd.CancellationDate,
                CancellationCause = rd.CancellationCause,
                ReferencedSubscriptionID = rd.ReferrerRecurringDiscount != null ? rd.ReferrerRecurringDiscount.SubscriptionID : rd.ReferringRecurringDiscounts.Any() ? rd.ReferringRecurringDiscounts.FirstOrDefault().SubscriptionID : (long?)null,
                ReferrerCode = rd.ReferrerRecurringDiscount != null ? rd.ReferrerRecurringDiscount.Subscription.ReferenceNo : null,
                ReferringCode = rd.ReferringRecurringDiscounts.Any() ? rd.ReferringRecurringDiscounts.FirstOrDefault().Subscription.ReferenceNo : null
            });

            SetupPages(page, ref viewResults);

            ViewBag.IsValidForReferralDiscount = new[] { (short)CustomerState.PreRegisterd, (short)CustomerState.Registered, (short)CustomerState.Reserved, (short)CustomerState.Active }.Contains(dbSubscription.State);
            return View(viewName: "DetailsTabs/RecurringDiscounts", model: viewResults.ToArray());
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [ValidateAntiForgeryToken]
        // POST: Client/ToggleArchiveScan
        public ActionResult ToggleArchiveScan(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            subscription.ArchiveScanned = !subscription.ArchiveScanned;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Clients")]
        [HttpGet]
        // GET: Client/Notes
        public ActionResult Notes(long id, int? page)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var viewResults = db.SubscriptionNotes.Where(note => note.SubscriptionID == dbSubscription.ID).OrderByDescending(note => note.Date).Select(note => new NoteViewModel()
            {
                ID = note.ID,
                SubscriptionID = note.SubscriptionID,
                WriterID = note.WriterID,
                Date = note.Date,
                Message = note.Message,
                WriterName = note.AppUser.Name
            });

            SetupPages(page, ref viewResults);

            return View(viewName: "DetailsTabs/Notes", model: viewResults.ToList());
        }

        [AuthorizePermission(Permissions = "Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/Notes
        public ActionResult Notes(long id, int? page, [Bind(Include = "Message", Prefix = "addedNote")]NoteViewModel addedNote)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            addedNote.SubscriptionID = dbSubscription.ID;
            addedNote.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.SubscriptionNotes.Add(new SubscriptionNote()
                {
                    WriterID = User.GiveUserId().Value,
                    SubscriptionID = addedNote.SubscriptionID,
                    Date = addedNote.Date,
                    Message = addedNote.Message
                });

                db.SaveChanges();
            }

            return RedirectToAction("Notes", "Client", new { id = dbSubscription.ID });
        }

        [AuthorizePermission(Permissions = "System Logs")]
        [HttpGet]
        // GET: Client/SystemLogs
        public ActionResult SystemLogs(long id, int? page)
        {
            var subscription = db.Subscriptions.Find(id);
            var logs = db.SystemLogs.Include(log => log.AppUser).Where(log => log.CustomerID == subscription.CustomerID || log.SubscriptionID == subscription.ID).OrderByDescending(log => log.Date).AsQueryable();

            SetupPages(page, ref logs);

            var processor = new SystemLogProcessor(Url);
            var results = logs.ToArray().Select(log => new SystemLogViewModel()
            {
                ID = log.ID,
                Date = log.Date,
                LogType = log.LogType,
                LogInterfaceType = log.Interface,
                LogInterfaceUsername = log.InterfaceUsername ?? "-",
                CustomerID = log.CustomerID,
                SubscriptionID = log.SubscriptionID,
                UserName = log.AppUser != null ? log.AppUser.Name : "-",
                ProcessedLog = processor.TranslateLog(log, log.Parameters)
            });

            return View(viewName: "DetailsTabs/SystemLogs", model: results);
        }

        [AuthorizePermission(Permissions = "Clients")]
        [HttpGet]
        // GET: Client/TariffChangeHistory
        public ActionResult TariffChangeHistory(long id, int? page)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var viewResults = db.SubscriptionTariffHistories.OrderByDescending(sth => sth.Date).Where(sth => sth.SubscriptionID == id).Select(sth => new TariffChangeHistoryViewModel()
            {
                Date = sth.Date,
                OldTariffName = sth.OldTariff.Name,
                NewTariffName = sth.NewTariff.Name,
            });

            SetupPages(page, ref viewResults);

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "DetailsTabs/TariffChangeHistory", model: viewResults.ToArray());
        }
    }
}