using RadiusR.Address;
using RadiusR.DB.Enums;
using RadiusR.DB.Settings;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public class AddressController : BaseController
    {
        [AuthorizePermission(Permissions = "Address Settings")]
        [HttpGet]
        // GET: Address/Settings
        public ActionResult Settings()
        {
            var results = new AddressSettingsViewModel()
            {
                AddressAPIType = AddressAPISettings.AddressAPIType,
                AddressAPIPassword = AddressAPISettings.AddressAPIPassword,
                AddressAPIUsername = AddressAPISettings.AddressAPIUsername,
                AddressAPIDirectPassword = AddressAPISettings.AddressAPIDirectPassword,
                AddressAPIDirectUserId = AddressAPISettings.AddressAPIDirectUserId.ToString()
            };

            return View(results);
        }

        [AuthorizePermission(Permissions = "Address Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Address/Settings
        public ActionResult Settings(AddressSettingsViewModel settings)
        {
            if(settings.AddressAPIType == (short)AddressAPIType.AddressQueryService)
            {
                settings.AddressAPIDirectUserId = "0";
                settings.AddressAPIDirectPassword = "-";

                ModelState.Remove("AddressAPIDirectUserId");
                ModelState.Remove("AddressAPIDirectPassword");
            }
            if (settings.AddressAPIType == (short)AddressAPIType.DirectAccess)
            {
                settings.AddressAPIUsername = "-";
                settings.AddressAPIPassword = "-";

                ModelState.Remove("AddressAPIUsername");
                ModelState.Remove("AddressAPIPassword");
            }

            if (ModelState.IsValid)
            {
                AddressAPISettings.Update(settings);
                AddressAPISettings.ClearCache();
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetProvinces
        public ActionResult GetPrivinces()
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetProvinces();

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetProvinceDistricts
        public ActionResult GetProvinceDistricts(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetProvinceDistricts(id);

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetDistrictRuralRegions
        public ActionResult GetDistrictRuralRegions(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetDistrictRuralRegions(id);

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetDistrictRuralRegions
        public ActionResult GetRuralRegionNeighbourhoods(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetRuralRegionNeighbourhoods(id);

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetDistrictRuralRegions
        public ActionResult GetNeighbourhoodStreets(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetNeighbourhoodStreets(id);

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetDistrictRuralRegions
        public ActionResult GetStreetBuildings(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetStreetBuildings(id);

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetDistrictRuralRegions
        public ActionResult GetBuildingApartments(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetBuildingApartments(id);

            return Json(addressResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: Address/GetDistrictRuralRegions
        public ActionResult GetApartmentAddress(long id)
        {
            var addressManager = new AddressManager();

            var addressResults = addressManager.GetApartmentAddress(id);

            return Json(addressResults);
        }
    }
}