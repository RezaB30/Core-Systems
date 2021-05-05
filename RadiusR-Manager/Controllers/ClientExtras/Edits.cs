using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Utilities.Extentions;
using RadiusR.SystemLogs;
using RadiusR_Manager.Models.Extentions;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RezaB.API.TCKValidation;
using RadiusR_Manager.Models.ViewModels.Customer;
using RadiusR_Manager.Models.ViewModels.Customer.SubscriptionItems;
using RadiusR.DB.DomainsCache;
using RadiusR_Manager.Models.ViewModels;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using RadiusR.DB.Utilities.ComplexOperations.Discounts;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TariffChanges;
using RadiusR.DB.QueryExtentions;
using RezaB.Data.Localization;
using RezaB.Web.Authentication;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        // GET: Client/EditCustomerGeneralInfo
        public ActionResult EditCustomerGeneralInfo(long id, long subId)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var generalInfo = new CustomerGeneralInfoViewModel()
            {
                BillingAddress = new AddressViewModel(dbSubscription.Customer.BillingAddress),
                ContactPhoneNo = dbSubscription.Customer.ContactPhoneNo,
                Culture = dbSubscription.Customer.Culture,
                Email = dbSubscription.Customer.Email,
                OtherPhoneNos = dbSubscription.Customer.CustomerAdditionalPhoneNoes.Select(phone => new CustomerGeneralInfoViewModel.PhoneNo() { Number = phone.PhoneNo }).ToList()
            };

            ViewBag.IsEdit = true;
            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/EditCustomerGeneralInfo", model: generalInfo);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/EditCustomerGeneralInfo
        public ActionResult EditCustomerGeneralInfo(long id, long subId, CustomerGeneralInfoViewModel generalInfo)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ModelState.Remove("CustomerType");

            if (ModelState.IsValid)
            {
                if (generalInfo.BillingAddress.ApartmentID != dbSubscription.Customer.BillingAddress.ApartmentID || generalInfo.BillingAddress.PostalCode != dbSubscription.Customer.BillingAddress.PostalCode || generalInfo.BillingAddress.Floor != dbSubscription.Customer.BillingAddress.Floor)
                {
                    generalInfo.BillingAddress.CopyToDBObject(dbSubscription.Customer.BillingAddress);
                }
                dbSubscription.Customer.ContactPhoneNo = generalInfo.ContactPhoneNo;
                dbSubscription.Customer.Culture = generalInfo.Culture;
                dbSubscription.Customer.Email = generalInfo.Email;
                generalInfo.OtherPhoneNos = generalInfo.OtherPhoneNos ?? new List<CustomerGeneralInfoViewModel.PhoneNo>();
                if (!dbSubscription.Customer.CustomerAdditionalPhoneNoes.Select(p => p.PhoneNo).OrderBy(p => p).SequenceEqual(generalInfo.OtherPhoneNos.Select(p => p.Number).OrderBy(p => p)))
                {
                    db.CustomerAdditionalPhoneNoes.RemoveRange(dbSubscription.Customer.CustomerAdditionalPhoneNoes);
                    dbSubscription.Customer.CustomerAdditionalPhoneNoes = generalInfo.OtherPhoneNos.Select(phone => new CustomerAdditionalPhoneNo() { PhoneNo = phone.Number }).ToList();
                }

                db.SystemLogs.Add(SystemLogProcessor.ChangeCustomer(User.GiveUserId(), dbSubscription.CustomerID, SystemLogInterface.MasterISS, null));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
            }

            ViewBag.IsEdit = true;
            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/EditCustomerGeneralInfo", model: generalInfo);
        }

        [AuthorizePermission(Permissions = "Edit ID Card")]
        [HttpGet]
        // GET: Client/EditCustomerIDCard
        public ActionResult EditCustomerIDCard(long id, long subId)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var idCard = new IDCardViewModel()
            {
                BirthDate = dbSubscription.Customer.BirthDate,
                CardType = dbSubscription.Customer.CustomerIDCard.TypeID,
                DateOfIssue = dbSubscription.Customer.CustomerIDCard.DateOfIssue,
                District = dbSubscription.Customer.CustomerIDCard.District,
                FirstName = dbSubscription.Customer.FirstName,
                LastName = dbSubscription.Customer.LastName,
                Neighbourhood = dbSubscription.Customer.CustomerIDCard.Neighbourhood,
                PageNo = dbSubscription.Customer.CustomerIDCard.PageNo,
                PassportNo = dbSubscription.Customer.CustomerIDCard.PassportNo,
                PlaceOfIssue = dbSubscription.Customer.CustomerIDCard.PlaceOfIssue,
                Province = dbSubscription.Customer.CustomerIDCard.Province,
                RowNo = dbSubscription.Customer.CustomerIDCard.RowNo,
                SerialNo = dbSubscription.Customer.CustomerIDCard.SerialNo,
                TCKNo = dbSubscription.Customer.CustomerIDCard.TCKNo,
                VolumeNo = dbSubscription.Customer.CustomerIDCard.VolumeNo
            };

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/EditCustomerIDCard", model: idCard);
        }

        [AuthorizePermission(Permissions = "Edit ID Card")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Client/EditCustomerIDCard
        public ActionResult EditCustomerIDCard(long id, long subId, IDCardViewModel idCard)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                var tckValidationResults = ValidateTCK(idCard);
                if ((!User.HasPermission("Skip TCK Validation") || idCard.SkipValidation != true) && (tckValidationResults.ResultType != TCKValidationResult.TCKValidationResultType.Success && tckValidationResults.ResultType != TCKValidationResult.TCKValidationResultType.NoValidationMethod))
                {
                    ViewBag.TCKValidationResults = tckValidationResults.ResultContent;
                    if (User.HasPermission("Skip TCK Validation"))
                        ViewBag.SkipValidation = true;
                }
                else
                {
                    dbSubscription.Customer.CustomerIDCard.DateOfIssue = idCard.DateOfIssue;
                    dbSubscription.Customer.CustomerIDCard.District = idCard.District;
                    dbSubscription.Customer.CustomerIDCard.Neighbourhood = idCard.Neighbourhood;
                    dbSubscription.Customer.CustomerIDCard.PageNo = idCard.PageNo;
                    dbSubscription.Customer.CustomerIDCard.PassportNo = idCard.PassportNo;
                    dbSubscription.Customer.CustomerIDCard.PlaceOfIssue = idCard.PlaceOfIssue;
                    dbSubscription.Customer.CustomerIDCard.Province = idCard.Province;
                    dbSubscription.Customer.CustomerIDCard.RowNo = idCard.RowNo;
                    dbSubscription.Customer.CustomerIDCard.SerialNo = idCard.SerialNo;
                    dbSubscription.Customer.CustomerIDCard.TCKNo = idCard.TCKNo;
                    dbSubscription.Customer.CustomerIDCard.TypeID = idCard.CardType.Value;
                    dbSubscription.Customer.CustomerIDCard.VolumeNo = idCard.VolumeNo;
                    dbSubscription.Customer.BirthDate = idCard.BirthDate.Value;
                    dbSubscription.Customer.FirstName = idCard.FirstName;
                    dbSubscription.Customer.LastName = idCard.LastName;

                    db.SystemLogs.Add(SystemLogProcessor.EditClientIdentityDocument(User.GiveUserId(), dbSubscription.CustomerID, SystemLogInterface.MasterISS, null));

                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
                }
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/EditCustomerIDCard", model: idCard);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        // GET: Client/EditCustomerIndividualInfo
        public ActionResult EditCustomerIndividualInfo(long id, long subId)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (dbSubscription.Customer.CustomerType != (short)CustomerType.Individual)
            {
                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            var individualInfo = new IndividualCustomerInfoViewModel()
            {
                BirthPlace = dbSubscription.Customer.BirthPlace,
                FathersName = dbSubscription.Customer.FathersName,
                FirstName = dbSubscription.Customer.FirstName,
                LastName = dbSubscription.Customer.LastName,
                MothersMaidenName = dbSubscription.Customer.MothersMaidenName,
                MothersName = dbSubscription.Customer.MothersName,
                Nationality = dbSubscription.Customer.Nationality,
                Profession = dbSubscription.Customer.Profession,
                Sex = dbSubscription.Customer.Sex,
                ResidencyAddress = new AddressViewModel(dbSubscription.Customer.Address)
            };

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.IsEdit = true;
            return View(viewName: "Edits/EditCustomerIndividualInfo", model: individualInfo);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/EditCustomerIndividualInfo
        public ActionResult EditCustomerIndividualInfo(long id, long subId, IndividualCustomerInfoViewModel individualInfo)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (dbSubscription.Customer.CustomerType != (short)CustomerType.Individual)
            {
                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbSubscription.Customer.BirthPlace = individualInfo.BirthPlace;
                dbSubscription.Customer.FathersName = individualInfo.FathersName;
                dbSubscription.Customer.MothersMaidenName = individualInfo.MothersMaidenName;
                dbSubscription.Customer.MothersName = individualInfo.MothersName;
                dbSubscription.Customer.Nationality = individualInfo.Nationality.Value;
                dbSubscription.Customer.Profession = individualInfo.Profession.Value;
                dbSubscription.Customer.Sex = individualInfo.Sex.Value;
                if (dbSubscription.Customer.Address.ApartmentID != individualInfo.ResidencyAddress.ApartmentID || individualInfo.ResidencyAddress.PostalCode != dbSubscription.Customer.Address.PostalCode || individualInfo.ResidencyAddress.Floor != dbSubscription.Customer.Address.Floor)
                {
                    individualInfo.ResidencyAddress.CopyToDBObject(dbSubscription.Customer.Address);
                }

                db.SystemLogs.Add(SystemLogProcessor.ChangeCustomer(User.GiveUserId(), dbSubscription.CustomerID, SystemLogInterface.MasterISS, null));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.IsEdit = true;
            return View(viewName: "Edits/EditCustomerIndividualInfo", model: individualInfo);

        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        // GET: Client/EditCustomerIndividualInfo
        public ActionResult EditCustomerCorporateInfo(long id, long subId)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (dbSubscription.Customer.CustomerType == (short)CustomerType.Individual)
            {
                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            var corporateInfo = new CorporateCustomerInfoViewModel()
            {
                CentralSystemNo = dbSubscription.Customer.CorporateCustomerInfo.CentralSystemNo,
                CompanyAddress = new AddressViewModel(dbSubscription.Customer.CorporateCustomerInfo.Address),
                ExecutiveBirthPlace = dbSubscription.Customer.BirthPlace,
                ExecutiveFathersName = dbSubscription.Customer.FathersName,
                ExecutiveFirstName = dbSubscription.Customer.FirstName,
                ExecutiveLastName = dbSubscription.Customer.LastName,
                ExecutiveMothersMaidenName = dbSubscription.Customer.MothersMaidenName,
                ExecutiveMothersName = dbSubscription.Customer.MothersName,
                ExecutiveNationality = dbSubscription.Customer.Nationality,
                ExecutiveProfession = dbSubscription.Customer.Profession,
                ExecutiveResidencyAddress = new AddressViewModel(dbSubscription.Customer.Address),
                ExecutiveSex = dbSubscription.Customer.Sex,
                TaxNo = dbSubscription.Customer.CorporateCustomerInfo.TaxNo,
                TaxOffice = dbSubscription.Customer.CorporateCustomerInfo.TaxOffice,
                Title = dbSubscription.Customer.CorporateCustomerInfo.Title,
                TradeRegistrationNo = dbSubscription.Customer.CorporateCustomerInfo.TradeRegistrationNo
            };

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.IsEdit = true;
            return View(viewName: "Edits/EditCustomerCorporateInfo", model: corporateInfo);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/EditCustomerIndividualInfo
        public ActionResult EditCustomerCorporateInfo(long id, long subId, CorporateCustomerInfoViewModel corporateInfo)
        {
            var dbSubscription = db.Subscriptions.Find(subId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (dbSubscription.Customer.CustomerType == (short)CustomerType.Individual)
            {
                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbSubscription.Customer.CorporateCustomerInfo.CentralSystemNo = corporateInfo.CentralSystemNo;
                dbSubscription.Customer.BirthPlace = corporateInfo.ExecutiveBirthPlace;
                dbSubscription.Customer.FathersName = corporateInfo.ExecutiveFathersName;
                dbSubscription.Customer.MothersMaidenName = corporateInfo.ExecutiveMothersMaidenName;
                dbSubscription.Customer.MothersName = corporateInfo.ExecutiveMothersName;
                dbSubscription.Customer.Nationality = corporateInfo.ExecutiveNationality.Value;
                dbSubscription.Customer.Profession = corporateInfo.ExecutiveProfession.Value;
                dbSubscription.Customer.Sex = corporateInfo.ExecutiveSex.Value;
                dbSubscription.Customer.CorporateCustomerInfo.TaxNo = corporateInfo.TaxNo;
                dbSubscription.Customer.CorporateCustomerInfo.TaxOffice = corporateInfo.TaxOffice;
                dbSubscription.Customer.CorporateCustomerInfo.Title = corporateInfo.Title;
                dbSubscription.Customer.CorporateCustomerInfo.TradeRegistrationNo = corporateInfo.TradeRegistrationNo;
                if (dbSubscription.Customer.Address.ApartmentID != corporateInfo.ExecutiveResidencyAddress.ApartmentID)
                    corporateInfo.ExecutiveResidencyAddress.CopyToDBObject(dbSubscription.Customer.Address);
                if (dbSubscription.Customer.CorporateCustomerInfo.Address.ApartmentID != corporateInfo.CompanyAddress.ApartmentID || corporateInfo.CompanyAddress.PostalCode != dbSubscription.Customer.CorporateCustomerInfo.Address.PostalCode || corporateInfo.CompanyAddress.Floor != dbSubscription.Customer.CorporateCustomerInfo.Address.Floor)
                    corporateInfo.CompanyAddress.CopyToDBObject(dbSubscription.Customer.CorporateCustomerInfo.Address);

                db.SystemLogs.Add(SystemLogProcessor.ChangeCustomer(User.GiveUserId(), dbSubscription.CustomerID, SystemLogInterface.MasterISS, null));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.IsEdit = true;
            return View(viewName: "Edits/EditCustomerCorporateInfo", model: corporateInfo);
        }

        [AuthorizePermission(Permissions = "Change Username")]
        [HttpGet]
        // GET: Client/ChangeUsername
        public ActionResult ChangeUsername(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var username = new UsernameEditViewModel()
            {
                Username = dbSubscription.RadiusAuthorization.Username.Substring(0, dbSubscription.RadiusAuthorization.Username.IndexOf('@')),
                Password = dbSubscription.RadiusAuthorization.Password,
                DomainName = dbSubscription.RadiusAuthorization.Username.Substring(dbSubscription.RadiusAuthorization.Username.IndexOf('@') + 1)
            };

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeUsername", model: username);
        }

        [AuthorizePermission(Permissions = "Change Username")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ChangeUsername
        public ActionResult ChangeUsername(long id, UsernameEditViewModel usernameModel)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            var domain = DomainsCache.GetDomainByID(dbSubscription.DomainID);
            if (domain == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                var sameUsername = usernameModel.Username == dbSubscription.RadiusAuthorization.Username.Split('@').FirstOrDefault();
                if (sameUsername || UsernameFactory.IsUsernameValid(domain, usernameModel.Username))
                {
                    var oldUsername = dbSubscription.RadiusAuthorization.Username;
                    var oldPassword = dbSubscription.RadiusAuthorization.Password;

                    if (!sameUsername)
                    {
                        dbSubscription.RadiusAuthorization.Username = usernameModel.Username + "@" + domain.Name;
                        db.SystemLogs.Add(SystemLogProcessor.ChangeSubscriberUsername(User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null, oldUsername, dbSubscription.RadiusAuthorization.Username));
                    }
                    if (oldPassword != usernameModel.Password)
                    {
                        dbSubscription.RadiusAuthorization.Password = usernameModel.Password;
                        db.SystemLogs.Add(SystemLogProcessor.ChangeSubscriberPassword(User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null, oldPassword, dbSubscription.RadiusAuthorization.Password));
                    }

                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
                }
                ModelState.AddModelError("Username", RadiusR.Localization.Validation.Common.UsernameExists);
            }

            usernameModel.DomainName = dbSubscription.RadiusAuthorization.Username.Substring(dbSubscription.RadiusAuthorization.Username.IndexOf('@') + 1);
            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeUsername", model: usernameModel);
        }


        [AuthorizePermission(Permissions = "Modify Clients")]
        // GET: Client/TransportActions
        public ActionResult TransportActions(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/TransportActions");
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        // GET: Client/ChangeInstallationAddress
        public ActionResult ChangeInstallationAddress(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var address = new AddressViewModel(subscription.Address);
            ViewBag.CustomerName = subscription.ValidDisplayName;
            var domain = DomainsCache.GetDomainByID(subscription.DomainID);
            ViewBag.DomainID = domain != null && domain.TelekomCredential != null ? domain.ID : (int?)null;
            return View(viewName: "Edits/ChangeInstallationAddress", model: address);
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ChangeInstallationAddress
        public ActionResult ChangeInstallationAddress(long id, AddressViewModel address)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                var oldAddress = subscription.Address.AddressText;
                address.CopyToDBObject(subscription.Address);
                // sytem log
                db.SystemLogs.Add(SystemLogProcessor.ChangeInstallationAddress(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null, oldAddress, address.AddressText));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            var domain = DomainsCache.GetDomainByID(subscription.DomainID);
            ViewBag.DomainID = domain != null && domain.TelekomCredential != null ? domain.ID : (int?)null;
            return View(viewName: "Edits/ChangeInstallationAddress", model: address);
        }

        [AuthorizePermission(Permissions = "Transport Subscription")]
        // GET: Client/Transport
        public ActionResult Transport(long id)
        {
            return RedirectToAction("Details", new { id = id, errorMessage = 9 });
        }

        [AuthorizePermission(Permissions = "Commitment")]
        // GET: Client/Commitment
        public ActionResult Commitment(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            SubscriptionCommitmentViewModel commitment = null;
            if (subscription.SubscriptionCommitment != null)
            {
                commitment = new SubscriptionCommitmentViewModel()
                {
                    CommitmentLength = subscription.SubscriptionCommitment.CommitmentLength,
                    CommitmentExpirationDate = subscription.SubscriptionCommitment.CommitmentExpirationDate
                };
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            ViewBag.HasCommitment = subscription.SubscriptionCommitment != null;
            return View(viewName: "Edits/Commitment", model: commitment);
        }

        [AuthorizePermission(Permissions = "Commitment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/Commitment
        public ActionResult Commitment(long id, SubscriptionCommitmentViewModel commitment, bool? remove = false)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (remove == true && subscription.SubscriptionCommitment != null)
            {
                db.SubscriptionCommitments.Remove(subscription.SubscriptionCommitment);
                // system log
                db.SystemLogs.Add(SystemLogProcessor.EditSubscriberCommitment(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
            }

            if (ModelState.IsValid)
            {
                if (subscription.SubscriptionCommitment != null)
                {
                    subscription.SubscriptionCommitment.CommitmentLength = commitment.CommitmentLength.Value;
                    subscription.SubscriptionCommitment.CommitmentExpirationDate = commitment.CommitmentExpirationDate.Value;
                }
                else
                {
                    subscription.SubscriptionCommitment = new SubscriptionCommitment()
                    {
                        CommitmentLength = commitment.CommitmentLength.Value,
                        CommitmentExpirationDate = commitment.CommitmentExpirationDate.Value
                    };
                }

                // system log
                db.SystemLogs.Add(SystemLogProcessor.EditSubscriberCommitment(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            ViewBag.HasCommitment = subscription.SubscriptionCommitment != null;
            return View(viewName: "Edits/Commitment", model: commitment);
        }

        [AuthorizePermission(Permissions = "Change Client Service")]
        [HttpGet]
        // GET: Client/ChangeService
        public ActionResult ChangeService(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.ServiceList = new SelectList(db.Services.Where(s => s.Domains.Select(d => d.ID).Contains(subscription.DomainID)).FilterActiveServices(subscription.ServiceID).Select(s => new { Name = s.Name, Value = s.ID }), "Value", "Name", subscription.ServiceID);
            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeService", model: new ChangeServiceViewModel()
            {
                ServiceID = subscription.ServiceID,
                BillingPeriod = subscription.PaymentDay
            });

        }

        [AuthorizePermission(Permissions = "Change Client Service")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ChangeService
        public ActionResult ChangeService(long id, ChangeServiceViewModel tariffChange)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            // no billing period for pre-paid
            var dbTariff = db.Services.Find(tariffChange.ServiceID);
            if (dbTariff != null && dbTariff.BillingType == (short)ServiceBillingType.PrePaid)
            {
                ModelState.Remove("BillingPeriod");
                tariffChange.BillingPeriod = 1;
            }

            if (ModelState.IsValid)
            {
                if (dbTariff == null || !dbTariff.Domains.Contains(dbSubscription.Domain))
                {
                    return RedirectToAction("Index", new { errorMessage = 2 });
                }

                var results = db.ChangeSubscriptionTariff(new TariffChangeOptions()
                {
                    ForceNow = false,
                    TariffID = tariffChange.ServiceID.Value,
                    SubscriptionID = dbSubscription.ID,
                    NewBillingPeriod = (short)tariffChange.BillingPeriod.Value,
                    Gateway = new GatewayOptions()
                    {
                        UserID = User.GiveUserId(),
                        InterfaceType = SystemLogInterface.MasterISS
                    }
                });

                if (results != TariffChangeResult.TariffChanged && results != TariffChangeResult.TariffChangeScheduled)
                {
                    return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 9 });
                }

                return RedirectToAction("Details", new { id = dbSubscription.ID, errorMessage = 0 });
            }

            ViewBag.ServiceList = new SelectList(db.Services.Where(s => s.Domains.Select(d => d.ID).Contains(dbSubscription.DomainID)).FilterActiveServices(tariffChange.ServiceID).Select(s => new { Name = s.Name, Value = s.ID }), "Value", "Name", tariffChange.ServiceID);
            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeService", model: new ChangeServiceViewModel()
            {
                ServiceID = tariffChange.ServiceID,
                BillingPeriod = tariffChange.BillingPeriod
            });
        }

        [AuthorizePermission(Permissions = "Change Client Service")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/ForceChangeTariff
        public ActionResult ForceChangeTariff(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (!dbSubscription.ChangeServiceTypeTasks.Any())
            {
                return RedirectToAction("Details", new { id = id, errorMessage = 9 });
            }
            var tariffChange = dbSubscription.ChangeServiceTypeTasks.LastOrDefault();

            var results = db.ChangeSubscriptionTariff(new TariffChangeOptions()
            {
                ForceNow = true,
                TariffID = tariffChange.NewServiceID,
                SubscriptionID = dbSubscription.ID,
                NewBillingPeriod = tariffChange.NewBillingPeriod,
                Gateway = new GatewayOptions()
                {
                    UserID = User.GiveUserId(),
                    InterfaceType = SystemLogInterface.MasterISS
                }
            });

            if (results != TariffChangeResult.TariffChanged)
            {
                return RedirectToAction("Details", new { id = id, errorMessage = 9 });
            }

            return RedirectToAction("Details", new { id = id, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Change Client Service")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/CancelChangeService
        public ActionResult CancelChangeService(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            //subscription.SubscriptionTariffChange = null;
            foreach (var scheduledTask in subscription.ChangeServiceTypeTasks)
            {
                var task = scheduledTask.SchedulerTask;
                db.ChangeServiceTypeTasks.Remove(scheduledTask);
                db.SchedulerTasks.Remove(task);

                db.SystemLogs.Add(SystemLogProcessor.CancelScheduledChangeService(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null));

            }

            db.SaveChanges();

            return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpGet]
        // GET: Client/EditGroups
        public ActionResult EditGroups(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.GroupList = new MultiSelectList(db.Groups.ToArray(), "ID", "Name", subscription.Groups.Select(g => g.ID));
            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/EditGroups");
        }

        [AuthorizePermission(Permissions = "Modify Clients")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/EditGroups
        public ActionResult EditGroups(long id, int[] groupIds)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            groupIds = groupIds.Distinct().ToArray();

            var dbGroups = db.Groups.Where(g => groupIds.Contains(g.ID)).ToArray();
            var addedGroups = dbGroups.Where(g => !subscription.Groups.Select(sg => sg.ID).Contains(g.ID)).ToArray();
            var removedGroups = subscription.Groups.Where(sg => !dbGroups.Select(g => g.ID).Contains(sg.ID)).ToArray();

            foreach (var group in addedGroups)
            {
                subscription.Groups.Add(group);
            }
            foreach (var group in removedGroups)
            {
                subscription.Groups.Remove(group);
            }

            db.SystemLogs.Add(SystemLogProcessor.ChangeSubscriberGroup(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null));
            db.SaveChanges();

            return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Change DSL No")]
        [HttpGet]
        // GET: Client/ChangeDSLNo
        public ActionResult ChangeDSLNo(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (subscription.SubscriptionTelekomInfo == null)
            {
                return RedirectToAction("Details", new { errorMessage = 9 });
            }

            var dslNoModel = new ChangeDSLNoViewModel()
            {
                DSLNo = subscription.SubscriptionTelekomInfo.SubscriptionNo
            };

            ViewBag.CustomerName = subscription.ValidDisplayName;
            ViewBag.DomainId = subscription.DomainID;
            return View(viewName: "Edits/ChangeDSLNo", model: dslNoModel);
        }

        [AuthorizePermission(Permissions = "Change DSL No")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ChangeDSLNo
        public ActionResult ChangeDSLNo(long id, ChangeDSLNoViewModel dslNoModel)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (subscription.SubscriptionTelekomInfo == null)
            {
                return RedirectToAction("Details", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                if (db.SubscriptionTelekomInfoes.Where(sti => sti.Subscription.State != (short)CustomerState.Cancelled).Any(tsi => tsi.SubscriptionNo == dslNoModel.DSLNo))
                {
                    ModelState.AddModelError("DSLNo", string.Format(RadiusR.Localization.Validation.Common.Unique, RadiusR.Localization.Model.RadiusR.TelekomSubscriberNo));
                }
                else
                {
                    var domain = DomainsCache.GetDomainByID(subscription.DomainID);
                    if (domain.TelekomCredential == null)
                    {
                        return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 9 });
                    }

                    var results = db.UpdateSubscriberTelekomInfoFromWebService(new TelekomSynchronizationOptions()
                    {
                        AppUserID = User.GiveUserId(),
                        LogInterface = SystemLogInterface.MasterISS,
                        DBSubscription = subscription,
                        DSLNo = dslNoModel.DSLNo
                    });
                    if (results.ResultCode != TelekomSynchronizationResultCodes.Success)
                    {
                        ViewBag.TelekomError = results.ResultCode == TelekomSynchronizationResultCodes.TelekomError ? string.Join(Environment.NewLine, results.TelekomExceptions.Select(tte => tte.GetShortMessage())) : GetSynchronizationErrorMessage(results.ResultCode);
                    }
                    else
                    {
                        var oldDSLNo = subscription.SubscriptionTelekomInfo.SubscriptionNo;

                        subscription.SubscriptionTelekomInfo.SubscriptionNo = dslNoModel.DSLNo;

                        db.SystemLogs.Add(SystemLogProcessor.ChangeSubscriberDSLNo(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null, oldDSLNo, dslNoModel.DSLNo));
                        db.SaveChanges();

                        return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
                    }
                }
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            ViewBag.DomainId = subscription.DomainID;
            return View(viewName: "Edits/ChangeDSLNo", model: dslNoModel);
        }

        [AuthorizePermission(Permissions = "Change Static IP")]
        [HttpGet]
        // GET: Client/ChangeStaticIP
        public ActionResult ChangeStaticIP(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var staticIP = new ChangeStaticIPViewModel()
            {
                IP = subscription.RadiusAuthorization.StaticIP
            };

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeStaticIP", model: staticIP);
        }

        [AuthorizePermission(Permissions = "Change Static IP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ChangeStaticIP
        public ActionResult ChangeStaticIP(long id, ChangeStaticIPViewModel staticIP)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                var oldIP = subscription.RadiusAuthorization.StaticIP;

                if (string.IsNullOrWhiteSpace(staticIP.IP))
                {
                    subscription.RadiusAuthorization.StaticIP = null;
                }
                else if (db.Subscriptions.Any(s => s.RadiusAuthorization.StaticIP == staticIP.IP))
                {
                    ModelState.AddModelError("IP", string.Format(RadiusR.Localization.Validation.Common.Unique, RadiusR.Localization.Model.RadiusR.StaticIP));
                }
                else
                {
                    subscription.RadiusAuthorization.StaticIP = staticIP.IP;
                }

                if (ModelState.IsValid)
                {
                    db.SystemLogs.Add(SystemLogProcessor.ChangeSubscriberStaticIP(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null, oldIP ?? "[]", subscription.RadiusAuthorization.StaticIP ?? "[]"));
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
                }
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeStaticIP", model: staticIP);
        }

        [AuthorizePermission(Permissions = "Modify Expiration Date")]
        [HttpGet]
        // GET: Client/ExtendExpirationDate
        public ActionResult ExtendExpirationDate(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (subscription.State != (short)CustomerState.Active || !subscription.RadiusAuthorization.ExpirationDate.HasValue)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var expirationDate = new ChangeExpirationDateViewModel()
            {
                NewDate = subscription.RadiusAuthorization.ExpirationDate.Value,
            };

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ExtendExpirationDate", model: expirationDate);
        }

        [AuthorizePermission(Permissions = "Modify Expiration Date")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ExtendExpirationDate
        public ActionResult ExtendExpirationDate(long id, [Bind(Include = "NewDate")] ChangeExpirationDateViewModel changedDate)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }
            if (subscription.State != (short)CustomerState.Active || !subscription.RadiusAuthorization.ExpirationDate.HasValue)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                var oldExpirationdate = subscription.RadiusAuthorization.ExpirationDate.HasValue ? subscription.RadiusAuthorization.ExpirationDate.Value.ToString("yyyy-MM-dd HH:mm") : "-";
                subscription.RadiusAuthorization.ExpirationDate = changedDate.NewDate;

                db.SystemLogs.Add(SystemLogProcessor.ExtendExpirationDate(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null, oldExpirationdate, changedDate.NewDate.ToString("yyyy-MM-dd HH:mm")));

                db.SaveChanges();

                return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ExtendExpirationDate", model: changedDate);
        }

        [AuthorizePermission(Permissions = "Edit Notes")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Cient/RemoveNote
        public ActionResult RemoveNote(long id)
        {
            var dbNote = db.SubscriptionNotes.Find(id);
            if (dbNote == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._21);
            }

            db.SubscriptionNotes.Remove(dbNote);
            db.SaveChanges();

            return RedirectToAction("Notes", "Client", new { id = dbNote.SubscriptionID });
        }

        [AuthorizePermission(Permissions = "Edit Notes")]
        [HttpGet]
        // GET: Client/EditNote
        public ActionResult EditNote(long id)
        {
            var dbNote = db.SubscriptionNotes.Find(id);
            if (dbNote == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._21);
            }

            var note = new NoteViewModel()
            {
                ID = dbNote.ID,
                SubscriptionID = dbNote.SubscriptionID,
                Date = dbNote.Date,
                Message = dbNote.Message
            };

            return View(viewName: "Edits/EditNote", model: note);
        }

        [AuthorizePermission(Permissions = "Edit Notes")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/EditNote
        public ActionResult EditNote(long id, [Bind(Include = "Message")] NoteViewModel note)
        {
            var dbNote = db.SubscriptionNotes.Find(id);
            if (dbNote == null)
            {
                return Content(RadiusR.Localization.Pages.ErrorMessages._21);
            }

            if (ModelState.IsValid)
            {
                dbNote.Message = note.Message;
                dbNote.WriterID = User.GiveUserId().Value;
                db.SaveChanges();

                return RedirectToAction("Notes", "Client", new { id = dbNote.SubscriptionID });
            }

            note = new NoteViewModel()
            {
                ID = dbNote.ID,
                SubscriptionID = dbNote.SubscriptionID,
                Date = dbNote.Date
            };

            return View(viewName: "Edits/EditNote", model: note);
        }

        [HttpGet]
        [AuthorizePermission(Permissions = "Discount")]
        // GET: Client/AddRecurringDiscount
        public ActionResult AddRecurringDiscount(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.FeeTypeList = new SelectList(new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetList().Select(li => new { Key = (short)li.Key, Value = li.Value }).Where(li => dbSubscription.Fees.Where(f => f.FeeTypeCost.IsAllTime).Select(f => f.FeeTypeID).Contains(li.Key) || li.Key == (short)FeeType.Tariff).ToArray(), "Key", "Value");
            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/AddRecurringDiscount", model: new RecurringDiscountViewModel());
        }

        [HttpPost]
        [AuthorizePermission(Permissions = "Discount")]
        [ValidateAntiForgeryToken]
        // POST: Client/AddRecurringDiscount
        public ActionResult AddRecurringDiscount(long id, RecurringDiscountViewModel recurringDiscount)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            FixRecurringDiscountModelState(string.Empty, recurringDiscount);

            if (ModelState.IsValid)
            {
                var dbRecurringDiscount = new RecurringDiscount()
                {
                    Amount = recurringDiscount._amount.Value,
                    ApplicationTimes = recurringDiscount.ApplicationTimes.Value,
                    ApplicationType = recurringDiscount.ApplicationType,
                    CreationTime = DateTime.Now,
                    DiscountType = recurringDiscount.DiscountType,
                    FeeTypeID = recurringDiscount.FeeTypeID,
                    IsDisabled = false,
                    OnlyFullInvoice = recurringDiscount.OnlyFullInvoice,
                    Description = recurringDiscount.Description
                };

                dbSubscription.RecurringDiscounts.Add(dbRecurringDiscount);

                db.SaveChanges();

                db.SystemLogs.Add(SystemLogProcessor.AddRecurringDiscount(dbRecurringDiscount.ID, User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null));
                db.SaveChanges();

                return Redirect(Url.Action("Details", new { id = dbSubscription.ID, errorMessage = 0 }) + "#additional-fees");
            }

            ViewBag.FeeTypeList = new SelectList(new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetList().Select(li => new { Key = (short)li.Key, Value = li.Value }).Where(li => dbSubscription.Fees.Where(f => f.FeeTypeCost.IsAllTime).Select(f => f.FeeTypeID).Contains(li.Key) || li.Key == (short)FeeType.Tariff).ToArray(), "Key", "Value", recurringDiscount.FeeTypeID);
            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/AddRecurringDiscount", model: recurringDiscount);
        }

        [AuthorizePermission(Permissions = "Discount")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Client/RemoveRecurringDiscount
        public ActionResult RemoveRecurringDiscount(long id, long subscriptionId)
        {
            var dbSubscription = db.Subscriptions.Find(subscriptionId);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var dbRecurringDiscount = db.RecurringDiscounts.Find(id);
            if (dbRecurringDiscount == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            DiscountUtilities.CancelRecurringDiscount(db, dbRecurringDiscount, new DiscountOperationOptions()
            {
                AppUserID = User.GiveUserId(),
                LogInterface = SystemLogInterface.MasterISS,
                CancellationCause = RadiusR.DB.Enums.RecurringDiscount.RecurringDiscountCancellationCause.Manual
            });

            db.SaveChanges();

            return RedirectToAction("RecurringDiscounts", new { id = dbSubscription.ID, errorMessage = 0 });
        }

        [AuthorizePermission(Permissions = "Add Referral Discount")]
        [HttpGet]
        // GET: Client/AddReferralDiscount
        public ActionResult AddReferralDiscount(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var validSpecialOffers = db.SpecialOffers.FilterActiveSpecialOffers().Select(so => new { Key = so.ID, Value = so.Name }).ToArray();
            ViewBag.SpecialOfferList = new SelectList(validSpecialOffers, "Key", "Value", (validSpecialOffers.Count() == 1) ? validSpecialOffers.FirstOrDefault().Key : (int?)null);
            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/AddReferralDiscount");
        }

        [AuthorizePermission(Permissions = "Add Referral Discount")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/AddReferralDiscount
        public ActionResult AddReferralDiscount(long id, SubscriptionReferralDiscountViewModel referralDiscount)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                Subscription referrerSubscription;
                SpecialOffer specialOffer;

                var validationResults = RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration.Registration.ValidateReferralDiscount(db, new RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration.CustomerRegistrationInfo.ReferralDiscountInfo()
                {
                    ReferenceNo = referralDiscount.ReferenceNo,
                    SpecialOfferID = referralDiscount.SpecialOfferID
                }, dbSubscription.Service, out referrerSubscription, out specialOffer);
                if (validationResults != null)
                {
                    foreach (var item in validationResults)
                    {
                        ModelState.AddModelError(item.Key, item.FirstOrDefault());
                    }
                }
                else if (referrerSubscription.ID == dbSubscription.ID)
                {
                    ModelState.AddModelError("ReferenceNo", RadiusR.Localization.Validation.ModelSpecific.SelfReferralDiscount);
                }
                else
                {
                    var discount = new RecurringDiscount(specialOffer);
                    discount.ReferrerRecurringDiscount = new RecurringDiscount(specialOffer);
                    discount.ReferrerRecurringDiscount.SubscriptionID = referrerSubscription.ID;

                    dbSubscription.RecurringDiscounts.Add(discount);
                    db.SaveChanges();

                    db.SystemLogs.Add(SystemLogProcessor.AddRecurringDiscount(discount.ID, User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null));
                    db.SystemLogs.Add(SystemLogProcessor.AddRecurringDiscount(discount.ReferrerRecurringDiscount.ID, User.GiveUserId(), referrerSubscription.ID, SystemLogInterface.MasterISS, null));
                    db.SaveChanges();

                    return Redirect(Url.Action("Details", new { id = dbSubscription.ID, errorMessage = 0 }) + "#additional-fees");
                }
            }

            var validSpecialOffers = db.SpecialOffers.FilterActiveSpecialOffers().Select(so => new { Key = so.ID, Value = so.Name }).ToArray();
            ViewBag.SpecialOfferList = new SelectList(validSpecialOffers, "Key", "Value", referralDiscount.SpecialOfferID);
            ViewBag.SubscriberName = dbSubscription.ValidDisplayName;
            return View(viewName: "Edits/AddReferralDiscount", model: referralDiscount);
        }

        [AuthorizePermission(Permissions = "Add Special Offer")]
        [HttpGet]
        // GET: Client/AddSpecialOffer
        public ActionResult AddSpecialOffer(long id)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.SpecialOffers = new SelectList(db.SpecialOffers.Where(so => !so.IsReferral).Select(so => new
            {
                Value = so.ID,
                Name = so.Name
            }).ToArray(), "Value", "Name");

            return View(viewName: "Edits/AddSpecialOffer");
        }

        [AuthorizePermission(Permissions = "Add Special Offer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/AddSpecialOffer
        public ActionResult AddSpecialOffer(long id, AddSubscriptionSpecialOfferViewModel specialOffer)
        {
            var dbSubscription = db.Subscriptions.Find(id);
            if (dbSubscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                var dbSpecialOffer = db.SpecialOffers.Find(specialOffer.SpecialOfferID.Value);
                if (dbSpecialOffer == null)
                {
                    return RedirectToAction("Index", new { errorMessage = 9 });
                }

                var dbRecurringDiscount = new RecurringDiscount()
                {
                    Amount = dbSpecialOffer.Amount,
                    ApplicationTimes = dbSpecialOffer.ApplicationTimes,
                    ApplicationType = dbSpecialOffer.ApplicationType,
                    CreationTime = DateTime.Now,
                    DiscountType = dbSpecialOffer.DiscountType,
                    FeeTypeID = dbSpecialOffer.FeeTypeID,
                    IsDisabled = false,
                    OnlyFullInvoice = dbSpecialOffer.OnlyFullInvoice,
                    Description = dbSpecialOffer.Name
                };

                dbSubscription.RecurringDiscounts.Add(dbRecurringDiscount);

                db.SaveChanges();

                db.SystemLogs.Add(SystemLogProcessor.AddRecurringDiscount(dbRecurringDiscount.ID, User.GiveUserId(), dbSubscription.ID, SystemLogInterface.MasterISS, null));
                db.SaveChanges();

                return Redirect(Url.Action("Details", new { id = dbSubscription.ID, errorMessage = 0 }) + "#additional-fees");
            }

            ViewBag.CustomerName = dbSubscription.ValidDisplayName;
            ViewBag.SpecialOffers = new SelectList(db.SpecialOffers.Where(so => !so.IsReferral).Select(so => new
            {
                Value = so.ID,
                Name = so.Name
            }).ToArray(), "Value", "Name", specialOffer.SpecialOfferID);

            return View(viewName: "Edits/AddSpecialOffer", model: specialOffer);
        }

        [AuthorizePermission(Permissions = "Change CLID")]
        [HttpGet]
        // GET: Client/ChangeCLID
        public ActionResult ChangeCLID(long id)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            var CLIDModel = new ChangeCLIDViewModel()
            {
                CLID = subscription.RadiusAuthorization.CLID
            };

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeCLID", model: CLIDModel);
        }

        [AuthorizePermission(Permissions = "Add Special Offer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Client/ChangeCLID
        public ActionResult ChangeCLID(long id, ChangeCLIDViewModel CLIDModel)
        {
            var subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return RedirectToAction("Index", new { errorMessage = 4 });
            }

            if (ModelState.IsValid)
            {
                var oldCLID = subscription.RadiusAuthorization.CLID;
                var newCLID = string.IsNullOrWhiteSpace(CLIDModel.CLID) ? null : CLIDModel.CLID;
                subscription.RadiusAuthorization.CLID = newCLID;
                db.SystemLogs.Add(SystemLogProcessor.ChangeCLID(User.GiveUserId(), subscription.ID, SystemLogInterface.MasterISS, null, oldCLID, newCLID));
                db.SaveChanges();

                return RedirectToAction("Details", new { id = subscription.ID, errorMessage = 0 });
            }

            ViewBag.CustomerName = subscription.ValidDisplayName;
            return View(viewName: "Edits/ChangeCLID", model: CLIDModel);
        }
    }
}