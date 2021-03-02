using RadiusR.DB;
using RadiusR.DB.DomainsCache;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Transition Operators")]
    public class TransitionOperatorsController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        // GET: TransitionOperator
        public ActionResult Index()
        {
            var viewResults = TransitionOperatorsCache.GetAllOperators().Select(to => new TransitionOperatorViewModel(to)).OrderBy(to => to.DisplayName).ToArray();
            return View(viewResults);
        }

        [HttpGet]
        // GET: TransitionOperators/Add
        public ActionResult Add()
        {
            return View(new TransitionOperatorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: TransitionOperators/Add
        public ActionResult Add(TransitionOperatorViewModel transitionOperator)
        {
            if (ModelState.IsValid)
            {
                if (db.TransitionOperators.Any(to => to.TelekomUsername.ToLower() == transitionOperator.Username.ToLower()))
                {
                    ModelState.AddModelError("Username", RadiusR.Localization.Validation.Common.ValueAlreadyExists);
                }
                else
                {
                    db.TransitionOperators.Add(new TransitionOperator()
                    {
                        DisplayName = transitionOperator.DisplayName.ToUpper(),
                        RemoteFolder = transitionOperator._remoteFolder,
                        TelekomUsername = transitionOperator.Username
                    });

                    db.SaveChanges();
                    TransitionOperatorsCache.ClearCache();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            return View(transitionOperator);
        }

        [HttpGet]
        // GET: TransitionOperators/Edit
        public ActionResult Edit(int id)
        {
            var cachedOperator = TransitionOperatorsCache.GetSpecificOperator(id);
            if (cachedOperator == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var transitionOperator = new TransitionOperatorViewModel(cachedOperator);
            ViewBag.OperatorName = cachedOperator.DisplayName;
            return View(viewName: "Add", model: transitionOperator);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: TransitionOperators/Edit
        public ActionResult Edit(int id, TransitionOperatorViewModel transitionOperator)
        {
            var cachedOperator = TransitionOperatorsCache.GetSpecificOperator(id);
            if (cachedOperator == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                if (db.TransitionOperators.Any(to => to.TelekomUsername.ToLower() == transitionOperator.Username.ToLower() && to.ID != cachedOperator.ID))
                {
                    ModelState.AddModelError("Username", RadiusR.Localization.Validation.Common.ValueAlreadyExists);
                }
                else
                {
                    var dbOperator = db.TransitionOperators.Find(id);
                    dbOperator.DisplayName = transitionOperator.DisplayName.ToUpper();
                    dbOperator.TelekomUsername = transitionOperator.Username;
                    dbOperator.RemoteFolder = transitionOperator._remoteFolder;

                    db.SaveChanges();
                    TransitionOperatorsCache.ClearCache();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }
            return View(transitionOperator);
        }
    }
}