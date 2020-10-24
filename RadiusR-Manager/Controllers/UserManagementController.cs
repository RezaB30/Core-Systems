using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.ViewModels;
using System.Data.Entity;
using RezaB.Web.CustomAttributes;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "User Management")]
    public class UserManagementController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        // GET: UserManagement
        public ActionResult Index(int? page, AppUserSearchViewModel search)
        {
            search = search ?? new AppUserSearchViewModel();
            var baseQuery = db.AppUsers.Include(user => user.Role).OrderByDescending(user => user.ID).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.Email))
            {
                baseQuery = baseQuery.Where(user => user.Email.Contains(search.Email));
            }
            if (!string.IsNullOrWhiteSpace(search.FullName))
            {
                baseQuery = baseQuery.Where(user => user.Name.Contains(search.FullName));
            }
            if (search.RoleID.HasValue)
            {
                baseQuery = baseQuery.Where(user => user.RoleID == search.RoleID);
            }

            var userId = User.GiveUserId();
            var viewResults = baseQuery.Select(user => new AppUserViewModel()
            {
                ID = user.ID,
                Email = user.Email,
                IsEnabled = user.IsEnabled,
                Name = user.Name,
                TCKNo = user.TCKNo,
                Phone = user.Phone,
                Role = user.Role,
                RoleID = user.RoleID,
                InternalCallCenterNo = user.InternalCallCenterNo,
                CanBeDeleted = false
                //CanBeDeleted = !user.Bills.Any()
                //    && !user.SubscriptionStateHistories.Any()
                //    && !user.SystemLogs.Any()
                //    && user.RoleID != (int)CommonRole.cashier
                //    && user.ID != userId
            });

            SetupPages(page, ref viewResults);

            ViewBag.Search = search;
            return View(viewResults);
        }

        // GET: UserManagement/Add
        public ActionResult Add()
        {
            ViewBag.InternalNos = new SelectList(CallCenterSettings.CallCenterInternalIDList.Except(db.AppUsers.Where(u => u.InternalCallCenterNo != null).Select(u => u.InternalCallCenterNo)).Select(no => new { Name = no, Value = no }), "Value", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: UserManagement/Add
        public ActionResult Add([Bind(Include = "Name,Email,Phone,Password,RoleID,TCKNo,InternalCallCenterNo")]AppUserViewModel appUser)
        {
            appUser.IsEnabled = true;
            if (ModelState.IsValid)
            {
                if (db.AppUsers.Any(user => user.Email == appUser.Email))
                {
                    ModelState.AddModelError("Email", RadiusR.Localization.Validation.Common.EmailAlreadyAssigned);
                }
                if (!string.IsNullOrEmpty(appUser.InternalCallCenterNo) && db.AppUsers.Any(user => user.InternalCallCenterNo == appUser.InternalCallCenterNo))
                {
                    ModelState.AddModelError("InternalCallCenterNo", RadiusR.Localization.Validation.Common.InternalNoAlreadyAssigned);
                }
                if (ModelState.IsValid)
                {
                    db.AppUsers.Add(new AppUser()
                    {
                        Email = appUser.Email,
                        IsEnabled = appUser.IsEnabled,
                        Name = appUser.Name,
                        Phone = appUser.Phone,
                        TCKNo = appUser.TCKNo,
                        InternalCallCenterNo = appUser.InternalCallCenterNo,
                        Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(appUser.Password),
                        RoleID = appUser.RoleID
                    });

                    db.SaveChanges();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }
            ViewBag.InternalNos = new SelectList(CallCenterSettings.CallCenterInternalIDList.Except(db.AppUsers.Where(u => u.InternalCallCenterNo != null).Select(u => u.InternalCallCenterNo)).Select(no => new { Name = no, Value = no }), "Value", "Name", appUser.InternalCallCenterNo);
            return View(appUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: UserManagement/Remove
        public ActionResult Remove(int id)
        {
            var appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            db.AppUsers.Remove(appUser);
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        // GET: UserManagement/Edit
        public ActionResult Edit(int id)
        {
            var appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            var AppUser = new AppUserViewModel()
            {
                ID = appUser.ID,
                Email = appUser.Email,
                Name = appUser.Name,
                TCKNo = appUser.TCKNo,
                Phone = appUser.Phone,
                RoleID = appUser.RoleID,
                CanBeDeleted = true,
                IsEnabled = appUser.IsEnabled,
                InternalCallCenterNo = appUser.InternalCallCenterNo
            };
            ViewBag.InternalNos = new SelectList(CallCenterSettings.CallCenterInternalIDList.Except(db.AppUsers.Where(u => u.InternalCallCenterNo != null && u.ID != appUser.ID).Select(u => u.InternalCallCenterNo)).Select(no => new { Name = no, Value = no }), "Value", "Name", appUser.InternalCallCenterNo);
            return View(AppUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: UserManagement/Edit
        public ActionResult Edit(int id, [Bind(Include = "Name,Email,Phone,RoleID,TCKNo,InternalCallCenterNo")]AppUserViewModel appUser)
        {
            var dbAppUser = db.AppUsers.Find(id);
            if (dbAppUser == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }
            if (!dbAppUser.Role.CanBeManuallyAssigned)
            {
                ModelState.Remove("RoleID");
                appUser.RoleID = dbAppUser.RoleID;
            }
            ModelState.Remove("Password");
            appUser.IsEnabled = dbAppUser.IsEnabled;

            if (ModelState.IsValid)
            {
                if (appUser.Email != dbAppUser.Email && db.AppUsers.Any(user => user.Email == appUser.Email))
                {
                    ModelState.AddModelError("Email", RadiusR.Localization.Validation.Common.EmailAlreadyAssigned);
                }
                if (!string.IsNullOrEmpty(appUser.InternalCallCenterNo) && db.AppUsers.Where(user => user.ID != dbAppUser.ID).Any(user => user.InternalCallCenterNo == appUser.InternalCallCenterNo))
                {
                    ModelState.AddModelError("InternalCallCenterNo", RadiusR.Localization.Validation.Common.InternalNoAlreadyAssigned);
                }
                if (ModelState.IsValid)
                {
                    dbAppUser.Name = appUser.Name;
                    dbAppUser.TCKNo = appUser.TCKNo;
                    dbAppUser.Phone = appUser.Phone;
                    dbAppUser.Email = appUser.Email;
                    dbAppUser.RoleID = appUser.RoleID;
                    dbAppUser.InternalCallCenterNo = appUser.InternalCallCenterNo;

                    db.Entry(dbAppUser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            ViewBag.InternalNos = new SelectList(CallCenterSettings.CallCenterInternalIDList.Except(db.AppUsers.Where(u => u.InternalCallCenterNo != null && u.ID != appUser.ID).Select(u => u.InternalCallCenterNo)).Select(no => new { Name = no, Value = no }), "Value", "Name", appUser.InternalCallCenterNo);
            return View(appUser);
        }

        // GET: UserManagement/ChangePassword
        public ActionResult ChangePassword(int id)
        {
            var dbAppUser = db.AppUsers.Find(id);
            if (dbAppUser == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            ViewBag.Name = dbAppUser.Name;
            ViewBag.UserId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: UserManagement/ChangePassword
        public ActionResult ChangePassword(int id, [Bind(Include = "NewPassword")] UserManagementChangePasswordViewModel changedPassword)
        {
            var dbAppUser = db.AppUsers.Find(id);
            if (dbAppUser == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            if (ModelState.IsValid)
            {
                dbAppUser.Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(changedPassword.NewPassword);

                db.Entry(dbAppUser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Edit", new { id = id, errorMessage = 0 });
            }

            ViewBag.Name = dbAppUser.Name;
            ViewBag.UserId = id;
            return View(changedPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: UserManagement/ChangeState
        public ActionResult ChangeState(int id)
        {
            if (id == User.GiveUserId())
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var currentUser = db.AppUsers.FirstOrDefault(user => user.ID == id);
            if (currentUser == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            currentUser.IsEnabled = !currentUser.IsEnabled;
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Roles = "admin")]
        // POST: UserManagement/SingInAs
        public ActionResult SignInAs(int id)
        {
            var currentUserId = User.GiveUserId();
            Request.GetOwinContext().SignOutUser();
            /*var errorMessage = */Request.GetOwinContext().SignInByUserId(id);
            //if (!string.IsNullOrEmpty(errorMessage) && currentUserId.HasValue)
            //{
            //    ViewBag.SignInError = errorMessage;
            //    Authenticator.SignInByUserId(currentUserId, Request.GetOwinContext());
            //    return View("SignInError");
            //}

            return RedirectToAction("Index", "Home");
        }
    }
}