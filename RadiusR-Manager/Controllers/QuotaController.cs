using RadiusR.DB;
using RadiusR.DB.Settings;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public class QuotaController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [HttpGet]
        [AuthorizePermission(Permissions = "Quota Settings")]
        // GET: Quota/Settings
        public ActionResult Settings()
        {
            return View(new QuotaSettingsViewModel(true));
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Quota Settings")]
        // POST: Quota/Settings
        public ActionResult Settings(QuotaSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                QuotaSettings.Update(settings);

                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        }

        [AuthorizePermission(Permissions = "Quota Settings")]
        // GET: Qouta/Packages
        public ActionResult Packages(int? page)
        {

            var packages = db.QuotaPackages.OrderByDescending(package => package.ID).Select(package => new QuotaPackageViewModel()
            {
                ID = package.ID,
                Name = package.Name,
                _amount = package.Amount,
                _price = package.Price
            });

            SetupPages(page, ref packages);

            return View(packages.ToList());
        }

        [AuthorizePermission(Permissions = "Quota Settings")]
        [HttpGet]
        // GET: Quota/AddPackage
        public ActionResult AddPackage()
        {
            return View();
        }

        [AuthorizePermission(Permissions = "Quota Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Quota/AddPackage
        public ActionResult AddPackage(QuotaPackageViewModel package)
        {
            if (ModelState.IsValid)
            {
                db.QuotaPackages.Add(new QuotaPackage()
                {
                    Amount = package._amount.Value,
                    Name = package.Name,
                    Price = package._price.Value
                });

                db.SaveChanges();

                return RedirectToAction("Packages", new { errorMessage = 0 });
            }
            return View(package);
        }

        [AuthorizePermission(Permissions = "Quota Settings")]
        [HttpGet]
        // GET: Quota/EditPackage
        public ActionResult EditPackage(int id)
        {
            var package = db.QuotaPackages.Find(id);
            if (package == null)
            {
                return RedirectToAction("Packages", new { errorMessage = 9 });
            }

            return View(viewName: "AddPackage", model: new QuotaPackageViewModel()
            {
                ID = package.ID,
                Name = package.Name,
                _amount = package.Amount,
                _price = package.Price
            });
        }

        [AuthorizePermission(Permissions = "Quota Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Quota/EditPackage
        public ActionResult EditPackage(int id, QuotaPackageViewModel package)
        {
            var dbPackage = db.QuotaPackages.Find(id);
            if (dbPackage == null)
            {
                return RedirectToAction("Packages", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbPackage.Amount = package._amount.Value;
                dbPackage.Name = package.Name;
                dbPackage.Price = package._price.Value;

                db.SaveChanges();
                return RedirectToAction("Packages", new { errorMessage = 0 });
            }

            return View(viewName: "AddPackage", model: package);
        }

        [AuthorizePermission(Permissions = "Quota Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Quota/RemovePackage
        public ActionResult RemovePackage(int id)
        {
            var package = db.QuotaPackages.Find(id);
            if (package == null)
            {
                return RedirectToAction("Packages", new { errorMessage = 9 });
            }

            db.QuotaPackages.Remove(package);
            db.SaveChanges();
            return RedirectToAction("Packages", new { errorMessage = 0 });
        }
    }
}