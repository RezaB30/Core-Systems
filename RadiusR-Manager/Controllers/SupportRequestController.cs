using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR_Manager.Models.ViewModels.Customer;
using System.Data.Entity;
using RezaB.Web.CustomAttributes;
using RezaB.Web;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    //[AuthorizePermission(Permissions = "Support Requests")]
    public class SupportRequestController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        [AuthorizePermission(Permissions = "Support Request Settings")]
        public ActionResult SupportGroups(int? page)
        {
            var viewResults = db.SupportGroups.OrderBy(sg => sg.ID).Select(sg => new SupportGroupViewModel()
            {
                ID = sg.ID,
                Name = sg.Name,
                LeaderName = sg.AppUser.Name,
                ActiveUsers = sg.SupportGroupUsers.Where(u => u.AppUser.IsEnabled).Count(),
                RelevantTypes = sg.SupportRequestTypes.Select(srt => new SupportRequestTypeViewModel()
                {
                    ID = srt.ID,
                    IsActive = !srt.IsDisabled,
                    IsStaffOnly = srt.IsStaffOnly,
                    Name = srt.Name
                }),
                IsActive = !sg.IsDisabled
            });

            SetupPages(page, ref viewResults);
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/AddSupportGroup
        public ActionResult AddSupportGroup(string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            ViewBag.ValidUsers = new SelectList(db.AppUsers.Where(user => user.IsEnabled).OrderBy(user => user.Name).Select(user => new { Name = user.Name, Value = user.ID }).ToArray(), "Value", "Name");
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View();
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: SupportRequest/AddSupportGroup
        public ActionResult AddSupportGroup(string redirectUrl, SupportGroupViewModel supportGroup)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            if (ModelState.IsValid)
            {
                db.SupportGroups.Add(new SupportGroup()
                {
                    Name = supportGroup.Name,
                    LeaderID = supportGroup.LeaderID.Value,
                    IsDisabled = false
                });

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.ValidUsers = new SelectList(db.AppUsers.Where(user => user.IsEnabled).OrderBy(user => user.Name).Select(user => new { Name = user.Name, Value = user.ID }).ToArray(), "Value", "Name", supportGroup.LeaderID);
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(supportGroup);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: SupportRequest/ToggleSupportGroupState
        public ActionResult ToggleSupportGroupState(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            var dbSupportGroup = db.SupportGroups.Find(id);
            if (dbSupportGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            dbSupportGroup.IsDisabled = !dbSupportGroup.IsDisabled;
            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/RenameSupportGroup
        public ActionResult RenameSupportGroup(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            var dbSupportGroup = db.SupportGroups.Find(id);
            if (dbSupportGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var supportGroupName = new SupportGroupRenameViewModel()
            {
                Name = dbSupportGroup.Name
            };

            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupName = dbSupportGroup.Name;
            return View(supportGroupName);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SupportRequest/RenameSupportGroup
        public ActionResult RenameSupportGroup(int id, string redirectUrl, SupportGroupRenameViewModel supportGroupName)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            var dbSupportGroup = db.SupportGroups.Find(id);
            if (dbSupportGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                dbSupportGroup.Name = supportGroupName.Name;

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupName = dbSupportGroup.Name;
            return View(supportGroupName);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/ChangeSupportGroupLeader
        public ActionResult ChangeSupportGroupLeader(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            var dbSupportGroup = db.SupportGroups.Find(id);
            if (dbSupportGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var changeSupportGroupLeader = new ChangeSupportGroupLeaderViewModel()
            {
                LeaderID = dbSupportGroup.LeaderID
            };

            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupName = dbSupportGroup.Name;
            ViewBag.ValidUsers = new SelectList(db.AppUsers.Where(user => user.IsEnabled).OrderBy(user => user.Name).Select(user => new { Name = user.Name, Value = user.ID }).ToArray(), "Value", "Name", changeSupportGroupLeader.LeaderID);

            return View(changeSupportGroupLeader);
        }


        [AuthorizePermission(Permissions = "Support Request Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SupportRequest/ChangeSupportGroupLeader
        public ActionResult ChangeSupportGroupLeader(int id, string redirectUrl, ChangeSupportGroupLeaderViewModel changeSupportGroupLeader)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            var dbSupportGroup = db.SupportGroups.Find(id);
            if (dbSupportGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                dbSupportGroup.LeaderID = changeSupportGroupLeader.LeaderID.Value;

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupName = dbSupportGroup.Name;
            ViewBag.ValidUsers = new SelectList(db.AppUsers.Where(user => user.IsEnabled).OrderBy(user => user.Name).Select(user => new { Name = user.Name, Value = user.ID }).ToArray(), "Value", "Name", changeSupportGroupLeader.LeaderID);

            return View(changeSupportGroupLeader);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        // GET: SupportRequest/SupportRequestTypes
        public ActionResult SupportRequestTypes(int? page)
        {
            var viewResults = db.SupportRequestTypes.OrderBy(srt => srt.ID).Select(srt => new SupportRequestTypeViewModel()
            {
                ID = srt.ID,
                Name = srt.Name,
                IsStaffOnly = srt.IsStaffOnly,
                IsActive = !srt.IsDisabled,
                SubTypes = srt.SupportRequestSubTypes.Select(srst => new SupportRequestSubTypeViewModel()
                {
                    ID = srst.ID,
                    IsActive = !srst.IsDisabled,
                    Name = srst.Name
                })
            });

            SetupPages(page, ref viewResults);

            return View(viewResults);
        }
    }
}