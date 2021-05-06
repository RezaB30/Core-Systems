using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR_Manager.Models.ViewModels.Customer;
using RadiusR_Manager.Models.ViewModels;
using RadiusR.DB;
using System.Data.Entity;
using RadiusR.DB.Enums;
using RadiusR.DB.DomainsCache;
using RezaB.TurkTelekom.WebServices.TTApplication;
using RadiusR.SystemLogs;
using RezaB.TurkTelekom.WebServices;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using RadiusR.DB.TelekomOperations;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration;
using RadiusR.DB.QueryExtentions;
using RezaB.Web;
using RezaB.Data.Localization;
using RezaB.Web.Authentication;
using RadiusR_Manager.Models.ViewModels.ClientStates;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        // GET: Client/Register
        public ActionResult Register()
        {
            ViewBag.Domains = new SelectList(DomainsCache.GetAllDomains().Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", null);
            ViewBag.Services = new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.BillingPeriods = new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.GroupList = new MultiSelectList(db.Groups.GetValidGroupsForSubscriptions().ToList(), "ID", "Name");
            ViewBag.AvailableFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray().Where(f => f.CanBeInitial).Select(f => new SubscriberFeesAddViewModel()
            {
                FeeTypeID = f.FeeTypeID,
                FeeTypeName = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(f.FeeTypeID),
                IsAllTime = f.IsAllTime,
                Variants = f.FeeTypeVariants.Any() ? f.FeeTypeVariants.Select(fv => new SubscriberFeesAddViewModel.FeeVariant()
                {
                    ID = fv.ID,
                    Name = fv.Title
                }) : null,
                CustomFees = !f.Cost.HasValue && !f.FeeTypeVariants.Any() ? Enumerable.Empty<SubscriberFeesAddViewModel.CustomFee>() : null
            }).ToArray();

            var validSpecialOffers = db.SpecialOffers.FilterActiveSpecialOffers().Select(so => new { Key = so.ID, Value = so.Name }).ToArray();
            ViewBag.SpecialOfferList = new SelectList(validSpecialOffers, "Key", "Value", (validSpecialOffers.Count() == 1) ? validSpecialOffers.FirstOrDefault().Key : (int?)null);

            return View(viewName: "AddWizard/Register", model: new CustomerRegistrationViewModel() { SubscriptionInfo = new CustomerSubscriptionViewModel() { RegistrationType = (short)SubscriptionRegistrationType.NewRegistration } });
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/Register
        public ActionResult Register(CustomerRegistrationViewModel registeredCustomer)
        {
            var dbFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray();
            var availableFees = dbFees.Where(f => f.CanBeInitial).Select(f => new SubscriberFeesAddViewModel()
            {
                FeeTypeID = f.FeeTypeID,
                FeeTypeName = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(f.FeeTypeID),
                IsAllTime = f.IsAllTime,
                Variants = f.FeeTypeVariants.Any() ? f.FeeTypeVariants.Select(fv => new SubscriberFeesAddViewModel.FeeVariant()
                {
                    ID = fv.ID,
                    Name = fv.Title
                }) : null,
                CustomFees = !f.Cost.HasValue && !f.FeeTypeVariants.Any() ? Enumerable.Empty<SubscriberFeesAddViewModel.CustomFee>() : null
            }).ToArray();
            {
                var selectedDomain = DomainsCache.GetDomainByID(registeredCustomer.SubscriptionInfo.DomainID);
                if (selectedDomain != null)
                {
                    UpdateTelekomPacketFromCache(registeredCustomer.SubscriptionInfo.TelekomDetailedInfo.TelekomTariffInfo, selectedDomain);
                    ViewBag.SelectedDomain = selectedDomain;
                }
            }

            if (!User.HasPermission("Referral Special Offer Usage"))
            {
                registeredCustomer.SubscriptionInfo.ReferralDiscount = null;
            }

            FixSubscriptionAddedFeesModelState("SubscriptionInfo.AddedFeesInfo", registeredCustomer.SubscriptionInfo.AddedFeesInfo, availableFees);
            FixCustomerAddressesModelState(registeredCustomer);
            FixCustomerTypeModelState(registeredCustomer);
            FixCustomerSubscriptionModelState("SubscriptionInfo", registeredCustomer.SubscriptionInfo);

            if (ModelState.IsValid)
            {
                var tckValidationResults = ValidateTCK(registeredCustomer.IDCard);
                if ((!User.HasPermission("Skip TCK Validation") || registeredCustomer.IDCard.SkipValidation != true) && (tckValidationResults.ResultType != TCKValidationResult.TCKValidationResultType.Success && tckValidationResults.ResultType != TCKValidationResult.TCKValidationResultType.NoValidationMethod))
                {
                    ViewBag.TCKValidationResults = tckValidationResults.ResultContent;
                    if (User.HasPermission("Skip TCK Validation"))
                        ViewBag.SkipValidation = true;
                }
                else
                {
                    Customer dbCustomer;
                    var registrationInfo = CreateCustomerInfoForRegistration(registeredCustomer);
                    var registrationResults = db.RegisterSubscriptionWithNewCustomer(registrationInfo, out dbCustomer, true);
                    if (!registrationResults.IsSuccess)
                    {
                        ViewBag.RegistrationValidations = registrationResults.ValidationMessages;
                        foreach (var item in registrationResults.ValidationMessages)
                        {
                            ModelState.AddModelError(item.Key, string.Join(Environment.NewLine, item.Select(ve => ve)));
                        }
                    }
                    else
                    {
                        // if new customer add it
                        if (registrationResults.DoesCustomerExist == false)
                            db.Customers.Add(dbCustomer);
                        // save db
                        db.SaveChanges();

                        // logs
                        var dbSubscription = dbCustomer.Subscriptions.OrderByDescending(s => s.MembershipDate).FirstOrDefault();
                        db.SystemLogs.Add(SystemLogProcessor.AddSubscription(User.GiveUserId(), dbSubscription.ID, dbCustomer.ID, (SubscriptionRegistrationType)dbSubscription.RegistrationType, SystemLogInterface.MasterISS, null, dbSubscription.SubscriberNo));
                        db.SaveChanges();

                        return RedirectToAction("Index", "Client", new { errorMessage = 0 });
                    }
                }
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetAllDomains().Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", registeredCustomer.SubscriptionInfo.DomainID != 0 ? registeredCustomer.SubscriptionInfo.DomainID : (int?)null);
            ViewBag.Services = registeredCustomer.SubscriptionInfo.DomainID != 0 && registeredCustomer.SubscriptionInfo.ServiceID != 0 ? new SelectList(db.Domains.Find(registeredCustomer.SubscriptionInfo.DomainID).Services.AsQueryable().FilterActiveServices().ToArray(), "ID", "Name", registeredCustomer.SubscriptionInfo.ServiceID) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.BillingPeriods = registeredCustomer.SubscriptionInfo.DomainID != 0 && registeredCustomer.SubscriptionInfo.ServiceID != 0 && registeredCustomer.SubscriptionInfo.BillingPeriod != 0 ? new SelectList(db.Services.Find(registeredCustomer.SubscriptionInfo.ServiceID).ServiceBillingPeriods, "DayOfMonth", "DayOfMonth", registeredCustomer.SubscriptionInfo.BillingPeriod) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.GroupList = new MultiSelectList(db.Groups.GetValidGroupsForSubscriptions().ToList(), "ID", "Name", registeredCustomer.SubscriptionInfo.GroupIds);
            ViewBag.AvailableFees = availableFees;

            var validSpecialOffers = db.SpecialOffers.FilterActiveSpecialOffers().Select(so => new { Key = so.ID, Value = so.Name }).ToArray();
            ViewBag.SpecialOfferList = new SelectList(validSpecialOffers, "Key", "Value", registeredCustomer.SubscriptionInfo.ReferralDiscount != null ? registeredCustomer.SubscriptionInfo.ReferralDiscount.SpecialOfferID : null);

            return View(viewName: "AddWizard/Register", model: registeredCustomer);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        // GET: Client/AddSubscription
        public ActionResult AddSubscription(long id)
        {
            var referenceSubscription = db.Subscriptions.Find(id);
            if (referenceSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetAllDomains().Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", null);
            ViewBag.Services = new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.BillingPeriods = new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.GroupList = new MultiSelectList(db.Groups.GetValidGroupsForSubscriptions().ToList(), "ID", "Name");
            ViewBag.AvailableFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray().Where(f => f.CanBeInitial).Select(f => new SubscriberFeesAddViewModel()
            {
                FeeTypeID = f.FeeTypeID,
                FeeTypeName = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(f.FeeTypeID),
                IsAllTime = f.IsAllTime,
                Variants = f.FeeTypeVariants.Any() ? f.FeeTypeVariants.Select(fv => new SubscriberFeesAddViewModel.FeeVariant()
                {
                    ID = fv.ID,
                    Name = fv.Title
                }) : null,
                CustomFees = !f.Cost.HasValue && !f.FeeTypeVariants.Any() ? Enumerable.Empty<SubscriberFeesAddViewModel.CustomFee>() : null
            }).ToArray();

            var validSpecialOffers = db.SpecialOffers.FilterActiveSpecialOffers().Select(so => new { Key = so.ID, Value = so.Name }).ToArray();
            ViewBag.SpecialOfferList = new SelectList(validSpecialOffers, "Key", "Value", (validSpecialOffers.Count() == 1) ? validSpecialOffers.FirstOrDefault().Key : (int?)null);

            ViewBag.CustomerName = referenceSubscription.ValidDisplayName;
            return View(viewName: "AddWizard/AddSubscription", model: new CustomerSubscriptionViewModel() { RegistrationType = (short)SubscriptionRegistrationType.NewRegistration });
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/AddSubscription
        public ActionResult AddSubscription(long id, CustomerSubscriptionViewModel subscription)
        {
            var referenceSubscription = db.Subscriptions.Find(id);
            if (referenceSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var dbFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray();
            var availableFees = dbFees.Where(f => f.CanBeInitial).Select(f => new SubscriberFeesAddViewModel()
            {
                FeeTypeID = f.FeeTypeID,
                FeeTypeName = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(f.FeeTypeID),
                IsAllTime = f.IsAllTime,
                Variants = f.FeeTypeVariants.Any() ? f.FeeTypeVariants.Select(fv => new SubscriberFeesAddViewModel.FeeVariant()
                {
                    ID = fv.ID,
                    Name = fv.Title
                }) : null,
                CustomFees = !f.Cost.HasValue && !f.FeeTypeVariants.Any() ? Enumerable.Empty<SubscriberFeesAddViewModel.CustomFee>() : null
            }).ToArray();
            {
                var selectedDomain = DomainsCache.GetDomainByID(subscription.DomainID);
                if (selectedDomain != null)
                {
                    UpdateTelekomPacketFromCache(subscription.TelekomDetailedInfo.TelekomTariffInfo, selectedDomain);
                    ViewBag.SelectedDomain = selectedDomain;
                }
            }

            FixSubscriptionAddedFeesModelState("AddedFeesInfo", subscription.AddedFeesInfo, availableFees);
            FixCustomerSubscriptionModelState(string.Empty, subscription);

            if (ModelState.IsValid)
            {
                var dbCustomer = db.Customers.Find(referenceSubscription.CustomerID);
                var registrationInfo = CreateSubscriptionInfoForRegistration(subscription);
                var registrationResults = db.RegisterSubscriptionForExistingCustomer(registrationInfo, dbCustomer);
                if (!registrationResults.IsSuccess)
                {
                    ViewBag.RegistrationValidations = registrationResults.ValidationMessages;
                    foreach (var item in registrationResults.ValidationMessages)
                    {
                        ModelState.AddModelError(item.Key, string.Join(Environment.NewLine, item.Select(ve => ve)));
                    }
                }
                else
                {
                    // save db
                    db.SaveChanges();
                    // logs
                    var dbSubscription = dbCustomer.Subscriptions.OrderByDescending(s => s.ID).FirstOrDefault();
                    db.SystemLogs.Add(SystemLogProcessor.AddSubscription(User.GiveUserId(), dbSubscription.ID, referenceSubscription.CustomerID, (SubscriptionRegistrationType)dbSubscription.RegistrationType, SystemLogInterface.MasterISS, null, dbSubscription.SubscriberNo));
                    db.SaveChanges();

                    return RedirectToAction("Index", "Client", new { errorMessage = 0 });
                }
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetAllDomains().Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", subscription.DomainID != 0 ? subscription.DomainID : (int?)null);
            ViewBag.Services = subscription.DomainID != 0 && subscription.ServiceID != 0 ? new SelectList(db.Domains.Find(subscription.DomainID).Services, "ID", "Name", subscription.ServiceID) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.BillingPeriods = subscription.DomainID != 0 && subscription.ServiceID != 0 && subscription.BillingPeriod != 0 ? new SelectList(db.Services.Find(subscription.ServiceID).ServiceBillingPeriods, "DayOfMonth", "DayOfMonth", subscription.BillingPeriod) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.GroupList = new MultiSelectList(db.Groups.GetValidGroupsForSubscriptions().ToList(), "ID", "Name", subscription.GroupIds);
            ViewBag.AvailableFees = availableFees;
            ViewBag.CustomerName = referenceSubscription.ValidDisplayName;

            var validSpecialOffers = db.SpecialOffers.FilterActiveSpecialOffers().Select(so => new { Key = so.ID, Value = so.Name }).ToArray();
            ViewBag.SpecialOfferList = new SelectList(validSpecialOffers, "Key", "Value", subscription.ReferralDiscount != null ? subscription.ReferralDiscount.SpecialOfferID : null);

            return View(viewName: "AddWizard/AddSubscription", model: subscription);
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpGet]
        // GET: Client/SendTelekomRegistration
        public ActionResult SendTelekomRegistration(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;

            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.Common.DomainNotFound;
                return View(viewName: "AddWizard/SendTelekomRegistration");
            }
            else if (domain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            else
            {
                ViewBag.SelectedDomain = domain;
                var selectedPacket = new
                {
                    PacketCode = 0,
                    TariffCode = 0
                };
                if (dbSubscription.SubscriptionTelekomInfo != null)
                {
                    selectedPacket = new
                    {
                        PacketCode = dbSubscription.SubscriptionTelekomInfo.PacketCode ?? 0,
                        TariffCode = dbSubscription.SubscriptionTelekomInfo.TariffCode ?? 0
                    };
                }
                var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, selectedPacket.PacketCode, selectedPacket.TariffCode);

                var telekomTariffViewModel = new UpdateTelekomInfoBeforeSendViewModel()
                {
                    TelekomTariffInfo = telekomTariff != null ? new TelekomTariffHelperViewModel(telekomTariff) : new TelekomTariffHelperViewModel(),
                    PSTN = dbSubscription.SubscriptionTelekomInfo.PSTN
                };
                return View(viewName: "AddWizard/SendTelekomRegistration", model: telekomTariffViewModel);
            }
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/SendTelekomRegistration
        public ActionResult SendTelekomRegistration(long id, string returnUrl, UpdateTelekomInfoBeforeSendViewModel TTPacket)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;

            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            ViewBag.SelectedDomain = domain;
            if (domain == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            else if (domain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            else if (ModelState.IsValid)
            {
                var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, TTPacket.TelekomTariffInfo.PacketCode.Value, TTPacket.TelekomTariffInfo.TariffCode.Value);
                if (telekomTariff == null)
                {
                    ViewBag.ErrorMessage = RadiusR.Localization.Validation.Common.InvalidTelekomPacket;
                    return View(viewName: "AddWizard/SendTelekomRegistration", model: TTPacket);
                }
                else
                {
                    if (dbSubscription.SubscriptionTelekomInfo == null)
                        dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo();
                    dbSubscription.SubscriptionTelekomInfo.SubscriptionNo = dbSubscription.SubscriptionTelekomInfo.SubscriptionNo ?? " ";
                    dbSubscription.SubscriptionTelekomInfo.TTCustomerCode = dbSubscription.SubscriptionTelekomInfo.TTCustomerCode > 0 ? dbSubscription.SubscriptionTelekomInfo.TTCustomerCode : domain.TelekomCredential.XDSLWebServiceCustomerCodeInt;
                    dbSubscription.SubscriptionTelekomInfo.TariffCode = TTPacket.TelekomTariffInfo.TariffCode.Value;
                    dbSubscription.SubscriptionTelekomInfo.PacketCode = TTPacket.TelekomTariffInfo.PacketCode.Value;
                    dbSubscription.SubscriptionTelekomInfo.XDSLType = TTPacket.TelekomTariffInfo.XDSLType.Value;
                    dbSubscription.SubscriptionTelekomInfo.PSTN = TTPacket.PSTN;

                    // save TT packet change
                    db.SaveChanges();

                    try
                    {
                        var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new RegisterSubscriptionOptions()
                        {
                            AppUserID = User.GiveUserId(),
                            LogInterface = SystemLogInterface.MasterISS
                        });

                        if (results.IsFatal)
                        {
                            throw results.InternalException;
                        }
                        else if (results.IsSuccess)
                        {
                            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        }
                        else
                        {
                            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                            TempData["ErrorResults"] = results;
                            return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }

                    return View(viewName: "AddWizard/SendTelekomRegistration", model: TTPacket);
                }

            }

            return View(viewName: "AddWizard/SendTelekomRegistration", model: TTPacket);
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpGet]
        // GET: Client/PrepareTransition
        public ActionResult PrepareTransition(long id, string returnUrl)
        {
            //var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            //UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            //ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            //var dbSubscription = db.Subscriptions.Find(id);
            //if (dbSubscription == null)
            //{
            //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
            //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            //}
            //var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            //if (domain == null)
            //{
            //    ViewBag.ErrorMessage = RadiusR.Localization.Pages.Common.DomainNotFound;
            //    return View(viewName: "AddWizard/PrepareTransition");
            //}
            //else if (domain.TelekomCredential == null)
            //{
            //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
            //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            //}

            //ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            //ViewBag.SelectedDomain = domain;
            //var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, dbSubscription.SubscriptionTelekomInfo?.PacketCode ?? 0, dbSubscription.SubscriptionTelekomInfo?.TariffCode ?? 0);

            //var viewResult = new PrepareTransitionViewModel()
            //{
            //    TransitionPSTN = dbSubscription.SubscriptionTelekomInfo?.PSTN,
            //    TransitionXDSLNo = dbSubscription.SubscriptionTelekomInfo?.SubscriptionNo,
            //    TelekomTariffInfo = telekomTariff == null ? new TelekomTariffHelperViewModel() : new TelekomTariffHelperViewModel(telekomTariff)
            //};

            //return View(viewName: "AddWizard/PrepareTransition", model: viewResult);
            return PrepareTransitionBase(id, returnUrl);
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [HttpGet]
        // GET: Client/PrepareTransitionForResend
        public ActionResult PrepareTransitionForResend(long id, string returnUrl)
        {
            return PrepareTransitionBase(id, returnUrl);
        }

        private ActionResult PrepareTransitionBase(long id, string returnUrl)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null)
            {
                ViewBag.ErrorMessage = RadiusR.Localization.Pages.Common.DomainNotFound;
                return View(viewName: "AddWizard/PrepareTransition");
            }
            else if (domain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.SelectedDomain = domain;
            var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, dbSubscription.SubscriptionTelekomInfo?.PacketCode ?? 0, dbSubscription.SubscriptionTelekomInfo?.TariffCode ?? 0);

            var viewResult = new PrepareTransitionViewModel()
            {
                TransitionPSTN = dbSubscription.SubscriptionTelekomInfo?.PSTN,
                TransitionXDSLNo = dbSubscription.SubscriptionTelekomInfo?.SubscriptionNo,
                TelekomTariffInfo = telekomTariff == null ? new TelekomTariffHelperViewModel() : new TelekomTariffHelperViewModel(telekomTariff)
            };

            return View(viewName: "AddWizard/PrepareTransition", model: viewResult);
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/PrepareTransition
        public ActionResult PrepareTransition(long id, string returnUrl, PrepareTransitionViewModel preparationModel)
        {
            //var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            //UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            //var dbSubscription = db.Subscriptions.Find(id);
            //if (dbSubscription == null)
            //{
            //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
            //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            //}

            //var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            //if (domain == null)
            //{
            //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
            //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            //}
            //else if (domain.TelekomCredential == null)
            //{
            //    UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
            //    return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            //}

            //if (ModelState.IsValid)
            //{
            //    var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, preparationModel.TelekomTariffInfo.PacketCode.Value, preparationModel.TelekomTariffInfo.TariffCode.Value);
            //    if (telekomTariff == null)
            //    {
            //        ViewBag.ErrorMessage = RadiusR.Localization.Validation.Common.InvalidTelekomPacket;
            //        return View(viewName: "AddWizard/PrepareTransition", model: preparationModel);
            //    }

            //    if (dbSubscription.SubscriptionTelekomInfo == null)
            //        dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo();

            //    dbSubscription.SubscriptionTelekomInfo.SubscriptionNo = preparationModel.TransitionXDSLNo;
            //    dbSubscription.SubscriptionTelekomInfo.PSTN = preparationModel.TransitionPSTN;
            //    dbSubscription.SubscriptionTelekomInfo.PacketCode = telekomTariff.PacketCode;
            //    dbSubscription.SubscriptionTelekomInfo.TariffCode = telekomTariff.TariffCode;
            //    dbSubscription.SubscriptionTelekomInfo.XDSLType = (short)telekomTariff.XDSLType;
            //    dbSubscription.SubscriptionTelekomInfo.IsPaperWorkNeeded = preparationModel.TelekomTariffInfo.IsPaperworkNeeded;
            //    dbSubscription.SubscriptionTelekomInfo.TTCustomerCode = domain.TelekomCredential.XDSLWebServiceCustomerCodeInt;

            //    db.SaveChanges();

            //    var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new RegisterSubscriptionOptions()
            //    {
            //        AppUserID = User.GiveUserId(),
            //        LogInterface = SystemLogInterface.MasterISS,
            //        ScheduleSMSes = false
            //    });

            //    if (results.IsFatal)
            //    {
            //        throw results.InternalException;
            //    }
            //    else if (results.IsSuccess)
            //    {
            //        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
            //        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            //    }
            //    else
            //    {
            //        UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
            //        TempData["ErrorResults"] = results;
            //        return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
            //    }
            //}

            //ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            //ViewBag.SelectedDomain = domain;
            //ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            //return View(viewName: "AddWizard/PrepareTransition", model: preparationModel);
            return PrepareTransitionConfirm(id, returnUrl, preparationModel, true);
        }

        [AuthorizePermission(Permissions = "Subscriber State")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/PrepareTransitionForResend
        public ActionResult PrepareTransitionForResend(long id, string returnUrl, PrepareTransitionViewModel preparationModel)
        {
            return PrepareTransitionConfirm(id, returnUrl, preparationModel, false);
        }

        private ActionResult PrepareTransitionConfirm(long id, string returnUrl, PrepareTransitionViewModel preparationModel, bool shouldChangeState)
        {
            var uri = new UriBuilder(Request.Url.GetLeftPart(UriPartial.Authority) + returnUrl);
            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);

            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "4", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }
            else if (domain.TelekomCredential == null)
            {
                UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "9", uri);
                return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
            }

            if (ModelState.IsValid)
            {
                var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, preparationModel.TelekomTariffInfo.PacketCode.Value, preparationModel.TelekomTariffInfo.TariffCode.Value);
                if (telekomTariff == null)
                {
                    ViewBag.ErrorMessage = RadiusR.Localization.Validation.Common.InvalidTelekomPacket;
                    return View(viewName: "AddWizard/PrepareTransition", model: preparationModel);
                }

                if (dbSubscription.SubscriptionTelekomInfo == null)
                    dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo();

                dbSubscription.SubscriptionTelekomInfo.SubscriptionNo = preparationModel.TransitionXDSLNo;
                dbSubscription.SubscriptionTelekomInfo.PSTN = preparationModel.TransitionPSTN;
                dbSubscription.SubscriptionTelekomInfo.PacketCode = telekomTariff.PacketCode;
                dbSubscription.SubscriptionTelekomInfo.TariffCode = telekomTariff.TariffCode;
                dbSubscription.SubscriptionTelekomInfo.XDSLType = (short)telekomTariff.XDSLType;
                dbSubscription.SubscriptionTelekomInfo.IsPaperWorkNeeded = preparationModel.TelekomTariffInfo.IsPaperworkNeeded;
                dbSubscription.SubscriptionTelekomInfo.TTCustomerCode = domain.TelekomCredential.XDSLWebServiceCustomerCodeInt;

                db.SaveChanges();
                // with state change
                if (shouldChangeState)
                {
                    var results = StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new RegisterSubscriptionOptions()
                    {
                        AppUserID = User.GiveUserId(),
                        LogInterface = SystemLogInterface.MasterISS,
                        ScheduleSMSes = false
                    });

                    if (results.IsFatal)
                    {
                        throw results.InternalException;
                    }
                    else if (results.IsSuccess)
                    {
                        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                    }
                    else
                    {
                        UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                        TempData["ErrorResults"] = results;
                        return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                    }
                }
                // without state change
                else
                {
                    // send service request
                    var transitionRegistrationResult = StateChangeUtilities.SendTransitionRegistration(dbSubscription);
                    if (transitionRegistrationResult.IsFatal)
                    {
                        throw transitionRegistrationResult.InternalException;
                    }
                    else if (transitionRegistrationResult.IsSuccess)
                    {
                        // add telekom work order
                        var addedTelekomWorkOrder = new TelekomWorkOrder()
                        {
                            AppUserID = User.GiveUserId(),
                            CreationDate = DateTime.Now,
                            IsOpen = true,
                            OperationTypeID = (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Transition,
                            OperationSubType = (short)(dbSubscription.SubscriptionTelekomInfo.XDSLType == (short)XDSLType.Fiber ? RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType.FTTX : RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType.XDSL),
                            SubscriptionID = dbSubscription.ID,
                            TransactionID = transitionRegistrationResult.TransactionID.Value
                        };
                        dbSubscription.TelekomWorkOrders.Add(addedTelekomWorkOrder);
                        // save
                        db.SaveChanges();
                        // work order log
                        db.SystemLogs.Add(SystemLogProcessor.CreateTelekomWorkOrder(User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null, addedTelekomWorkOrder.ID, new RadiusR.SystemLogs.Parameters.TelekomWorkOrderDetails(addedTelekomWorkOrder)));
                        // save
                        db.SaveChanges();

                        // check attachments
                        var validateTransitionFilesResult = StateChangeUtilities.ValidateAttachmentsForTransition(dbSubscription);
                        if (!validateTransitionFilesResult.IsValid)
                        {
                            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                            TempData["ErrorResults"] = new StateChangeResult(RadiusR.Localization.Pages.Common.FileManagerError, validateTransitionFilesResult.FileManagerException);
                            return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                        }
                        // upload files
                        var attachmentUploadResult = StateChangeUtilities.UploadTransitionFiles(dbSubscription, transitionRegistrationResult.TransactionID.Value, validateTransitionFilesResult);
                        if (attachmentUploadResult.IsSuccess)
                        {
                            UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                            return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
                        }
                        else
                        {
                            UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                            TempData["ErrorResults"] = attachmentUploadResult.ErrorResult;
                            return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                        }
                    }
                    else
                    {
                        UrlUtilities.RemoveQueryStringParameter("errorMessage", uri);
                        TempData["ErrorResults"] = transitionRegistrationResult;
                        return RedirectToAction("StateChangeError", new { returnUrl = uri.Uri.PathAndQuery + uri.Fragment });
                    }
                }
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.SelectedDomain = domain;
            ViewBag.ReturnUrl = uri.Uri.PathAndQuery + uri.Fragment;

            return View(viewName: "AddWizard/PrepareTransition", model: preparationModel);
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Modify Clients")]
        [AjaxCall]
        // POST: Client/TransferringSubscriptionValidation
        public ActionResult TransferringSubscriptionValidation(string transferringSubscriptionNo)
        {
            var foundSub = db.Subscriptions.FirstOrDefault(s => s.SubscriberNo == transferringSubscriptionNo);
            if (foundSub == null || !foundSub.IsActive)
            {
                return Json(new
                {
                    IsValid = false,
                    ValidationMessage = RadiusR.Localization.Validation.ModelSpecific.InvalidSubscriptionNo
                });
            }
            if (foundSub.SubscriptionTransferredFromHistories.Any(sth => !sth.Date.HasValue) || foundSub.SubscriptionTransferredToHistories.Any(sth => !sth.Date.HasValue))
            {
                return Json(new
                {
                    IsValid = false,
                    ValidationMessage = RadiusR.Localization.Validation.ModelSpecific.SubscriptionHasPendingTransfer
                });
            }

            return Json(new
            {
                IsValid = true,
                SubId = foundSub.ID,
                SubName = foundSub.ValidDisplayName,
                BBK = foundSub.Address.ApartmentID,
                PostalCode = foundSub.Address.PostalCode,
                Floor = foundSub.Address.Floor,
                DomainID = foundSub.DomainID,
                TariffID = foundSub.ServiceID,
                BillingPeriod = foundSub.PaymentDay
            });
        }
    }
}