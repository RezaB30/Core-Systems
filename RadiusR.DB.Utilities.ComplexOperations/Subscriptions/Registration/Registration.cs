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
        public static RegistrationResult RegisterSubscriptionWithNewCustomer(this RadiusREntities db, CustomerRegistrationInfo registrationInfo, out Customer registeredCustomer, bool skipIDCardValidation = false)
        {
            registeredCustomer = null;
            // validate inputs
            var validationResults = registrationInfo.Validate();
            if (validationResults != null)
                return new RegistrationResult()
                {
                    ValidationMessages = validationResults
                };
            // check customer type with registraion info
            var customerType = registrationInfo.GeneralInfo.CustomerType.Value;
            if ((customerType == Enums.CustomerType.Individual && registrationInfo.IndividualInfo == null) || (customerType != Enums.CustomerType.Individual && registrationInfo.CorporateInfo == null))
            {
                // insufficient data for this customer type 
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "GeneralInfo.CustomerType", ErrorMessage = Resources.RegistrationValidationMessages.InsufficientDataForCustomerType } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            // id card validation
            if (!skipIDCardValidation)
            {
                var validationClient = new TCKValidationClient();
                switch (registrationInfo.IDCard.CardType.Value)
                {
                    case Enums.IDCardTypes.TCIDCardWithChip:
                        if (string.IsNullOrWhiteSpace(registrationInfo.IDCard.SerialNo))
                            return new RegistrationResult()
                            {
                                ValidationMessages = new[] { new { Key = "IDCard.SerialNo", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                            };
                        else
                        {
                            var checkResult = validationClient.ValidateNewTCK(registrationInfo.IDCard.TCKNo, registrationInfo.IDCard.FirstName.ToUpper(), registrationInfo.IDCard.LastName.ToUpper(), registrationInfo.IDCard.BirthDate.Value, registrationInfo.IDCard.SerialNo);
                            if (checkResult == false)
                                return new RegistrationResult()
                                {
                                    ValidationMessages = new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                };
                        }
                        break;
                    case Enums.IDCardTypes.TCBirthCertificate:
                        if (string.IsNullOrWhiteSpace(registrationInfo.IDCard.SerialNo) || registrationInfo.IDCard.SerialNo.Length < 9)
                            return new RegistrationResult()
                            {
                                ValidationMessages = new[] { new { Key = "IDCard.SerialNo", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                            };
                        else
                        {
                            var checkResult = validationClient.ValidateOldTCK(registrationInfo.IDCard.TCKNo, registrationInfo.IDCard.FirstName.ToUpper(), registrationInfo.IDCard.LastName.ToUpper(), registrationInfo.IDCard.BirthDate.Value, registrationInfo.IDCard.SerialNo.Substring(3), registrationInfo.IDCard.SerialNo.Substring(0, 3));
                            if (checkResult == false)
                                return new RegistrationResult()
                                {
                                    ValidationMessages = new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                };
                        }
                        break;
                    case Enums.IDCardTypes.TCDrivingLisence:
                    case Enums.IDCardTypes.TCForeignerIDCard:
                        {
                            var checkResult = validationClient.ValidateOthers(registrationInfo.IDCard.TCKNo, registrationInfo.IDCard.FirstName.ToUpper(), registrationInfo.IDCard.LastName.ToUpper(), registrationInfo.IDCard.BirthDate.Value);
                            if (checkResult == false)
                                return new RegistrationResult()
                                {
                                    ValidationMessages = new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                };
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
                        return new RegistrationResult()
                        {
                            ValidationMessages = new[] { new { Key = "IDCard", ErrorMessage = Resources.RegistrationValidationMessages.IDCardValidationFailed } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                        };
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
                registeredCustomer.FirstName = registrationInfo.IDCard.FirstName.Trim();
                registeredCustomer.LastName = registrationInfo.IDCard.LastName.Trim();
                registeredCustomer.BirthDate = registrationInfo.IDCard.BirthDate.Value;
                // individual
                if (customerType == Enums.CustomerType.Individual)
                {
                    registeredCustomer.BirthPlace = registrationInfo.IndividualInfo.BirthPlace.Trim();
                    registeredCustomer.FathersName = registrationInfo.IndividualInfo.FathersName.Trim();
                    registeredCustomer.MothersMaidenName = registrationInfo.IndividualInfo.MothersMaidenName.Trim();
                    registeredCustomer.MothersName = registrationInfo.IndividualInfo.MothersName.Trim();
                    registeredCustomer.Nationality = (short)registrationInfo.IndividualInfo.Nationality.Value;
                    registeredCustomer.Profession = (short)registrationInfo.IndividualInfo.Profession.Value;
                    registeredCustomer.Address = registrationInfo.IndividualInfo.ResidencyAddress.GetDbObject();
                    registeredCustomer.Sex = (short)registrationInfo.IndividualInfo.Sex.Value;
                }
                // corporate
                else
                {
                    registeredCustomer.BirthPlace = registrationInfo.CorporateInfo.ExecutiveBirthPlace.Trim();
                    registeredCustomer.FathersName = registrationInfo.CorporateInfo.ExecutiveFathersName.Trim();
                    registeredCustomer.MothersMaidenName = registrationInfo.CorporateInfo.ExecutiveMothersMaidenName.Trim();
                    registeredCustomer.MothersName = registrationInfo.CorporateInfo.ExecutiveMothersName.Trim();
                    registeredCustomer.Nationality = (short)registrationInfo.CorporateInfo.ExecutiveNationality.Value;
                    registeredCustomer.Profession = (short)registrationInfo.CorporateInfo.ExecutiveProfession.Value;
                    registeredCustomer.Address = registrationInfo.CorporateInfo.ExecutiveResidencyAddress.GetDbObject();
                    registeredCustomer.Sex = (short)registrationInfo.CorporateInfo.ExecutiveSex.Value;

                    registeredCustomer.CorporateCustomerInfo = new CorporateCustomerInfo()
                    {
                        CentralSystemNo = registrationInfo.CorporateInfo.CentralSystemNo.Trim(),
                        Address = registrationInfo.CorporateInfo.CompanyAddress.GetDbObject(),
                        TaxNo = registrationInfo.CorporateInfo.TaxNo.Trim(),
                        TaxOffice = registrationInfo.CorporateInfo.TaxOffice,
                        Title = registrationInfo.CorporateInfo.Title.Trim(),
                        TradeRegistrationNo = registrationInfo.CorporateInfo.TradeRegistrationNo.Trim()
                    };
                }
            }
            // add subscription db object to customer
            var subscriptionValidationResults = db.RegisterSubscriptionForExistingCustomer(registrationInfo.SubscriptionInfo, registeredCustomer);
            if (!subscriptionValidationResults.IsSuccess)
            {
                registeredCustomer = null;
                return new RegistrationResult()
                {
                    ValidationMessages = subscriptionValidationResults.ValidationMessages.SelectMany(vr => vr.Select(vr2 => new { Key = "SubscriptionInfo." + vr.Key, Value = vr2 })).ToLookup(kvp => kvp.Key, kvp => kvp.Value)
                };
            }
            // success
            if (customerExists)
            {
                return new RegistrationResult()
                {
                    DoesCustomerExist = true
                };
            }
            else
            {
                return new RegistrationResult()
                {
                    DoesCustomerExist = false
                };
            }
        }

        public static RegistrationResult RegisterSubscriptionForExistingCustomer(this RadiusREntities db, CustomerRegistrationInfo.SubscriptionRegistrationInfo registrationInfo, Customer referenceCustomer)
        {
            // prevent unnecessary inputs based on registration type
            if (registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer || registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transition)
            {
                if (registrationInfo.TelekomDetailedInfo != null)
                {
                    // telekom info not valid for this registration type
                    return new RegistrationResult()
                    {
                        ValidationMessages = new[] { new { Key = "TelekomDetailedInfo", ErrorMessage = Resources.RegistrationValidationMessages.TelekomInfoNotValidForRegistrationType } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                    };
                }
            }

            // validate inputs
            var validationResults = registrationInfo.Validate();
            if (validationResults != null)
                return new RegistrationResult()
                {
                    ValidationMessages = validationResults
                };
            // validate logics
            if (registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer && !registrationInfo.TransferringSubsciptionID.HasValue)
            {
                // transferring subscription id required
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "TransferringSubsciptionID", ErrorMessage = string.Format(Resources.ValidationMessages.Required, "TransferringSubsciptionID") } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            var selectedDomain = DomainsCache.DomainsCache.GetDomainByID(registrationInfo.DomainID.Value);
            if (selectedDomain == null)
            {
                // invalid domain
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "DomainID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidDomain } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            var selectedTariff = db.Services.FirstOrDefault(s => s.ID == registrationInfo.ServiceID && s.Domains.Select(d => d.ID).Contains(selectedDomain.ID));
            if (selectedTariff == null)
            {
                // invalid tariff
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "TariffID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTariff } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            if (selectedTariff.HasBilling && !selectedTariff.ServiceBillingPeriods.Any(sbp => sbp.DayOfMonth == registrationInfo.BillingPeriod))
            {
                // invalid billing period
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "BillingPeriod", ErrorMessage = Resources.RegistrationValidationMessages.InvalidBillingPeriod } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            if (selectedDomain.TelekomCredential == null && registrationInfo.TelekomDetailedInfo != null)
            {
                // telekom info not valid for this domain
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "TelekomDetailedInfo", ErrorMessage = Resources.RegistrationValidationMessages.TelekomInfoNotValidForDomain } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            // prevent wrong registration types for non-telekom domains
            if ((registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer || registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transition) && selectedDomain.TelekomCredential == null)
            {
                // invalid registration type for non-telekom domain
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "DomainID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidDomain } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            // referall discount ligics
            Subscription referrerSubscription = null;
            SpecialOffer specialOffer = null;
            if (registrationInfo.ReferralDiscount != null/*!string.IsNullOrEmpty(registrationInfo.ReferenceNo)*/)
            {
                var results = ValidateReferralDiscount(db, registrationInfo.ReferralDiscount, selectedTariff, out referrerSubscription, out specialOffer, "ReferralDiscount");
                if (results != null)
                    return new RegistrationResult()
                    {
                        ValidationMessages = results
                    };
            }
            // validate transfer
            if (registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer)
            {
                registrationInfo.TelekomDetailedInfo = null;
                var transferringSubscription = db.Subscriptions.Find(registrationInfo.TransferringSubsciptionID);
                if (transferringSubscription == null || !transferringSubscription.IsActive || transferringSubscription.SubscriptionTransferredFromHistories.Any(sth => !sth.Date.HasValue) || transferringSubscription.DomainID != registrationInfo.DomainID)
                {
                    // invalid transferring subscription
                    return new RegistrationResult()
                    {
                        ValidationMessages = new[] { new { Key = "TransferringSubsciptionID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTransferringSubscription } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                    };
                }
            }
            // validate partner info
            PartnerAvailableTariff selectedPartnerTariff = null;
            if (registrationInfo.RegisteringPartner != null)
            {
                var dbPartner = db.Partners.Find(registrationInfo.RegisteringPartner.PartnerID);
                if (dbPartner == null)
                    // invalid partner
                    return new RegistrationResult()
                    {
                        ValidationMessages = new[] { new { Key = "RegisteringPartner.PartnerID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidPartner } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                    };

                if (!dbPartner.PartnerPermissions.Any(p => p.Permission == (short)Enums.PartnerPermissions.Sale))
                {
                    // invalid partner permission
                    return new RegistrationResult()
                    {
                        ValidationMessages = new[] { new { Key = "RegisteringPartner.PartnerID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidPartner } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                    };
                }
                selectedPartnerTariff = dbPartner.PartnerGroup.PartnerAvailableTariffs.FirstOrDefault(pat => pat.TariffID == registrationInfo.ServiceID && pat.DomainID == selectedDomain.ID);
                if (selectedPartnerTariff == null)
                {
                    // invalid tariff for partner
                    return new RegistrationResult()
                    {
                        ValidationMessages = new[] { new { Key = "ServiceID", ErrorMessage = Resources.RegistrationValidationMessages.InvalidTariffForPartner } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                    };
                }
            }
            var dbGroups = new List<Group>();
            if (registrationInfo.GroupIds != null && registrationInfo.GroupIds.Any())
            {
                dbGroups = db.Groups.Where(g => g.IsActive && registrationInfo.GroupIds.Contains(g.ID)).ToList();
                if (dbGroups.Count() != registrationInfo.GroupIds.Count())
                {
                    // invalid groups
                    return new RegistrationResult()
                    {
                        ValidationMessages = new[] { new { Key = "GroupIDs", ErrorMessage = Resources.RegistrationValidationMessages.InvalidGroups } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                    };
                }
            }
            // create database object
            var dbFees = db.FeeTypeCosts.Include(f => f.FeeTypeVariants).ToArray();
            var addedFees = registrationInfo.AddedFeesInfo != null ? registrationInfo.AddedFeesInfo.SelectMany(af => af.GetDBObjects(dbFees)).ToList() : new List<Fee>();
            // invalid fees
            if (addedFees.Any(af => af == null))
            {
                // invalid added fees
                return new RegistrationResult()
                {
                    ValidationMessages = new[] { new { Key = "AddedFeesInfo", ErrorMessage = Resources.RegistrationValidationMessages.InvalidAddedFees } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                };
            }
            var dbSubscription = new Subscription()
            {
                RegistrationType = (short)registrationInfo.RegistrationType.Value,
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
                    Allowance = selectedPartnerTariff.Allowance,
                    PartnerID = registrationInfo.RegisteringPartner.PartnerID.Value,
                    TariffID = registrationInfo.ServiceID.Value,
                    AllowanceState = (short)Enums.PartnerAllowanceState.OnHold
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
                : null,
                SubscriptionTransferredToHistories = registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer ? new List<SubscriptionTransferHistory>()
                {
                    new SubscriptionTransferHistory()
                    {
                        Date = null,
                        From = registrationInfo.TransferringSubsciptionID.Value
                    }
                } : null
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
                            return new RegistrationResult()
                            {
                                ValidationMessages = new[] { new { Key = "TelekomDetailedInfo.SubscriberNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.TelekomSyncError, response.InternalException.GetShortMessage()) } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                            };
                        }
                        else
                        {
                            var selectedTelekomTariff = TelekomTariffsCache.GetSpecificTariff(selectedDomain, response.Data.PacketCode.Value, response.Data.TariffCode.Value);
                            if (selectedTelekomTariff == null)
                            {
                                // invalid telekom tariff
                                return new RegistrationResult()
                                {
                                    ValidationMessages = new[] { new { Key = "TelekomDetailedInfo.SubscriberNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.InvalidTelekomTariff, response.Data.PacketCode, response.Data.TariffCode) } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                };
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
                                return new RegistrationResult()
                                {
                                    ValidationMessages = new[] { new { Key = "TelekomDetailedInfo.TelekomTariffInfo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.InvalidTelekomTariff, registrationInfo.TelekomDetailedInfo.TelekomTariffInfo.PacketCode, registrationInfo.TelekomDetailedInfo.TelekomTariffInfo.TariffCode) } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                };
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
                    // for all registration that should pass telekom check
                    dbSubscription.State = (short)Enums.CustomerState.PreRegisterd;

                    // transfer & transition registration types and no telekom info
                    if (registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer || registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transition)
                    {
                        // transfer
                        if (registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transfer)
                        {
                            // empty telekom info
                            dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo()
                            {
                                SubscriptionNo = " ",
                                TTCustomerCode = !string.IsNullOrWhiteSpace(registrationInfo.TelekomDetailedInfo?.CustomerCode) ? long.Parse(registrationInfo.TelekomDetailedInfo.CustomerCode) : selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt
                            };
                        }
                        // transition
                        else if (registrationInfo.RegistrationType == Enums.SubscriptionRegistrationType.Transition)
                        {
                            // check with telekom if transition is valid
                            {
                                var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TTChurnApplicationClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword);
                                var response = serviceClient.ChurnAvailability(registrationInfo.TransitionXDSLNo);
                                if (response.InternalException != null)
                                {
                                    // error checking churn availability
                                    return new RegistrationResult()
                                    {
                                        ValidationMessages = new[] { new { Key = "TransitionXDSLNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.InvalidTransition, response.InternalException.GetShortMessage()) } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                    };
                                }
                                if (!response.Data.IsValid)
                                {
                                    // error checking churn availability
                                    return new RegistrationResult()
                                    {
                                        ValidationMessages = new[] { new { Key = "TransitionXDSLNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.InvalidTransition, "No Error Returned.") } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                    };
                                }
                            }
                            // transition telekom info
                            dbSubscription.SubscriptionTelekomInfo = new SubscriptionTelekomInfo()
                            {
                                SubscriptionNo = registrationInfo.TransitionXDSLNo ?? " ",
                                PSTN = registrationInfo.TransitionPSTN,
                                TTCustomerCode = selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt
                            };
                            // get transition operator if possible
                            if (!string.IsNullOrWhiteSpace(registrationInfo.TransitionXDSLNo))
                            {
                                var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TTChurnApplicationClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword);
                                var response = serviceClient.GetOperatorByXDSLNo(registrationInfo.TransitionXDSLNo);
                                if (response.InternalException != null)
                                {
                                    // could not get operator from service
                                    return new RegistrationResult()
                                    {
                                        ValidationMessages = new[] { new { Key = "TransitionXDSLNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.TelekomSyncError, response.InternalException.GetShortMessage()) } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                    };
                                }
                                var cachedOperator = TransitionOperatorsCache.GetAllOperators().FirstOrDefault(to => to.Username == response.Data);
                                if (cachedOperator == null)
                                {
                                    // the found operator is not registered
                                    return new RegistrationResult()
                                    {
                                        ValidationMessages = new[] { new { Key = "TransitionXDSLNo", ErrorMessage = string.Format(Resources.RegistrationValidationMessages.OperatorIsNotRegistered, response.Data) } }.ToLookup(item => item.Key, item => item.ErrorMessage)
                                    };
                                }
                                // set operator
                                dbSubscription.SubscriptionTelekomInfo.OperatorID = cachedOperator.ID;
                            }
                        }
                    }
                }
            }
            // add to customer
            referenceCustomer.Subscriptions.Add(dbSubscription);
            // return no error           
            return new RegistrationResult()
            {
                ValidationMessages = null
            };
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
