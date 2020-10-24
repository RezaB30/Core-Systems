using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "VPOS Settings")]
    public class VPOSSettingsController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [HttpGet]
        // GET: VPOSSettings
        public ActionResult Index(int? page)
        {
            var selectedVPOS = db.AppSettings.FirstOrDefault(setting => setting.Key == "SelectedVPOS");
            var selectedVPOSID = selectedVPOS != null ? Convert.ToInt32(selectedVPOS.Value) : (int?)null;
            var results = db.VPOSLists.OrderBy(vpos => vpos.ID).Select(vpos => new VPOSSettingsViewModel()
            {
                ID = vpos.ID,
                MerchantID = vpos.MerchantID,
                StoreKey = vpos.StoreKey,
                UserID = vpos.UserID,
                Password = vpos.UserPass,
                VPOSType = vpos.VPOSTypeID,
                IsSelected = vpos.ID == selectedVPOSID
            });
            SetupPages(page, ref results);

            return View(results.ToArray());
        }

        [HttpGet]
        // GET: VPOSSettings/Add
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: VPOSSettings/Add
        public ActionResult Add(VPOSSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                db.VPOSLists.Add(new VPOSList()
                {
                    VPOSTypeID = settings.VPOSType.Value,
                    MerchantID = settings.MerchantID,
                    StoreKey = settings.StoreKey,
                    UserID = settings.UserID,
                    UserPass = settings.Password
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(settings);
        }

        [HttpGet]
        // GET: VPOSSettings/Edit
        public ActionResult Edit(int id)
        {
            var dbSettings = db.VPOSLists.Find(id);
            if (dbSettings == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var settings = new VPOSSettingsViewModel()
            {
                ID = dbSettings.ID,
                MerchantID = dbSettings.MerchantID,
                StoreKey = dbSettings.StoreKey,
                VPOSType = dbSettings.VPOSTypeID,
                UserID = dbSettings.UserID,
                Password = dbSettings.UserPass
            };

            ViewBag.IsEdit = true;
            return View(viewName: "Add", model: settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: VPOSSettings/Edit
        public ActionResult Edit(int id, VPOSSettingsViewModel settings)
        {
            var dbSettings = db.VPOSLists.Find(id);
            if (dbSettings == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbSettings.VPOSTypeID = settings.VPOSType.Value;
                dbSettings.MerchantID = settings.MerchantID;
                dbSettings.StoreKey = settings.StoreKey;
                dbSettings.UserID = settings.UserID;
                dbSettings.UserPass = settings.Password;

                db.SaveChanges();

                VPOSSettings.RefreshCache();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.IsEdit = true;
            return View(viewName: "Add", model: settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: VPOSSettings/Remove
        public ActionResult Remove(int id)
        {
            var dbSettings = db.VPOSLists.Find(id);
            if (dbSettings == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var selectedVPOS = db.AppSettings.FirstOrDefault(setting => setting.Key == "SelectedVPOS");
            var selectedVPOSID = selectedVPOS != null ? Convert.ToInt32(selectedVPOS.Value) : (int?)null;
            if (dbSettings.ID == selectedVPOSID)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            db.VPOSLists.Remove(dbSettings);

            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: VPOSSettings/Select
        public ActionResult Select(int id)
        {
            var dbSettings = db.VPOSLists.Find(id);
            if (dbSettings == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var selectedVPOS = db.AppSettings.FirstOrDefault(setting => setting.Key == "SelectedVPOS");
            selectedVPOS.Value = dbSettings.ID.ToString();

            db.SaveChanges();

            VPOSSettings.RefreshCache();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }
    }
}