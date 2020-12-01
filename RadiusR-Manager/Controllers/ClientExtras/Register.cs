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
            ViewBag.GroupList = new MultiSelectList(db.Groups.ToList(), "ID", "Name");
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

            return View(viewName: "AddWizard/Register", model: new CustomerRegistrationViewModel());
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
                    var validationResults = db.RegisterSubscriptionWithNewCustomer(registrationInfo, out dbCustomer, true);
                    if (validationResults != null)
                    {
                        ViewBag.RegistrationValidations = validationResults;
                        foreach (var item in validationResults)
                        {
                            ModelState.AddModelError(item.Key, string.Join(Environment.NewLine, item.Select(ve => ve)));
                        }
                    }
                    else
                    {
                        // if new customer add it
                        if (dbCustomer.ID <= 0)
                            db.Customers.Add(dbCustomer);
                        // save db
                        db.SaveChanges();

                        // logs
                        var dbSubscription = dbCustomer.Subscriptions.OrderByDescending(s => s.MembershipDate).FirstOrDefault();
                        db.SystemLogs.Add(SystemLogProcessor.AddSubscription(User.GiveUserId(), dbSubscription.ID, dbCustomer.ID, SystemLogInterface.MasterISS, null, dbSubscription.SubscriberNo));
                        db.SaveChanges();

                        return RedirectToAction("Index", "Client", new { errorMessage = 0 });
                    }

                    #region pre 5.03.06
                    //var selectedDomain = DomainsCache.GetDomainByID(registeredCustomer.SubscriptionInfo.DomainID);
                    //registeredCustomer.SubscriptionInfo.GroupIds = registeredCustomer.SubscriptionInfo.GroupIds ?? Enumerable.Empty<int>();
                    //registeredCustomer.SubscriptionInfo.GroupIds = registeredCustomer.SubscriptionInfo.GroupIds.Distinct();

                    //// check for existing customer
                    //var dbCustomer = db.Customers.FirstOrDefault(c => c.CustomerIDCard.TCKNo == registeredCustomer.IDCard.TCKNo && c.CustomerType == (short)registeredCustomer.GeneralInfo.CustomerType);
                    //if (dbCustomer == null)
                    //{
                    //    dbCustomer = new Customer()
                    //    {
                    //        Address = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? (registeredCustomer.IndividualInfo.ResidencySameAsSetupAddress == true ? registeredCustomer.SubscriptionInfo.SetupAddress.GetDatabaseObject() : registeredCustomer.IndividualInfo.ResidencyAddress.GetDatabaseObject())
                    //    : (registeredCustomer.CorporateInfo.ExecutiveResidencySameAsSetupAddress == true ? registeredCustomer.SubscriptionInfo.SetupAddress.GetDatabaseObject() : registeredCustomer.CorporateInfo.ExecutiveResidencyAddress.GetDatabaseObject()),
                    //        BillingAddress = registeredCustomer.GeneralInfo.BillingSameAsSetupAddress == true ? registeredCustomer.SubscriptionInfo.SetupAddress.GetDatabaseObject() : registeredCustomer.GeneralInfo.BillingAddress.GetDatabaseObject(),
                    //        BirthDate = registeredCustomer.IDCard.BirthDate.Value,
                    //        BirthPlace = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.BirthPlace : registeredCustomer.CorporateInfo.ExecutiveBirthPlace,
                    //        ContactPhoneNo = registeredCustomer.GeneralInfo.ContactPhoneNo,
                    //        CustomerAdditionalPhoneNoes = (registeredCustomer.GeneralInfo.OtherPhoneNos != null && registeredCustomer.GeneralInfo.OtherPhoneNos.Any()) ? registeredCustomer.GeneralInfo.OtherPhoneNos.Select(pn => new CustomerAdditionalPhoneNo()
                    //        {
                    //            PhoneNo = pn.Number
                    //        }).ToList() : null,
                    //        Culture = registeredCustomer.GeneralInfo.Culture,
                    //        CorporateCustomerInfo = registeredCustomer.GeneralInfo.CustomerType != (short)CustomerType.Individual ? new CorporateCustomerInfo()
                    //        {
                    //            Address = registeredCustomer.CorporateInfo.CompanySameAsSetupAddress == true ? registeredCustomer.SubscriptionInfo.SetupAddress.GetDatabaseObject() : registeredCustomer.CorporateInfo.CompanyAddress.GetDatabaseObject(),
                    //            CentralSystemNo = registeredCustomer.CorporateInfo.CentralSystemNo,
                    //            TaxNo = registeredCustomer.CorporateInfo.TaxNo,
                    //            TaxOffice = registeredCustomer.CorporateInfo.TaxOffice,
                    //            Title = registeredCustomer.CorporateInfo.Title,
                    //            TradeRegistrationNo = registeredCustomer.CorporateInfo.TradeRegistrationNo
                    //        } : null,
                    //        CustomerIDCard = new CustomerIDCard()
                    //        {
                    //            DateOfIssue = registeredCustomer.IDCard.DateOfIssue,
                    //            District = registeredCustomer.IDCard.District,
                    //            Neighbourhood = registeredCustomer.IDCard.Neighbourhood,
                    //            PageNo = registeredCustomer.IDCard.PageNo,
                    //            PassportNo = registeredCustomer.IDCard.PassportNo,
                    //            PlaceOfIssue = registeredCustomer.IDCard.PlaceOfIssue,
                    //            Province = registeredCustomer.IDCard.Province,
                    //            RowNo = registeredCustomer.IDCard.RowNo,
                    //            SerialNo = registeredCustomer.IDCard.SerialNo,
                    //            TCKNo = registeredCustomer.IDCard.TCKNo,
                    //            TypeID = registeredCustomer.IDCard.CardType.Value,
                    //            VolumeNo = registeredCustomer.IDCard.VolumeNo
                    //        },
                    //        CustomerType = registeredCustomer.GeneralInfo.CustomerType.Value,
                    //        Email = registeredCustomer.GeneralInfo.Email,
                    //        FathersName = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.FathersName : registeredCustomer.CorporateInfo.ExecutiveFathersName,
                    //        FirstName = registeredCustomer.IDCard.FirstName,
                    //        LastName = registeredCustomer.IDCard.LastName,
                    //        MothersMaidenName = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.MothersMaidenName : registeredCustomer.CorporateInfo.ExecutiveMothersMaidenName,
                    //        MothersName = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.MothersName : registeredCustomer.CorporateInfo.ExecutiveMothersName,
                    //        Nationality = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.Nationality.Value : registeredCustomer.CorporateInfo.ExecutiveNationality.Value,
                    //        Profession = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.Profession.Value : registeredCustomer.CorporateInfo.ExecutiveProfession.Value,
                    //        Sex = registeredCustomer.GeneralInfo.CustomerType == (short)CustomerType.Individual ? registeredCustomer.IndividualInfo.Sex.Value : registeredCustomer.CorporateInfo.ExecutiveSex.Value,
                    //    };

                    //    dbCustomer.Subscriptions = new List<Subscription>();

                    //    db.Customers.Add(dbCustomer);
                    //}

                    //var dbGroups = db.Groups.ToArray().Where(g => registeredCustomer.SubscriptionInfo.GroupIds.Contains(g.ID)).ToArray();

                    //CachedTelekomTariff selectedTelekomTariff = null;
                    //if (registeredCustomer.SubscriptionInfo.TTPacket != null && registeredCustomer.SubscriptionInfo.TTPacket.PacketCode.HasValue && registeredCustomer.SubscriptionInfo.TTPacket.TariffCode.HasValue)
                    //    selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, registeredCustomer.SubscriptionInfo.TTPacket.PacketCode.Value, registeredCustomer.SubscriptionInfo.TTPacket.TariffCode.Value);
                    //else if (selectedDomain.TelekomCredential != null && !string.IsNullOrEmpty(registeredCustomer.SubscriptionInfo.TelekomInfo.SubscriberNo))
                    //{
                    //    var telekomServiceClient = new TelekomSubscriberInfoServiceClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword);
                    //    var response = telekomServiceClient.GetSubscriberDetailedInfo(registeredCustomer.SubscriptionInfo.TelekomInfo.SubscriberNo);
                    //    if (response.InternalException != null || !response.Data.PacketCode.HasValue || !response.Data.TariffCode.HasValue)
                    //    {
                    //        ViewBag.TelekomError = response.InternalException != null ? response.InternalException.GetShortMessage() : RadiusR.Localization.Validation.Common.TelekomError;
                    //        ModelState.AddModelError("TelekomError", RadiusR.Localization.Validation.Common.TelekomError);
                    //    }
                    //    else
                    //    {
                    //        selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, response.Data.PacketCode.Value, response.Data.TariffCode.Value);
                    //    }
                    //}

                    //var dbSubsciption = new Subscription()
                    //{
                    //    Address = registeredCustomer.SubscriptionInfo.SetupAddress.GetDatabaseObject(),
                    //    DomainID = registeredCustomer.SubscriptionInfo.DomainID,
                    //    Fees = registeredCustomer.SubscriptionInfo.AddedFees.Select(af => af.GetDBObject(dbFees)).SelectMany(f => f).Where(f => f != null).ToList(),
                    //    Groups = dbGroups,
                    //    MembershipDate = DateTime.Today,
                    //    PaymentDay = registeredCustomer.SubscriptionInfo.BillingPeriod,
                    //    ServiceID = registeredCustomer.SubscriptionInfo.ServiceID,
                    //    SimultaneousUse = 1,
                    //    State = (short)CustomerState.Registered,
                    //    StaticIP = registeredCustomer.SubscriptionInfo.StaticIP,
                    //    SubscriptionCommitment = registeredCustomer.SubscriptionInfo.Commitment != null && registeredCustomer.SubscriptionInfo.Commitment.CommitmentLength.HasValue ? new SubscriptionCommitment()
                    //    {
                    //        CommitmentLength = registeredCustomer.SubscriptionInfo.Commitment.CommitmentLength.Value,
                    //        CommitmentExpirationDate = registeredCustomer.SubscriptionInfo.Commitment.CommitmentExpirationDate.Value
                    //    } : null,
                    //    SubscriberNo = UsernameFactory.GenerateUniqueSubscriberNo(selectedDomain),
                    //    Username = string.IsNullOrWhiteSpace(registeredCustomer.SubscriptionInfo.Username) ? UsernameFactory.GenerateUsername(selectedDomain) : registeredCustomer.SubscriptionInfo.Username + "@" + selectedDomain.Name,
                    //    RadiusPassword = Authenticator.GenerateInternetPassword(),
                    //    SubscriptionTelekomInfo = selectedDomain.TelekomCredential != null ? new SubscriptionTelekomInfo()
                    //    {
                    //        PSTN = registeredCustomer.SubscriptionInfo.TelekomInfo.PSTN,
                    //        SubscriptionNo = registeredCustomer.SubscriptionInfo.TelekomInfo.SubscriberNo,
                    //        TTCustomerCode = !string.IsNullOrEmpty(registeredCustomer.SubscriptionInfo.TelekomInfo.CustomerCode) ? long.Parse(registeredCustomer.SubscriptionInfo.TelekomInfo.CustomerCode) : selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt,
                    //        PacketCode = selectedTelekomTariff != null ? selectedTelekomTariff.PacketCode : 0,
                    //        TariffCode = selectedTelekomTariff != null ? selectedTelekomTariff.TariffCode : 0,
                    //        XDSLType = selectedTelekomTariff != null ? (short)selectedTelekomTariff.XDSLType : (short)0,
                    //        IsPaperWorkNeeded = registeredCustomer.SubscriptionInfo.TTPacket.IsPaperworkNeeded
                    //    } : null
                    //};
                    //dbCustomer.Subscriptions.Add(dbSubsciption);

                    //// telekom registration
                    //if (selectedDomain.TelekomCredential != null && (registeredCustomer.SubscriptionInfo.TelekomInfo == null || string.IsNullOrEmpty(registeredCustomer.SubscriptionInfo.TelekomInfo.SubscriberNo)))
                    //{
                    //    dbSubsciption.SubscriptionTelekomInfo.SubscriptionNo = " ";
                    //    dbSubsciption.State = (short)CustomerState.PreRegisterd;
                    //}

                    //if (ModelState.IsValid)
                    //{
                    //    // save db
                    //    db.SaveChanges();
                    //    // logs
                    //    db.SystemLogs.Add(SystemLogProcessor.AddSubscription(User.GiveUserId(), dbSubsciption.ID, dbCustomer.ID, dbSubsciption.SubscriberNo));
                    //    db.SaveChanges();

                    //    return RedirectToAction("Index", "Client", new { errorMessage = 0 });
                    //}
                    #endregion
                }
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetAllDomains().Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", registeredCustomer.SubscriptionInfo.DomainID != 0 ? registeredCustomer.SubscriptionInfo.DomainID : (int?)null);
            ViewBag.Services = registeredCustomer.SubscriptionInfo.DomainID != 0 && registeredCustomer.SubscriptionInfo.ServiceID != 0 ? new SelectList(db.Domains.Find(registeredCustomer.SubscriptionInfo.DomainID).Services.AsQueryable().FilterActiveServices().ToArray(), "ID", "Name", registeredCustomer.SubscriptionInfo.ServiceID) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.BillingPeriods = registeredCustomer.SubscriptionInfo.DomainID != 0 && registeredCustomer.SubscriptionInfo.ServiceID != 0 && registeredCustomer.SubscriptionInfo.BillingPeriod != 0 ? new SelectList(db.Services.Find(registeredCustomer.SubscriptionInfo.ServiceID).ServiceBillingPeriods, "DayOfMonth", "DayOfMonth", registeredCustomer.SubscriptionInfo.BillingPeriod) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.GroupList = new MultiSelectList(db.Groups.ToList(), "ID", "Name", registeredCustomer.SubscriptionInfo.GroupIds);
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
            ViewBag.GroupList = new MultiSelectList(db.Groups.ToList(), "ID", "Name");
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
            return View(viewName: "AddWizard/AddSubscription", model: new CustomerSubscriptionViewModel());
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
                var validationResults = db.RegisterSubscriptionForExistingCustomer(registrationInfo, dbCustomer);
                if (validationResults != null)
                {
                    ViewBag.RegistrationValidations = validationResults;
                    foreach (var item in validationResults)
                    {
                        ModelState.AddModelError(item.Key, string.Join(Environment.NewLine, item.Select(ve => ve)));
                    }
                }
                else
                {
                    var dbSubscription = dbCustomer.Subscriptions.OrderByDescending(s => s.ID).FirstOrDefault();
                    // save db
                    db.SaveChanges();
                    // logs
                    db.SystemLogs.Add(SystemLogProcessor.AddSubscription(User.GiveUserId(), dbSubscription.ID, referenceSubscription.CustomerID, SystemLogInterface.MasterISS, null, dbSubscription.SubscriberNo));
                    db.SaveChanges();

                    return RedirectToAction("Index", "Client", new { errorMessage = 0 });
                }

                #region pre 5.03.06
                //var selectedDomain = DomainsCache.GetDomainByID(subscription.DomainID);
                //subscription.GroupIds = subscription.GroupIds ?? Enumerable.Empty<int>();
                //subscription.GroupIds = subscription.GroupIds.Distinct().ToArray();

                //var dbGroups = db.Groups.ToArray().Where(g => subscription.GroupIds.Contains(g.ID)).ToArray();

                //CachedTelekomTariff selectedTelekomTariff = null;
                //if (subscription.TelekomDetailedInfo.TelekomTariffInfo != null && subscription.TelekomDetailedInfo.TelekomTariffInfo.PacketCode.HasValue && subscription.TelekomDetailedInfo.TelekomTariffInfo.TariffCode.HasValue)
                //    selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, subscription.TelekomDetailedInfo.TelekomTariffInfo.PacketCode.Value, subscription.TelekomDetailedInfo.TelekomTariffInfo.TariffCode.Value);
                //else if (selectedDomain.TelekomCredential != null && !string.IsNullOrEmpty(subscription.TelekomDetailedInfo.SubscriberNo))
                //{
                //    var telekomServiceClient = new TelekomSubscriberInfoServiceClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword);
                //    var response = telekomServiceClient.GetSubscriberDetailedInfo(subscription.TelekomDetailedInfo.SubscriberNo);
                //    if (response.InternalException != null || !response.Data.PacketCode.HasValue || !response.Data.TariffCode.HasValue)
                //    {
                //        ViewBag.TelekomError = response.InternalException != null ? response.InternalException.GetShortMessage() : RadiusR.Localization.Validation.Common.TelekomError;
                //        ModelState.AddModelError("TelekomError", RadiusR.Localization.Validation.Common.TelekomError);
                //    }
                //    else
                //    {
                //        selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, response.Data.PacketCode.Value, response.Data.TariffCode.Value);
                //    }
                //}

                //Subscription dbSubsciption = null;
                //if (ModelState.IsValid)
                //{
                //    dbSubsciption = new Subscription()
                //    {
                //        Address = subscription.SetupAddress.GetDatabaseObject(),
                //        DomainID = subscription.DomainID,
                //        Fees = subscription.AddedFeesInfo.Select(af => af.GetDBObject(dbFees)).SelectMany(f => f).Where(f => f != null).ToList(),
                //        Groups = dbGroups,
                //        MembershipDate = DateTime.Today,
                //        PaymentDay = subscription.BillingPeriod,
                //        ServiceID = subscription.ServiceID,
                //        SimultaneousUse = 1,
                //        State = (short)CustomerState.Registered,
                //        StaticIP = subscription.StaticIP,
                //        SubscriptionCommitment = subscription.CommitmentInfo != null && subscription.CommitmentInfo.CommitmentLength.HasValue ? new SubscriptionCommitment()
                //        {
                //            CommitmentLength = subscription.CommitmentInfo.CommitmentLength.Value,
                //            CommitmentExpirationDate = subscription.CommitmentInfo.CommitmentExpirationDate.Value
                //        } : null,
                //        SubscriberNo = UsernameFactory.GenerateUniqueSubscriberNo(selectedDomain),
                //        Username = UsernameFactory.GenerateUsername(selectedDomain),
                //        RadiusPassword = Authenticator.GenerateInternetPassword(),
                //        SubscriptionTelekomInfo = selectedDomain.TelekomCredential != null ? new SubscriptionTelekomInfo()
                //        {
                //            PSTN = subscription.TelekomDetailedInfo.PSTN,
                //            SubscriptionNo = subscription.TelekomDetailedInfo.SubscriberNo,
                //            TTCustomerCode = !string.IsNullOrEmpty(subscription.TelekomDetailedInfo.CustomerCode) ? long.Parse(subscription.TelekomDetailedInfo.CustomerCode) : selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt,
                //            PacketCode = selectedTelekomTariff != null ? selectedTelekomTariff.PacketCode : 0,
                //            TariffCode = selectedTelekomTariff != null ? selectedTelekomTariff.TariffCode : 0,
                //            XDSLType = selectedTelekomTariff != null ? (short)selectedTelekomTariff.XDSLType : (short)0,
                //            IsPaperWorkNeeded = subscription.TelekomDetailedInfo.TelekomTariffInfo.IsPaperworkNeeded
                //        } : null
                //    };

                //    dbSubsciption.CustomerID = referenceSubscription.CustomerID;

                //    // telekom registration
                //    if (selectedDomain.TelekomCredential != null && (subscription.TelekomDetailedInfo == null || string.IsNullOrEmpty(subscription.TelekomDetailedInfo.SubscriberNo)))
                //    {
                //        dbSubsciption.SubscriptionTelekomInfo.SubscriptionNo = " ";
                //        dbSubsciption.State = (short)CustomerState.PreRegisterd;
                //    }
                //}
                //if (ModelState.IsValid)
                //{
                //    db.Subscriptions.Add(dbSubsciption);
                //    // save db
                //    db.SaveChanges();
                //    // logs
                //    db.SystemLogs.Add(SystemLogProcessor.AddSubscription(User.GiveUserId(), dbSubsciption.ID, referenceSubscription.CustomerID, dbSubsciption.SubscriberNo));
                //    db.SaveChanges();

                //    return RedirectToAction("Index", "Client", new { errorMessage = 0 });
                //}
                #endregion
            }

            ViewBag.Domains = new SelectList(DomainsCache.GetAllDomains().Select(d => new { Name = d.Name, Value = d.ID }), "Value", "Name", subscription.DomainID != 0 ? subscription.DomainID : (int?)null);
            ViewBag.Services = subscription.DomainID != 0 && subscription.ServiceID != 0 ? new SelectList(db.Domains.Find(subscription.DomainID).Services, "ID", "Name", subscription.ServiceID) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.BillingPeriods = subscription.DomainID != 0 && subscription.ServiceID != 0 && subscription.BillingPeriod != 0 ? new SelectList(db.Services.Find(subscription.ServiceID).ServiceBillingPeriods, "DayOfMonth", "DayOfMonth", subscription.BillingPeriod) : new SelectList(Enumerable.Empty<object>(), null);
            ViewBag.GroupList = new MultiSelectList(db.Groups.ToList(), "ID", "Name", subscription.GroupIds);
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
                        PacketCode = dbSubscription.SubscriptionTelekomInfo.PacketCode,
                        TariffCode = dbSubscription.SubscriptionTelekomInfo.TariffCode
                    };
                }
                var telekomTariff = TelekomTariffsCache.GetSpecificTariff(domain, selectedPacket.PacketCode, selectedPacket.TariffCode);
                if (telekomTariff == null)
                {
                    return View(viewName: "AddWizard/SendTelekomRegistration");
                }
                else
                {
                    var telekomTariffViewModel = new UpdateTelekomInfoBeforeSendViewModel()
                    {
                        TelekomTariffInfo = new TelekomTariffHelperViewModel(telekomTariff),
                        PSTN = dbSubscription.SubscriptionTelekomInfo.PSTN
                    };
                    return View(viewName: "AddWizard/SendTelekomRegistration", model: telekomTariffViewModel);
                }

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
                        StateChangeUtilities.ChangeSubscriptionState(dbSubscription.ID, new RegisterSubscriptionOptions()
                        {
                            AppUserID = User.GiveUserId(),
                            LogInterface = SystemLogInterface.MasterISS
                        });

                        UrlUtilities.AddOrModifyQueryStringParameter("errorMessage", "0", uri);
                        return Redirect(uri.Uri.PathAndQuery + uri.Fragment);
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
    }
}