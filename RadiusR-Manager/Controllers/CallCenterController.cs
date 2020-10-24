using RadiusR.DB;
using RadiusR.Verimor;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Routing;

namespace RadiusR_Manager.Controllers
{
    public class CallCenterController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AllowAnonymous]
        [HttpPost]
        // POST: CallCenter/EventListener
        public ActionResult EventListener(VerimorEvent verimorEvent)
        {
            if (verimorEvent.direction == "inbound" && verimorEvent.event_type == "ringing" && !string.IsNullOrEmpty(verimorEvent.caller_id_number))
            {
                var cacheObject = new EventCacheObject()
                {
                    RawEvent = verimorEvent
                };

                cacheObject.PhoneNo = verimorEvent.caller_id_number.StartsWith("0") ? verimorEvent.caller_id_number.Substring(1) : verimorEvent.caller_id_number.StartsWith(AppSettings.CountryPhoneCode) ? verimorEvent.caller_id_number.Substring(AppSettings.CountryPhoneCode.Length) : verimorEvent.caller_id_number;
                var subIDs = db.Customers.Where(customer => customer.ContactPhoneNo == cacheObject.PhoneNo).SelectMany(customer => customer.Subscriptions.Select(subscription => subscription.ID)).ToArray();
                cacheObject.SubscriptionIDs = subIDs;
                if (subIDs.Count() == 1)
                {
                    cacheObject.DisplayName = db.Subscriptions.Find(subIDs.FirstOrDefault()).ValidDisplayName;
                }
                else if(subIDs.Count() > 0)
                {
                    cacheObject.DisplayName = db.Subscriptions.Find(subIDs.FirstOrDefault()).ValidDisplayName;
                }

                cacheObject.LinkText = RadiusR.Localization.Pages.Common.Show;

                EventCache.SaveEvent(cacheObject);
            }

            return new EmptyResult();
        }

        [HttpPost]
        // POST: CallCenter/GetEvents
        public ActionResult GetEvents(string lastUUID, string lastEventType)
        {
            if (User.HasInternalNo())
            {
                var cacheObject = EventCache.RetrieveEvent(User.GiveInternalNo(), lastUUID, lastEventType);
                if (cacheObject != null)
                {
                    cacheObject.Message = string.Format(RadiusR.Localization.Pages.Common.IncomingCallMessage, cacheObject.DisplayName ?? cacheObject.PhoneNo);
                    if (cacheObject.SubscriptionIDs.Count() == 1)
                    {
                        cacheObject.Url = Url.Action("Details", "Client", new { id = cacheObject.SubscriptionIDs.FirstOrDefault() });
                    }
                    else if (cacheObject.SubscriptionIDs.Count() > 0)
                    {
                        cacheObject.Url = Url.Action("Index", "Client", new { Phone = cacheObject.PhoneNo });
                    }
                }
                return cacheObject != null ? Json(cacheObject) : Json(new { });
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        // GET: CallCenter/WebPhone
        public ActionResult WebPhone()
        {
            if (User.HasInternalNo())
            {
                VerimorClient client = new VerimorClient();
                var response = client.GetWebPhoneToken(CallCenterSettings.CallCenterAPIKey, User.GiveInternalNo());
                if (response.IsSuccess)
                {
                    return Redirect("https://oim.verimor.com.tr/webphone?token=" + response.Data);
                }
                else
                {
                    return Content(response.Data);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [AuthorizePermission(Permissions = "Call Center Settings")]
        [HttpGet]
        // GET: CallCenter/Settings
        public ActionResult Settings()
        {
            var results = new CallCenterSettingsViewModel(true);

            return View(results);
        }

        [AuthorizePermission(Permissions = "Call Center Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: CallCenter/Settings
        public ActionResult Settings(CallCenterSettingsViewModel settings)
        {
            settings.InternalIDList = settings.InternalIDList ?? Enumerable.Empty<string>();
            if (settings.InternalIDList.Any(s => s.Length > 10))
                ModelState.AddModelError("InternalIDList", RadiusR.Localization.Validation.Common.InvalidInternalIDs);
            if (ModelState.IsValid)
            {
                CallCenterSettings.Update(settings);
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        }

        [HttpPost]
        // GET: CallCenter/CallNumber
        public ActionResult CallNumber(string number)
        {
            if (User.HasInternalNo())
            {
                VerimorClient client = new VerimorClient();
                var response = client.CallNumber(CallCenterSettings.CallCenterAPIKey, User.GiveInternalNo(), number);

                return Json(new { success = response.IsSuccess });
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
    }
}