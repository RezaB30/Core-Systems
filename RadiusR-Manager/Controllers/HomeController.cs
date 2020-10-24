using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB.Enums;

namespace RadiusR_Manager.Controllers
{
    public class HomeController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [HttpGet]
        // GET: Home
        public ActionResult Index()
        {
            if (User.IsInRole("cashier"))
            {
                return View("Cashier");
            }
            ViewBag.UnfinishedSupportRequestCount = db.SubscriptionSupportRequests.Count(request => request.StateID == (short)SupportRequestStateID.Sent);
            ViewBag.UnfinishedWorkOrderCount = db.CustomerSetupTasks.Count(task => !task.CompletionDate.HasValue);
            return View();
        }

        [Authorize(Roles = "cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Home
        public ActionResult Index([Bind(Include = "")]CashierClientSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RadiusREntities db = new RadiusREntities())
                {
                    var query = db.Subscriptions.AsQueryable();
                    if (!string.IsNullOrEmpty(model.SubscriberNo))
                    {
                        query = query.Where(client => client.SubscriberNo == model.SubscriberNo);
                    }
                    if (!string.IsNullOrEmpty(model.PhoneNo))
                    {
                        query = query.Where(client => client.Customer.ContactPhoneNo == model.PhoneNo);
                    }
                    //if (!string.IsNullOrEmpty(model.TCKNo))
                    //{
                    //    query = query.Where(client => client.TCNo == model.TCKNo);
                    //}
                    var dbClient = query.FirstOrDefault();
                    if (dbClient != null)
                    {
                        if (!dbClient.HasBilling)
                        {
                            return RedirectToAction("ExtendPackage", "Client", new { id = dbClient.ID });
                        }
                        else
                        {
                            return RedirectToAction("ClientPayment", "Bill", new { id = dbClient.ID });
                        }
                    }
                    ModelState.AddModelError("results", RadiusR.Localization.Pages.ErrorMessages._4);
                    return View("Cashier", model);
                }
            }
            return View("Cashier", model);
        }
    }
}