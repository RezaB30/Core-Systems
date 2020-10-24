using RadiusR.DB;
using RadiusR_Manager.Helpers;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.Helpers;
using RadiusR_Manager.Models;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Role Management")]
    public class RoleController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        // GET: Role
        public ActionResult Index(int? page)
        {
            var viewResults = db.Roles.OrderByDescending(role => role.ID).Select(role => new RoleViewModel()
            {
                ID = role.ID,
                Name = role.Name,
                IsSystemRole = role.IsSystemRole,
                CanBeManuallyAssigned = role.CanBeManuallyAssigned,
                HasUsers = role.AppUsers.Any(),
                _source = role
            });

            SetupPages(page, ref viewResults);
            return View(viewResults);
        }

        // GET: Role/Add
        public ActionResult Add()
        {
            var editableRole = new EditableRoleViewModel()
            {
                Permissions = FillPermissionTree(Enumerable.Empty<long>())
            };
            return View(editableRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Role/Add
        public ActionResult Add([Bind(Include = "Name")] EditableRoleViewModel role, string Permissions)
        {
            role.Permissions = FillPermissionTree(Enumerable.Empty<long>());

            var selectedPermissions = new List<long>();
            try
            {
                selectedPermissions.AddRange(Permissions.Split(',').Select(p => long.Parse(p)));
                role.Permissions = FillPermissionTree(selectedPermissions);
                if (selectedPermissions.Count() > 0)
                {
                    ModelState.Remove("Permissions");
                }
                else
                {
                    ModelState.AddModelError("Permissions", RadiusR.Localization.Validation.Common.InvalidPermissionSelection);
                }
                if (!HasValidPermissionPrequisites(selectedPermissions))
                {
                    ModelState.AddModelError("Permissions", RadiusR.Localization.Validation.Common.InvalidPermissionSelection);
                }
            }
            catch
            {
                ModelState.AddModelError("Permissions", RadiusR.Localization.Validation.Common.InvalidPermissionSelection);
            }

            if (ModelState.IsValid)
            {
                db.Roles.Add(new Role()
                {
                    ID = db.Roles.Max(r=>r.ID) + 1,
                    CanBeManuallyAssigned = true,
                    IsSystemRole = false,
                    Name = role.Name,
                    Permissions = db.Permissions.Where(permission=> selectedPermissions.Contains(permission.ID)).ToList()
                });

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Role/Remove
        public ActionResult Remove(int id)
        {
            var dbRole = db.Roles.Find(id);
            if (dbRole == null)
            {
                return RedirectToAction("Index", new { errorMessage = 24 });
            }
            if (dbRole.AppUsers.Any())
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            dbRole.Permissions.Clear();
            db.Roles.Remove(dbRole);
            db.SaveChanges();
            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        // GET: Role/Edit
        public ActionResult Edit(int id)
        {
            var dbRole = db.Roles.Find(id);
            if (dbRole == null)
            {
                return RedirectToAction("Index", new { errorMessage = 24 });
            }

            var role = new EditableRoleViewModel()
            {
                ID = dbRole.ID,
                Name = dbRole.Name,
                Permissions = FillPermissionTree(dbRole.Permissions.Select(r=>(long)r.ID))
            };

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Role/Edit
        public ActionResult Edit(int id, [Bind(Include = "Name")] EditableRoleViewModel role, string Permissions)
        {
            var dbRole = db.Roles.Find(id);
            if (dbRole == null)
            {
                return RedirectToAction("Index", new { errorMessage = 24 });
            }

            role.Permissions = FillPermissionTree(Enumerable.Empty<long>());

            var selectedPermissions = new List<long>();
            try
            {
                selectedPermissions.AddRange(Permissions.Split(',').Select(p => long.Parse(p)));
                role.Permissions = FillPermissionTree(selectedPermissions);
                if (selectedPermissions.Count() > 0)
                {
                    ModelState.Remove("Permissions");
                }
                else
                {
                    ModelState.AddModelError("Permissions", RadiusR.Localization.Validation.Common.InvalidPermissionSelection);
                }
                if (!HasValidPermissionPrequisites(selectedPermissions))
                {
                    ModelState.AddModelError("Permissions", RadiusR.Localization.Validation.Common.InvalidPermissionSelection);
                }
            }
            catch
            {
                ModelState.AddModelError("Permissions", RadiusR.Localization.Validation.Common.InvalidPermissionSelection);
            }

            if (ModelState.IsValid)
            {
                dbRole.Name = role.Name;
                dbRole.Permissions.Clear();
                dbRole.Permissions = db.Permissions.Where(permission => selectedPermissions.Contains(permission.ID)).ToList();
                
                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            return View(role);
        }

        /// <summary>
        /// Fills tree collection for permission selector helper.
        /// </summary>
        /// <param name="rolePermissions">Role's selected permissions</param>
        /// <returns>Tree collection needed for helper</returns>
        private IEnumerable<TreeCollection> FillPermissionTree(IEnumerable<long> rolePermissions)
        {
            var allPermissions = db.Permissions.ToArray();
            var allRootPermissions = allPermissions.Where(permission => permission.Permission2 == null).ToArray();
            var resultCollection = new List<TreeCollection>();
            foreach (var permission in allRootPermissions)
            {
                resultCollection.Add(new TreeCollection()
                {
                    ID = permission.ID,
                    Name = permission.Name,
                    IsSelected = rolePermissions.Contains(permission.ID),
                    _sub = _createNode(rolePermissions, permission.ID, allPermissions)
                });
            }
            return resultCollection;
        }

        private IEnumerable<TreeCollection> _createNode(IEnumerable<long> rolePermissions, long rootId, IEnumerable<Permission> allPermissions)
        {
            var results = new List<TreeCollection>();
            foreach (var permission in allPermissions.Where(p => p.PrequisiteID == rootId))
            {
                results.Add(new TreeCollection()
                {
                    ID = permission.ID,
                    Name = permission.Name,
                    IsSelected = rolePermissions.Contains(permission.ID),
                    _sub = _createNode(rolePermissions, permission.ID, allPermissions)
                });
            }
            return results;
        }

        private bool HasValidPermissionPrequisites(IEnumerable<long> selectedPermissions)
        {
            var toCheckList = db.Permissions.Where(permission => selectedPermissions.Contains(permission.ID) && permission.PrequisiteID.HasValue);
            foreach (var permission in toCheckList)
            {
                if (!selectedPermissions.Contains(permission.PrequisiteID.Value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}