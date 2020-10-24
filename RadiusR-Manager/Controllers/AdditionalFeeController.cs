using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Additional Fees")]
    public class AdditionalFeeController : BaseController
    {
        RadiusREntities sqldb = new RadiusREntities();

        // GET: AdditionalFee
        public ActionResult Index()
        {
            var viewResults = sqldb.FeeTypeCosts.Select(ft => new AdditionalFeeViewModel()
            {
                FeeTypeID = ft.FeeTypeID,
                _price = ft.Cost,
                TaxTypes = ft.TaxRates.Select(rate=> new TaxRateViewModel() {
                    _rate = rate.Rate,
                    ID = rate.ID
                }),
                HasVariants = ft.HasVariants
            });
            return View(viewResults.ToList());
        }

        [HttpGet]
        // GET: AdditionalFee/Edit/id
        public ActionResult Edit(short id)
        {
            var dbFeeType = sqldb.FeeTypeCosts.Find(id);
            if (dbFeeType == null)
            {
                return RedirectToAction("Index", new { errorMessage = 6 });
            }
            if (dbFeeType.HasVariants)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var additionalFee = new AdditionalFeeViewModel()
            {
                FeeTypeID = dbFeeType.FeeTypeID,
                _price = dbFeeType.Cost
            };
            return View(additionalFee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: AdditionalFee/Edit
        public ActionResult Edit([Bind(Include = "FeeTypeID,Price")]AdditionalFeeViewModel additionalFee)
        {
            if (ModelState.IsValid)
            {
                var dbFeeType = sqldb.FeeTypeCosts.Find(additionalFee.FeeTypeID);
                if (dbFeeType == null)
                {
                    return RedirectToAction("Index", new { errorMessage = 6 });
                }
                if (dbFeeType.HasVariants)
                {
                    return RedirectToAction("Index", new { errorMessage = 9 });
                }

                dbFeeType.Cost = additionalFee._price;
                sqldb.SaveChanges();
                return RedirectToAction("Index", new { errorMessage = 0 });
            }
            return View(additionalFee);
        }
    }
}