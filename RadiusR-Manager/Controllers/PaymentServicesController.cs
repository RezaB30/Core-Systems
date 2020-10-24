using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Payment Services")]
    public class PaymentServicesController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        // GET: PaymentServices
        public ActionResult Index(int? page)
        {
            var viewResults = db.RadiusRBillingServices.OrderBy(service => service.ID).Select(service => new RadiusRBillingViewModel()
            {
                ID = service.ID,
                Name = service.Name,
                Username = service.Username,
                HasPayments = service.ExternalPayments.Any()
            });
            SetupPages(page, ref viewResults);
            return View(viewResults);
        }

        [HttpGet]
        // GET: PaymenServices/Add
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PaymenServices/Add
        public ActionResult Add([Bind(Include = "Name,Username,Password")] RadiusRBillingViewModel paymentService)
        {
            if (ModelState.IsValid)
            {
                db.RadiusRBillingServices.Add(new RadiusRBillingService()
                {
                    Name = paymentService.Name,
                    Username = paymentService.Username,
                    Password = RadiusR.DB.Passwords.PasswordUtilities.HashLowSecurityPassword(paymentService.Password)
                });

                db.SaveChanges();
                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(paymentService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PaymentServices/Remove
        public ActionResult Remove(int id)
        {
            var paymentService = db.RadiusRBillingServices.Find(id);
            if (paymentService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }
            if (paymentService.ExternalPayments.Any())
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            db.RadiusRBillingServices.Remove(paymentService);
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpGet]
        // GET: PaymentServices/Edit
        public ActionResult Edit(int id)
        {
            var paymentService = db.RadiusRBillingServices.Find(id);
            if (paymentService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            var viewResults = new RadiusRBillingViewModel()
            {
                ID = paymentService.ID,
                Name = paymentService.Name,
                Username = paymentService.Username
            };

            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PaymentServices/Edit
        public ActionResult Edit(int id,[Bind(Include = "Name,Username")]RadiusRBillingViewModel paymentService)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var dbPaymentService = db.RadiusRBillingServices.Find(id);
                if (dbPaymentService == null)
                {
                    return RedirectToAction("Index", new { errorMessage = 23 });
                }

                dbPaymentService.Username = paymentService.Username;
                dbPaymentService.Name = paymentService.Name;

                db.SaveChanges();
                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(paymentService);
        }

        [HttpGet]
        // GET: PaymentServices/ResetPassword
        public ActionResult ResetPassword(int id)
        {
            var paymentService = db.RadiusRBillingServices.Find(id);
            if (paymentService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            var viewResults = new RadiusRBillingViewModel()
            {
                ID = paymentService.ID,
                Name = paymentService.Name,
                Username = paymentService.Username
            };

            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: PaymentServices/ResetPassword
        public ActionResult ResetPassword(int id, [Bind(Include = "Password")]RadiusRBillingViewModel paymentService)
        {
            ModelState.Remove("Name");
            ModelState.Remove("Username");
            
            var dbPaymentService = db.RadiusRBillingServices.Find(id);
            if (dbPaymentService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 23 });
            }

            if (ModelState.IsValid)
            {
                dbPaymentService.Password = RadiusR.DB.Passwords.PasswordUtilities.HashLowSecurityPassword(paymentService.Password);

                db.SaveChanges();
                return RedirectToAction("Edit", new { id = id, errorMessage = 0 });
            }

            paymentService.ID = dbPaymentService.ID;
            paymentService.Name = dbPaymentService.Name;
            return View(paymentService);
        }
    }
}