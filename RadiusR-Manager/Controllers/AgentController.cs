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
using RadiusR.DB.QueryExtentions;

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
                    baseQuery = baseQuery.Where(a => a.IsEnabled);
                }
            }
            else
            {
                baseQuery = baseQuery.Where(a => a.IsEnabled);
            }

            var intermediateQuery = baseQuery.Select(a => new
            {
                ID = a.ID,
                Address = a.Address,
                Allowance = a.Allowance,
                CompanyTitle = a.CompanyTitle,
                Email = a.Email,
                ExecutiveName = a.ExecutiveName,
                IsEnabled = a.IsEnabled,
                PhoneNo = a.PhoneNo,
                TaxOffice = a.TaxOffice,
                TaxNo = a.TaxNo,
                SubCount = a.Subscriptions.Count()
            }).OrderByDescending(a => a.SubCount).AsQueryable();

            SetupPages(page, ref intermediateQuery);

            var viewResults = intermediateQuery.ToArray().Select(a => new AgentsListViewModel()
            {
                ID = a.ID,
                Address = new AddressViewModel(a.Address),
                _allowance = a.Allowance,
                CompanyTitle = a.CompanyTitle,
                Email = a.Email,
                ExecutiveName = a.ExecutiveName,
                IsEnabled = a.IsEnabled,
                PhoneNo = a.PhoneNo,
                TaxOffice = a.TaxOffice,
                TaxNo = a.TaxNo,
                SubCount = a.SubCount
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
                    IsEnabled = true,
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

            agent.IsEnabled = !agent.IsEnabled;
            agent.CustomerSetupUser.IsEnabled = agent.IsEnabled;

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

        [AuthorizePermission(Permissions = "Agent Settings")]
        [HttpGet]
        // GET: Agent/Settings
        public ActionResult Settings()
        {
            var settingsModel = new AgentsSettingsViewModel()
            {
                AgentsNonCashPaymentCommission = AgentsSettings.AgentsNonCashPaymentCommission
            };

            return View(settingsModel);
        }

        [AuthorizePermission(Permissions = "Agent Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/Settings
        public ActionResult Settings(AgentsSettingsViewModel settingsModel)
        {
            if (ModelState.IsValid)
            {
                AgentsSettings.Update(settingsModel);
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settingsModel);
        }

        [AuthorizePermission(Permissions = "Agent Allowances")]
        [HttpGet]
        // GET: Agent/Allowances
        public ActionResult Allowances(int? page, AgentAllowancesSearchViewModel search)
        {
            var baseQuery = db.AgentCollections.OrderByDescending(ac => ac.CreationDate).AsQueryable();
            if (search != null)
            {
                if (search.CreationDateStart.HasValue)
                {
                    baseQuery = baseQuery.Where(ac => ac.CreationDate >= search.CreationDateStart);
                }
                if (search.CreationDateEnd.HasValue)
                {
                    baseQuery = baseQuery.Where(ac => ac.CreationDate <= search.CreationDateEnd);
                }
                if (search.PaymentDateStart.HasValue)
                {
                    baseQuery = baseQuery.Where(ac => ac.PaymentDate >= search.PaymentDateStart);
                }
                if (search.PaymentDateEnd.HasValue)
                {
                    baseQuery = baseQuery.Where(ac => ac.PaymentDate <= search.PaymentDateEnd);
                }
                if (search.AgentID.HasValue)
                {
                    baseQuery = baseQuery.Where(ac => ac.AgentID == search.AgentID);
                }
            }

            var viewResults = baseQuery.Select(ac => new AgentCollectionViewModel()
            {
                ID = ac.ID,
                AgentName = ac.Agent.CompanyTitle,
                CreationDate = ac.CreationDate,
                CreatorName = ac.Creator.Name,
                PaymentDate = ac.PaymentDate,
                PayerName = ac.Payer.Name,
                _allowanceAmount = ac.Bills.SelectMany(b => b.BillFees.Where(bf => bf.FeeTypeID == (short)RadiusR.DB.Enums.FeeType.Tariff)).Select(bf => bf.CurrentCost - (bf.Discount != null ? bf.Discount.Amount : 0m)).DefaultIfEmpty(0m).Sum()
            });

            SetupPages(page, ref viewResults);

            ViewBag.Search = search;
            ViewBag.Agents = new SelectList(db.Agents.OrderBy(a => a.CompanyTitle).Select(a => new { Name = a.CompanyTitle, Value = a.ID }), "Value", "Name", search?.AgentID);

            return View(viewResults);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/WorkAreas
        public ActionResult WorkAreas(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            var viewResults = db.WorkAreas.Where(wa => wa.Agent.ID == dbAgent.ID).OrderBy(wa => wa.ID).Select(wa => new PartnerWorkAreaViewModel()
            {
                ID = wa.ID,
                ProvinceID = wa.ProvinceID,
                DistrictID = wa.DistrictID,
                RuralCode = wa.RuralCode,
                NeighbourhoodID = wa.NeighbourhoodID,
                DistrictName = wa.DistrictName,
                NeighbourhoodName = wa.NeighbourhoodName,
                ProvinceName = wa.ProvinceName
            });

            SetupPages(page, ref viewResults);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.AgentName = dbAgent.CompanyTitle;

            return View(viewResults.ToArray());
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/AddWorkArea
        public ActionResult AddWorkArea(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.AgentName = dbAgent.CompanyTitle;

            return View();
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/AddWorkArea
        public ActionResult AddWorkArea(int id, string returnUrl, PartnerWorkAreaViewModel workArea)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (ModelState.IsValid)
            {
                if (dbAgent.WorkAreas.Any(wa => wa.ProvinceID == workArea.ProvinceID && !wa.DistrictID.HasValue && !wa.RuralCode.HasValue && !wa.NeighbourhoodID.HasValue) ||
                    (workArea.DistrictID.HasValue && dbAgent.WorkAreas.Any(wa => wa.DistrictID == workArea.DistrictID && !wa.RuralCode.HasValue && !wa.NeighbourhoodID.HasValue)) ||
                    (workArea.RuralCode.HasValue && dbAgent.WorkAreas.Any(wa => wa.RuralCode == workArea.RuralCode && !wa.NeighbourhoodID.HasValue)) ||
                    (workArea.NeighbourhoodID.HasValue && dbAgent.WorkAreas.Any(wa => wa.NeighbourhoodID == workArea.NeighbourhoodID)))
                {
                    ViewBag.ConflictError = RadiusR.Localization.Validation.Common.PartnerWorkAreaConflict;
                }
                else
                {
                    // remove inclusive areas
                    {
                        IEnumerable<WorkArea> inclusiveAreas = Enumerable.Empty<WorkArea>();
                        if (!workArea.DistrictID.HasValue)
                        {
                            inclusiveAreas = dbAgent.WorkAreas.Where(wa => wa.ProvinceID == workArea.ProvinceID).ToArray();
                        }
                        else if (!workArea.RuralCode.HasValue)
                        {
                            inclusiveAreas = dbAgent.WorkAreas.Where(wa => wa.DistrictID == workArea.DistrictID).ToArray();
                        }
                        else if (!workArea.NeighbourhoodID.HasValue)
                        {
                            inclusiveAreas = dbAgent.WorkAreas.Where(wa => wa.RuralCode == workArea.RuralCode).ToArray();
                        }

                        if (inclusiveAreas.Any())
                            db.WorkAreas.RemoveRange(inclusiveAreas);
                    }

                    // add new work area
                    dbAgent.WorkAreas.Add(new WorkArea()
                    {
                        DistrictID = workArea.DistrictID,
                        DistrictName = workArea.DistrictName,
                        NeighbourhoodID = workArea.NeighbourhoodID,
                        NeighbourhoodName = workArea.NeighbourhoodName,
                        ProvinceID = workArea.ProvinceID,
                        ProvinceName = workArea.ProvinceName,
                        RuralCode = workArea.RuralCode
                    });

                    db.SaveChanges();

                    return RedirectToAction("WorkAreas", new { id = dbAgent.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
                }
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.PartnerName = dbAgent.CompanyTitle;

            return View(workArea);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/RemoveWorkArea
        public ActionResult RemoveWorkArea(int id, string returnUrl, long workAreaID)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });
            var dbWorkArea = db.WorkAreas.Find(workAreaID);
            if (dbWorkArea == null)
                return RedirectToAction("WorkAreas", new { id = dbAgent.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            db.WorkAreas.Remove(dbWorkArea);

            db.SaveChanges();

            return RedirectToAction("WorkAreas", new { id = dbAgent.ID, returnUrl = uri.Uri.PathAndQuery + uri.Fragment, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/Tariffs
        public ActionResult Tariffs(int id, string returnUrl, int? page)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            var viewResults = db.AgentTariffs.OrderBy(at => at.Service.Name).Where(at => at.AgentID == dbAgent.ID).Select(at => new AgentTariffViewModel()
            {
                TariffID = at.TariffID,
                TariffName = at.Service.Name,
                _price = at.Service.Price,
                IsActive = at.Service.IsActive,
                DomainID = at.DomainID,
                DomainName = at.Domain.Name
            });

            SetupPages(page, ref viewResults);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.AgentName = dbAgent.CompanyTitle;

            return View(viewResults.ToArray());
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [HttpGet]
        // GET: Agent/AddTariff
        public ActionResult AddTariff(int id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.AgentName = dbAgent.CompanyTitle;
            ViewBag.DomainList = new SelectList(db.Domains.ToArray(), "ID", "Name");
            //ViewBag.Tariffs = new SelectList(db.Services.OrderBy(s => s.Name).FilterActiveServices().Where(s => !s.Agents.Any(a => a.ID == dbAgent.ID)).Select(s => new { Name = s.Name, Value = s.ID }), "Value", "Name");

            return View();
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/AddTariff
        public ActionResult AddTariff(int id, string returnUrl, AddAgentTariffViewModel tariff)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            if (ModelState.IsValid)
            {
                var dbTariff = db.Services.Find(tariff.TariffID);
                if (dbAgent.AgentTariffs.Any(at => at.TariffID == tariff.TariffID && at.DomainID == tariff.DomianID))
                {
                    ModelState.AddModelError("TariffID", RadiusR.Localization.Validation.Common.ValueAlreadyExists);
                }
                else
                {
                    if (dbTariff == null || !dbTariff.IsActive)
                    {
                        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                    }

                    dbAgent.AgentTariffs.Add(new AgentTariff()
                    {
                        DomainID = tariff.DomianID.Value,
                        TariffID = tariff.TariffID.Value
                    });
                    db.SaveChanges();

                    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                }
            }

            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;
            ViewBag.AgentName = dbAgent.CompanyTitle;
            ViewBag.DomainList = new SelectList(db.Domains.ToArray(), "ID", "Name", tariff.DomianID);
            //if (tariff.DomianID.HasValue)
            //    ViewBag.Tariffs = new SelectList(db.Services.OrderBy(s => s.Name).FilterActiveServices().Where(s => s.Domains.Select(d => d.ID).Contains(tariff.DomianID)).Where(s => !s.AgentTariffs.Any(at => at.AgentID == dbAgent.ID)).Select(s => new { Name = s.Name, Value = s.ID }), "Value", "Name", tariff?.TariffID);

            return View(tariff);
        }

        [AuthorizePermission(Permissions = "Modify Agents")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Agent/RemoveTariff
        public ActionResult RemoveTariff(int id, string returnUrl, int tariffId, int domainId)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);

            var dbAgent = db.Agents.Find(id);
            if (dbAgent == null)
                return RedirectToAction("Index", new { errorMessage = 9 });
            var dbAgentTariff = db.AgentTariffs.FirstOrDefault(at => at.TariffID == tariffId && at.DomainID == domainId && at.AgentID == id);
            if (dbAgentTariff == null)
                return RedirectToAction("Index", new { errorMessage = 9 });


            dbAgent.AgentTariffs.Remove(dbAgentTariff);
            db.SaveChanges();

            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            return RedirectToAction("Tariffs", new { errorMessage = 0, id = id, returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
        }
    }
}