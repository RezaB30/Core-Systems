using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RadiusR_Manager.Models.ViewModels;
using RadiusR.DB.DomainsCache;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.API.TCKValidation;
using RadiusR.DB.Enums;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using RezaB.TurkTelekom.WebServices.InfrastructureInfo;
using RadiusR.DB;
using RezaB.TurkTelekom.WebServices.Exceptions;
using RadiusR.SystemLogs;
using RezaB.TurkTelekom.WebServices.TTApplication;
using RadiusR.DB.TelekomOperations;
using RezaB.TurkTelekom.WebServices;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.Authentication;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        private void UpdateTelekomPacketFromCache(TelekomTariffHelperViewModel telekomTariff, CachedDomain domain)
        {
            if (telekomTariff != null && domain != null && telekomTariff.TariffCode.HasValue && telekomTariff.PacketCode.HasValue)
            {
                var cachedTariff = TelekomTariffsCache.GetSpecificTariff(domain, telekomTariff.PacketCode.Value, telekomTariff.TariffCode.Value);
                if (cachedTariff != null)
                {
                    telekomTariff.SpeedName = cachedTariff.SpeedName;
                    telekomTariff.MonthlyStaticFee = cachedTariff.MonthlyStaticFee;
                    telekomTariff.SpeedCode = cachedTariff.SpeedCode;
                    telekomTariff.SpeedDetails = cachedTariff.SpeedDetails;
                    telekomTariff.TariffName = cachedTariff.TariffName;
                    telekomTariff.XDSLType = (short)cachedTariff.XDSLType;
                }
            }
        }

        private class TCKValidationResult
        {
            public enum TCKValidationResultType
            {
                Success = 1,
                Step1Failure = 2,
                Step2Failure = 3,
                NoValidationMethod = 4
            }

            public TCKValidationResultType ResultType { get; set; }

            public string ResultContent { get; set; }
        }


        private TCKValidationResult ValidateTCK(IDCardViewModel IDCard)
        {
            var resultsContent = string.Empty;

            var client = new TCKValidationClient();
            bool checkResult = false;
            switch ((IDCardTypes)IDCard.CardType)
            {
                case IDCardTypes.TCIDCardWithChip:
                    if (string.IsNullOrWhiteSpace(IDCard.SerialNo))
                        return new TCKValidationResult()
                        {
                            ResultType = TCKValidationResult.TCKValidationResultType.Step1Failure,
                            ResultContent = "<div class='text-danger'>" + RadiusR.Localization.Validation.Common.TCKValidationStep1 + "</div>"
                        };
                    else
                        checkResult = client.ValidateNewTCK(IDCard.TCKNo, IDCard.FirstName.ToUpper(), IDCard.LastName.ToUpper(), IDCard.BirthDate.Value, IDCard.SerialNo);
                    break;
                case IDCardTypes.TCBirthCertificate:
                    if (string.IsNullOrWhiteSpace(IDCard.SerialNo) || IDCard.SerialNo.Length < 9)
                        return new TCKValidationResult()
                        {
                            ResultType = TCKValidationResult.TCKValidationResultType.Step1Failure,
                            ResultContent = "<div class='text-danger'>" + RadiusR.Localization.Validation.Common.TCKValidationStep1 + "</div>"
                        };
                    else
                        checkResult = client.ValidateOldTCK(IDCard.TCKNo, IDCard.FirstName.ToUpper(), IDCard.LastName.ToUpper(), IDCard.BirthDate.Value, IDCard.SerialNo.Substring(3), IDCard.SerialNo.Substring(0, 3));
                    break;
                case IDCardTypes.TCDrivingLisence:
                case IDCardTypes.TCForeignerIDCard:
                    checkResult = client.ValidateOthers(IDCard.TCKNo, IDCard.FirstName.ToUpper(), IDCard.LastName.ToUpper(), IDCard.BirthDate.Value);
                    break;
                case IDCardTypes.TCPassportWithChip:
                case IDCardTypes.OldTCPassportDarkBlue:
                case IDCardTypes.OldTCPassportGreen:
                case IDCardTypes.OldTCPassportGrey:
                case IDCardTypes.OldTCPassportRed:
                case IDCardTypes.TCPassportTemporary:
                case IDCardTypes.ForeignerPassport:
                case IDCardTypes.FlightCrewCertificate:
                case IDCardTypes.ShipmansCertificate:
                case IDCardTypes.NATOOrderDocument:
                case IDCardTypes.TravelDocument:
                case IDCardTypes.BorderCrossingDocument:
                case IDCardTypes.ShipCommanderApprovedPersonnelList:
                case IDCardTypes.TCProsecutorJudgeIDCard:
                case IDCardTypes.TCLawyerIDCard:
                case IDCardTypes.TCTemporaryIDCard:
                case IDCardTypes.TCBlueCard:
                case IDCardTypes.TCInternationalFamilyCertificate:
                default:
                    return new TCKValidationResult()
                    {
                        ResultType = TCKValidationResult.TCKValidationResultType.NoValidationMethod,
                        ResultContent = "<div class='text-danger'>" + RadiusR.Localization.Pages.Common.NoTCKValidationMethod + "<div>"
                    };
            }
            if (checkResult)
            {
                return new TCKValidationResult()
                {
                    ResultType = TCKValidationResult.TCKValidationResultType.Success,
                    ResultContent = "<div style='color: green;'>" + RadiusR.Localization.Pages.Common.TCKValidationSuccess + "</div>"
                };
            }
            else
            {
                return new TCKValidationResult()
                {
                    ResultType = TCKValidationResult.TCKValidationResultType.Step2Failure,
                    ResultContent = "<div class='text-danger'>" + RadiusR.Localization.Validation.Common.TCKValidationStep2 + "</div>"
                };
            }
        }

        private string GetSynchronizationErrorMessage(RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization.TelekomSynchronizationResultCodes errorCode)
        {
            switch (errorCode)
            {
                case RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization.TelekomSynchronizationResultCodes.InvalidDomain:
                    return RadiusR.Localization.Validation.ModelSpecific.InvalidDomain;
                case RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization.TelekomSynchronizationResultCodes.SubscriptionHasNoTelekomInfo:
                    return RadiusR.Localization.Validation.ModelSpecific.SubscriptionHasNoTelekomInfo;
                case RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization.TelekomSynchronizationResultCodes.SynchronizedUsernameExists:
                    return RadiusR.Localization.Validation.Common.UsernameExists;
                case RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization.TelekomSynchronizationResultCodes.InvalidOptions:
                    return RadiusR.Localization.Validation.ModelSpecific.InvalidSynchronizationOptions;
                default:
                    return null;
            }
        }

        #region Filler Methods

        public CustomerRegistrationInfo CreateCustomerInfoForRegistration(CustomerRegistrationViewModel customerModel)
        {
            var results = new CustomerRegistrationInfo()
            {
                IDCard = new CustomerRegistrationInfo.IDCardInfo()
                {
                    BirthDate = customerModel.IDCard.BirthDate,
                    CardType = (IDCardTypes?)customerModel.IDCard.CardType,
                    DateOfIssue = customerModel.IDCard.DateOfIssue,
                    District = customerModel.IDCard.District,
                    FirstName = customerModel.IDCard.FirstName,
                    LastName = customerModel.IDCard.LastName,
                    Neighbourhood = customerModel.IDCard.Neighbourhood,
                    PageNo = customerModel.IDCard.PageNo,
                    PassportNo = customerModel.IDCard.PassportNo,
                    PlaceOfIssue = customerModel.IDCard.PlaceOfIssue,
                    Province = customerModel.IDCard.Province,
                    RowNo = customerModel.IDCard.RowNo,
                    SerialNo = customerModel.IDCard.SerialNo,
                    TCKNo = customerModel.IDCard.TCKNo,
                    VolumeNo = customerModel.IDCard.VolumeNo
                },
                GeneralInfo = new CustomerRegistrationInfo.CustomerGeneralInfo()
                {
                    BillingAddress = customerModel.GeneralInfo.BillingSameAsSetupAddress == true ? CreateAddressForRegistration(customerModel.SubscriptionInfo.SetupAddress) : CreateAddressForRegistration(customerModel.GeneralInfo.BillingAddress),
                    ContactPhoneNo = customerModel.GeneralInfo.ContactPhoneNo,
                    Culture = customerModel.GeneralInfo.Culture,
                    CustomerType = (CustomerType?)customerModel.GeneralInfo.CustomerType,
                    Email = customerModel.GeneralInfo.Email,
                    OtherPhoneNos = customerModel.GeneralInfo.OtherPhoneNos != null ? customerModel.GeneralInfo.OtherPhoneNos.Select(pn => new CustomerRegistrationInfo.PhoneNoListItem() { Number = pn.Number }) : null
                }
            };
            if (customerModel.GeneralInfo.CustomerType == (short)CustomerType.Individual)
            {
                results.IndividualInfo = new CustomerRegistrationInfo.IndividualCustomerInfo()
                {
                    BirthPlace = customerModel.IndividualInfo.BirthPlace,
                    FathersName = customerModel.IndividualInfo.FathersName,
                    MothersMaidenName = customerModel.IndividualInfo.MothersMaidenName,
                    MothersName = customerModel.IndividualInfo.MothersName,
                    Nationality = (CountryCodes?)customerModel.IndividualInfo.Nationality,
                    Profession = (Profession?)customerModel.IndividualInfo.Profession,
                    Sex = (Sexes?)customerModel.IndividualInfo.Sex,
                    ResidencyAddress = customerModel.IndividualInfo.ResidencySameAsSetupAddress == true ? CreateAddressForRegistration(customerModel.SubscriptionInfo.SetupAddress) : CreateAddressForRegistration(customerModel.IndividualInfo.ResidencyAddress)
                };
            }
            else if (customerModel.GeneralInfo.CustomerType.HasValue)
            {
                results.CorporateInfo = new CustomerRegistrationInfo.CorporateCustomerInfo()
                {
                    CentralSystemNo = customerModel.CorporateInfo.CentralSystemNo,
                    CompanyAddress = customerModel.CorporateInfo.CompanySameAsSetupAddress == true ? CreateAddressForRegistration(customerModel.SubscriptionInfo.SetupAddress) : CreateAddressForRegistration(customerModel.CorporateInfo.CompanyAddress),
                    ExecutiveBirthPlace = customerModel.CorporateInfo.ExecutiveBirthPlace,
                    ExecutiveFathersName = customerModel.CorporateInfo.ExecutiveFathersName,
                    ExecutiveMothersMaidenName = customerModel.CorporateInfo.ExecutiveMothersMaidenName,
                    ExecutiveMothersName = customerModel.CorporateInfo.ExecutiveMothersName,
                    ExecutiveNationality = (CountryCodes?)customerModel.CorporateInfo.ExecutiveNationality,
                    ExecutiveProfession = (Profession?)customerModel.CorporateInfo.ExecutiveProfession,
                    ExecutiveResidencyAddress = customerModel.CorporateInfo.ExecutiveResidencySameAsSetupAddress == true ? CreateAddressForRegistration(customerModel.SubscriptionInfo.SetupAddress) : CreateAddressForRegistration(customerModel.CorporateInfo.ExecutiveResidencyAddress),
                    ExecutiveSex = (Sexes?)customerModel.CorporateInfo.ExecutiveSex,
                    TaxNo = customerModel.CorporateInfo.TaxNo,
                    TaxOffice = customerModel.CorporateInfo.TaxOffice,
                    Title = customerModel.CorporateInfo.Title,
                    TradeRegistrationNo = customerModel.CorporateInfo.TradeRegistrationNo
                };
            }

            if (customerModel.SubscriptionInfo != null)
                results.SubscriptionInfo = CreateSubscriptionInfoForRegistration(customerModel.SubscriptionInfo);

            return results;
        }

        public CustomerRegistrationInfo.SubscriptionRegistrationInfo CreateSubscriptionInfoForRegistration(CustomerSubscriptionViewModel subscriptionModel)
        {
            var results = new CustomerRegistrationInfo.SubscriptionRegistrationInfo()
            {
                AddedFeesInfo = subscriptionModel.AddedFeesInfo != null ? subscriptionModel.AddedFeesInfo.Select(af => new CustomerRegistrationInfo.SubscriptionAddedFeeItem()
                {
                    FeeType = (FeeType?)af.FeeTypeID,
                    InstallmentCount = af.InstallmentCount,
                    VariantType = af.SelectedVariantID,
                    CustomFeesInfo = af.CustomFees != null ? af.CustomFees.Select(cf => new CustomerRegistrationInfo.SubscriptionCustomAddedFeeItem()
                    {
                        InstallmentCount = cf.Installment,
                        Price = cf._price,
                        Title = cf.Title
                    }) : null
                }) : null,
                BillingPeriod = subscriptionModel.BillingPeriod,
                CommitmentInfo = subscriptionModel.CommitmentInfo != null && (subscriptionModel.CommitmentInfo.CommitmentLength.HasValue || subscriptionModel.CommitmentInfo.CommitmentExpirationDate.HasValue) ? new CustomerRegistrationInfo.CustomerCommitmentInfo()
                {
                    CommitmentExpirationDate = subscriptionModel.CommitmentInfo.CommitmentExpirationDate,
                    CommitmentLength = (CommitmentLength?)subscriptionModel.CommitmentInfo.CommitmentLength
                } : null,
                DomainID = subscriptionModel.DomainID,
                GroupIds = subscriptionModel.GroupIds,
                SetupAddress = CreateAddressForRegistration(subscriptionModel.SetupAddress),
                StaticIP = subscriptionModel.StaticIP,
                ServiceID = subscriptionModel.ServiceID,
                Username = subscriptionModel.Username
            };

            if (subscriptionModel.TelekomDetailedInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(subscriptionModel.TelekomDetailedInfo.CustomerCode) || !string.IsNullOrWhiteSpace(subscriptionModel.TelekomDetailedInfo.PSTN) || !string.IsNullOrWhiteSpace(subscriptionModel.TelekomDetailedInfo.SubscriberNo))
                {
                    results.TelekomDetailedInfo = new CustomerRegistrationInfo.SubscriptionTelekomInfoDetails()
                    {
                        CustomerCode = subscriptionModel.TelekomDetailedInfo.CustomerCode,
                        PSTN = subscriptionModel.TelekomDetailedInfo.PSTN,
                        SubscriberNo = subscriptionModel.TelekomDetailedInfo.SubscriberNo
                    };
                }

                if (subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo != null && (subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.PacketCode.HasValue || subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.TariffCode.HasValue || subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.XDSLType.HasValue))
                {
                    results.TelekomDetailedInfo = results.TelekomDetailedInfo ?? new CustomerRegistrationInfo.SubscriptionTelekomInfoDetails();
                    results.TelekomDetailedInfo.TelekomTariffInfo = new CustomerRegistrationInfo.SubscriptionTelekomTariffInfo()
                    {
                        IsPaperworkNeeded = subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.IsPaperworkNeeded,
                        PacketCode = subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.PacketCode,
                        TariffCode = subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.TariffCode,
                        XDSLType = (XDSLType?)subscriptionModel.TelekomDetailedInfo.TelekomTariffInfo.XDSLType
                    };
                }

            }
            if (subscriptionModel.ReferralDiscount != null && !string.IsNullOrEmpty(subscriptionModel.ReferralDiscount.ReferenceNo)/*!string.IsNullOrEmpty(subscriptionModel.ReferenceNo)*/)
            {
                results.ReferralDiscount = new CustomerRegistrationInfo.ReferralDiscountInfo()
                {
                    ReferenceNo = subscriptionModel.ReferralDiscount.ReferenceNo,
                    SpecialOfferID = subscriptionModel.ReferralDiscount.SpecialOfferID
                };
            }
            return results;
        }

        public CustomerRegistrationInfo.AddressInfo CreateAddressForRegistration(AddressViewModel addressModel)
        {
            return new CustomerRegistrationInfo.AddressInfo()
            {
                AddressNo = addressModel.AddressNo,
                AddressText = addressModel.AddressText,
                ApartmentID = addressModel.ApartmentID,
                ApartmentNo = addressModel.ApartmentNo,
                DistrictID = addressModel.DistrictID,
                DistrictName = addressModel.DistrictName,
                DoorID = addressModel.DoorID,
                DoorNo = addressModel.DoorNo,
                Floor = addressModel.Floor,
                NeighbourhoodID = addressModel.NeighbourhoodID,
                NeighbourhoodName = addressModel.NeighborhoodName,
                PostalCode = addressModel.PostalCode,
                ProvinceID = addressModel.ProvinceID,
                ProvinceName = addressModel.ProvinceName,
                RuralCode = addressModel.RuralCode,
                StreetID = addressModel.StreetID,
                StreetName = addressModel.StreetName
            };
        }

        #endregion
    }
}