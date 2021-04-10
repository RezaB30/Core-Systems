using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.ModelExtentions;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class CustomerDetailsViewModel
    {
        public long ID { get; set; }

        public string DisplayName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerIdentity")]
        public IDCardViewModel IDCard { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerGeneralInfo")]
        public CustomerGeneralInfoViewModel GeneralInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IndividualCustomerInfo")]
        public IndividualCustomerInfoViewModel IndividualInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CorporateCustomerInfo")]
        public CorporateCustomerInfoViewModel CorporateInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriptionInfo")]
        public SubscriptionDetailsViewModel SubscriptionInfo { get; set; }

        public IEnumerable<RelatedSubscriptionsViewModel> RelatedSubscriptions { get; set; }

        public CustomerDetailsViewModel(Subscription dbSubscription, RadiusREntities db)
        {
            var domain = RadiusR.DB.DomainsCache.DomainsCache.GetDomainByID(dbSubscription.DomainID);
            var telekomTariff = dbSubscription.SubscriptionTelekomInfo?.XDSLType != null ? RadiusR.DB.DomainsCache.TelekomTariffsCache.GetSpecificTariff(domain, dbSubscription.SubscriptionTelekomInfo.PacketCode.Value, dbSubscription.SubscriptionTelekomInfo.TariffCode.Value) : null;
            var currentQoutaAndUsage = dbSubscription.GetQuotaAndUsageInfo();
            //var currentBillingPeriod = dbSubscription.GetCurrentBillingPeriod();

            ID = dbSubscription.CustomerID;
            DisplayName = dbSubscription.ValidDisplayName;
            IDCard = new IDCardViewModel()
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
            GeneralInfo = new CustomerGeneralInfoViewModel()
            {
                BillingAddress = new AddressViewModel(dbSubscription.Customer.BillingAddress),
                ContactPhoneNo = dbSubscription.Customer.ContactPhoneNo,
                Culture = dbSubscription.Customer.Culture,
                CustomerType = dbSubscription.Customer.CustomerType,
                Email = dbSubscription.Customer.Email,
                OtherPhoneNos = dbSubscription.Customer.CustomerAdditionalPhoneNoes.Select(phone => new CustomerGeneralInfoViewModel.PhoneNo() { Number = phone.PhoneNo }).ToList()
            };
            if (dbSubscription.Customer.CustomerType == (short)CustomerType.Individual)
            {
                IndividualInfo = new IndividualCustomerInfoViewModel()
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
            }
            else
            {
                CorporateInfo = new CorporateCustomerInfoViewModel()
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
            }
            SubscriptionInfo = new SubscriptionDetailsViewModel()
            {
                RegistrationInfo = new SubscriptionRegistrationInfoViewModel()
                {
                    RegistrationType = dbSubscription.RegistrationType,
                    TransferHistory = dbSubscription.SubscriptionTransferredFromHistories.Select(sth => new TransferHistoryViewModel()
                    {
                        TransferredToSubscriberNo = sth.TransferredSubscription.SubscriberNo,
                        Date = sth.Date
                    }).Concat(dbSubscription.SubscriptionTransferredToHistories.Select(sth => new TransferHistoryViewModel()
                    {
                        TransferredFromSubscriberNo = sth.TransferrerSubscription.SubscriberNo,
                        Date = sth.Date
                    })).OrderBy(th => th.Date).ToArray()
                },
                ArchiveNo = dbSubscription.ID,
                Commitment = dbSubscription.SubscriptionCommitment != null ? new CommitmentViewModel()
                {
                    CommitmentLength = dbSubscription.SubscriptionCommitment.CommitmentLength,
                    CommitmentExpirationDate = dbSubscription.SubscriptionCommitment.CommitmentExpirationDate
                } : null,
                Groups = string.Join(",", dbSubscription.Groups.Select(g => g.Name)),
                InstallationAddress = new AddressViewModel(dbSubscription.Address),
                IsScanned = dbSubscription.ArchiveScanned,
                OnlinePassword = dbSubscription.OnlinePassword,
                Password = dbSubscription.RadiusAuthorization.Password,
                Username = dbSubscription.RadiusAuthorization.Username,
                State = dbSubscription.State,
                SubscriberNo = dbSubscription.SubscriberNo,
                ReferenceNo = dbSubscription.ReferenceNo,
                TariffInfo = new SubscriptionTariffInfoViewModel()
                {
                    ActivationDate = dbSubscription.ActivationDate,
                    BillingPeriod = dbSubscription.PaymentDay,
                    CancellationDate = dbSubscription.EndDate,
                    DaysRemaining = dbSubscription.DaysRemaining,
                    DomainName = dbSubscription.Domain.Name,
                    ExpirationDate = dbSubscription.RadiusAuthorization.ExpirationDate,
                    RegistrationDate = dbSubscription.MembershipDate,
                    StaticIP = dbSubscription.RadiusAuthorization.StaticIP,
                    TariffID = dbSubscription.ServiceID,
                    TariffName = dbSubscription.Service.Name,
                    RemainingQuota = currentQoutaAndUsage != null ? currentQoutaAndUsage.RemainingQuota : (long?)null,
                    ReactivationDate = (dbSubscription.State == (short)CustomerState.Disabled && dbSubscription.ChangeStateTasks.FirstOrDefault() != null) ? dbSubscription.ChangeStateTasks.FirstOrDefault().SchedulerTask.ExecuteDate : (DateTime?)null,
                    RecurringPaymentType = dbSubscription.RecurringPaymentSubscription != null ? dbSubscription.RecurringPaymentSubscription.RadiusRBillingService.Name : dbSubscription.MobilExpressAutoPayment != null ? "MobilExpress" : null,
                    CanHaveQuotaSale = dbSubscription.Service.CanHaveQuotaSale,
                    InQueueTariffID = dbSubscription.ChangeServiceTypeTasks.Any() ? dbSubscription.ChangeServiceTypeTasks.FirstOrDefault().NewServiceID : (int?)null,
                    CurrentBillingPeriodStartDate = currentQoutaAndUsage != null ? currentQoutaAndUsage.PeriodStart : (DateTime?)null,
                    CurrentBillingPeriodEndDate = currentQoutaAndUsage != null ? currentQoutaAndUsage.PeriodEnd : (DateTime?)null,
                    HasBilling = dbSubscription.HasBilling,
                    LastTariffChangeDate = dbSubscription.LastTariffChangeDate,
                    TariffChange = (dbSubscription.ChangeServiceTypeTasks.Any()) ? new SubscriptionScheduledTariffChangeViewModel()
                    {
                        NewScheduledTariffName = dbSubscription.ChangeServiceTypeTasks.LastOrDefault().Service.Name,
                        NewScheduledTariffActivationDate = dbSubscription.ChangeServiceTypeTasks.LastOrDefault().SchedulerTask.ExecuteDate
                    } : null
                },
                TelekomInfo = dbSubscription.SubscriptionTelekomInfo != null ? new SubscriptionTelekomInfoViewModel()
                {
                    SubscriberNo = dbSubscription.SubscriptionTelekomInfo.SubscriptionNo,
                    CustomerCode = dbSubscription.SubscriptionTelekomInfo.TTCustomerCode.ToString(),
                    PSTN = dbSubscription.SubscriptionTelekomInfo.PSTN,
                    RedbackName = dbSubscription.SubscriptionTelekomInfo.RedbackName,
                    TariffName = telekomTariff != null ? telekomTariff.TariffName : null,
                    OperatorName = dbSubscription.SubscriptionTelekomInfo.TransitionOperator?.DisplayName
                } : null,
                CancellationInfo = dbSubscription.SubscriptionCancellation != null ? new ClientCancellationViewModel()
                {
                    ReasonID = dbSubscription.SubscriptionCancellation.ReasonID,
                    ReasonText = dbSubscription.SubscriptionCancellation.ReasonText
                } : null
            };
            RelatedSubscriptions = dbSubscription.Customer.Subscriptions.Where(sub => sub.ID != dbSubscription.ID).Select(sub => new RelatedSubscriptionsViewModel()
            {
                ID = sub.ID,
                State = sub.State,
                SubscriberNo = sub.SubscriberNo
            });
        }
    }
}
