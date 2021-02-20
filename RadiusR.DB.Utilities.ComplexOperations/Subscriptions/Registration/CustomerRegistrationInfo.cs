using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Data.Validation;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration
{
    public class CustomerRegistrationInfo : ValidatableBase
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
        public IDCardInfo IDCard { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
        public CustomerGeneralInfo GeneralInfo { get; set; }

        public IndividualCustomerInfo IndividualInfo { get; set; }

        public CorporateCustomerInfo CorporateInfo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
        public SubscriptionRegistrationInfo SubscriptionInfo { get; set; }

        public class IDCardInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.IDCardTypes? CardType { get; set; }

            [RegularExpression(@"^[A-Z|0-9]{7,10}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string PassportNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string FirstName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string LastName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [RegularExpression(@"^[0-9]{11}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string TCKNo { get; set; }

            [MaxLength(10, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string VolumeNo { get; set; }

            [MaxLength(10, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string RowNo { get; set; }

            [MaxLength(10, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string PageNo { get; set; }

            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string Province { get; set; }

            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string District { get; set; }

            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string Neighbourhood { get; set; }

            [MaxLength(12, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string SerialNo { get; set; }

            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string PlaceOfIssue { get; set; }

            public DateTime? DateOfIssue { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public DateTime? BirthDate { get; set; }
        }

        public class CustomerGeneralInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.CustomerType? CustomerType { get; set; }

            [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            [MaxLength(250, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string Email { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [RegularExpression(@"^[a-zA-Z]{2}(-[a-zA-Z]{2}){0,1}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string Culture { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [RegularExpression(@"^[1-9]{1}[0-9]{9}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string ContactPhoneNo { get; set; }

            public IEnumerable<PhoneNoListItem> OtherPhoneNos { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public AddressInfo BillingAddress { get; set; }
        }

        public class IndividualCustomerInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.Sexes? Sex { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.CountryCodes? Nationality { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string FathersName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string MothersName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string MothersMaidenName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string BirthPlace { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.Profession? Profession { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public AddressInfo ResidencyAddress { get; set; }
        }

        public class CorporateCustomerInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(250, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string Title { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [RegularExpression(@"^[0-9]{10,11}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string TaxNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string TaxOffice { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(16, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string CentralSystemNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(6, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string TradeRegistrationNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string ExecutiveFathersName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string ExecutiveMothersName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string ExecutiveMothersMaidenName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.Sexes? ExecutiveSex { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.CountryCodes? ExecutiveNationality { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(150, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string ExecutiveBirthPlace { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.Profession? ExecutiveProfession { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public AddressInfo ExecutiveResidencyAddress { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public AddressInfo CompanyAddress { get; set; }
        }

        public class SubscriptionRegistrationInfo : ValidatableBase
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.SubscriptionRegistrationType? RegistrationType { get; set; }

            public long? TransferringSubsciptionID { get; set; }

            [RegularExpression(@"^[0-9]{10}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string TransitionXDSLNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? DomainID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? ServiceID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public AddressInfo SetupAddress { get; set; }

            //[Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(250, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string Username { get; set; }

            [RegularExpression(@"^([0-1][0-9][0-9]|2[0-4][0-9]|25[0-5]|\d{1,2})\.([0-1][0-9][0-9]|2[0-4][0-9]|25[0-5]|\d{1,2})\.([0-1][0-9][0-9]|2[0-4][0-9]|25[0-5]|\d{1,2})\.([0-1][0-9][0-9]|2[0-4][0-9]|25[0-5]|\d{1,2})$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string StaticIP { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? BillingPeriod { get; set; }

            public IEnumerable<int> GroupIds { get; set; }

            //[Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public CustomerCommitmentInfo CommitmentInfo { get; set; }

            public IEnumerable<SubscriptionAddedFeeItem> AddedFeesInfo { get; set; }

            public SubscriptionTelekomInfoDetails TelekomDetailedInfo { get; set; }

            public RegisteringPartnerInfo RegisteringPartner { get; set; }

            public ReferralDiscountInfo ReferralDiscount { get; set; }

            //[RegularExpression(@"^[A-Z0-9]{6}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            //public string ReferenceNo { get; set; }

            //public int? SpecialOfferID { get; set; }
        }

        public class SubscriptionTelekomTariffInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public RezaB.TurkTelekom.WebServices.XDSLType? XDSLType { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? PacketCode { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? TariffCode { get; set; }

            public bool? IsPaperworkNeeded { get; set; }
        }

        public class SubscriptionTelekomInfoDetails
        {
            [RegularExpression(@"^[0-9]{10}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string SubscriberNo { get; set; }

            [RegularExpression(@"^[0-9]{10}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string CustomerCode { get; set; }

            [RegularExpression(@"^[1-9]{1}[0-9]{9}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string PSTN { get; set; }

            public SubscriptionTelekomTariffInfo TelekomTariffInfo { get; set; }
        }

        public class AddressInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? ProvinceID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? DistrictID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? RuralCode { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? NeighbourhoodID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? StreetID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? DoorID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? ApartmentID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string AddressText { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? PostalCode { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string Floor { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public long? AddressNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string ProvinceName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string DistrictName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string NeighbourhoodName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string StreetName { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string DoorNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string ApartmentNo { get; set; }

            public Address GetDbObject()
            {
                return new Address()
                {
                    AddressNo = AddressNo.Value,
                    AddressText = AddressText,
                    ApartmentID = ApartmentID.Value,
                    ApartmentNo = ApartmentNo,
                    DistrictID = DistrictID.Value,
                    DistrictName = DistrictName,
                    DoorID = DoorID.Value,
                    DoorNo = DoorNo,
                    Floor = Floor,
                    NeighborhoodID = NeighbourhoodID.Value,
                    NeighborhoodName = NeighbourhoodName,
                    PostalCode = PostalCode.Value,
                    ProvinceID = ProvinceID.Value,
                    ProvinceName = ProvinceName,
                    RuralCode = RuralCode.Value,
                    StreetID = StreetID.Value,
                    StreetName = StreetName
                };
            }
        }

        public class PhoneNoListItem
        {
            [RegularExpression(@"^[1-9]{1}[0-9]{9}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public string Number { get; set; }
        }

        public class CustomerCommitmentInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.CommitmentLength? CommitmentLength { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public DateTime? CommitmentExpirationDate { get; set; }
        }

        public class SubscriptionAddedFeeItem
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public Enums.FeeType? FeeType { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? InstallmentCount { get; set; }

            public int? VariantType { get; set; }

            public IEnumerable<SubscriptionCustomAddedFeeItem> CustomFeesInfo { get; set; }

            public List<Fee> GetDBObjects(IEnumerable<FeeTypeCost> allDBFees)
            {
                if (!FeeType.HasValue || allDBFees.FirstOrDefault(f => f.FeeTypeID == (short)FeeType) == null)
                    return null;
                var currentFee = allDBFees.FirstOrDefault(f => f.FeeTypeID == (short)FeeType);
                var currentVariant = currentFee.FeeTypeVariants.Any() ? currentFee.FeeTypeVariants.FirstOrDefault(fv => fv.ID == VariantType) : null;
                if (currentFee.FeeTypeVariants.Any() && currentVariant == null)
                    return null;
                if (!currentFee.Cost.HasValue && currentVariant == null && (CustomFeesInfo == null || !CustomFeesInfo.Any()))
                    return null;
                // for custom fee
                if (CustomFeesInfo != null && CustomFeesInfo.Any())
                {
                    return CustomFeesInfo.Select(cf => new Fee()
                    {
                        Cost = cf.Price.Value,
                        Date = DateTime.Today,
                        Description = cf.Title,
                        //FeeDescription = new FeeDescription() { Description = cf.Title},
                        FeeTypeID = currentFee.FeeTypeID,
                        InstallmentBillCount = currentFee.IsAllTime ? (short)1 : (short)cf.InstallmentCount,
                    }).ToList();
                }
                // for variant fee
                if (currentVariant != null)
                {
                    return new List<Fee>()
                    {
                        new Fee()
                        {
                            Cost = currentVariant.Price,
                            Date = DateTime.Today,
                            FeeTypeID = currentFee.FeeTypeID,
                            FeeTypeVariantID = currentVariant.ID,
                            InstallmentBillCount = currentFee.IsAllTime ? (short)1 : (short)InstallmentCount
                        }
                    };
                }
                // no variant fee
                return new List<Fee>()
                {
                    new Fee()
                    {
                        Cost = currentFee.Cost,
                        InstallmentBillCount = currentFee.IsAllTime ? (short)1 : (short)InstallmentCount,
                        Date = DateTime.Today,
                        FeeTypeID = currentFee.FeeTypeID
                    }
                };
            }
        }

        public class SubscriptionCustomAddedFeeItem
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [MaxLength(50, ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "MaxLength")]
            public string Title { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [PositiveDecimal(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "PositiveNo")]
            public decimal? Price { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? InstallmentCount { get; set; }
        }

        public class RegisteringPartnerInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? PartnerID { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public decimal? Allowance { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public decimal? AllowanceThreshold { get; set; }
        }

        public class ReferralDiscountInfo
        {
            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            [RegularExpression(@"^[A-Z0-9]{6}$", ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "RegexValidation")]
            public string ReferenceNo { get; set; }

            [Required(ErrorMessageResourceType = typeof(Resources.ValidationMessages), ErrorMessageResourceName = "Required")]
            public int? SpecialOfferID { get; set; }
        }
    }
}
