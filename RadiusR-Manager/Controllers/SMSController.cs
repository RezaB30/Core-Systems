using RadiusR.DB;
using RadiusR.SMS;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RadiusR_Manager.Models.Extentions;
using System.Web.Routing;
using System.Text.RegularExpressions;
using RadiusR.DB.ModelExtentions;
using RezaB.Web.CustomAttributes;
using RezaB.Web;

namespace RadiusR_Manager.Controllers
{
    public class SMSController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        Regex SMSParameterRegex = new Regex(@"^\(\[(.+)\]\)$");

        [AuthorizePermission(Permissions = "SMS Texts")]
        // GET: SMS
        public ActionResult Index(string id = "tr-tr")
        {
            var viewResults = db.SMSTexts.Where(sms => sms.Culture.ToLower() == id.ToLower()).OrderBy(sms => sms.TypeID).Select(sms => new SMSViewModel()
            {
                Culture = sms.Culture,
                Type = sms.TypeID,
                Text = sms.Text,
                IsActive = !sms.IsDisabled
            }).ToList();

            var rm = new ResourceManager(typeof(RadiusR.Localization.Pages.SMSHints));
            viewResults.ForEach(sms => sms.ValidParameters = SMSParamaterRepository.GetValidSMSParameters((RadiusR.DB.Enums.SMSType)sms.Type).Select(par => new SMSParameterViewModel() { Name = par.Name, DisplayName = rm.GetString(SMSParameterRegex.Matches(par.Name)[0].Groups[1].Value) }));

            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "SMS Texts")]
        // POST: SMS
        public ActionResult Index([Bind(Prefix = "item", Include = "Type,Culture,Text,IsActive")] IEnumerable<SMSViewModel> SMS, string id = "tr-tr")
        {
            if (ModelState.IsValid)
            {
                var dbSms = db.SMSTexts.ToList();
                foreach (var item in SMS)
                {
                    var current = dbSms.FirstOrDefault(sms => sms.TypeID == item.Type && sms.Culture == id);
                    current.Text = item.Text;
                    var currentType = dbSms.Where(sms => sms.TypeID == item.Type).ToArray();
                    foreach (var typeItem in currentType)
                    {
                        typeItem.IsDisabled = !item.IsActive;
                    }
                    //db.Entry(current).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();
                
                return RedirectToAction("Index", new { id = id, errorMessage = 0 });
            }

            var rm = new ResourceManager(typeof(RadiusR.Localization.Pages.SMSHints));
            SMS.ToList().ForEach(sms => sms.ValidParameters = SMSParamaterRepository.GetValidSMSParameters((RadiusR.DB.Enums.SMSType)sms.Type).Select(par => new SMSParameterViewModel() { Name = par.Name, DisplayName = rm.GetString(SMSParameterRegex.Matches(par.Name)[0].Groups[1].Value) }));

            return View(SMS);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "SMS")]
        // GET: SMS/Send
        public ActionResult Send(CustomerSearchViewModel search)
        {
            // clear search address validations
            {
                var names = ModelState.Where(s => s.Key.StartsWith("Address.")).Select(s => s.Key).ToArray();
                foreach (var name in names)
                {
                    ModelState.Remove(name);
                }
            }

            ViewBag.SelectedClientsCount = db.Subscriptions.AsQueryable().FilterBySearchViewModel(search, db, User).Count();
            ViewBag.Services = new SelectList(db.Services.ToArray(), "Name", "Name", search.ServiceName);
            ViewBag.Groups = new SelectList(db.Groups.ToArray(), "ID", "Name", search.GroupID);

            var rm = new ResourceManager(typeof(RadiusR.Localization.Pages.SMSHints));
            var sms = new GroupSMSViewModel()
            {
                Text = "",
                ValidParameters = SMSParamaterRepository.GetValidSMSParameters(null).Select(par => new SMSParameterViewModel() { Name = par.Name, DisplayName = rm.GetString(SMSParameterRegex.Matches(par.Name)[0].Groups[1].Value) })
            };
            ViewBag.SearchModel = search;
            return View(sms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Batch SMS")]
        // POST: SMS/Send
        public ActionResult Send(CustomerSearchViewModel search, [Bind(Include = "GroupID,ServiceID,Text")]GroupSMSViewModel sms)
        {
            // clear model state
            {
                var keys = ModelState.AsEnumerable().Select(item => item.Key).ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] != "Text")
                    {
                        ModelState.Remove(keys[i]);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                var clients = db.Subscriptions.PrepareForSMS().OrderBy(client => client.ID).AsQueryable().FilterBySearchViewModel(search, db, User);
                var totalClientCount = clients.Count();
                var sentCount = 0;
                var batchSize = 500;
                SMSService SMS = new SMSService();
                while (sentCount < totalClientCount)
                {
                    var currentBatch = clients.Skip(sentCount).Take(batchSize).ToList();
                    var sentSMSes = new List<SMSArchive>();
                    foreach (var client in currentBatch)
                    {
                        sentSMSes.Add(SMS.SendSubscriberSMS(client, rawText: sms.Text));
                    }
                    using (RadiusREntities smsTemp = new RadiusREntities())
                    {
                        smsTemp.SMSArchives.AddRangeSafely(sentSMSes);
                        smsTemp.SaveChanges();
                    }
                    sentCount += batchSize;
                }

                UriBuilder uri = new UriBuilder(Url.Action("Index", "Client", search, Request.Url.Scheme));
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery);
            }

            ViewBag.SelectedClientsCount = db.Subscriptions.AsQueryable().FilterBySearchViewModel(search, db, User).Count();
            ViewBag.Services = new SelectList(db.Services.ToArray(), "Name", "Name", search.ServiceName);
            ViewBag.Groups = new SelectList(db.Groups.ToArray(), "ID", "Name", search.GroupID);

            var rm = new ResourceManager(typeof(RadiusR.Localization.Pages.SMSHints));
            sms.ValidParameters = SMSParamaterRepository.GetValidSMSParameters(null).Select(par => new SMSParameterViewModel() { Name = par.Name, DisplayName = rm.GetString(SMSParameterRegex.Matches(par.Name)[0].Groups[1].Value) });
            ViewBag.SearchModel = search;
            return View(sms);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Clients")]
        // GET: SMS/ClientSMS
        public ActionResult ClientSMS(int? page, long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._4);
            }

            var viewResult = db.SMSArchives.Where(sms => sms.SubscriptionID == dbSubscription.ID).OrderByDescending(sms => sms.Date).Select(sms => new SMSArchiveViewModel()
            {
                ID = sms.ID,
                Date = sms.Date,
                Text = sms.Text,
                Type = sms.SMSTypeID
            });

            SetupPages(page, ref viewResult);
            ViewBag.ClientSMS = new ClientSMSViewModel()
            {
                ClientID = id
            };
            return View(viewResult.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Clients")]
        // POST: SMS/ClientSMS
        public ActionResult ClientSMS(int? page, long id, [Bind(Include = "ClientID,SMSText", Prefix = "SendSMSModel")]ClientSMSViewModel SMS)
        {
            var dbClient = db.Subscriptions.Find(id);
            if (dbClient == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._4);
            }

            var viewResult = dbClient.SMSArchives.OrderByDescending(sms => sms.Date).Select(sms => new SMSArchiveViewModel()
            {
                ID = sms.ID,
                Date = sms.Date,
                Text = sms.Text
            }).AsQueryable();

            SetupPages(page, ref viewResult);

            if (ModelState.IsValid)
            {
                SMSService SMSSrv = new SMSService();
                db.SMSArchives.AddSafely(SMSSrv.SendSubscriberSMS(dbClient, rawText: SMS.SMSText));
                db.SaveChanges();

                return RedirectToAction("ClientSMS", new { errorMessage = 0 });
            }

            ViewBag.ClientSMS = SMS;
            return View(viewResult.ToList());
        }

        // GET: SMS/Settings
        [AuthorizePermission(Permissions = "SMS Settings")]
        public ActionResult Settings()
        {
            var settings = SMSService.GetSettings();
            var viewResults = new SMSSettingsViewModel()
            {
                CachingLength = settings.CachingLength.ToString(),
                ServiceUsername = settings.ServiceUsername,
                ServicePassword = settings.ServicePassword,
                ServiceTitle = settings.ServiceTitle,
                IsActive = settings.IsActive,
                APIType = settings.APIType
            };
            return View(viewResults);
        }

        // POST: SMS/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "SMS Settings")]
        public ActionResult Settings([Bind(Include = "CachingLength,ServiceUsername,ServicePassword,ServiceTitle,IsActive,APIType")]SMSSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                settings.UpdateDatabase();
                SMSService.ReloadSettings();
                return RedirectToAction("Settings", "SMS", new { errorMessage = 0 });
            }

            return View(settings);
        }
    }
}