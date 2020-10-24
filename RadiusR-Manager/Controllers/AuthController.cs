using RadiusR.DB;
using RadiusR.SMS;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web;
using RezaB.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public class AuthController : BaseController
    {
        [AllowAnonymous]
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        // GET: Login
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // POST: Login
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var state = Request.GetOwinContext().SignInUser(model.Email, model.Password);
                if (!state)
                {
                    ModelState.AddModelError("", RadiusR.Localization.Errors.Common.InvalidUserPass);
                    return View(model);
                }
                return Redirect(GetRedirectUrl(Request.QueryString["ReturnUrl"]));
            }
            return View(model);
        }

        // POST: LogOff
        public ActionResult LogOff()
        {
            Request.GetOwinContext().SignOutUser();
            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Manage
        public ActionResult Manage(string redirectUrl)
        {
            var changePassword = new ChangePasswordViewModel() { redirectUrl = redirectUrl };
            return View(changePassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Auth/Manage
        public ActionResult Manage([Bind(Include = "OldPassword,NewPassword,redirectUrl")]ChangePasswordViewModel changePassword)
        {
            var uri = new UriBuilder(changePassword.redirectUrl);
            if (ModelState.IsValid)
            {
                var userId = User.GiveUserId();
                if(userId == null)
                {
                    ModelState.AddModelError("", RadiusR.Localization.Errors.Common.UserNotFound);
                    return View(changePassword);
                }

                using (RadiusREntities sqldb = new RadiusREntities())
                {
                    var appUser = sqldb.AppUsers.FirstOrDefault(user => user.ID == userId.Value);
                    if (appUser == null)
                    {
                        ModelState.AddModelError("", RadiusR.Localization.Errors.Common.UserNotFound);
                        return View(changePassword);
                    }

                    if(appUser.Password != RadiusR.DB.Passwords.PasswordUtilities.HashPassword(changePassword.OldPassword))
                    {
                        ModelState.AddModelError("", RadiusR.Localization.Errors.Common.WrongPassword);
                        return View(changePassword);
                    }

                    appUser.Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(changePassword.NewPassword);
                    sqldb.SaveChanges();

                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery);
                }
            }

            return View(changePassword);
        }

        [AllowAnonymous]
        [HttpGet]
        // GET: Auth/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Auth/ForgotPassword
        public ActionResult ForgotPassword([Bind(Include = "Email,Phone")]ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    var dbAppUser = db.AppUsers.FirstOrDefault(u => u.Email == model.Email);
                    if (dbAppUser == null || dbAppUser.Phone != model.Phone)
                    {
                        ModelState.AddModelError("General", RadiusR.Localization.Validation.Common.InvalidInput);
                    }
                    else
                    {
                        var randomNumber = RadiusR.DB.Passwords.PasswordUtilities.GenerateInternetPassword();
                        Session["change_password_token"] = randomNumber;
                        Session["change_password_email"] = model.Email;
                        SMSService SMS = new SMSService();
                        SMS.SendGenericSMS(dbAppUser.Phone, Thread.CurrentThread.CurrentCulture.Name, RadiusR.DB.Enums.SMSType.ForgotPassword, new Dictionary<string, object>()
                        {
                            {SMSParamaterRepository.SMSParameterNameCollection.SMSCode, randomNumber }
                        });
                        return RedirectToAction("ForgotPasswordConfirm");
                    }
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        // GET: Auth/ForgotPasswordConfirm
        public ActionResult ForgotPasswordConfirm()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        // POST: Auth/ForgotPasswordConfirm
        public ActionResult ForgotPasswordConfirm([Bind(Include = "SMSCode,NewPassword,Email")] ForgotPasswordConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Session["change_password_token"] == null || (string)Session["change_password_token"] != model.SMSCode)
                {
                    ModelState.AddModelError("SMSCode", RadiusR.Localization.Validation.Common.InvalidInput);
                    return View(model);
                }

                using (RadiusREntities db = new RadiusREntities())
                {
                    var dbAppUser = db.AppUsers.FirstOrDefault(u => u.Email == model.Email);
                    if (dbAppUser == null)
                    {
                        return RedirectToAction("ForgotPassword");
                    }

                    dbAppUser.Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(model.NewPassword);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }

            return View(model);
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("Index", "Home");
            }

            return returnUrl;
        }
    }
}