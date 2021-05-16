using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.ViewModels.Search;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.Extentions;
using RezaB.Web;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "Agents")]
    public class AgentController : BaseController
    {
        private RadiusREntities db = new RadiusREntities();
        // GET: Agent
        public ActionResult Index(int? page, AgentSearchViewModel search)
        {
            var baseQuery = db.Agents.Include(a => a.Address).OrderByDescending(a => a.CompanyTitle).AsQueryable();
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.CompanyTitle))
                {
                    baseQuery = baseQuery.Where(a => a.CompanyTitle.Contains(search.CompanyTitle));
                }
                if (!search.ShowDisabled)
                {
                    baseQuery = baseQuery.Where(a => a.IsEnanbled);
                }
            }
            else
            {
                baseQuery = baseQuery.Where(a => a.IsEnanbled);
            }

            SetupPages(page, ref baseQuery);
            var viewResults = baseQuery.ToArray().Select(a => new AgentsListViewModel()
            {
                ID = a.ID,
                Address = new AddressViewModel(a.Address),
                _allowance = a.Allowance,
                CompanyTitle = a.CompanyTitle,
                Email = a.Email,
                ExecutiveName = a.ExecutiveName,
                IsEnabled = a.IsEnanbled,
                PhoneNo = a.PhoneNo,
                TaxOffice = a.TaxOffice,
                TaxNo = a.TaxNo,
            });

            ViewBag.Search = search;
            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/Add
        public ActionResult Add()
        {
            var agentModel = new AgentViewModel() { Password = RadiusR.DB.Passwords.PasswordUtilities.GenerateSecurePassword(8) };
            return View(agentModel);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/Add
        public ActionResult Add(AgentViewModel agentModel)
        {
            if (ModelState.IsValid)
            {
                if (db.Agents.Any(a => a.Email == agentModel.Email))
                {
                    ModelState.AddModelError("Email", RadiusR.Localization.Validation.Common.UsernameExists);
                    return View(agentModel);
                }

                var agent = new Agent()
                {
                    Address = agentModel.Address.GetDatabaseObject(),
                    Allowance = agentModel._allowance.Value,
                    CompanyTitle = agentModel.CompanyTitle,
                    Email = agentModel.Email,
                    ExecutiveName = agentModel.ExecutiveName,
                    IsEnanbled = true,
                    Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(agentModel.Password),
                    PhoneNo = agentModel.PhoneNo,
                    TaxNo = agentModel.TaxNo,
                    TaxOffice = agentModel.TaxOffice,
                    CustomerSetupUser = new CustomerSetupUser()
                    {
                        IsEnabled = true,
                        Username = RadiusR.DB.RandomCode.RandomUsernameGenerator.GenerateUniqueSetupServiceUsername(32),
                        Password = RadiusR.DB.Passwords.PasswordUtilities.HashLowSecurityPassword(RadiusR.DB.Passwords.PasswordUtilities.GenerateSecurePassword(16)),
                        Name = new string(agentModel.CompanyTitle.Take(100).ToArray())
                    }
                };
                db.Agents.Add(agent);
                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }
            return View(agentModel);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/Edit
        public ActionResult Edit(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var agent = db.Agents.Find(id);
            if (agent == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var agentModel = new AgentViewModel()
            {
                Address = new AddressViewModel(agent.Address),
                _allowance = agent.Allowance,
                CompanyTitle = agent.CompanyTitle,
                Email = agent.Email,
                ExecutiveName = agent.ExecutiveName,
                PhoneNo = agent.PhoneNo,
                TaxNo = agent.TaxNo,
                TaxOffice = agent.TaxOffice
            };

            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(viewName: "Add", model: agentModel);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/Edit
        public ActionResult Edit(int id, string returnUrl, AgentViewModel agentModel)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var agent = db.Agents.Find(id);
            if (agent == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            if (db.Agents.Any(a => a.Email == agentModel.Email && a.ID != agent.ID))
            {
                ModelState.AddModelError("Email", RadiusR.Localization.Validation.Common.UsernameExists);
                ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
                return View(viewName: "Add", model: agentModel);
            }

            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                agentModel.Address.CopyToDBObject(agent.Address);
                agent.Allowance = agentModel._allowance.Value;
                agent.CompanyTitle = agentModel.CompanyTitle;
                agent.Email = agentModel.Email;
                agent.ExecutiveName = agentModel.ExecutiveName;
                agent.PhoneNo = agentModel.PhoneNo;
                agent.TaxNo = agentModel.TaxNo;
                agent.TaxOffice = agentModel.TaxOffice;
                agent.CustomerSetupUser.Name = new string(agentModel.CompanyTitle.Take(100).ToArray());

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(viewName: "Add", model: agentModel);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/ToggleState
        public ActionResult ToggleState(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var agent = db.Agents.Find(id);
            if (agent == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            agent.IsEnanbled = !agent.IsEnanbled;
            agent.CustomerSetupUser.IsEnabled = agent.IsEnanbled;

            db.SaveChanges();

            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/ChangePassword
        public ActionResult ChangePassword(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var agent = db.Agents.Find(id);
            if (agent == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.AgentName = agent.CompanyTitle;
            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View();
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/ChangePassword
        public ActionResult ChangePassword(int id, string returnUrl, AgentPasswordUpdateViewModel passwordModel)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var agent = db.Agents.Find(id);
            if (agent == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                agent.Password = RadiusR.DB.Passwords.PasswordUtilities.HashPassword(passwordModel.Password);

                db.SaveChanges();

                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.AgentName = agent.CompanyTitle;
            ViewBag.BackUrl = uri.Uri.PathAndQuery + uri.Fragment;
            return View(passwordModel);
        }
    }
}