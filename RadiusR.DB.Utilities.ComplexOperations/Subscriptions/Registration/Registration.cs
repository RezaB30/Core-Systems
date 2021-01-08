using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RadiusR.DB.DomainsCache;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using RezaB.API.TCKValidation;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration
{
    public static class Registration
    {
        public static ILookup<string, string> RegisterSubscriptionWithNewCustomer(this RadiusREntities db, CustomerRegistrationInfo registrationInfo, out Customer registeredCustomer, bool skipIDCardValidation = false)
        {
            registeredCustomer = null;
            // validate inputs
            var validationResults = registrationInfo.Validate();
            if (validationResults != null)
                return validationResults;
            // check customer type with registraion info
            var customerType = registrationInfo.GeneralInfo.CustomerType.Value;
            if ((customerType == Enums.CustomerType.Individual && registrationInfo.IndividualInfo == null) || (customerType != Enums.CustomerType.Individual && registrationInfo.CorporateInfo == null))
            {
                // insufficient data for this customer type 
                return new[] { new { Key = "GeneralInfo.CustomerType", ErrorMessage = Resources.RegistrationValidationMessages.InsufficientDataForCustomerType } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            // id card validation
            if (!skipIDCardValidation)
            {
                var validationClient = new TCKValidationClient();
                switch (registrationInfo.IDCard.CardType.Value)
                {
                    case Enums.IDCardTypes.TCIDCardWithChip:
                        if (string.IsNullOrWhiteSpace(registrationInfo.IDCard.SerialNo))
                            return new[] { new { Key = "IDCard.SerialNo", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                        else
                        {
                            var checkResult = validationClient.ValidateNewTCK(registrationInfo.IDCard.TCKNo, registrationInfo.IDCard.FirstName.ToUpper(), registrationInfo.IDCard.LastName.ToUpper(), registrationInfo.IDCard.BirthDate.Value, registrationInfo.IDCard.SerialNo);
                            if (checkResult == false)
                                return new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                        }
                        break;
                    case Enums.IDCardTypes.TCBirthCertificate:
                        if (string.IsNullOrWhiteSpace(registrationInfo.IDCard.SerialNo) || registrationInfo.IDCard.SerialNo.Length < 9)
                            return new[] { new { Key = "IDCard.SerialNo", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                        else
                        {
                            var checkResult = validationClient.ValidateOldTCK(registrationInfo.IDCard.TCKNo, registrationInfo.IDCard.FirstName.ToUpper(), registrationInfo.IDCard.LastName.ToUpper(), registrationInfo.IDCard.BirthDate.Value, registrationInfo.IDCard.SerialNo.Substring(3), registrationInfo.IDCard.SerialNo.Substring(0, 3));
                            if (checkResult == false)
                                return new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                        }
                        break;
                    case Enums.IDCardTypes.TCDrivingLisence:
                    case Enums.IDCardTypes.TCForeignerIDCard:
                        {
                            var checkResult = validationClient.ValidateOthers(registrationInfo.IDCard.TCKNo, registrationInfo.IDCard.FirstName.ToUpper(), registrationInfo.IDCard.LastName.ToUpper(), registrationInfo.IDCard.BirthDate.Value);
                            if (checkResult == false)
                                return new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                        }
                        break;
                    case Enums.IDCardTypes.TCPassportWithChip:
                    case Enums.IDCardTypes.OldTCPassportDarkBlue:
                    case Enums.IDCardTypes.OldTCPassportGreen:
                    case Enums.IDCardTypes.OldTCPassportGrey:
                    case Enums.IDCardTypes.OldTCPassportRed:
                    case Enums.IDCardTypes.TCPassportTemporary:
                    case Enums.IDCardTypes.ForeignerPassport:
                    case Enums.IDCardTypes.FlightCrewCertificate:
                    case Enums.IDCardTypes.ShipmansCertificate:
                    case Enums.IDCardTypes.NATOOrderDocument:
                    case Enums.IDCardTypes.TravelDocument:
                    case Enums.IDCardTypes.BorderCrossingDocument:
                    case Enums.IDCardTypes.ShipCommanderApprovedPersonnelList:
                    case Enums.IDCardTypes.TCProsecutorJudgeIDCard:
                    case Enums.IDCardTypes.TCLawyerIDCard:
                    case Enums.IDCardTypes.TCTemporaryIDCard:
                    case Enums.IDCardTypes.TCBlueCard:
                    case Enums.IDCardTypes.TCInternationalFamilyCertificate:
                    default:
                        return new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                }
            }
            // create db object for customer + id card
            registeredCustomer = db.Customers.FirstOrDefault(c => c.CustomerType == (short)customerType && c.CustomerIDCard.TCKNo == registrationInfo.IDCard.TCKNo);
            var customerExists = true;
            if (registeredCustomer == null)
            {
                customerExists = false;
                registeredCustomer = new Customer()
                {
                    BillingAddress = registrationInfo.GeneralInfo.BillingAddress.GetDbObject(),
                    ContactPhoneNo = registrationInfo.GeneralInfo.ContactPhoneNo,
                    Culture = registrationInfo.GeneralInfo.Culture,
                    CustomerType = (short)registrationInfo.GeneralInfo.CustomerType,
                    Email = registrationInfo.GeneralInfo.Email,
                    CustomerAdditionalPhoneNoes = registrationInfo.GeneralInfo.OtherPhoneNos != null ? registrationInfo.GeneralInfo.OtherPhoneNos.Select(pn => new CustomerAdditionalPhoneNo() { PhoneNo = pn.Number }).ToList() : null,
                    CustomerIDCard = new CustomerIDCard()
                    {
                        DateOfIssue = registrationInfo.IDCard.DateOfIssue,
                        District = registrationInfo.IDCard.District,
                        Neighbourhood = registrationInfo.IDCard.Neighbourhood,
                        PageNo = registrationInfo.IDCard.PageNo,
                        PassportNo = registrationInfo.IDCard.PassportNo,
                        PlaceOfIssue = registrationInfo.IDCard.PlaceOfIssue,
                        Province = registrationInfo.IDCard.Province,
                        RowNo = registrationInfo.IDCard.RowNo,
                        SerialNo = registrationInfo.IDCard.SerialNo,
                        TCKNo = registrationInfo.IDCard.TCKNo,
                        TypeID = (short)registrationInfo.IDCard.CardType.Value,
                        VolumeNo = registrationInfo.IDCard.VolumeNo
                    }
                };
                // general
                registeredCustomer.FirstName = registrationInfo.IDCard.FirstName;
                registeredCustomer.LastName = registrationInfo.IDCard.LastName;
                registeredCustomer.BirthDate = registrationInfo.IDCard.BirthDate.Value;
                // individual
                if (customerType == Enums.CustomerType.Individual)
                {
                    registeredCustomer.BirthPlace = registrationInfo.IndividualInfo.BirthPlace;
                    registeredCustomer.FathersName = registrationInfo.IndividualInfo.FathersName;
                    registeredCustomer.MothersMaidenName = registrationInfo.IndividualInfo.MothersMaidenName;
                    registeredCustomer.MothersName = registrationInfo.IndividualInfo.MothersName;
                    registeredCustomer.Nationality = (short)registrationInfo.IndividualInfo.Nationality.Value;
                    registeredCustomer.Profession = (short)registrationInfo.IndividualInfo.Profession.Value;
                    registeredCustomer.Address = registrationInfo.IndividualInfo.ResidencyAddress.GetDbObject();
                    registeredCustomer.Sex = (short)registrationInfo.IndividualInfo.Sex.Value;
                }
                // corporate
                else
                {
                    registeredCustomer.BirthPlace = registrationInfo.CorporateInfo.ExecutiveBirthPlace;
                    registeredCustomer.FathersName = registrationInfo.CorporateInfo.ExecutiveFathersName;
                    registeredCustomer.MothersMaidenName = registrationInfo.CorporateInfo.ExecutiveMothersMaidenName;
                    registeredCustomer.MothersName = registrationInfo.CorporateInfo.ExecutiveMothersName;
                    registeredCustomer.Nationality = (short)registrationInfo.CorporateInfo.ExecutiveNationality.Value;
                    registeredCustomer.Profession = (short)registrationInfo.CorporateInfo.ExecutiveProfession.Value;
                    registeredCustomer.Address = registrationInfo.CorporateInfo.ExecutiveResidencyAddress.GetDbObject();
                    registeredCustomer.Sex = (short)registrationInfo.CorporateInfo.ExecutiveSex.Value;

                    registeredCustomer.CorporateCustomerInfo = new CorporateCustomerInfo()
                    {
                        CentralSystemNo = registrationInfo.CorporateInfo.CentralSystemNo,
                        Address = registrationInfo.CorporateInfo.CompanyAddress.GetDbObject(),
                        TaxNo = registrationInfo.CorporateInfo.TaxNo,
                        TaxOffice = registrationInfo.CorporateInfo.TaxOffice,
                        Title = registrationInfo.CorporateInfo.Title,
                        TradeRegistrationNo = registrationInfo.CorporateInfo.TradeRegistrationNo
                    };
                }
            }
            // add subscription db object to customer
            validationResults = db.RegisterSubscriptionForExistingCustomer(registrationInfo.SubscriptionInfo, registeredCustomer);
            if (validationResults != null)
            {
                registeredCustomer = null;
                return validationResults.SelectMany(vr => vr.Select(vr2 => new { Key = "SubscriptionInfo." + vr.Key, Value = vr2 })).ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            }
            // success
            if (customerExists)
            {
                registeredCustomer = null;
            }
            return null;
        }

        public static ILookup<string, string> RegisterSubscriptionForExistingCustomer(this RadiusREntities db, CustomerRegistrationInfo.SubscriptionRegistrationInfo registrationInfo, Customer referenceCustomer)
        {
            // validate inputs
            var validationResults = registrationInfo.Validate();
            if (validationResults != null)
                return validationResults;
            // validate logics
            var selectedDomain = DomainsCache.DomainsCache.GetDomainByID(registrationInfo.DomainID.Value);
            if (selectedDomain == null)
            {
                // invalid domain
                return new[] { new { Key = "DomainID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidDomain } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            var selectedTariff = db.Services.FirstOrDefault(s => s.ID == registrationInfo.ServiceID && s.Domains.Select(d => d.ID).Contains(selectedDomain.ID));
            if (selectedTariff == null)
            {
                // invalid tariff
                return new[] { new { Key = "TariffID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTariff } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            if (selectedTariff.HasBilling && !selectedTariff.ServiceBillingPeriods.Any(sbp => sbp.DayOfMonth == registrationInfo.BillingPeriod))
            {
                // invalid billing period
                return new[] { new { Key = "BillingPeriod", ErrorMessage = Resources.RegistrationValidationMessages.InvalidBillingPeriod } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            if (selectedDomain.TelekomCredential == null && registrationInfo.TelekomDetailedInfo != null)
            {
                // telekom info not valid for this domain
                return new[] { new { Key = "TelekomDetailedInfo", ErrorMessage = Resources.RegistrationValidationMessages.TelekomInfoNotValidForDomain } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            // referall discount ligics
            Subscription referrerSubscription = null;
            SpecialOffer specialOffer = null;
            if (registrationInfo.ReferralDiscount != null/*!string.IsNullOrEmpty(registrationInfo.ReferenceNo)*/)
            {
                var results = ValidateReferralDiscount(db, registrationInfo.ReferralDiscount, selectedTariff, out referrerSubscription, out specialOffer, "ReferralDiscount");
                if (results != null)
                    return results;
            }

            // validate partner info
            if (registrationInfo.RegisteringPartner != null)
            {
                var dbPartner = db.Partners.Find(registrationInfo.RegisteringPartner.PartnerID);
                if (dbPartner == null)
                    // invalid partner
                    return new[] { new { Key = "RegisteringPartner.PartnerID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidPartner } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                var commitmentLength = registrationInfo.CommitmentInfo != null ? (short?)registrationInfo.CommitmentInfo.CommitmentLength : null;
                if (!dbPartner.PartnerGroup.PartnerAvailableTariffs.Any(pat => pat.TariffID == registrationInfo.ServiceID && pat.Commitment == commitmentLength))
                {
                    // invalid tariff for partner
                    return new[] { new { Key = "ServiceID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTariffForPartner }, new { Key = "CommitmentInfo.CommitmentLength", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTariffForPartner } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                }
            }
            var dbGroups = new List<Group>();
            if (registrationInfo.GroupIds != null && registrationInfo.GroupIds.Any())
            {
                dbGroups = db.Groups.Where(g => registrationInfo.GroupIds.Contains(g.ID)).ToList();
                if (dbGroups.Count() != registrationInfo.GroupIds.Count())
                {
                    // invalid groups
                    return new[] { new { Key = "GroupIDs", ErrorMessage = Resources.RegistrationValidationMessages.InvalidGroups } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                }
            }
            // create database object
            var dbFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray();
            var addedFees = registrationInfo.AddedFeesInfo != null ? registrationInfo.AddedFeesInfo.SelectMany(af => af.GetDBObjects(dbFees)).ToList() : new List<Fee>();
            // invalid fees
            if (addedFees.Any(af => af == null))
            {
                // invalid added fees
                return new[] { new { Key = "AddedFeesInfo", ErrorMessage = Resources.RegistrationValidationMessages.InvalidAddedFees } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            var dbSubscription = new Subscription()
            {
                Address = registrationInfo.SetupAddress.GetDbObject(),
                DomainID = selectedDomain.ID,
                Fees = registrationInfo.AddedFeesInfo != null ? registrationInfo.AddedFeesInfo.SelectMany(af => af.GetDBObjects(dbFees)).ToList() : null,
                Groups = dbGroups,
                MembershipDate = DateTime.Now,
                PaymentDay = registrationInfo.BillingPeriod.Value,
                RadiusPassword = RandomGenerator.GenerateRadiusPassword(),
                ServiceID = selectedTariff.ID,
                SimultaneousUse = 1,
                State = (short)Enums.CustomerState.Registered,
                StaticIP = registrationInfo.StaticIP,
                SubscriberNo = UsernameFactory.GenerateUniqueSubscriberNo(selectedDomain),
                ReferenceNo = UsernameFactory.GenerateUniqueReferenceNo(),
                SubscriptionCommitment = registrationInfo.CommitmentInfo != null ? new SubscriptionCommitment()
                {
                    CommitmentLength = (short)registrationInfo.CommitmentInfo.CommitmentLength,
                    CommitmentExpirationDate = registrationInfo.CommitmentInfo.CommitmentExpirationDate.Value
                } : null,
                Username = string.IsNullOrEmpty(registrationInfo.Username) ? UsernameFactory.GenerateUsername(selectedDomain) : registrationInfo.Username + "@" + selectedDomain.Name,
                PartnerRegisteredSubscription = registrationInfo.RegisteringPartner != null ? new PartnerRegisteredSubscription()
                {
                    Allowance = Math.Abs(registrationInfo.RegisteringPartner.Allowance.Value),
                    AllowanceThreshold = Math.Abs(registrationInfo.RegisteringPartner.AllowanceThreshold.Value),
                    Commitment = registrationInfo.CommitmentInfo != null ? (short?)registrationInfo.CommitmentInfo.CommitmentLength : null,
                    PartnerID = registrationInfo.RegisteringPartner.PartnerID.Value,
                    TariffID = registrationInfo.ServiceID.Value,
                    RegistrationDate = DateTime.Now
                } : null,
                RecurringDiscounts = specialOffer != null ?
                new List<RecurringDiscount>()
                {
                    new RecurringDiscount()
                    {
                        Amount = specialOffer.Amount,
                        ApplicationTimes = specialOffer.ApplicationTimes,
                        ApplicationType = specialOffer.ApplicationType,
                        CreationTime = DateTime.Now,
                        Description = specialOffer.Name,
                        DiscountType = specialOffer.DiscountType,
                        FeeTypeID = specialOffer.FeeTypeID,
                        IsDisabled = false,
                        OnlyFullInvoice = specialOffer.OnlyFullInvoice,
                        ReferrerRecurringDiscount = new RecurringDiscount()
                        {
                            Amount = specialOffer.Amount,
                            ApplicationTimes = specialOffer.ApplicationTimes,
                            ApplicationType = specialOffer.ApplicationType,
                            CreationTime = DateTime.Now,
                            Description = specialOffer.Name,
                            DiscountType = specialOffer.DiscountType,
                            FeeTypeID = specialOffer.FeeTypeID,
                            IsDisabled = false,
                            OnlyFullInvoice = specialOffer.OnlyFullInvoice,
                            SubscriptionID = referrerSubscription.ID
                        }
                    }
                }
                : null
            };
            // telekom domain specific
            if (selectedDomain.TelekomCredential != null)
            {
                // has attached telekom info
                if (registrationInfo.TelekomDetailedInfo != null)
                {
                    // has xdsl no means it has been registered to telekom previously
                    if (!string.IsNullOrWhiteSpace(registrationInfo.TelekomDetailedInfo.SubscriberNo))
                    {
                        var telekomServiceClient = new TelekomSubscriberInfoServiceClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword);
                        var response = telekomServiceClient.GetSubscriberDetailedInfo(registrationInfo.TelekomDetailedInfo.SubscriberNo);
                        if (response.InternalException != null || !response.Data.PacketCode.HasValue || !response.Data.TariffCode.HasValue)
                        {
                            // could not syncronize with telekom
                            return new[] { new { Key = "TelekomDetailedInfo.SubscriberNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.TelekomSyncError, response.InternalException.GetShortMessage()) } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                        }
                        else
                        {
                            var selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, response.Data.PacketCode.Value, response.Data.TariffCode.Value);
                            if (selectedTelekomTariff == null)
                            {
                                // invalid telekom tariff
                                return new[] { new { Key = "TelekomDetailedInfo.SubscriberNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.InvalidTelekomTariff, response.Data.PacketCode, response.Data.TariffCode) } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                            }
                            dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo()
                            {
                                PacketCode = selectedTelekomTariff.PacketCode,
                                PSTN = registrationInfo.TelekomDetailedInfo.PSTN,
                                SubscriptionNo = registrationInfo.TelekomDetailedInfo.SubscriberNo,
                                TariffCode = selectedTelekomTariff.TariffCode,
                                TTCustomerCode = response.Data.CustomerCode ?? selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt,
                                XDSLType = response.Data.DSLType.HasValue ? (short)response.Data.DSLType.Value : (short)0
                            };
                        }
                    }
                    // has no xdsl no.
                    else
                    {
                        dbSubscription.State = (short)Enums.CustomerState.PreRegisterd;
                        // has telekom tariff info attached
                        if (registrationInfo.TelekomDetailedInfo.TelekomTariffInfo != null)
                        {
                            var selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, registrationInfo.TelekomDetailedInfo.TelekomTariffInfo.PacketCode.Value, registrationInfo.TelekomDetailedInfo.TelekomTariffInfo.TariffCode.Value);
                            if (selectedTelekomTariff == null)
                            {
                                // invalid telekom tariff
                                return new[] { new { Key = "TelekomDetailedInfo.TelekomTariffInfo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.InvalidTelekomTariff, registrationInfo.TelekomDetailedInfo.TelekomTariffInfo.PacketCode, registrationInfo.TelekomDetailedInfo.TelekomTariffInfo.TariffCode) } }.ToLookup(item => item.Key, item => item.ErrorMessage);
                            }
                            dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo()
                            {
                                TTCustomerCode = !string.IsNullOrWhiteSpace(registrationInfo.TelekomDetailedInfo.CustomerCode) ? long.Parse(registrationInfo.TelekomDetailedInfo.CustomerCode) : selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt,
                                SubscriptionNo = " ",
                                PacketCode = selectedTelekomTariff.PacketCode,
                                TariffCode = selectedTelekomTariff.TariffCode,
                                XDSLType = (short)selectedTelekomTariff.XDSLType,
                                PSTN = registrationInfo.TelekomDetailedInfo.PSTN
                            };
                        }
                        // no telekom tariff info attached
                        else
                        {
                            dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo()
                            {
                                SubscriptionNo = " ",
                                PSTN = registrationInfo.TelekomDetailedInfo.PSTN,
                                TTCustomerCode = string.IsNullOrWhiteSpace(registrationInfo.TelekomDetailedInfo.CustomerCode) ? long.Parse(registrationInfo.TelekomDetailedInfo.CustomerCode) : selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt
                            };
                        }
                    }
                }
                // no attached telekom info
                else
                {
                    dbSubscription.State = (short)Enums.CustomerState.PreRegisterd;
                }
            }
            // add to customer
            referenceCustomer.Subscriptions.Add(dbSubscription);
            // return no error           
            return null;
        }

        public static ILookup<string, string> ValidateReferralDiscount(RadiusREntities db, CustomerRegistrationInfo.ReferralDiscountInfo referralDiscount, Service selectedTariff, out Subscription referrerSubscription, out SpecialOffer specialOffer, string prefix = null)
        {
            referrerSubscription = null;
            specialOffer = null;
            // check reference no validity
            referrerSubscription = db.Subscriptions.FirstOrDefault(s => s.ReferenceNo == referralDiscount.ReferenceNo);
            if (referrerSubscription == null || referrerSubscription.IsCancelled || referrerSubscription.State == (short)Enums.CustomerState.Disabled)
            {
                // invalid reference no
                return new[] { new { Key = prefix + "ReferenceNo", ErrorMessage = Resources.RegistrationValidationMessages.InvalidReferenceNo } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            if (!referralDiscount.SpecialOfferID.HasValue)
            {
                // should have special offer
                return new[] { new { Key = prefix + "SpecialOfferID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidSpecialOffer } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }
            if (!selectedTariff.HasBilling || !referrerSubscription.Service.HasBilling)
            {
                // not valid for pre-paid tariffs (both sides)
                return new[] { new { Key = prefix + "ReferenceNo", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTariffTypeForReferralDiscount } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }

            specialOffer = db.SpecialOffers.Find(referralDiscount.SpecialOfferID.Value);

            if (!specialOffer.IsReferral || specialOffer.StartDate > DateTime.Now.Date || specialOffer.EndDate < DateTime.Now.Date)
            {
                // non referral special offer
                return new[] { new { Key = prefix + "SpecialOfferID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidSpecialOffer } }.ToLookup(item => item.Key, item => item.ErrorMessage);
            }

            return null;
        }
    }
}
