using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.CSVModels;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;
using RezaB.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Taxes")]
    public class TaxController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [HttpGet]
        // GET: Tax/StampDuty
        public ActionResult StampDuty(int? page, [Bind(Prefix = "search")] StampDutySearchViewModel search)
        {
            var results = Enumerable.Empty<SubscriptionListDisplayViewModel>().AsQueryable();
            search = search ?? new StampDutySearchViewModel();
            if (search.Date != null && search.Date.IsValid)
            {
                results = db.Subscriptions.Where(client => client.MembershipDate.Month == search.Date.Month && client.MembershipDate.Year == search.Date.Year)
                    .OrderBy(client => client.MembershipDate)
                    .Select(client => new SubscriptionListDisplayViewModel()
                    {
                        Name = client.Customer.CorporateCustomerInfo != null ? client.Customer.CorporateCustomerInfo.Title : client.Customer.FirstName + " " + client.Customer.LastName,
                        RegistrationDate = client.MembershipDate,
                        SubscriberNo = client.SubscriberNo,
                        _servicePrice = client.Service.Price
                    });
            }

            SetupPages(page, ref results);

            ViewBag.Search = search;
            return View(results);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        //[ActionName("StampDuty")]
        // POST: Tax/StampDuty
        public ActionResult StampDuty([Bind(Prefix = "search")] StampDutySearchViewModel search)
        {
            var results = Enumerable.Empty<MonthlyClientCSVModel>();
            search = search ?? new StampDutySearchViewModel();
            if (search.Date != null && search.Date.IsValid)
            {
                results = db.Subscriptions.Where(client => client.MembershipDate.Month == search.Date.Month && client.MembershipDate.Year == search.Date.Year)
                    .Select(client => new SubscriptionListDisplayViewModel()
                    {
                        Name = client.Customer.CorporateCustomerInfo != null ? client.Customer.CorporateCustomerInfo.Title : client.Customer.FirstName + " " + client.Customer.LastName,
                        RegistrationDate = client.MembershipDate,
                        SubscriberNo = client.SubscriberNo,
                        _servicePrice = client.Service.Price
                    }).OrderBy(client => client.RegistrationDate).ToList().Select(client => new MonthlyClientCSVModel()
                    {
                        ValidDisplayName = client.Name,
                        MembershipDate = client.RegistrationDate.Value.ToShortDateString(),
                        ServicePrice = client.ServicePrice,
                        SubscriptionNo = client.SubscriberNo
                    }).ToList();
            }
            else
            {
                return RedirectToAction("StampDuty", new RouteValueDictionary { { "search.Date", search.Date.ToString() }, { "errorMessage", 9 } });
            }

            return File(CSVGenerator.GetStream(results, "\t"), @"text/csv", RadiusR.Localization.Pages.Common.StampDuty + "_" + search.Date.Year + "_" + search.Date.MonthName + ".csv");
        }

        [HttpGet]
        // GET: Tax/TaxRates
        public ActionResult TaxRates()
        {
            var viewResults = db.TaxRates.Select(rate => new TaxRateViewModel()
            {
                ID = rate.ID,
                _rate = rate.Rate
            });

            return View(viewResults);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Tax/TaxRates
        public ActionResult TaxRates([Bind(Include = "ID,Rate")] IEnumerable<TaxRateViewModel> rates)
        {
            if (ModelState.IsValid)
            {
                var dbList = db.TaxRates.ToList();
                if (rates.Select(r => r.ID).Except(dbList.Select(r => r.ID)).Any())
                {
                    return RedirectToAction("TaxRates", new { errorMessage = 9 });
                }

                foreach (var taxRate in rates)
                {
                    dbList.FirstOrDefault(tr => tr.ID == taxRate.ID).Rate = taxRate._rate;
                }

                db.SaveChanges();
                return RedirectToAction("TaxRates", new { errorMessage = 0 });
            }

            return View(rates);
        }
    }
}