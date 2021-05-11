using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR_Manager.Models.ViewModels.PartnerRegisters;
using RadiusR.DB;
using System.Data.Entity;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        [AuthorizePermission(Permissions = "Partner Registers")]
        [HttpGet]
        // GET: Client/PartnerRegisters
        public ActionResult PartnerRegisters(int? page, PartnerRegistersSearchViewModel search)
        {
            var baseQuery = db.Subscriptions
                .Include(s => s.Customer.CorporateCustomerInfo)
                .Include(s => s.PartnerRegisteredSubscription.Partner)
                .Include(s => s.RadiusAuthorization)
                .OrderByDescending(s => s.MembershipDate).Where(s => s.PartnerRegisteredSubscription != null);
            if (search != null)
            {
                if (search.PartnerID.HasValue)
                {
                    baseQuery = baseQuery.Where(s => s.PartnerRegisteredSubscription.PartnerID == search.PartnerID);
                }
                if (search.RegistrationStartDate.HasValue)
                {
                    baseQuery = baseQuery.Where(s => s.MembershipDate >= search.RegistrationStartDate);
                }
                if (search.RegistrationEndDate.HasValue)
                {
                    baseQuery = baseQuery.Where(s => s.MembershipDate <= search.RegistrationEndDate);
                }
                if (search.State.HasValue)
                {
                    baseQuery = baseQuery.Where(s => s.State == search.State);
                }
                if (!string.IsNullOrWhiteSpace(search.SubscriberNo))
                {
                    baseQuery = baseQuery.Where(s => s.SubscriberNo.Contains(search.SubscriberNo));
                }
            }

            SetupPages(page, ref baseQuery);

            var viewResults = baseQuery.ToArray().Select(s => new PartnerRegisteredSubscriptionViewModel()
            {
                CustomerName = s.ValidDisplayName,
                PartnerName = s.PartnerRegisteredSubscription.Partner.Title,
                RegistrationDate = s.MembershipDate,
                State = s.State,
                SubscriberNo = s.SubscriberNo,
                SubscriptionID = s.ID,
                Username = s.RadiusAuthorization.Username
            });

            ViewBag.Partners = new SelectList(db.Partners.OrderBy(p => p.Title).Select(p => new { Name = p.Title, Value = p.ID }), "Value", "Name", search.PartnerID);
            return View(viewName: "PartnerRegisters/PartnerRegisters", model: viewResults);
        }
    }
}