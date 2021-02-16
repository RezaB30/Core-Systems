using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RezaB.Web.CustomAttributes;
using System.Data.Entity;
using RezaB.Web;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Groups")]
    public class GroupController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        // GET: Group
        public ActionResult Index(int? page, [Bind(Prefix = "search")] GroupSearchViewModel search)
        {
            var baseQuery = db.Groups.Include(g => g.Subscriptions).OrderBy(group => group.Name).AsQueryable();
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.GroupName))
                {
                    baseQuery = baseQuery.Where(g => g.Name.Contains(search.GroupName));
                }
            }
            var viewResults = baseQuery.Select(group => new GroupViewModel()
            {
                ID = group.ID,
                Name = group.Name,
                IsActive = group.IsActive,
                _subscriptionCount = group.Subscriptions.Count(),
                CanBeChanged = group.ID != CustomerWebsiteSettings.CustomerWebsiteRegistrationGroupID
            }).AsQueryable();

            SetupPages(page, ref viewResults);
            ViewBag.Search = search;
            return View(viewResults.ToList());
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Modify Groups")]
        // GET: Group/Add
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Groups")]
        // POST: Group/Add
        public ActionResult Add([Bind(Include = "Name")] GroupViewModel group)
        {
            if (ModelState.IsValid)
            {
                group.Name = group.Name.ToUpper().Trim();
                if (db.Groups.Any(g=>g.Name.ToUpper() == group.Name))
                {
                    ModelState.AddModelError("Name", RadiusR.Localization.Validation.Common.ValueExists);
                }
                else
                {
                    db.Groups.Add(new Group()
                    {
                        Name = group.Name,
                        IsActive = true
                    });

                    db.SaveChanges();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Groups")]
        // POST: Group/ToggleActive
        public ActionResult ToggleActive(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbGroup = db.Groups.Find(id);
            if (dbGroup == null || dbGroup.ID == CustomerWebsiteSettings.CustomerWebsiteRegistrationGroupID)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "18", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            dbGroup.IsActive = !dbGroup.IsActive;
            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Groups")]
        // POST: Group/Remove
        public ActionResult Remove(int id)
        {
            var dbGroup = db.Groups.Find(id);
            if (dbGroup == null || dbGroup.ID == CustomerWebsiteSettings.CustomerWebsiteRegistrationGroupID)
            {
                return RedirectToAction("Index", new { errorMessage = 18 });
            }
            if (dbGroup.Subscriptions.Any())
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            dbGroup.Subscriptions.Clear();
            db.Groups.Remove(dbGroup);
            db.SaveChanges();

            return RedirectToAction("index", new { errorMessage = 0 });
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Modify Groups")]
        // GET: Group/Edit
        public ActionResult Edit(int id)
        {
            var dbGroup = db.Groups.Find(id);
            if (dbGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 18 });
            }

            var group = new GroupViewModel()
            {
                ID = dbGroup.ID,
                Name = dbGroup.Name
            };
            return View(group);
        }

        [AuthorizePermission(Permissions = "Modify Groups")]
        // POST: Group/Edit
        public ActionResult Edit([Bind(Include = "ID,Name")] GroupViewModel group)
        {
            var dbGroup = db.Groups.Find(group.ID);
            if (dbGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 18 });
            }

            if (ModelState.IsValid)
            {
                group.Name = group.Name.ToUpper().Trim();
                if (db.Groups.Any(g => g.ID != dbGroup.ID && g.Name.ToUpper() == group.Name))
                {
                    ModelState.AddModelError("Name", RadiusR.Localization.Validation.Common.ValueExists);
                }
                else
                {
                    dbGroup.Name = group.Name;

                    db.SaveChanges();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            return View(group);
        }
    }
}