using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Cashiers", Roles = "cashier")]
    public class CashierController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Cashiers")]
        // GET: Cashier
        public ActionResult Index(int? page)
        {
            var viewResults = db.Cashiers.OrderByDescending(cashier => cashier.ID).Select(cashier => new CashierViewModel()
            {
                ID = cashier.ID,
                CompanyTitle = cashier.CompanyTitle,
                _profitCut = cashier.ProfitCut,
                Address = cashier.Address,
                CashierBalances = cashier.CashierBalances.Select(balance => new CashierBalanceViewModel()
                {
                    ID = balance.ID,
                    _amount = balance.Amount,
                    Date = balance.Date,
                    CashierID = balance.CashierID
                }),
                IsEnabled = cashier.IsEnabled,
                TaxNo = cashier.TaxNo,
                TaxRegion = cashier.TaxRegion,
                User = new AppUserViewModel()
                {
                    ID = cashier.AppUser.ID,
                    Email = cashier.AppUser.Email,
                    Name = cashier.AppUser.Name
                }
            });

            SetupPages(page, ref viewResults);

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        // GET: Cashier/Add
        public ActionResult Add()
        {
            var cashier = new CashierViewModel()
            {
                User = new AppUserViewModel()
            };
            return View(cashier);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cashier/Add
        public ActionResult Add([Bind(Include = "CompanyTitle,ProfitCut,TaxRegion,TaxNo,PhoneNo,Address,User")]CashierViewModel cashier)
        {
            ModelState.Remove("User.RoleID");
            ModelState.Remove("User.IsEnabled");

            if (ModelState.IsValid)
            {
                db.Cashiers.Add(new Cashier()
                {
                    CompanyTitle = cashier.CompanyTitle,
                    ProfitCut = cashier._profitCut,
                    Address = cashier.Address,
                    IsEnabled = true,
                    TaxNo = cashier.TaxNo,
                    TaxRegion = cashier.TaxNo,
                    AppUser = new AppUser()
                    {
                        IsEnabled = true,
                        Email = cashier.User.Email,
                        Name = cashier.User.Name,
                        Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(cashier.User.Password),
                        RoleID = (int)CommonRole.cashier,
                        Phone = cashier.User.Phone
                    }
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(cashier);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cashier/ChangeState
        public ActionResult ChangeState(int id)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 3 });
            }
            dbCashier.IsEnabled = !dbCashier.IsEnabled;
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cashier/Remove
        public ActionResult Remove(int id)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 26 });
            }
            if (dbCashier.CashierBalances.Any() || dbCashier.AppUser.Bills.Any())
            {
                return RedirectToAction("Index", new { errorMessage = 27 });
            }

            var dbUser = dbCashier.AppUser;
            db.Cashiers.Remove(dbCashier);
            db.AppUsers.Remove(dbUser);
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Cashiers")]
        // GET: Cashier/Details
        public ActionResult Details(int id)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 26 });
            }

            var cashier = new CashierViewModel()
            {
                ID = dbCashier.ID,
                CompanyTitle = dbCashier.CompanyTitle,
                _profitCut = dbCashier.ProfitCut,
                Address = dbCashier.Address,
                TaxNo = dbCashier.TaxNo,
                TaxRegion = dbCashier.TaxRegion,
                IsEnabled = dbCashier.IsEnabled,
                User = new AppUserViewModel()
                {
                    Email = dbCashier.AppUser.Email,
                    Name = dbCashier.AppUser.Name,
                    Phone = dbCashier.AppUser.Phone
                },
                CashierBalances = dbCashier.CashierBalances.Select(balance => new CashierBalanceViewModel()
                {
                    ID = balance.ID,
                    Date = balance.Date,
                    _amount = balance.Amount
                })
            };

            return View(cashier);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        // GET: Cashier/Edit
        public ActionResult Edit(int id)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 26 });
            }

            var cashier = new CashierViewModel()
            {
                ID = dbCashier.ID,
                CompanyTitle = dbCashier.CompanyTitle,
                _profitCut = dbCashier.ProfitCut,
                Address = dbCashier.Address,
                TaxNo = dbCashier.TaxNo,
                TaxRegion = dbCashier.TaxRegion,
                User = new AppUserViewModel()
                {
                    Email = dbCashier.AppUser.Email,
                    Name = dbCashier.AppUser.Name,
                    Phone = dbCashier.AppUser.Phone
                },
                CashierBalances = dbCashier.CashierBalances.Select(balance => new CashierBalanceViewModel()
                {
                    ID = balance.ID,
                    Date = balance.Date,
                    _amount = balance.Amount
                })
            };

            return View(cashier);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cashier/Edit
        public ActionResult Edit(int id, [Bind(Include = "CompanyTitle,ProfitCut,TaxRegion,TaxNo,PhoneNo,Address,User")]CashierViewModel cashier)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 26 });
            }

            ModelState.Remove("User.RoleID");
            ModelState.Remove("User.IsEnabled");
            ModelState.Remove("User.Password");

            if (ModelState.IsValid)
            {
                dbCashier.CompanyTitle = cashier.CompanyTitle;
                dbCashier.Address = cashier.Address;
                dbCashier.ProfitCut = cashier._profitCut;
                dbCashier.AppUser.Name = cashier.User.Name;
                dbCashier.AppUser.Email = cashier.User.Email;
                dbCashier.AppUser.Phone = cashier.User.Phone;
                dbCashier.TaxNo = cashier.TaxNo;
                dbCashier.TaxRegion = cashier.TaxRegion;

                db.Entry(dbCashier).State = db.Entry(dbCashier.AppUser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = dbCashier.ID, errorMessage = 0 });
            }

            cashier.ID = dbCashier.ID;
            return View(cashier);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers", Roles = "cashier")]
        [HttpGet]
        // GET: Cashier/ManageCredit
        public ActionResult ManageCredit(int id, int? page)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 3 });
            }

            var cashier = new CashierViewModel()
            {
                ID = dbCashier.ID,
                CompanyTitle = dbCashier.CompanyTitle,
                Address = dbCashier.Address,
                TaxNo = dbCashier.TaxNo,
                TaxRegion = dbCashier.TaxRegion,
                IsEnabled = dbCashier.IsEnabled,
                CashierBalances = dbCashier.CashierBalances.OrderByDescending(credit => credit.Date).Select(credit => new CashierBalanceViewModel()
                {
                    ID = credit.ID,
                    Details = credit.Details,
                    Date = credit.Date,
                    _amount = credit.Amount
                }),
                User = new AppUserViewModel()
                {
                    Name = dbCashier.AppUser.Name,
                    Phone = dbCashier.AppUser.Phone,
                    Email = dbCashier.AppUser.Email
                }
            };

            var dealerBalances = cashier.CashierBalances.AsQueryable();
            SetupPages(page, ref dealerBalances);

            ViewBag.EditCreditModel = new EditCreditViewModel();
            return View(cashier);
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cashier/AddCredit
        public ActionResult AddCredit([Bind(Prefix = "editCreditModel", Include = "AddingAmount,Details")]EditCreditViewModel model, int id)
        {
            var dbCashier = db.Cashiers.Find(id);
            if (dbCashier == null)
            {
                return RedirectToAction("Index", new { errorMessage = 3 });
            }

            ModelState.Remove("editCreditModel.SubtractingAmount");
            if (ModelState.IsValid)
            {
                dbCashier.CashierBalances.Add(new CashierBalance()
                {
                    Date = DateTime.Now,
                    Details = model.Details,
                    CashierID = dbCashier.ID,
                    Amount = model._addingAmount.Value
                });

                db.SaveChanges();

                return RedirectToAction("ManageCredit", new { id = id, errorMessage = 0 });
            }

            return RedirectToAction("ManageCredit", new { id = id, errorMessage = 11 });
        }

        [AuthorizePermission(Permissions = "Modify Cashiers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cashiers/SubtractCredit
        public ActionResult SubtractCredit([Bind(Prefix = "editCreditModel", Include = "SubtractingAmount,Details")]EditCreditViewModel model, int id)
        {
            var dbCashiers = db.Cashiers.Find(id);
            if (dbCashiers == null)
            {
                return RedirectToAction("Index", new { errorMessage = 3 });
            }

            ModelState.Remove("editCreditModel.AddingAmount");
            if (ModelState.IsValid)
            {
                dbCashiers.CashierBalances.Add(new CashierBalance()
                {
                    Date = DateTime.Now,
                    Details = model.Details,
                    CashierID = dbCashiers.ID,
                    Amount = model._subtractingAmount.Value * -1m
                });

                db.SaveChanges();

                return RedirectToAction("ManageCredit", new { id = id, errorMessage = 0 });
            }

            return RedirectToAction("ManageCredit", new { id = id, errorMessage = 11 });
        }
    }
}