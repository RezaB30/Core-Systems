using RadiusR.DB;
using RadiusR.DB.QueryExtentions;
using RadiusR.DB.ModelExtentions;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR_Manager.Models.Extentions;
using RadiusR_Manager.Models.ViewModels;
using System.Text;
using RadiusR.DB.DomainsCache;
using RezaB.Web;
using RezaB.Data.Localization;
using System.Data.SqlClient;
using System.Data.Entity;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Partners")]
    public class PartnerController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        // GET: Partner
        public ActionResult Index(int? page, PartnerSearchViewModel search)
        {
            search = search ?? new PartnerSearchViewModel();
            var baseQuery = db.Partners.OrderBy(p => p.ID).AsQueryable();

            if (search.GroupID.HasValue)
            {
                baseQuery = baseQuery.Where(p => p.GroupID == search.GroupID);
            }
            if (!string.IsNullOrWhiteSpace(search.Title))
            {
                baseQuery = baseQuery.Where(p => p.Title.Contains(search.Title));
            }
            if (!string.IsNullOrWhiteSpace(search.Email))
            {
                baseQuery = baseQuery.Where(p => p.Email.Contains(search.Email));
            }
            if (!search.ShowDisabled)
            {
                baseQuery = baseQuery.Where(p => p.IsActive);
            }

            var viewResults = baseQuery.Select(p => new PartnerViewModel()
            {
                ID = p.ID,
                Email = p.Email,
                ExecutiveFirstName = p.ExecutiveFirstName,
                ExecutiveLastName = p.ExecutiveLastName,
                ExecutiveTCK = p.ExecutiveTCK,
                IsActive = p.IsActive,
                MaxActiveUsers = p.MaxActiveUsers,
                Password = p.Password,
                PhoneNo = p.PhoneNo,
                TaxNo = p.TaxNo,
                TaxOffice = p.TaxOffice,
                Title = p.Title,
                CurrentActiveUsers = p.PartnerSubUsers.Count(su => su.IsActive),
                GroupName = p.PartnerGroup.Name
            });

            SetupPages(page, ref viewResults);

            ViewBag.Groups = new SelectList(db.PartnerGroups.OrderBy(pg => pg.Name).ToArray(), "ID", "Name", search.GroupID);
            ViewBag.Search = search;
            return View(viewResults.ToArray());
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/Add
        public ActionResult Add()
        {
            var partner = new PartnerViewModel() { Password = RadiusR.DB.Passwords.PasswordUtilities.GenerateSecurePassword(8) };
            ViewBag.Groups = new SelectList(db.PartnerGroups.OrderBy(pg => pg.Name).ToArray(), "ID", "Name");
            return View(partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/Add
        public ActionResult Add(PartnerViewModel partner)
        {
            if (ModelState.IsValid)
            {
                if (db.Partners.Any(p => p.Email == partner.Email))
                {
                    ModelState.AddModelError("Email", RadiusR.Localization.Validation.Common.UsernameExists);
                    return View(partner);
                }

                db.Partners.Add(new Partner()
                {
                    Address = partner.Address.GetDatabaseObject(),
                    Email = partner.Email,
                    ExecutiveFirstName = partner.ExecutiveFirstName,
                    ExecutiveLastName = partner.ExecutiveLastName,
                    ExecutiveTCK = partner.ExecutiveTCK,
                    IsActive = true,
                    MaxActiveUsers = partner.MaxActiveUsers.Value,
                    Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(partner.Password),
                    PhoneNo = partner.PhoneNo,
                    TaxNo = partner.TaxNo,
                    TaxOffice = partner.TaxOffice,
                    Title = partner.Title,
                    GroupID = partner.GroupID.Value,
                    SetupAllowance = partner._setupAllowance.Value,
                    PaymentAllowance = partner._paymentAllowance.Value,
                    MinAmountForAllowance = partner._minAmountForAllowance.Value
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.Groups = new SelectList(db.PartnerGroups.OrderBy(pg => pg.Name).ToArray(), "ID", "Name", partner.GroupID);
            return View(partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/ToggleState
        public ActionResult ToggleState(int id, string returnUrl)
        {
            var uri = new UriBuilder(returnUrl);

            var partner = db.Partners.Find(id);
            if (partner == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            partner.IsActive = !partner.IsActive;

            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        // GET: Partner/SubUserDetails
        public ActionResult SubUserDetails(int id, int? page, string returnUrl)
        {
            var uri = new UriBuilder(returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var partner = db.Partners.Find(id);
            if (partner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            var viewResults = db.PartnerSubUsers.Where(psu => psu.PartnerID == partner.ID).OrderBy(psu => psu.ID).Select(psu => new PartnerSubUserViewModel()
            {
                ID = psu.ID,
                IsActive = psu.IsActive,
                Name = psu.Name
            });

            SetupPages(page, ref viewResults);

            ViewBag.PartnerName = partner.Title;
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(viewResults);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/Edit
        public ActionResult Edit(int id, string returnUrl)
        {
            var uri = new UriBuilder(returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            var partner = new PartnerViewModel()
            {
                ID = dbPartner.ID,
                Address = new AddressViewModel(dbPartner.Address),
                Email = dbPartner.Email,
                ExecutiveFirstName = dbPartner.ExecutiveFirstName,
                ExecutiveLastName = dbPartner.ExecutiveLastName,
                ExecutiveTCK = dbPartner.ExecutiveTCK,
                MaxActiveUsers = dbPartner.MaxActiveUsers,
                PhoneNo = dbPartner.PhoneNo,
                TaxNo = dbPartner.TaxNo,
                TaxOffice = dbPartner.TaxOffice,
                Title = dbPartner.Title,
                GroupID = dbPartner.GroupID,
                _paymentAllowance = dbPartner.PaymentAllowance,
                _setupAllowance = dbPartner.SetupAllowance,
                _minAmountForAllowance = dbPartner.MinAmountForAllowance
            };

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.IsEdit = true;
            ViewBag.Groups = new SelectList(db.PartnerGroups.OrderBy(pg => pg.Name).ToArray(), "ID", "Name", partner.GroupID);

            return View(viewName: "Add", model: partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/Edit
        public ActionResult Edit(int id, string returnUrl, PartnerViewModel partner)
        {
            var uri = new UriBuilder(returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                if (dbPartner.Email != partner.Email && db.Partners.Any(p => p.Email == partner.Email))
                    ModelState.AddModelError("Email", RadiusR.Localization.Validation.Common.UsernameExists);
                else
                {
                    dbPartner.Title = partner.Title;
                    dbPartner.Email = partner.Email;
                    dbPartner.ExecutiveFirstName = partner.ExecutiveFirstName;
                    dbPartner.ExecutiveLastName = partner.ExecutiveLastName;
                    dbPartner.ExecutiveTCK = partner.ExecutiveTCK;
                    dbPartner.MaxActiveUsers = partner.MaxActiveUsers.Value;
                    dbPartner.PhoneNo = partner.PhoneNo;
                    dbPartner.TaxNo = partner.TaxNo;
                    dbPartner.TaxOffice = partner.TaxOffice;
                    partner.Address.CopyToDBObject(dbPartner.Address);
                    dbPartner.GroupID = partner.GroupID.Value;
                    if (dbPartner.CustomerSetupUser != null)
                        dbPartner.CustomerSetupUser.Name = new string(dbPartner.Title.Take(100).ToArray());
                    dbPartner.PaymentAllowance = partner._paymentAllowance.Value;
                    dbPartner.SetupAllowance = partner._setupAllowance.Value;
                    dbPartner.MinAmountForAllowance = partner._minAmountForAllowance.Value;

                    db.SaveChanges();

                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                }
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.IsEdit = true;
            ViewBag.Groups = new SelectList(db.PartnerGroups.OrderBy(pg => pg.Name).ToArray(), "ID", "Name", partner.GroupID);

            return View(viewName: "Add", model: partner);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/ChangePassword
        public ActionResult ChangePassword(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/ChangePassword
        public ActionResult ChangePassword(int id, string returnUrl, PartnerChangePasswordViewModel passwordChange)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            if (ModelState.IsValid)
            {
                dbPartner.Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(passwordChange.Password);

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            return View(passwordChange);
        }

        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/Settings
        public ActionResult Settings(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            return View();
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/WorkAreas
        public ActionResult WorkAreas(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var viewResults = db.WorkAreas.Where(wa => wa.PartnerID == dbPartner.ID).OrderBy(wa => wa.ID).Select(wa => new PartnerWorkAreaViewModel()
            {
                ID = wa.ID,
                ProvinceID = wa.ProvinceID,
                DistrictID = wa.DistrictID,
                RuralCode = wa.RuralCode,
                NeighbourhoodID = wa.NeighbourhoodID,
                DistrictName = wa.DistrictName,
                NeighbourhoodName = wa.NeighbourhoodName,
                ProvinceName = wa.ProvinceName
            });

            SetupPages(page, ref viewResults);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;

            return View(viewResults.ToArray());
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/AddWorkArea
        public ActionResult AddWorkArea(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/AddWorkArea
        public ActionResult AddWorkArea(int id, string returnUrl, PartnerWorkAreaViewModel workArea)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (ModelState.IsValid)
            {
                if (dbPartner.WorkAreas.Any(wa => wa.ProvinceID == workArea.ProvinceID && !wa.DistrictID.HasValue && !wa.RuralCode.HasValue && !wa.NeighbourhoodID.HasValue) ||
                    (workArea.DistrictID.HasValue && dbPartner.WorkAreas.Any(wa => wa.DistrictID == workArea.DistrictID && !wa.RuralCode.HasValue && !wa.NeighbourhoodID.HasValue)) ||
                    (workArea.RuralCode.HasValue && dbPartner.WorkAreas.Any(wa => wa.RuralCode == workArea.RuralCode && !wa.NeighbourhoodID.HasValue)) ||
                    (workArea.NeighbourhoodID.HasValue && dbPartner.WorkAreas.Any(wa => wa.NeighbourhoodID == workArea.NeighbourhoodID)))
                {
                    ViewBag.ConflictError = RadiusR.Localization.Validation.Common.PartnerWorkAreaConflict;
                }
                else
                {
                    // remove inclusive areas
                    {
                        IEnumerable<WorkArea> inclusiveAreas = Enumerable.Empty<WorkArea>();
                        if (!workArea.DistrictID.HasValue)
                        {
                            inclusiveAreas = dbPartner.WorkAreas.Where(wa => wa.ProvinceID == workArea.ProvinceID).ToArray();
                        }
                        else if (!workArea.RuralCode.HasValue)
                        {
                            inclusiveAreas = dbPartner.WorkAreas.Where(wa => wa.DistrictID == workArea.DistrictID).ToArray();
                        }
                        else if (!workArea.NeighbourhoodID.HasValue)
                        {
                            inclusiveAreas = dbPartner.WorkAreas.Where(wa => wa.RuralCode == workArea.RuralCode).ToArray();
                        }

                        if (inclusiveAreas.Any())
                            db.WorkAreas.RemoveRange(inclusiveAreas);
                    }

                    // add new work area
                    dbPartner.WorkAreas.Add(new WorkArea()
                    {
                        DistrictID = workArea.DistrictID,
                        DistrictName = workArea.DistrictName,
                        NeighbourhoodID = workArea.NeighbourhoodID,
                        NeighbourhoodName = workArea.NeighbourhoodName,
                        ProvinceID = workArea.ProvinceID,
                        ProvinceName = workArea.ProvinceName,
                        RuralCode = workArea.RuralCode
                    });

                    db.SaveChanges();

                    return RedirectToAction("WorkAreas", new { id = dbPartner.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
                }
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;

            return View(workArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/RemoveWorkArea
        public ActionResult RemoveWorkArea(int id, string returnUrl, long workAreaID)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });
            var dbWorkArea = db.WorkAreas.Find(workAreaID);
            if (dbWorkArea == null)
                return RedirectToAction("WorkAreas", new { id = dbPartner.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            db.WorkAreas.Remove(dbWorkArea);

            db.SaveChanges();

            return RedirectToAction("WorkAreas", new { id = dbPartner.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/Permission
        public ActionResult Permissions(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var viewResults = db.PartnerPermissions.Where(pp => pp.PartnerID == dbPartner.ID).OrderBy(pp => pp.Permission).Select(pp => new PartnerPermissionViewModel()
            {
                Permission = pp.Permission
            });

            SetupPages(page, ref viewResults);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;

            return View(viewResults);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/EditPermissions
        public ActionResult EditPermissions(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var selectedPermissions = dbPartner.PartnerPermissions.Select(pp => pp.Permission);

            var itemList = new LocalizedList<RadiusR.DB.Enums.PartnerPermissions, RadiusR.Localization.Lists.PartnerPermissions>().GenericList;

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.Permissions = new MultiSelectList(itemList, "ID", "Name", selectedPermissions);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/EditPermissions
        public ActionResult EditPermissions(int id, string returnUrl, PartnerPermissionSelectionViewModel partnerPermissions)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var selectedPermissions = dbPartner.PartnerPermissions.Select(pp => pp.Permission);

            if (ModelState.IsValid)
            {
                partnerPermissions.Permissions = partnerPermissions.Permissions ?? new short[0];
                partnerPermissions.Permissions = partnerPermissions.Permissions.Distinct().ToArray();

                // check setup service user
                // added setup permission
                if (!dbPartner.PartnerPermissions.Any(pp => pp.Permission == (short)RadiusR.DB.Enums.PartnerPermissions.Setup) && partnerPermissions.Permissions.Contains((short)RadiusR.DB.Enums.PartnerPermissions.Setup))
                {
                    if (dbPartner.CustomerSetupUser == null)
                    {
                        dbPartner.CustomerSetupUser = new CustomerSetupUser()
                        {
                            IsEnabled = true,
                            Name = new string(dbPartner.Title.Take(100).ToArray()),
                            Username = GenerateUniqueSetupServiceUsername(32),
                            Password = RadiusR.DB.Passwords.PasswordUtilities.HashLowSecurityPassword(RadiusR.DB.Passwords.PasswordUtilities.GenerateSecurePassword(16))
                        };
                    }
                    else
                    {
                        dbPartner.CustomerSetupUser.IsEnabled = true;
                    }
                }
                // removed setup permission
                else if (dbPartner.PartnerPermissions.Any(pp => pp.Permission == (short)RadiusR.DB.Enums.PartnerPermissions.Setup) && !partnerPermissions.Permissions.Contains((short)RadiusR.DB.Enums.PartnerPermissions.Setup))
                {
                    if (dbPartner.CustomerSetupUser != null)
                    {
                        if (!dbPartner.CustomerSetupUser.CustomerSetupTasks.Any())
                            db.CustomerSetupUsers.Remove(dbPartner.CustomerSetupUser);
                        else
                            dbPartner.CustomerSetupUser.IsEnabled = false;
                    }
                }

                // add permissions
                db.PartnerPermissions.RemoveRange(dbPartner.PartnerPermissions);
                foreach (var item in partnerPermissions.Permissions)
                {
                    dbPartner.PartnerPermissions.Add(new PartnerPermission()
                    {
                        Permission = item
                    });
                }


                db.SaveChanges();

                return RedirectToAction("Permissions", new { id = dbPartner.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
            }

            var itemList = new LocalizedList<RadiusR.DB.Enums.PartnerPermissions, RadiusR.Localization.Lists.PartnerPermissions>().GenericList;

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.Permissions = new MultiSelectList(itemList, "ID", "Name", partnerPermissions.Permissions);

            return View(partnerPermissions);
        }

        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/Credits
        public ActionResult Credits(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (!dbPartner.PartnerPermissions.Any(p => p.Permission == (short)RadiusR.DB.Enums.PartnerPermissions.Payment))
                ViewBag.PermissionWarning = RadiusR.Localization.Pages.Common.PartnerCreditPermissionWarning;

            var viewResults = db.PartnerCredits.Where(pc => pc.PartnerID == dbPartner.ID).OrderByDescending(pc => pc.Date).Select(pc => new PartnerCreditViewModel()
            {
                ID = pc.ID,
                _amount = pc.Amount,
                Date = pc.Date,
                BillID = pc.BillID,
                Details = pc.Details
            });

            SetupPages(page, ref viewResults);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.CreditsTotal = db.PartnerCredits.Where(pc => pc.PartnerID == dbPartner.ID).Select(pc => pc.Amount).DefaultIfEmpty(0m).Sum().ToString("###,##0.00");

            return View(viewResults);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/AddCredits
        public ActionResult AddCredits(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.PageTitle = RadiusR.Localization.Pages.Common.AddCredits;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/AddCredits
        public ActionResult AddCredits(int id, string returnUrl, PartnerCreditEditViewModel addedCredits)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (ModelState.IsValid)
            {
                dbPartner.PartnerCredits.Add(new PartnerCredit()
                {
                    Amount = addedCredits._amount.Value,
                    Details = addedCredits.Details,
                    Date = DateTime.Now
                });

                db.SaveChanges();

                return RedirectToAction("Credits", new { id = dbPartner.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.PageTitle = RadiusR.Localization.Pages.Common.AddCredits;

            return View(addedCredits);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/SubtractCredits
        public ActionResult SubtractCredits(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.PageTitle = RadiusR.Localization.Pages.Common.SubtractCredits;

            return View(viewName: "AddCredits");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/SubtractCredits
        public ActionResult SubtractCredits(int id, string returnUrl, PartnerCreditEditViewModel subtractedCredits)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (ModelState.IsValid)
            {
                var currentTotal = db.PartnerCredits.Where(pc => pc.PartnerID == dbPartner.ID).Select(pc => pc.Amount).DefaultIfEmpty(0m).Sum();
                if (currentTotal < subtractedCredits._amount.Value)
                {
                    ModelState.AddModelError("Amount", RadiusR.Localization.Validation.ModelSpecific.SubtractedAmountLessThanTotalCredits);
                }
                else
                {
                    dbPartner.PartnerCredits.Add(new PartnerCredit()
                    {
                        Amount = -1m * subtractedCredits._amount.Value,
                        Details = subtractedCredits.Details,
                        Date = DateTime.Now
                    });

                    db.SaveChanges();

                    return RedirectToAction("Credits", new { id = dbPartner.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
                }
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.PageTitle = RadiusR.Localization.Pages.Common.AddCredits;

            return View(viewName: "AddCredits", model: subtractedCredits);
        }

        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/ValidTariffs
        public ActionResult ValidTariffs(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbPartnerGroup = db.PartnerGroups.Find(id);
            if (dbPartnerGroup == null)
                return RedirectToAction("Groups", new { errorMessage = 9 });

            var viewResults = db.PartnerAvailableTariffs.Where(pat => pat.PartnerGroupID == dbPartnerGroup.ID).OrderByDescending(pat => pat.ID).Select(pat => new PartnerAvailableTariffViewModel()
            {
                ID = pat.ID,
                DomianID = pat.DomainID,
                DomainName = pat.Domain.Name,
                TariffID = pat.TariffID,
                TariffName = pat.Service.Name,
                _allowance = pat.Allowance,
                //_allowanceThreshold = pat.AllowanceThreshold,
                //Commitment = pat.Commitment
            });

            SetupPages(page, ref viewResults);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerGroupName = dbPartnerGroup.Name;

            return View(viewResults.ToArray());
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/AddValidTariff
        public ActionResult AddValidTariff(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartnerGroup = db.PartnerGroups.Find(id);
            if (dbPartnerGroup == null)
                return RedirectToAction("Groups", new { errorMessage = 9 });

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerGroupName = dbPartnerGroup.Name;
            ViewBag.DomainList = new SelectList(db.Domains.ToArray(), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/AddValidTariff
        public ActionResult AddValidTariff(int id, string returnUrl, PartnerAvailableTariffViewModel availableTariff)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartnerGroup = db.PartnerGroups.Find(id);
            if (dbPartnerGroup == null)
                return RedirectToAction("Groups", new { errorMessage = 9 });

            if (ModelState.IsValid)
            {
                var dbDomain = db.Domains.Find(availableTariff.DomianID);
                if (dbDomain == null)
                {
                    return RedirectToAction("ValidTariffs", new { id = dbPartnerGroup.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 9 });
                }
                var dbTariff = db.Services.Find(availableTariff.TariffID);
                if (dbTariff == null || !dbTariff.Domains.Any(d => d.ID == dbDomain.ID))
                {
                    return RedirectToAction("ValidTariffs", new { id = dbPartnerGroup.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 2 });
                }
                if (availableTariff._allowance > dbTariff.Price)
                {
                    ModelState.AddModelError("Allowance", string.Format(RadiusR.Localization.Validation.ModelSpecific.AllowanceGreaterThanTariffFee, dbTariff.Price.ToString("###,##0.00")));
                }
                else if (dbPartnerGroup.PartnerAvailableTariffs.Any(pat => pat.TariffID == availableTariff.TariffID && pat.DomainID == availableTariff.DomianID))
                {
                    ModelState.AddModelError("TariffID", RadiusR.Localization.Validation.ModelSpecific.TariffAlreadyAdded);
                }
                else
                {
                    db.PartnerAvailableTariffs.Add(new PartnerAvailableTariff()
                    {
                        Allowance = availableTariff._allowance.Value,
                        //AllowanceThreshold = availableTariff._allowanceThreshold.Value,
                        //Commitment = availableTariff.Commitment,
                        PartnerGroupID = dbPartnerGroup.ID,
                        TariffID = dbTariff.ID,
                        DomainID = dbDomain.ID
                    });

                    db.SaveChanges();

                    return RedirectToAction("ValidTariffs", new { id = dbPartnerGroup.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
                }
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerGroupName = dbPartnerGroup.Name;
            ViewBag.DomainList = new SelectList(db.Domains.ToArray(), "ID", "Name", availableTariff.DomianID);

            return View(availableTariff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/RemoveValidTariff
        public ActionResult RemoveValidTariff(int id, string returnUrl, long validTariffId)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartnerGroup = db.PartnerGroups.Find(id);
            if (dbPartnerGroup == null)
                return RedirectToAction("Groups", new { errorMessage = 9 });
            var validTariff = db.PartnerAvailableTariffs.Find(validTariffId);
            if (validTariff == null)
                return RedirectToAction("Groups", new { errorMessage = 9 });

            db.PartnerAvailableTariffs.Remove(validTariff);

            db.SaveChanges();

            return RedirectToAction("ValidTariffs", new { id = dbPartnerGroup.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: Partner/Groups
        public ActionResult Groups(int? page)
        {
            var viewResults = db.PartnerGroups.OrderBy(pg => pg.Name).Select(pg => new PartnerGroupViewModel()
            {
                ID = pg.ID,
                Name = pg.Name,
                PartnerCount = pg.Partners.Count()
            });

            SetupPages(page, ref viewResults);

            return View(viewResults.ToArray());
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Management")]
        // GET: partner/AddGroup
        public ActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: partner/AddGroup
        public ActionResult AddGroup(PartnerGroupViewModel group)
        {
            if (ModelState.IsValid)
            {
                db.PartnerGroups.Add(new PartnerGroup()
                {
                    Name = group.Name
                });

                db.SaveChanges();

                return RedirectToAction("Groups", new { errorMessage = 0 });
            }

            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Management")]
        // POST: Partner/RemoveGroup
        public ActionResult RemoveGroup(int id)
        {
            var dbPartnerGroup = db.PartnerGroups.Find(id);
            if (dbPartnerGroup == null || dbPartnerGroup.Partners.Any())
            {
                return RedirectToAction("Groups", new { errorMessage = 9 });
            }

            if (dbPartnerGroup.PartnerAvailableTariffs.Any())
                db.PartnerAvailableTariffs.RemoveRange(dbPartnerGroup.PartnerAvailableTariffs);

            db.PartnerGroups.Remove(dbPartnerGroup);

            db.SaveChanges();

            return RedirectToAction("Groups", new { errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Partner API Settings")]
        [HttpGet]
        // GET: Partner/APISettings
        public ActionResult APISettings()
        {
            var viewResults = new PartnerAPISettingsViewModel()
            {
                PartnerAPIKey = RadiusR.DB.Settings.PartnerAPISettings.PartnerAPIKey
            };

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Partner API Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("APISettings")]
        // POST: Partner/APISettings
        public ActionResult APISettingsReset()
        {
            var settings = new PartnerAPISettingsViewModel()
            {
                PartnerAPIKey = Guid.NewGuid().ToString()
            };

            RadiusR.DB.Settings.PartnerAPISettings.Update(settings);
            RadiusR.DB.Settings.PartnerAPISettings.ClearCache();

            return RedirectToAction("APISettings", new { errorMessage = 0 });
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Collections")]
        // GET: Partner/Allowances
        public ActionResult Allowances(int? page)
        {
            var results = db.Partners.OrderBy(p => p.Title).Select(p => new PartnerViewModel()
            {
                Title = p.Title,
                ExecutiveFirstName = p.ExecutiveFirstName,
                ExecutiveLastName = p.ExecutiveLastName,
                GroupName = p.PartnerGroup.Name,
                IsActive = p.IsActive,
                ID = p.ID,
            });

            SetupPages(page, ref results);

            return View(results);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Collections")]
        // GET: Partner/SetupAllowance
        public ActionResult SetupAllowance(int id, int? page)
        {

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Allowances", new { errorMessage = 9 });

            var results = db.PartnerCollections.Where(pc => pc.PartnerID == dbPartner.ID && pc.CollectionType == (short)RadiusR.DB.Enums.PartnerCollectionType.Setup).OrderByDescending(pc => pc.CreationDate).Select(pc => new PartnerCollectionViewModel()
            {
                ID = pc.ID,
                CreationDate = pc.CreationDate,
                Creator = pc.CreatorUser.Name,
                PaymentDate = pc.PaymentDate,
                Payer = pc.PayerUser.Name,
                _total = pc.CustomerSetupTasks.Select(cst => cst.Allowance ?? 0m).DefaultIfEmpty(0m).Sum()
            });

            SetupPages(page, ref results);

            ViewBag.CollectionType = RadiusR.DB.Enums.PartnerCollectionType.Setup;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.AllowanceDetails = db.GetAllowanceDetails(dbPartner.ID, RadiusR.DB.Enums.PartnerCollectionType.Setup).Select(item => new PartnerAllowanceDetailsViewModel(item.Key, item.Value)).ToArray();
            return View("PartnerCollectionList", results);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Collections")]
        // GET: Partner/SalesAllowance
        public ActionResult SalesAllowance(int id, int? page)
        {
            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
                return RedirectToAction("Allowances", new { errorMessage = 9 });

            var results = db.PartnerCollections.Where(pc => pc.PartnerID == dbPartner.ID && pc.CollectionType == (short)RadiusR.DB.Enums.PartnerCollectionType.Sales).OrderByDescending(pc => pc.CreationDate).Select(pc => new PartnerCollectionViewModel()
            {
                ID = pc.ID,
                CreationDate = pc.CreationDate,
                Creator = pc.CreatorUser.Name,
                PaymentDate = pc.PaymentDate,
                Payer = pc.PayerUser.Name,
                _total = pc.PartnerRegisteredSubscriptions.Select(prs => prs.Allowance).DefaultIfEmpty(0m).Sum()
            });

            SetupPages(page, ref results);

            ViewBag.CollectionType = RadiusR.DB.Enums.PartnerCollectionType.Sales;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.AllowanceDetails = db.GetAllowanceDetails(dbPartner.ID, RadiusR.DB.Enums.PartnerCollectionType.Sales).Select(item => new PartnerAllowanceDetailsViewModel(item.Key, item.Value)).ToArray();
            return View("PartnerCollectionList", results);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Collection Creation")]
        // GET: Partner/CreateSetupCollection
        public ActionResult CreateSetupCollection(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var results = db.CustomerSetupTasks
                .Where(cst => cst.SetupUserID == dbPartner.CustomerSetupUserID && cst.TaskStatus == (short)RadiusR.DB.Enums.CustomerSetup.TaskStatuses.Completed && cst.AllowanceState == (short)RadiusR.DB.Enums.PartnerAllowanceState.OnHold)
                .OrderByDescending(cst => cst.CompletionDate)
                .Select(cst => new PartnerCollectionDetailsViewModel()
                {
                    ID = cst.ID,
                    IssueDate = cst.TaskIssueDate,
                    CompletionDate = cst.CompletionDate,
                    SubscriptionID = cst.SubscriptionID,
                    SubscriberNo = cst.Subscription.SubscriberNo,
                    _allowance = cst.Allowance,
                    AllowanceState = cst.AllowanceState
                });

            ViewBag.Total = (results.Select(r => r._allowance).DefaultIfEmpty(0m).Sum() ?? 0m).ToString("###,##0.00");

            SetupPages(page, ref results);

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.CollectionType = RadiusR.DB.Enums.PartnerCollectionType.Setup;
            return View("CollectionDetails", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Collection Creation")]
        // POST: Partner/CreateSetupCollection
        public ActionResult CreateSetupCollection(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var resultIds = db.CustomerSetupTasks
                .Where(cst => cst.SetupUserID == dbPartner.CustomerSetupUserID && cst.TaskStatus == (short)RadiusR.DB.Enums.CustomerSetup.TaskStatuses.Completed && cst.AllowanceState == (short)RadiusR.DB.Enums.PartnerAllowanceState.OnHold)
                .Select(cst => cst.ID).ToArray();

            // to prevent unchecked changes
            {
                var temp = new CustomerSetupTask()
                {
                    PartnerCollectionID = 0,
                    AllowanceState = 0
                };
            }
            // transaction scope
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var newCollection = new PartnerCollection()
                    {
                        CollectionType = (short)RadiusR.DB.Enums.PartnerCollectionType.Setup,
                        CreationDate = DateTime.Now,
                        CreatorID = User.GiveUserId().Value,
                        PartnerID = dbPartner.ID
                    };
                    db.PartnerCollections.Add(newCollection);
                    db.SaveChanges();

                    db.Database.ExecuteSqlCommand($"UPDATE CustomerSetupTask SET AllowanceState = @newState, PartnerCollectionID = @collectionID WHERE ID in ({string.Join(",", resultIds)})", new SqlParameter("@newState", (short)RadiusR.DB.Enums.PartnerAllowanceState.Accepted), new SqlParameter("@collectionID", newCollection.ID));
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Collection Creation")]
        // GET: Partner/CreateSalesCollection
        public ActionResult CreateSalesCollection(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var results = db.PartnerRegisteredSubscriptions
                .Where(prs => prs.PartnerID == dbPartner.ID && prs.AllowanceState == (short)RadiusR.DB.Enums.PartnerAllowanceState.OnHold && prs.Subscription.Bills.Any(b => b.Source == (short)RadiusR.DB.Enums.BillSources.System && (DbFunctions.DiffMonths(b.PeriodStart, b.PeriodEnd) == 1 && b.PeriodStart.Value.Day <= b.PeriodEnd.Value.Day) || (DbFunctions.DiffMonths(b.PeriodStart, b.PeriodEnd) > 1)))
                .OrderByDescending(prs => prs.Subscription.MembershipDate)
                .Select(prs => new PartnerCollectionDetailsViewModel()
                {
                    ID = prs.SubscriptionID,
                    IssueDate = prs.Subscription.MembershipDate,
                    CompletionDate = null,
                    SubscriptionID = prs.SubscriptionID,
                    SubscriberNo = prs.Subscription.SubscriberNo,
                    _allowance = prs.Allowance,
                    AllowanceState = prs.AllowanceState
                });

            ViewBag.Total = (results.Select(r => r._allowance).DefaultIfEmpty(0m).Sum() ?? 0m).ToString("###,##0.00");

            SetupPages(page, ref results);

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbPartner.Title;
            ViewBag.CollectionType = RadiusR.DB.Enums.PartnerCollectionType.Sales;
            return View("CollectionDetails", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Collection Creation")]
        // POST: Partner/CreateSalesCollection
        public ActionResult CreateSalesCollection(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartner = db.Partners.Find(id);
            if (dbPartner == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var resultIds = db.PartnerRegisteredSubscriptions
                .Where(prs => prs.PartnerID == dbPartner.ID && prs.AllowanceState == (short)RadiusR.DB.Enums.PartnerAllowanceState.OnHold).FilterSubscriptionsWithAtLeastOneFullBill()
                .Select(prs => prs.SubscriptionID).ToArray();

            // to prevent unchecked changes
            {
                var temp = new PartnerRegisteredSubscription()
                {
                    PartnerCollectionID = 0,
                    AllowanceState = 0,
                    SubscriptionID = 0
                };
            }
            // transaction scope
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var newCollection = new PartnerCollection()
                    {
                        CollectionType = (short)RadiusR.DB.Enums.PartnerCollectionType.Sales,
                        CreationDate = DateTime.Now,
                        CreatorID = User.GiveUserId().Value,
                        PartnerID = dbPartner.ID
                    };
                    db.PartnerCollections.Add(newCollection);
                    db.SaveChanges();

                    db.Database.ExecuteSqlCommand($"UPDATE PartnerRegisteredSubscription SET AllowanceState = @newState, PartnerCollectionID = @collectionID WHERE SubscriptionID in ({string.Join(",", resultIds)})", new SqlParameter("@newState", (short)RadiusR.DB.Enums.PartnerAllowanceState.Accepted), new SqlParameter("@collectionID", newCollection.ID));
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Partner Collections")]
        // GET: Partner/CollectionDetails
        public ActionResult CollectionDetails(long id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartnerCollection = db.PartnerCollections.Find(id);
            if (dbPartnerCollection == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (dbPartnerCollection.CollectionType == (short)RadiusR.DB.Enums.PartnerCollectionType.Setup)
            {
                var results = db.CustomerSetupTasks.Where(cst => cst.PartnerCollectionID == dbPartnerCollection.ID).OrderByDescending(cst => cst.CompletionDate).Select(cst => new PartnerCollectionDetailsViewModel()
                {
                    ID = cst.ID,
                    IssueDate = cst.TaskIssueDate,
                    CompletionDate = cst.CompletionDate,
                    SubscriptionID = cst.SubscriptionID,
                    SubscriberNo = cst.Subscription.SubscriberNo,
                    _allowance = cst.Allowance,
                    AllowanceState = cst.AllowanceState
                });

                ViewBag.Total = (results.Select(r => r._allowance).DefaultIfEmpty(0m).Sum() ?? 0m).ToString("###,##0.00");

                SetupPages(page, ref results);

                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
                ViewBag.PartnerName = dbPartnerCollection.Partner.Title;
                return View("CollectionDetails", results);
            }
            else if (dbPartnerCollection.CollectionType == (short)RadiusR.DB.Enums.PartnerCollectionType.Sales)
            {
                var results = db.PartnerRegisteredSubscriptions
               .Where(prs => prs.PartnerCollectionID == dbPartnerCollection.ID)
               .OrderByDescending(prs => prs.Subscription.MembershipDate)
               .Select(prs => new PartnerCollectionDetailsViewModel()
               {
                   ID = prs.SubscriptionID,
                   IssueDate = prs.Subscription.MembershipDate,
                   CompletionDate = null,
                   SubscriptionID = prs.SubscriptionID,
                   SubscriberNo = prs.Subscription.SubscriberNo,
                   _allowance = prs.Allowance,
                   AllowanceState = prs.AllowanceState
               });

                ViewBag.Total = (results.Select(r => r._allowance).DefaultIfEmpty(0m).Sum() ?? 0m).ToString("###,##0.00");

                SetupPages(page, ref results);

                UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
                ViewBag.PartnerName = dbPartnerCollection.Partner.Title;
                return View("CollectionDetails", results);
            }
            else
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Partner Collection Payment")]
        // POST: Partner/PayPartnerCollection
        public ActionResult PayPartnerCollection(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbPartnerCollection = db.PartnerCollections.Find(id);
            if (dbPartnerCollection == null || dbPartnerCollection.PaymentDate.HasValue)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }


            // transaction scope
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    dbPartnerCollection.PayerID = User.GiveUserId().Value;
                    dbPartnerCollection.PaymentDate = DateTime.Now;
                    db.SaveChanges();

                    if (dbPartnerCollection.CollectionType == (short)RadiusR.DB.Enums.PartnerCollectionType.Setup)
                    {
                        // to prevent unchecked changes
                        {
                            var temp = new CustomerSetupTask()
                            {
                                PartnerCollectionID = 0,
                                AllowanceState = 0
                            };
                        }

                        db.Database.ExecuteSqlCommand($"UPDATE CustomerSetupTask SET AllowanceState = @newState WHERE PartnerCollectionID = @collectionId", new SqlParameter("@newState", (short)RadiusR.DB.Enums.PartnerAllowanceState.Paid), new SqlParameter("@collectionId", dbPartnerCollection.ID));
                        transaction.Commit();
                    }
                    else if (dbPartnerCollection.CollectionType == (short)RadiusR.DB.Enums.PartnerCollectionType.Sales)
                    {
                        // to prevent unchecked changes
                        {
                            var temp = new PartnerRegisteredSubscription()
                            {
                                PartnerCollectionID = 0,
                                AllowanceState = 0
                            };
                        }

                        db.Database.ExecuteSqlCommand($"UPDATE PartnerRegisteredSubscription SET AllowanceState = @newState WHERE PartnerCollectionID = @collectionId", new SqlParameter("@newState", (short)RadiusR.DB.Enums.PartnerAllowanceState.Paid), new SqlParameter("@collectionId", dbPartnerCollection.ID));
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        private string GenerateUniqueSetupServiceUsername(int length)
        {
            var palette = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rand = new Random();

            string results = string.Empty;
            do
            {
                var generatedText = new StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    generatedText.Append(palette[rand.Next(palette.Length)]);
                }
                results = generatedText.ToString();
            } while (db.CustomerSetupUsers.Any(csu => csu.Username == results));

            return results;
        }
    }
}