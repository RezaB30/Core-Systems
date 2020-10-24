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
    public class FeeTypeVariantController : BaseController
    {
        RadiusREntities sqldb = new RadiusREntities();

        [HttpGet]
        // GET: FeeTypeVariant/Index/id
        public ActionResult Index(short id)
        {
            var dbFeeType = sqldb.FeeTypeCosts.Find(id);
            if (!dbFeeType.HasVariants)
            {
                return RedirectToAction("Index", "AdditionalFee", new { errorMessage = 9 });
            }
            var viewResults = sqldb.FeeTypeVariants.OrderBy(variant => variant.ID).Where(variant => variant.FeeTypeID == id).Select(variant => new FeeTypeVariantViewModel()
            {
                ID = variant.ID,
                Title = variant.Title,
                FeeTypeID = variant.FeeTypeID,
                _price = variant.Price,
                FeeType = new AdditionalFeeViewModel()
                {
                    FeeTypeID = variant.FeeTypeCost.FeeTypeID,
                    IsAllTime = variant.FeeTypeCost.IsAllTime,
                    TaxTypes = variant.FeeTypeCost.TaxRates.Select(rate => new TaxRateViewModel()
                    {
                        ID = rate.ID,
                        _rate = rate.Rate
                    }),
                    _price = variant.FeeTypeCost.Cost,
                }
            });

            ViewBag.FeeTypeID = id;
            return View(viewResults.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: FeeTypeVariant/Index/id
        public ActionResult Index(short id, [Bind(Include = "ID,Title,Price")]IEnumerable<FeeTypeVariantViewModel> variants)
        {
            var dbFeeType = sqldb.FeeTypeCosts.Find(id);
            if (!dbFeeType.HasVariants)
            {
                return RedirectToAction("Index", "AdditionalFee", new { errorMessage = 9 });
            }

            variants = variants ?? Enumerable.Empty<FeeTypeVariantViewModel>();
            var variantsArray = variants.ToList();
            if (ModelState.IsValid)
            {
                variantsArray.ForEach(variant => variant.FeeTypeID = id);

                var dbFeeTypeVariants = sqldb.FeeTypeVariants.Where(variant => variant.FeeTypeID == id).ToList();

                foreach (var item in variantsArray)
                {
                    // edit existing
                    if (item.ID.HasValue)
                    {
                        var dbItem = dbFeeTypeVariants.FirstOrDefault(variant => variant.ID == item.ID.Value);
                        dbItem.Title = item.Title;
                        dbItem.Price = item._price;
                    }
                    // add new ones
                    else
                    {
                        sqldb.FeeTypeVariants.Add(new FeeTypeVariant()
                        {
                            Title = item.Title,
                            Price = item._price,
                            FeeTypeID = item.FeeTypeID
                        });
                    }
                }
                // delete extras
                var toRemove = dbFeeTypeVariants.Where(variant => !variantsArray.Select(va => va.ID).Contains(variant.ID)).ToArray();
                if(toRemove.Any(ftv=> ftv.Fees.Any()))
                {
                    return RedirectToAction("Index", "AdditionalFee", new { errorMessage = 7 });
                }
                sqldb.FeeTypeVariants.RemoveRange(toRemove);

                // save to database
                sqldb.SaveChanges();

                return RedirectToAction("Index", "AdditionalFee", new { errorMessage = 0 });
            }

            ViewBag.FeeTypeID = id;
            return View(variants);
        }
    }
}