using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Groups")]
    public class GroupController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        // GET: Group
        public ActionResult Index(int? page)
        {
            var viewResults = db.Groups.OrderBy(group => group.Name).Select(group => new GroupViewModel()
            {
                ID = group.ID,
                Name = group.Name,
                Subscriptions = group.Subscriptions
            }).AsQueryable();
            SetupPages(page, ref viewResults);
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
                db.Groups.Add(new Group()
                {
                    Name = group.Name
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(Permissions = "Modify Groups")]
        // POST: Group/Remove
        public ActionResult Remove(int id)
        {
            var dbGroup = db.Groups.Find(id);
            if (dbGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 18 });
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
        public ActionResult Edit([Bind(Include = "ID,Name")]GroupViewModel group)
        {
            var dbGroup = db.Groups.Find(group.ID);
            if (dbGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 18 });
            }

            if (ModelState.IsValid)
            {
                dbGroup.Name = group.Name;

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(group);
        }
    }
}