using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Radius Settings")]
    public class RadiusSettingsController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        // GET: RadiusSettings
        public ActionResult Index()
        {
            var dbRadiusSettings = db.RadiusDefaults.ToList();
            var radiusSettings = new RadiusSettingsViewModel()
            {
                AccountingInterimInterval = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "AccountingInterimInterval").Value,
                FramedProtocol = short.Parse(dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "FramedProtocol").Value),
                DailyDisconnectionTime = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "DailyDisconnectionTime").Value,
                _includeICMP = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "IncludeICMP").Value,
                NASListRefreshInterval = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "NASListRefreshInterval").Value,
                RadiusSettingsRefreshInterval = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "RadiusSettingsRefreshInterval").Value
            };
            return View(radiusSettings);
        }

        // POST: RadiusSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RadiusSettingsViewModel radiusSettings)
        {
            if (ModelState.IsValid)
            {
                var dbRadiusSettings = db.RadiusDefaults.ToList();
                var accountingInterimInterval = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "AccountingInterimInterval");
                var FramedProtocol = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "FramedProtocol");
                var DailyDisconnectionTime = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "DailyDisconnectionTime");
                var IncludeICMP = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "IncludeICMP");
                var NASListRefreshInterval = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "NASListRefreshInterval");
                var RadiusSettingsRefreshInterval = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "RadiusSettingsRefreshInterval");
                var ExpiredPoolName = dbRadiusSettings.FirstOrDefault(defs => defs.Attribute == "ExpiredPoolName");

                accountingInterimInterval.Value = radiusSettings.AccountingInterimInterval;
                FramedProtocol.Value = radiusSettings.FramedProtocol.ToString();
                DailyDisconnectionTime.Value = radiusSettings.DailyDisconnectionTime;
                IncludeICMP.Value = radiusSettings._includeICMP;
                NASListRefreshInterval.Value = radiusSettings.NASListRefreshInterval;
                RadiusSettingsRefreshInterval.Value = radiusSettings.RadiusSettingsRefreshInterval;

                db.SaveChanges();
                return RedirectToAction("Index", new { errorMessage = 0 });

            }
            return View(radiusSettings);
        }
    }
}