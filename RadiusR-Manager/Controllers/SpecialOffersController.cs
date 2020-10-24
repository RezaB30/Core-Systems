using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Data.Localization;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Special Offers")]
    public class SpecialOffersController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        // GET: SpecialOffers
        public ActionResult Index(int? page)
        {
            var viewResults = db.SpecialOffers.OrderByDescending(so => so.CreationTime).Select(so => new SpecialOfferViewModel()
            {
                ID = so.ID,
                CreationTime = so.CreationTime,
                EndDate = so.EndDate,
                IsReferral = so.IsReferral,
                Name = so.Name,
                StartDate = so.StartDate,
                DiscountInfo = new RecurringDiscountViewModel()
                {
                    _amount = so.Amount,
                    ApplicationTimes = so.ApplicationTimes,
                    ApplicationType = so.ApplicationType,
                    DiscountType = so.DiscountType,
                    FeeTypeID = so.FeeTypeID,
                    OnlyFullInvoice = so.OnlyFullInvoice
                }
            });

            SetupPages(page, ref viewResults);

            return View(viewResults.ToArray());
        }

        [HttpGet]
        // GET: SpecialOffers/Add
        public ActionResult Add()
        {
            var specialOffer = new SpecialOfferViewModel() { DiscountInfo = new RecurringDiscountViewModel() };
            ViewBag.FeeTypeList = new SelectList(new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetList().Select(li => new { Key = (short)li.Key, Value = li.Value }).Where(li => li.Key == (short)FeeType.Tariff).ToArray(), "Key", "Value");

            return View(specialOffer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SpecialOffers/Add
        public ActionResult Add(SpecialOfferViewModel specialOffer)
        {
            ClientController.FixRecurringDiscountModelState(ModelState, "DiscountInfo.", specialOffer.DiscountInfo);

            if (specialOffer.StartDate >= specialOffer.EndDate)
                ModelState.AddModelError("StartDate", RadiusR.Localization.Validation.ModelSpecific.StartDateCannotBeGreaterThanEndDate);

            if (ModelState.IsValid)
            {
                db.SpecialOffers.Add(new SpecialOffer()
                {
                    Amount = specialOffer.DiscountInfo._amount.Value,
                    ApplicationTimes = specialOffer.DiscountInfo.ApplicationTimes.Value,
                    ApplicationType = specialOffer.DiscountInfo.ApplicationType,
                    CreationTime = DateTime.Now,
                    DiscountType = specialOffer.DiscountInfo.DiscountType,
                    EndDate = specialOffer.EndDate.Value,
                    FeeTypeID = specialOffer.DiscountInfo.FeeTypeID,
                    IsReferral = specialOffer.IsReferral,
                    Name = specialOffer.Name,
                    OnlyFullInvoice = specialOffer.DiscountInfo.OnlyFullInvoice,
                    StartDate = specialOffer.StartDate.Value
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.FeeTypeList = new SelectList(new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetList().Select(li => new { Key = (short)li.Key, Value = li.Value }).Where(li => li.Key == (short)FeeType.Tariff).ToArray(), "Key", "Value", specialOffer.DiscountInfo.FeeTypeID);
            return View(specialOffer);
        }

        [HttpGet]
        // GET: SpecialOffer/Edit
        public ActionResult Edit(int id)
        {
            var dbSpecialOffer = db.SpecialOffers.Find(id);
            if (dbSpecialOffer == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var specialOffer = new SpecialOfferViewModel()
            {
                DiscountInfo = new RecurringDiscountViewModel()
                {
                    _amount = dbSpecialOffer.Amount,
                    ApplicationTimes = dbSpecialOffer.ApplicationTimes,
                    ApplicationType = dbSpecialOffer.ApplicationType,
                    DiscountType = dbSpecialOffer.DiscountType,
                    FeeTypeID = dbSpecialOffer.FeeTypeID,
                    OnlyFullInvoice = dbSpecialOffer.OnlyFullInvoice
                },
                EndDate = dbSpecialOffer.EndDate,
                ID = dbSpecialOffer.ID,
                IsReferral = dbSpecialOffer.IsReferral,
                Name = dbSpecialOffer.Name,
                StartDate = dbSpecialOffer.StartDate
            };

            ViewBag.FeeTypeList = new SelectList(new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetList().Select(li => new { Key = (short)li.Key, Value = li.Value }).Where(li => li.Key == (short)FeeType.Tariff).ToArray(), "Key", "Value", specialOffer.DiscountInfo.FeeTypeID);

            return View(viewName: "Add", model: specialOffer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SpecialOffers/Edit
        public ActionResult Edit(int id, SpecialOfferViewModel specialOffer)
        {
            ClientController.FixRecurringDiscountModelState(ModelState, "DiscountInfo.", specialOffer.DiscountInfo);

            if (specialOffer.StartDate >= specialOffer.EndDate)
                ModelState.AddModelError("StartDate", RadiusR.Localization.Validation.ModelSpecific.StartDateCannotBeGreaterThanEndDate);

            if (ModelState.IsValid)
            {
                var dbSpecialOffer = db.SpecialOffers.Find(id);
                if (dbSpecialOffer == null)
                {
                    return RedirectToAction("Index", new { errorMessage = 9 });
                }

                dbSpecialOffer.Amount = specialOffer.DiscountInfo._amount.Value;
                dbSpecialOffer.ApplicationTimes = specialOffer.DiscountInfo.ApplicationTimes.Value;
                dbSpecialOffer.ApplicationType = specialOffer.DiscountInfo.ApplicationType;
                dbSpecialOffer.CreationTime = DateTime.Now;
                dbSpecialOffer.DiscountType = specialOffer.DiscountInfo.DiscountType;
                dbSpecialOffer.EndDate = specialOffer.EndDate.Value;
                dbSpecialOffer.FeeTypeID = specialOffer.DiscountInfo.FeeTypeID;
                dbSpecialOffer.IsReferral = specialOffer.IsReferral;
                dbSpecialOffer.Name = specialOffer.Name;
                dbSpecialOffer.OnlyFullInvoice = specialOffer.DiscountInfo.OnlyFullInvoice;
                dbSpecialOffer.StartDate = specialOffer.StartDate.Value;

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            ViewBag.FeeTypeList = new SelectList(new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetList().Select(li => new { Key = (short)li.Key, Value = li.Value }).Where(li => li.Key == (short)FeeType.Tariff).ToArray(), "Key", "Value", specialOffer.DiscountInfo.FeeTypeID);
            return View(viewName: "Add", model: specialOffer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SpecialOffers/Remove
        public ActionResult Remove(int id)
        {
            var dbSpecialOffer = db.SpecialOffers.Find(id);
            if (dbSpecialOffer == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            db.SpecialOffers.Remove(dbSpecialOffer);
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }
    }
}