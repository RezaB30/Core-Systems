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
using RadiusR_Manager.Models.ViewModels.SupportRequestModels;
using RadiusR.DB.ModelExtentions;
using RadiusR.SMS;
using RadiusR.DB.RandomCode;
using RadiusR.FileManagement;
using RadiusR.FileManagement.SpecialFiles;

namespace RadiusR_Manager.Controllers
{
    //[AuthorizePermission(Permissions = "Support Requests")]
    public class SupportRequestController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        #region Create
        [AuthorizePermission(Permissions = "Create Support Request")]
        [HttpGet]
        // GET: SupportRequest/Create
        public ActionResult Create(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.RequestTypes = new SelectList(db.SupportRequestTypes.Where(sr => !sr.IsDisabled).OrderBy(sr => sr.Name).Select(sr => new { Name = sr.Name, Value = sr.ID }), "Value", "Name");
            return View();
        }

        [AuthorizePermission(Permissions = "Create Support Request")]
        [HttpPost]
        // POST: SupportRequest/Create
        public ActionResult Create(long id, string returnUrl, SupportRequestCreateViewModel newSupportRequest)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                var currentRequestType = db.SupportRequestTypes.Find(newSupportRequest.TypeID.Value);
                var currentRequestSubType = db.SupportRequestSubTypes.Find(newSupportRequest.SubTypeID.Value);
                if (currentRequestType == null || currentRequestType.IsDisabled || currentRequestSubType == null || currentRequestSubType.IsDisabled || currentRequestSubType.SupportRequestTypeID != currentRequestType.ID)
                {
                    ModelState.AddModelError("TypeID", RadiusR.Localization.Validation.Common.InvalidInput);
                    ModelState.AddModelError("SubTypeID", RadiusR.Localization.Validation.Common.InvalidInput);
                }
                else
                {
                    var dbSupportRequest = new SupportRequest()
                    {
                        Date = DateTime.Now,
                        IsVisibleToCustomer = !currentRequestType.IsStaffOnly && newSupportRequest.IsVisibleToCustomer,
                        StateID = (short)SupportRequestStateID.InProgress,
                        SubscriptionID = dbSubscription.ID,
                        SubTypeID = currentRequestSubType.ID,
                        SupportPin = CodeGenerator.GenerateSupportRequestPIN(),
                        SupportRequestProgresses = new List<SupportRequestProgress>()
                        {
                            new SupportRequestProgress()
                            {
                                ActionType = (short)SupportRequestActionTypes.Create,
                                AppUserID = User.GiveUserId(),
                                Date = DateTime.Now,
                                IsVisibleToCustomer = newSupportRequest.IsVisibleToCustomer,
                                Message = newSupportRequest.Message
                            }
                        },
                        TypeID = currentRequestType.ID
                    };

                    db.SupportRequests.Add(dbSupportRequest);
                    db.SaveChanges();

                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                }
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.RequestTypes = new SelectList(db.SupportRequestTypes.Where(sr => !sr.IsDisabled).OrderBy(sr => sr.Name).Select(sr => new { Name = sr.Name, Value = sr.ID }), "Value", "Name", newSupportRequest.TypeID);
            ViewBag.RequestSubTypes = new SelectList(db.SupportRequestSubTypes.Where(srst => srst.SupportRequestTypeID == newSupportRequest.TypeID).Where(srst => !srst.IsDisabled).OrderBy(srst => srst.Name).Select(srst => new
            {
                Name = srst.Name,
                Value = srst.ID
            }), "Value", "Name", newSupportRequest.SubTypeID);
            return View(newSupportRequest);
        }

        [HttpPost]
        [AjaxCall]
        // POST: SupportRequest/GetSubTypes
        public ActionResult GetSubTypes(int id)
        {
            var currentRequestType = db.SupportRequestTypes.Find(id);
            if (currentRequestType == null)
            {
                return Json(new { ErrorOccured = true, ErrorMessage = RadiusR.Localization.Validation.Common.InvalidInput });
            }

            return Json(new
            {
                ErrorOccured = false,
                Data = currentRequestType.SupportRequestSubTypes.Where(srst => !srst.IsDisabled).OrderBy(srst => srst.Name).Select(srst => new
                {
                    Name = srst.Name,
                    Code = srst.ID
                })
            });
        }
        #endregion
        #region Inbox
        [HttpGet]
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
            var userCanGloballyRead = User.HasPermission("Global Support Request Read");
            var viewResults = db.SupportGroups.Where(sg => userCanGloballyRead || userIsInGroups.Contains(sg.ID)).ToArray().Select(sg => new SupportGroupRequestListViewModel()
            {
                GroupID = sg.ID,
                GroupName = sg.Name,
                IsLeader = userCanGloballyRead || sg.LeaderID == currentUserId,
                GroupInbox = (userCanGloballyRead || userIsLeaderInGroups.Contains(sg.ID)) ? db.GetSupportGroupInbox(sg.ID).Count() /*sg.SupportRequestTypes.Select(srt => srt.SupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done && !sr.AssignedGroupID.HasValue && !sr.RedirectedGroupID.HasValue).Count()).DefaultIfEmpty(0).Sum()*/ : 0,
                GroupRedirectedInbox = (userCanGloballyRead || userIsLeaderInGroups.Contains(sg.ID)) ? db.GetSupportGroupRedirectInbox(sg.ID).Count() /*sg.RedirectedSupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done && sr.RedirectedGroupID == sg.ID).Count()*/ : 0,
                GroupInProgress = (userCanGloballyRead || userIsLeaderInGroups.Contains(sg.ID)) ? db.GetSupportGroupInProgressInbox(sg.ID).Count() /*sg.AssignedSupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done && sr.AssignedGroupID == sg.ID).Count()*/ : 0,
                PersonalInbox = db.GetSupportUserInbox(sg.ID, currentUserId.Value).Count() /*sg.AssignedSupportRequests.Where(sr => sr.StateID != (short)SupportRequestStateID.Done && sr.AssignedUserID == currentUserId).Count()*/
            }).ToArray();

            return View(viewResults);
        }

        [HttpGet]
        // GET: SupportRequest/GroupInbox
        public ActionResult GroupInbox(int id, int? page)
        {
            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var userCanGloballyRead = User.HasPermission("Global Support Request Read");
            var userGroups = User.GiveSupportGroups();
            if (!userCanGloballyRead && !userGroups.Any(item => item.GroupId == currentGroup.ID && item.IsLeader))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var viewResults = db.GetSupportGroupInbox(currentGroup.ID).GetViewModels();

            SetupPages(page, ref viewResults);
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.GroupId = currentGroup.ID;
            ViewBag.InboxTitle = $"{ViewBag.GroupName}-{RadiusR.Localization.Model.RadiusR.GroupInbox}";
            ViewBag.CanShare = true;
            ViewBag.IsRedirect = false;
            return View(viewName: "Inbox", model: viewResults);
        }

        [HttpGet]
        // GET: SupportRequest/GroupRedirectInbox
        public ActionResult GroupRedirectInbox(int id, int? page)
        {
            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var userCanGloballyRead = User.HasPermission("Global Support Request Read");
            var userGroups = User.GiveSupportGroups();
            if (!userCanGloballyRead && !userGroups.Any(item => item.GroupId == currentGroup.ID && item.IsLeader))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var viewResults = db.GetSupportGroupRedirectInbox(currentGroup.ID).GetViewModels();

            SetupPages(page, ref viewResults);
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.GroupId = currentGroup.ID;
            ViewBag.InboxTitle = $"{ViewBag.GroupName}-{RadiusR.Localization.Model.RadiusR.GroupRedirectedInbox}";
            ViewBag.CanShare = true;
            ViewBag.IsRedirect = true;
            return View(viewName: "Inbox", model: viewResults);
        }

        [HttpGet]
        // GET: SupportRequest/GroupInProgress
        public ActionResult GroupInProgress(int id, int? page)
        {
            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var userCanGloballyRead = User.HasPermission("Global Support Request Read");
            var userGroups = User.GiveSupportGroups();
            if (!userCanGloballyRead && !userGroups.Any(item => item.GroupId == currentGroup.ID && item.IsLeader))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var viewResults = db.GetSupportGroupInProgressInbox(currentGroup.ID).GetViewModels();

            SetupPages(page, ref viewResults);
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.GroupId = currentGroup.ID;
            ViewBag.InboxTitle = $"{ViewBag.GroupName}-{RadiusR.Localization.Model.RadiusR.GroupInProgress}";
            return View(viewName: "Inbox", model: viewResults);
        }

        [HttpGet]
        // GET: SupportRequest/Inbox
        public ActionResult Inbox(int id, int? page)
        {
            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var userGroups = User.GiveSupportGroups();
            if (!userGroups.Any(item => item.GroupId == currentGroup.ID))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var currentUserId = User.GiveUserId();
            var viewResults = db.GetSupportUserInbox(currentGroup.ID, currentUserId.Value).GetViewModels();

            SetupPages(page, ref viewResults);
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.GroupId = currentGroup.ID;
            ViewBag.InboxTitle = $"{ViewBag.GroupName}-{RadiusR.Localization.Model.RadiusR.PersonalInbox}";
            return View(viewName: "Inbox", model: viewResults);
        }

        [HttpGet]
        // GET: SupportRequest/FinishedRequests
        public ActionResult FinishedRequests(int id, int? page)
        {
            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var userCanGloballyRead = User.HasPermission("Global Support Request Read");
            var userGroups = User.GiveSupportGroups();
            if (!userCanGloballyRead && !userGroups.Any(item => item.GroupId == currentGroup.ID && item.IsLeader))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var viewResults = db.GetSupportGroupFinishedRequests(currentGroup.ID).GetViewModels();

            SetupPages(page, ref viewResults);
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.GroupId = currentGroup.ID;
            ViewBag.InboxTitle = $"{ViewBag.GroupName}-{RadiusR.Localization.Pages.Common.FinishedRequests}";
            return View(viewName: "Inbox", model: viewResults);
        }
        #endregion
        #region Details
        [HttpGet]
        // GET: SupportRequest/Details
        public ActionResult Details(long id, int? groupId, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequest = db.SupportRequests.Find(id);
            if (currentSupportRequest == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var userGroupPermissions = GetSupportRequestPermissions(currentSupportRequest, groupId);

            if (!userGroupPermissions.CanRead)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.GroupPermissions = userGroupPermissions.BaseClaim;

            var viewResults = new SupportRequestDetailsViewModel()
            {
                RequestInfo = new SupportRequestListViewModel(currentSupportRequest),
                Stages = currentSupportRequest.SupportRequestProgresses.Select(srp => new SupportRequestStageViewModel()
                {
                    ActionType = srp.ActionType,
                    CommittingUser = srp.AppUserID.HasValue ? srp.AppUser.Name : null,
                    Date = srp.Date,
                    GroupName = srp.SetGroupID.HasValue ? srp.SupportGroup.Name : null,
                    ID = srp.ID,
                    IsVisibleToCustomer = srp.IsVisibleToCustomer,
                    Message = srp.Message,
                    NewState = srp.NewState,
                    OldState = srp.OldState
                }).ToArray()
            };

            var currentUserId = User.GiveUserId();
            groupId = groupId ?? ((currentSupportRequest.AssignedGroupID != null && currentSupportRequest.AssignedSupportGroup.SupportGroupUsers.Any(sgu => sgu.AppUserID == currentUserId)) ? currentSupportRequest.AssignedGroupID : null);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupId = groupId;
            ViewBag.GroupUsers = new SelectList(db.SupportGroupUsers.Where(sgu => sgu.SupportGroupID == groupId).Select(sgu => new { Value = sgu.AppUserID, Name = sgu.AppUser.Name }).ToArray(), "Value", "Name");
            ViewBag.RedirectGroups = new SelectList(db.SupportGroups.Where(sg => !sg.IsDisabled && sg.ID != groupId).Select(sg => new { Value = sg.ID, Name = sg.Name }).ToArray(), "Value", "Name");
            return View(viewResults);
        }

        [AjaxCall]
        [HttpPost]
        // POST: SupportRequest/RequestAttachmentList
        public ActionResult RequestAttachmentList(long id, int? groupId)
        {
            var currentSupportRequest = db.SupportRequests.Find(id);
            if (currentSupportRequest == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var userGroupPermissions = GetSupportRequestPermissions(currentSupportRequest, groupId);

            if (!userGroupPermissions.CanRead)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            var fileManager = new MasterISSFileManager();
            var results = fileManager.GetSupportRequestAttachmentList(currentSupportRequest.ID);
            if (results.InternalException != null)
            {
                return Json(new
                {
                    errorMessage = RadiusR.Localization.Errors.Common.FileManagerError
                });
            }

            return Json(new
            {
                errorMessage = (string)null,
                fileList = results.Result.Select(att => new
                {
                    stageId = att.StageId,
                    fileName = att.FileName,
                    fileExtention = att.FileExtention,
                    serverSideName = att.ServerSideName
                })
            });
        }

        [HttpGet]
        // GET: SupportRequest/GetSupportAttachment
        public ActionResult GetSupportAttachment(long id, string fileName, int? groupId)
        {
            var currentSupportRequest = db.SupportRequests.Find(id);
            if (currentSupportRequest == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var userGroupPermissions = GetSupportRequestPermissions(currentSupportRequest, groupId);

            if (!userGroupPermissions.CanRead)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            var fileManager = new MasterISSFileManager();
            var result = fileManager.GetSupportRequestAttachment(currentSupportRequest.ID, fileName);
            if (result.InternalException != null)
            {
                return Content(RadiusR.Localization.Pages.Common.FileManagerError);
            }

            return File(result.Result.Content, result.Result.FileDetail.MIMEType, $"{result.Result.FileDetail.FileName}.{result.Result.FileDetail.FileExtention}");
        }

        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/Details
        public ActionResult ProcessRequest(long id, int? groupId, string returnUrl, [Bind(Prefix = "processParameters")] SupportRequestProcessViewModel processParameters, HttpPostedFileBase[] attachments)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentSupportRequest = db.SupportRequests.Find(id);
            if (currentSupportRequest == null || currentSupportRequest.StateID == (short)SupportRequestStateID.Done)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var isCurrentlyAssigned = currentSupportRequest.AssignedGroupID.HasValue;

            // check permissions
            var currentGroupExtendedPermissions = GetSupportRequestPermissions(currentSupportRequest, groupId);
            var currentGroupPermissions = currentGroupExtendedPermissions.BaseClaim;
            if (currentGroupPermissions == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            // check attachments
            attachments = attachments ?? new HttpPostedFileBase[0];
            attachments = attachments.Where(att => att != null && att.ContentLength > 0).ToArray();
            if (attachments.Any(att => att.ContentLength > CustomerWebsiteSettings.MaxSupportAttachmentSize))
            {
                ModelState.AddModelError("attachments", RadiusR.Localization.Validation.Common.FileIsTooLarge);
            }

            if (processParameters.ActionType.HasValue)
            {
                if (processParameters.AddedMessage != null)
                    processParameters.AddedMessage = processParameters.AddedMessage.Trim(new[] { ' ', '\r', '\n' });
                switch ((SupportRequestActionTypes)processParameters.ActionType)
                {
                    case SupportRequestActionTypes.Create:
                        {
                            ModelState.Remove("processParameters.SelectedUserID");
                            ModelState.Remove("processParameters.SelectedGroupID");
                            if (ModelState.IsValid)
                            {
                                var newSupportRequestPrgress = new SupportRequestProgress()
                                {
                                    ActionType = processParameters.ActionType.Value,
                                    AppUserID = User.GiveUserId(),
                                    Date = DateTime.Now,
                                    Message = processParameters.AddedMessage,
                                    IsVisibleToCustomer = processParameters.IsVisibleToCustomer && currentGroupPermissions.CanWriteToCustomer,
                                    SetGroupID = currentGroupPermissions.GroupId,
                                    SupportRequestID = currentSupportRequest.ID
                                };
                                db.SupportRequestProgresses.Add(newSupportRequestPrgress);
                                db.SaveChanges();

                                var fileManager = new MasterISSFileManager();
                                bool errorOccured = false;
                                foreach (var file in attachments)
                                {
                                    var fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                                    var fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
                                    var results = fileManager.SaveSupportRequestAttachment(currentSupportRequest.ID, new FileManagerSupportRequestAttachmentWithContent(file.InputStream, new FileManagerSupportRequestAttachment(newSupportRequestPrgress.ID, fileName, fileExtention)));
                                    if (results.InternalException != null)
                                        errorOccured = true;
                                }

                                if (errorOccured)
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "28", uri);
                                else
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                        }
                        break;
                    case SupportRequestActionTypes.RedirectToGroup:
                        {
                            if (!currentGroupPermissions.IsLeader && !currentGroupPermissions.CanRedirect)
                            {
                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                            ModelState.Remove("processParameters.SelectedUserID");
                            if (ModelState.IsValid)
                            {
                                var newSupportRequestPrgress = new SupportRequestProgress()
                                {
                                    ActionType = processParameters.ActionType.Value,
                                    AppUserID = User.GiveUserId(),
                                    Date = DateTime.Now,
                                    Message = processParameters.AddedMessage,
                                    IsVisibleToCustomer = processParameters.IsVisibleToCustomer && currentGroupPermissions.CanWriteToCustomer,
                                    SetGroupID = currentGroupPermissions.GroupId,
                                    SupportRequestID = currentSupportRequest.ID
                                };
                                var selectedGroup = db.SupportGroups.Where(g => !g.IsDisabled).FirstOrDefault(g => g.ID == processParameters.SelectedGroupID);
                                if (selectedGroup == null)
                                {
                                    ModelState.AddModelError("processParameters.SelectedGroupID", RadiusR.Localization.Validation.Common.InvalidInput);
                                    break;
                                }
                                currentSupportRequest.RedirectedGroupID = selectedGroup.ID;
                                db.SupportRequestProgresses.Add(newSupportRequestPrgress);
                                db.SaveChanges();

                                var fileManager = new MasterISSFileManager();
                                bool errorOccured = false;
                                foreach (var file in attachments)
                                {
                                    var fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                                    var fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
                                    var results = fileManager.SaveSupportRequestAttachment(currentSupportRequest.ID, new FileManagerSupportRequestAttachmentWithContent(file.InputStream, new FileManagerSupportRequestAttachment(newSupportRequestPrgress.ID, fileName, fileExtention)));
                                    if (results.InternalException != null)
                                        errorOccured = true;
                                }

                                if (errorOccured)
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "28", uri);
                                else
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);

                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                        }
                        break;
                    case SupportRequestActionTypes.AssignToMember:
                        {
                            if (!currentGroupPermissions.CanAssignToStaff)
                            {
                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                            ModelState.Remove("processParameters.SelectedGroupID");
                            ModelState.Remove("processParameters.Message");
                            if (ModelState.IsValid)
                            {
                                var newSupportRequestPrgress = new SupportRequestProgress()
                                {
                                    ActionType = processParameters.ActionType.Value,
                                    AppUserID = User.GiveUserId(),
                                    Date = DateTime.Now,
                                    Message = processParameters.AddedMessage,
                                    IsVisibleToCustomer = processParameters.IsVisibleToCustomer && currentGroupPermissions.CanWriteToCustomer,
                                    SetGroupID = currentGroupPermissions.GroupId,
                                    SupportRequestID = currentSupportRequest.ID
                                };
                                currentSupportRequest.AssignedGroupID = currentGroupPermissions.GroupId;
                                var selectedUser = db.SupportGroups.Find(currentGroupPermissions.GroupId).SupportGroupUsers.FirstOrDefault(sgu => sgu.AppUserID == processParameters.SelectedUserID);
                                if (selectedUser == null)
                                {
                                    ModelState.AddModelError("processParameters.SelectedUserID", RadiusR.Localization.Validation.Common.InvalidInput);
                                    break;
                                }
                                currentSupportRequest.AssignedUserID = processParameters.SelectedUserID;
                                db.SupportRequestProgresses.Add(newSupportRequestPrgress);
                                db.SaveChanges();
                                // send SMS
                                if (!isCurrentlyAssigned && currentSupportRequest.SubscriptionID.HasValue && currentSupportRequest.IsVisibleToCustomer)
                                {
                                    var smsClient = new SMSService();
                                    db.SMSArchives.AddSafely(smsClient.SendSubscriberSMS(currentSupportRequest.Subscription, SMSType.SupportRequestInProgress, new Dictionary<string, object>() { { SMSParamaterRepository.SMSParameterNameCollection.SupportPIN, currentSupportRequest.SupportPin } }));
                                    db.SaveChanges();
                                }

                                var fileManager = new MasterISSFileManager();
                                bool errorOccured = false;
                                foreach (var file in attachments)
                                {
                                    var fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                                    var fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
                                    var results = fileManager.SaveSupportRequestAttachment(currentSupportRequest.ID, new FileManagerSupportRequestAttachmentWithContent(file.InputStream, new FileManagerSupportRequestAttachment(newSupportRequestPrgress.ID, fileName, fileExtention)));
                                    if (results.InternalException != null)
                                        errorOccured = true;
                                }

                                if (errorOccured)
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "28", uri);
                                else
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);

                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                        }
                        break;
                    case SupportRequestActionTypes.ChangeState:
                        {
                            if (!currentGroupPermissions.IsLeader && !currentGroupPermissions.CanChangeState)
                            {
                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                            ModelState.Remove("processParameters.SelectedUserID");
                            ModelState.Remove("processParameters.SelectedGroupID");
                            if (ModelState.IsValid)
                            {
                                var newSupportRequestPrgress = new SupportRequestProgress()
                                {
                                    ActionType = processParameters.ActionType.Value,
                                    AppUserID = User.GiveUserId(),
                                    Date = DateTime.Now,
                                    Message = processParameters.AddedMessage,
                                    IsVisibleToCustomer = true,
                                    SetGroupID = currentGroupPermissions.GroupId,
                                    SupportRequestID = currentSupportRequest.ID,
                                    OldState = currentSupportRequest.StateID,
                                    NewState = (short)SupportRequestStateID.Done
                                };
                                db.SupportRequestProgresses.Add(newSupportRequestPrgress);
                                currentSupportRequest.StateID = (short)SupportRequestStateID.Done;
                                currentSupportRequest.AssignedUserID = null;
                                currentSupportRequest.AssignedGroupID = currentGroupPermissions.GroupId;
                                db.SaveChanges();
                                // send SMS
                                if (currentSupportRequest.IsVisibleToCustomer)
                                {
                                    var smsClient = new SMSService();
                                    db.SMSArchives.AddSafely(smsClient.SendSubscriberSMS(currentSupportRequest.Subscription, SMSType.SupportRequestResolved, new Dictionary<string, object>() { { SMSParamaterRepository.SMSParameterNameCollection.SupportPIN, currentSupportRequest.SupportPin } }));
                                    db.SaveChanges();
                                }

                                var fileManager = new MasterISSFileManager();
                                bool errorOccured = false;
                                foreach (var file in attachments)
                                {
                                    var fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                                    var fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
                                    var results = fileManager.SaveSupportRequestAttachment(currentSupportRequest.ID, new FileManagerSupportRequestAttachmentWithContent(file.InputStream, new FileManagerSupportRequestAttachment(newSupportRequestPrgress.ID, fileName, fileExtention)));
                                    if (results.InternalException != null)
                                        errorOccured = true;
                                }

                                if (errorOccured)
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "28", uri);
                                else
                                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);

                                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                            }
                        }
                        break;
                    default:
                        {
                            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        }
                }
                ViewBag.ProcessParameters = processParameters;
                return Details(id, groupId, returnUrl);
            }
            else
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
        }
        #endregion
        #region Share Requests
        // GET: SupportRequest/ShareRequests
        public ActionResult ShareRequests(int id, string returnUrl, bool isRedirect)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var userCanGloballyModify = User.HasPermission("Global Support Request Change");
            var userGroups = User.GiveSupportGroups();
            if (!userCanGloballyModify && !userGroups.Any(item => item.GroupId == currentGroup.ID && item.IsLeader))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.GroupUserList = new MultiSelectList(currentGroup.SupportGroupUsers.Select(sgu => new { Name = sgu.AppUser.Name, Value = sgu.AppUserID }).OrderBy(sgu => sgu.Name), "Value", "Name");
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.TotalRequestCount = isRedirect ? db.GetSupportGroupRedirectInbox(currentGroup.ID).Count() : db.GetSupportGroupInbox(currentGroup.ID).Count();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: SupportRequest/ShareRequests
        public ActionResult ShareRequests(int id, string returnUrl, bool isRedirect, ShareSupportRequestsViewModel sharedUsers)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var currentGroup = db.SupportGroups.Find(id);
            if (currentGroup == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var userCanGloballyModify = User.HasPermission("Global Support Request Change");
            var userGroups = User.GiveSupportGroups();
            if (!userCanGloballyModify && !userGroups.Any(item => item.GroupId == currentGroup.ID && item.IsLeader))
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                sharedUsers.SelectedUserIds = sharedUsers.SelectedUserIds.Distinct();
                if (sharedUsers.SelectedUserIds.Except(currentGroup.SupportGroupUsers.Select(sgu => sgu.AppUserID)).Any() || !sharedUsers.SelectedUserIds.Any())
                {
                    ModelState.AddModelError("SelectedUserIds", RadiusR.Localization.Validation.Common.InvalidInput);
                }
                else
                {
                    // find relevant requests
                    SupportRequest[] relevantRequests;
                    if (isRedirect)
                    {
                        relevantRequests = db.GetSupportGroupRedirectInbox(currentGroup.ID).Include(sr => sr.Subscription).ToArray();
                    }
                    else
                    {
                        relevantRequests = db.GetSupportGroupInbox(currentGroup.ID).Include(sr => sr.Subscription).ToArray();
                    }
                    // assign all to this group
                    var assignmentState = new Dictionary<long, bool>();
                    foreach (var item in relevantRequests)
                    {
                        // status
                        assignmentState.Add(item.ID, item.AssignedGroupID.HasValue);
                        // assign group
                        item.AssignedGroupID = currentGroup.ID;
                    }
                    // share requests
                    var staffCount = sharedUsers.SelectedUserIds.Count();
                    var requestCount = relevantRequests.Count();
                    var sharingRatio = (decimal)requestCount / (decimal)staffCount;

                    var staffIds = sharedUsers.SelectedUserIds.ToArray();
                    var staffIterator = 0;
                    var currentUserID = User.GiveUserId().Value;
                    // assign requests
                    for (int i = 0; i < requestCount; i++)
                    {


                        relevantRequests[i].AssignedGroupID = currentGroup.ID;
                        relevantRequests[i].AssignedUserID = staffIds[staffIterator];
                        db.SupportRequestProgresses.Add(new SupportRequestProgress()
                        {
                            SupportRequestID = relevantRequests[i].ID,
                            ActionType = (short)SupportRequestActionTypes.AssignToMember,
                            IsVisibleToCustomer = false,
                            Date = DateTime.Now,
                            AppUserID = currentUserID,
                            SetGroupID = currentGroup.ID,
                            Message = RadiusR.Localization.Pages.Common.ShareRequests
                        });

                        if ((staffIterator + 1) * sharingRatio <= i + 1 && staffIterator < staffCount - 1)
                        {
                            staffIterator++;
                        }
                    }
                    // save
                    db.SaveChanges();
                    // SMS round
                    foreach (var currentSupportRequest in relevantRequests)
                    {
                        // send SMS
                        if (currentSupportRequest.SubscriptionID.HasValue && currentSupportRequest.IsVisibleToCustomer && !assignmentState[currentSupportRequest.ID])
                        {
                            var smsClient = new SMSService();
                            db.SMSArchives.AddSafely(smsClient.SendSubscriberSMS(currentSupportRequest.Subscription, SMSType.SupportRequestInProgress, new Dictionary<string, object>() { { SMSParamaterRepository.SMSParameterNameCollection.SupportPIN, currentSupportRequest.SupportPin } }));
                        }
                    }
                    // save SMSes
                    db.SaveChanges();

                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                }
            }

            ViewBag.GroupUserList = new MultiSelectList(currentGroup.SupportGroupUsers.Select(sgu => new { Name = sgu.AppUser.Name, Value = sgu.AppUserID }).OrderBy(sgu => sgu.Name), "Value", "Name", sharedUsers.SelectedUserIds);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.GroupName = currentGroup.Name;
            ViewBag.TotalRequestCount = isRedirect ? db.GetSupportGroupRedirectInbox(currentGroup.ID).Count() : db.GetSupportGroupInbox(currentGroup.ID).Count();
            return View(sharedUsers);
        }
        #endregion
        #region Management
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
                CanRedirect = sgu.CanRedirect,
                CanWriteToCustomer = sgu.CanWriteToCustomer
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
                        CanRedirect = addedUser.CanRedirect,
                        CanWriteToCustomer = addedUser.CanWriteToCustomer,
                        SupportGroupID = currentSupportGroup.ID,
                        CanAssignToStaff = addedUser.CanAssignToStaff
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
                CanChangeState = currentGroupUser.CanChangeState,
                CanRedirect = currentGroupUser.CanRedirect,
                CanWriteToCustomer = currentGroupUser.CanWriteToCustomer,
                CanAssignToStaff = currentGroupUser.CanAssignToStaff,
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
                currentGroupUser.CanRedirect = groupUser.CanRedirect;
                currentGroupUser.CanWriteToCustomer = groupUser.CanWriteToCustomer;
                currentGroupUser.CanAssignToStaff = groupUser.CanAssignToStaff;

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
        #endregion
        #region Private Methods
        private ExtendedSupportGroupClaim GetSupportRequestPermissions(SupportRequest supportRequest, int? currentGroupId)
        {

            // local check
            ExtendedSupportGroupClaim results = new ExtendedSupportGroupClaim(null);
            var userGroupClaims = User.GiveSupportGroups();
            if (currentGroupId.HasValue)
            {
                var groupClaim = userGroupClaims.FirstOrDefault(claim => claim.GroupId == currentGroupId);
                if (groupClaim != null)
                {
                    var isValid = ((supportRequest.AssignedGroupID == groupClaim.GroupId || supportRequest.RedirectedGroupID == groupClaim.GroupId || supportRequest.SupportRequestType.SupportGroups.Any(sg => sg.ID == groupClaim.GroupId)) && groupClaim.IsLeader) || (supportRequest.AssignedUserID == User.GiveUserId() && supportRequest.AssignedGroupID == groupClaim.GroupId);
                    if (isValid)
                        results = new ExtendedSupportGroupClaim(groupClaim);
                }
            }
            else
            {
                var standardClaim = userGroupClaims.FirstOrDefault(claim => claim.GroupId == supportRequest.AssignedGroupID);
                if (standardClaim != null)
                {
                    var isValid = (supportRequest.AssignedUserID == User.GiveUserId() && supportRequest.AssignedGroupID == standardClaim.GroupId) || standardClaim.IsLeader;
                    if (isValid)
                        results = new ExtendedSupportGroupClaim(standardClaim);
                }
            }

            // global check
            var userCanGloballyRead = User.HasPermission("Global Support Request Read");
            var userCanGloballyModify = User.HasPermission("Global Support Request Change");
            if (userCanGloballyModify)
            {
                var currentGroupID = currentGroupId ?? supportRequest.AssignedGroupID;
                if (currentGroupID.HasValue)
                {
                    results = new ExtendedSupportGroupClaim(new SupportGroupClaim()
                    {
                        CanChangeState = true,
                        CanRedirect = true,
                        CanWriteToCustomer = true,
                        CanAssignToStaff = true,
                        GroupId = currentGroupID.Value,
                        IsLeader = true
                    });
                }
            }
            else if (results == null && !userCanGloballyRead)
            {
                new ExtendedSupportGroupClaim(null, false);
            }

            return results;
        }
        #endregion
    }
}