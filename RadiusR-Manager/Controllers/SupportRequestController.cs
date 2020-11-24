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
using RadiusR.DB.Enums.SupportRequests;


namespace RadiusR_Manager.Controllers
{
    //[AuthorizePermission(Permissions = "Support Requests")]
    public class SupportRequestController : BaseController
    {
        RadiusREntities db = new RadiusREntities();

        // GET: SupportRequest/Index
        public ActionResult Index()
        {
            var userSupportGroups = User.GiveSupportGroups();
            var currentUserId = User.GiveUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Index", "Home", new { errorMessage = 0 });
            }

            var userIsLeaderInGroups = userSupportGroups.Where(item => item.IsLeader).Select(item => item.GroupId).ToArray();
            var userIsInGroups = userSupportGroups.Select(item => item.GroupId).ToArray();
            var viewResults = db.SupportGroups.Where(sg => userIsInGroups.Contains(sg.ID)).Select(sg => new SupportGroupRequestListViewModel()
            {
                GroupID = sg.ID,
                GroupName = sg.Name,
                IsLeader = sg.LeaderID == currentUserId,
                GroupInbox = userIsLeaderInGroups.Contains(sg.ID) ? sg.SupportRequestTypes.Select(srt => srt.SupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done).Count()).DefaultIfEmpty(0).Sum() : 0,
                GroupRedirectedInbox = userIsLeaderInGroups.Contains(sg.ID) ? sg.RedirectedSupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done).Count() : 0,
                GroupInProgress = userIsLeaderInGroups.Contains(sg.ID) ? sg.AssignedSupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done).Count() : 0,
                PersonalInbox = sg.AssignedSupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done && sr.AssignedUserID == currentUserId).Count()
            }).ToArray();
            
            return View(viewResults);
        }

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
                SubTypes = srt.SupportRequestSubTypes.OrderBy(srst => srst.IsDisabled).ThenByDescending(srst => srst.ID).Select(srst => new SupportRequestSubTypeViewModel()
                {
                    ID = srst.ID,
                    IsActive = !srst.IsDisabled,
                    Name = srst.Name
                })
            });

            SetupPages(page, ref viewResults);

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/AddSupportRequestType
        public ActionResult AddSupportRequestType(string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View();
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/AddSupportRequestType
        public ActionResult AddSupportRequestType(string redirectUrl, SupportRequestTypeViewModel addedType)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (ModelState.IsValid)
            {
                db.SupportRequestTypes.Add(new SupportRequestType()
                {
                    IsDisabled = false,
                    IsStaffOnly = addedType.IsStaffOnly,
                    Name = addedType.Name
                });

                db.SaveChanges();
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(addedType);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SupportRequest/ToggleSupportRequestTypeState
        public ActionResult ToggleSupportRequestTypeState(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestType = db.SupportRequestTypes.Find(id);
            if (currentSupportRequestType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            currentSupportRequestType.IsDisabled = !currentSupportRequestType.IsDisabled;
            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/RenameSupportRequestType
        public ActionResult RenameSupportRequestType(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestType = db.SupportRequestTypes.Find(id);
            if (currentSupportRequestType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var viewResults = new SupportRequestTypeViewModel()
            {
                ID = currentSupportRequestType.ID,
                Name = currentSupportRequestType.Name
            };

            ViewBag.RequestTypeName = currentSupportRequestType.Name;
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/RenameSupportRequestType
        public ActionResult RenameSupportRequestType(int id, string redirectUrl, SupportRequestTypeViewModel supportRequestType)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestType = db.SupportRequestTypes.Find(id);
            if (currentSupportRequestType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                currentSupportRequestType.Name = supportRequestType.Name;
                db.SaveChanges();
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RequestTypeName = currentSupportRequestType.Name;
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(supportRequestType);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/AddSupportRequestSubType
        public ActionResult AddSupportRequestSubType(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestType = db.SupportRequestTypes.Find(id);
            if (currentSupportRequestType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RequestTypeName = currentSupportRequestType.Name;
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View();
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/AddSupportRequestSubType
        public ActionResult AddSupportRequestSubType(int id, string redirectUrl, SupportRequestSubTypeViewModel supportRequestSubType)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestType = db.SupportRequestTypes.Find(id);
            if (currentSupportRequestType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                currentSupportRequestType.SupportRequestSubTypes.Add(new SupportRequestSubType()
                {
                    IsDisabled = false,
                    Name = supportRequestSubType.Name
                });
                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RequestTypeName = currentSupportRequestType.Name;
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View();
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/RenameSupportRequestSubType
        public ActionResult RenameSupportRequestSubType(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestSubType = db.SupportRequestSubTypes.Find(id);
            if (currentSupportRequestSubType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var viewResults = new SupportRequestSubTypeViewModel()
            {
                ID = currentSupportRequestSubType.ID,
                Name = currentSupportRequestSubType.Name
            };

            ViewBag.RequestSubTypeName = currentSupportRequestSubType.Name;
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: SupportRequest/RenameSupportRequestSubType
        public ActionResult RenameSupportRequestSubType(int id, string redirectUrl, SupportRequestSubTypeViewModel supportRequestSubType)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequestSubType = db.SupportRequestSubTypes.Find(id);
            if (currentSupportRequestSubType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                currentSupportRequestSubType.Name = supportRequestSubType.Name;
                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.RequestSubTypeName = currentSupportRequestSubType.Name;
            ViewBag.RedirectUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(supportRequestSubType);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/ToggleSupportRequestSubType
        public ActionResult ToggleSupportRequestSubType(int id, string redirectUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + redirectUrl);

            var currentSupportRequestSubType = db.SupportRequestSubTypes.Find(id);
            if (currentSupportRequestSubType == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            // has relative data or is disabled (gets disabled/enabled)
            if (currentSupportRequestSubType.IsDisabled || db.SupportRequests.Any(request => request.SubTypeID == currentSupportRequestSubType.ID))
            {
                currentSupportRequestSubType.IsDisabled = !currentSupportRequestSubType.IsDisabled;
            }
            // has no relative data (gets removed)
            else
            {
                db.SupportRequestSubTypes.Remove(currentSupportRequestSubType);
            }

            db.SaveChanges();
            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/SupportGroupUsers
        public ActionResult SupportGroupUsers(int id, int? page)
        {
            var currentSupportGroup = db.SupportGroups.Find(id);
            if (currentSupportGroup == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }
            var viewResults = db.SupportGroupUsers.Where(sgu => sgu.SupportGroupID == currentSupportGroup.ID).OrderBy(sgu => sgu.AppUser.Name).Select(sgu => new SupportGroupUserViewModel()
            {
                UserName = sgu.AppUser.Name,
                UserID = sgu.AppUserID,
                CanChangeState = sgu.CanChangeState,
                CanCreate = sgu.CanCreate
            });

            SetupPages(page, ref viewResults);
            ViewBag.GroupName = currentSupportGroup.Name;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/AddGroupUser
        public ActionResult AddGroupUser(int id)
        {
            var currentSupportGroup = db.SupportGroups.Find(id);
            if (currentSupportGroup == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }

            ViewBag.ValidUsers = new SelectList(db.AppUsers.Where(user => user.IsEnabled && !user.SupportGroupUsers.Any(sgu => sgu.SupportGroupID == currentSupportGroup.ID)).Select(user => new { Name = user.Name, Value = user.ID }).ToArray(), "Value", "Name");
            ViewBag.GroupName = currentSupportGroup.Name;
            return View();
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/AddGroupUser
        public ActionResult AddGroupUser(int id, SupportGroupUserViewModel addedUser)
        {
            var currentSupportGroup = db.SupportGroups.Find(id);
            if (currentSupportGroup == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                if (db.AppUsers.FirstOrDefault(user => user.IsEnabled && user.ID == addedUser.UserID) == null)
                {
                    ModelState.AddModelError("UserID", RadiusR.Localization.Validation.Common.InvalidInput);
                }
                else
                {
                    db.SupportGroupUsers.Add(new SupportGroupUser()
                    {
                        AppUserID = addedUser.UserID.Value,
                        CanChangeState = addedUser.CanChangeState,
                        CanCreate = addedUser.CanCreate,
                        SupportGroupID = currentSupportGroup.ID
                    });

                    db.SaveChanges();
                    return RedirectToAction("SupportGroupUsers", new { id = currentSupportGroup.ID, errorMessage = 0 });
                }
            }

            ViewBag.ValidUsers = new SelectList(db.AppUsers.Where(user => user.IsEnabled && !user.SupportGroupUsers.Any(sgu => sgu.SupportGroupID == currentSupportGroup.ID)).Select(user => new { Name = user.Name, Value = user.ID }).ToArray(), "Value", "Name", addedUser.UserID);
            ViewBag.GroupName = currentSupportGroup.Name;
            return View(addedUser);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/RemoveGroupUser
        public ActionResult RemoveGroupUser(int userId, int groupId)
        {
            var currentGroupUser = db.SupportGroupUsers.FirstOrDefault(sgu => sgu.AppUserID == userId && sgu.SupportGroupID == groupId);
            if (currentGroupUser != null)
            {
                db.SupportGroupUsers.Remove(currentGroupUser);
                db.SaveChanges();
            }
            return RedirectToAction("SupportGroupUsers", new { id = groupId, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/GroupUserPermissions
        public ActionResult GroupUserPermissions(int userId, int groupId)
        {
            var currentGroupUser = db.SupportGroupUsers.FirstOrDefault(sgu => sgu.AppUserID == userId && sgu.SupportGroupID == groupId);
            if (currentGroupUser == null)
            {
                return RedirectToAction("SupportGroupUsers", new { errorMessage = 9 });
            }

            var viewResults = new SupportGroupUserViewModel()
            {
                UserID = currentGroupUser.AppUserID,
                CanCreate = currentGroupUser.CanCreate,
                CanChangeState = currentGroupUser.CanChangeState,
                UserName = currentGroupUser.AppUser.Name
            };

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/GroupUserPermissions
        public ActionResult GroupUserPermissions(int userId, int groupId, SupportGroupUserViewModel groupUser)
        {
            var currentGroupUser = db.SupportGroupUsers.FirstOrDefault(sgu => sgu.AppUserID == userId && sgu.SupportGroupID == groupId);
            if (currentGroupUser == null)
            {
                return RedirectToAction("SupportGroupUsers", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                currentGroupUser.CanChangeState = groupUser.CanChangeState;
                currentGroupUser.CanCreate = groupUser.CanCreate;

                db.SaveChanges();
                return RedirectToAction("SupportGroupUsers", new { id = currentGroupUser.SupportGroupID, errorMessage = 0 });
            }

            return View(groupUser);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpGet]
        // GET: SupportRequest/AddSupportGroupRequestType
        public ActionResult AddSupportGroupRequestType(int id)
        {
            var currentSupportGroup = db.SupportGroups.Find(id);
            if (currentSupportGroup == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }

            ViewBag.GroupName = currentSupportGroup.Name;
            var currentlyAddedIds = currentSupportGroup.SupportRequestTypes.Select(srt2 => srt2.ID).ToArray();
            ViewBag.ValidTypes = new SelectList(db.SupportRequestTypes.Where(srt => !srt.IsDisabled && !currentlyAddedIds.Contains(srt.ID)).Select(srt => new { Value = srt.ID, Name = srt.Name }).ToArray(), "Value", "Name");
            return View();
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/AddSupportGroupRequestType
        public ActionResult AddSupportGroupRequestType(int id, AddSupportGroupRequestTypeViewModel addedRequestType)
        {
            var currentSupportGroup = db.SupportGroups.Find(id);
            if (currentSupportGroup == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                var selectedSupportRequestType = db.SupportRequestTypes.Find(addedRequestType.RequestTypeID.Value);
                if (selectedSupportRequestType == null || selectedSupportRequestType.IsDisabled)
                {
                    ModelState.AddModelError("RequestTypeID", RadiusR.Localization.Validation.Common.InvalidInput);
                }
                else
                {
                    currentSupportGroup.SupportRequestTypes.Add(selectedSupportRequestType);
                    db.SaveChanges();
                    return RedirectToAction("SupportGroups", new { errorMessage = 0 });
                }
            }

            ViewBag.GroupName = currentSupportGroup.Name;
            ViewBag.ValidTypes = new SelectList(db.SupportRequestTypes.Where(srt => !srt.IsDisabled && !currentSupportGroup.SupportRequestTypes.Select(srt2 => srt2.ID).Contains(srt.ID)).Select(srt => new { Value = srt.ID, Name = srt.Name }).ToArray(), "Value", "Name", addedRequestType.RequestTypeID);
            return View(addedRequestType);
        }

        [AuthorizePermission(Permissions = "Support Request Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/RemoveSupportGroupRequestType
        public ActionResult RemoveSupportGroupRequestType(int id, int requestTypeId)
        {
            var currentSupportGroup = db.SupportGroups.Find(id);
            if (currentSupportGroup == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }

            var currentRequestType = db.SupportRequestTypes.Find(requestTypeId);
            if (currentRequestType == null)
            {
                return RedirectToAction("SupportGroups", new { errorMessage = 9 });
            }

            currentSupportGroup.SupportRequestTypes.Remove(currentRequestType);
            db.SaveChanges();
            return RedirectToAction("SupportGroups", new { errorMessage = 0 });
        }
    }
}