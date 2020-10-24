using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using RadiusR_Manager.Models.ViewModels.Customer;
using RezaB.Web.CustomAttributes;
using RadiusR.DB.Localization.Bills;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class BillViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public DateTime IssueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DueDate")]
        public DateTime DueDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingPeriod")]
        public DateTime? PeriodStart { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingPeriod")]
        public DateTime? PeriodEnd { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PayDate")]
        [UIHint("ExactTime")]
        public DateTime? PayDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "State")]
        [EnumType(typeof(BillState), typeof(RadiusR.Localization.Lists.BillState))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short StateID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentType")]
        [EnumType(typeof(PaymentType), typeof(RadiusR.Localization.Lists.PaymentType))]
        [UIHint("LocalizedList")]
        public short PaymentTypeID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentGateway")]
        public string PaymentGateway { get; set; }

        public int? PaymentGatewayID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentType")]
        [UIHint("PaymentTypeDescription")]
        public object PaymentTypeDescription
        {
            get
            {
                return new BillPaymentTypeDescription
                {
                    PaymentTypeID = PaymentTypeID,
                    PaymentGateway = PaymentGateway
                };
            }
        }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? AccountantID { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Customer")]
        public SubscriptionListDisplayViewModel Subscription { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Accountant")]
        public AppUser Accountant { get; set; }

        public IEnumerable<BillFeeViewModel> BillFees { get; set; }

        public decimal _totalCost { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Total")]
        [UIHint("Currency")]
        public string TotalCost
        {
            get
            {
                return _totalCost.ToString("###,##0.00");
            }
        }

        public decimal _totalDiscount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TotalDiscount")]
        [UIHint("Currency")]
        public string TotalDiscount
        {
            get
            {
                return _totalDiscount.ToString("###,##0.00");
            }
        }
        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "PayingAmount")]
        [UIHint("Currency")]
        public string TotalPayableAmount
        {
            get
            {
                return _totalPayableAmount.ToString("###,##0.00");
            }
        }

        public decimal _totalPayableAmount
        {
            get
            {
                return Math.Max(_totalCost - _totalDiscount, 0m);
            }
        }

        //public decimal PartialPercentage { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Credit")]
        [UIHint("Currency")]
        public ClientCreditViewModel.CreditViewModel CreditPay { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillType")]
        [EnumType(typeof(BillSources), typeof(RadiusR.Localization.Lists.BillType))]
        [UIHint("LocalizedList")]
        public short Source { get; set; }

        internal short? ServicePeriodLength { get; set; }

        public class BillPaymentTypeDescription
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentType")]
            [EnumType(typeof(PaymentType), typeof(RadiusR.Localization.Lists.PaymentType))]
            [UIHint("LocalizedList")]
            public short PaymentTypeID { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PaymentGateway")]
            public string PaymentGateway { get; set; }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EBillIsSent")]
        public bool EBillIsSent { get; set; }

        public EBillViewModel EBill { get; set; }
    }

    public static class BillViewModelExtentions
    {
        public static IEnumerable<BillViewModel> GetViewModels(this IEnumerable<Bill> bills)
        {
            return bills.Select(bill => bill.GetViewModel());
        }

        public static IEnumerable<BillViewModel> GetViewModels(this IQueryable<Bill> bills)
        {
            return bills
                .Include(bill => bill.Subscription.Service).Include(bill => bill.Subscription.Customer.CorporateCustomerInfo)
                .Include(bill => bill.ExternalPayment.RadiusRBillingService).Include(bill => bill.ExternalPayment.OfflinePaymentGateway)
                .Include(bill => bill.PartnerBillPayment.Partner).Include(bill => bill.PartnerBillPayment.PartnerSubUser)
                .Include(bill => bill.BillFees.Select(bf => bf.Fee.FeeTypeVariant)).Include(bill => bill.BillFees.Select(bf => bf.Discount))
                .Include(bill => bill.EBill)
                .AsEnumerable()
                .Select(bill => new BillViewModel()
                {
                    Accountant = bill.AppUser,
                    AccountantID = bill.AccountantID,
                    Subscription = new SubscriptionListDisplayViewModel()
                    {
                        ID = bill.SubscriptionID,
                        Name = bill.Subscription.Customer.CorporateCustomerInfo != null ? bill.Subscription.Customer.CorporateCustomerInfo.Title : bill.Subscription.Customer.FirstName + " " + bill.Subscription.Customer.LastName
                    },
                    ServicePeriodLength = (short?)null,
                    SubscriptionID = bill.SubscriptionID,
                    ID = bill.ID,
                    IssueDate = bill.IssueDate,
                    DueDate = bill.DueDate,
                    PayDate = bill.PayDate,
                    PeriodStart = bill.PeriodStart,
                    PeriodEnd = bill.PeriodEnd,
                    StateID = bill.BillStatusID,
                    PaymentTypeID = bill.PaymentTypeID,
                    PaymentGatewayID = (bill.ExternalPayment != null) ? bill.ExternalPayment.ExternalUserID ?? bill.ExternalPayment.OfflineUserID : (int?)null,
                    PaymentGateway = (bill.ExternalPayment != null && bill.ExternalPayment.ExternalUserID != null) ? bill.ExternalPayment.RadiusRBillingService.Name : bill.PartnerBillPayment != null ? bill.PartnerBillPayment.Partner.Title + (bill.PartnerBillPayment.PartnerSubUser != null ? " (" + bill.PartnerBillPayment.PartnerSubUser.Name + ")" : string.Empty) : (bill.ExternalPayment != null && bill.ExternalPayment.OfflineUserID != null) ? bill.ExternalPayment.OfflinePaymentGateway.Name : null,
                    BillFees = bill.BillFees.Select(fee => new BillFeeViewModel()
                    {
                        DisplayName = fee.GetDisplayName(),
                        ID = fee.ID,
                        InstallmentCount = fee.InstallmentCount,
                        _currentCost = fee.CurrentCost,
                        _discountAmount = fee.DiscountID.HasValue ? fee.Discount.Amount : (decimal?)null,
                        StartDate = fee.StartDate,
                        EndDate = fee.EndDate
                    }),
                    EBillIsSent = bill.EBill != null,
                    Source = bill.Source,
                    _totalCost = bill.GetTotalCost(),
                    _totalDiscount = bill.GetTotalDiscount(),
                    EBill = bill.EBill != null ? new EBillViewModel()
                    {
                        BillCode = bill.EBill.BillCode,
                        Date = bill.EBill.Date,
                        EBillType = bill.EBill.EBillType,
                        ReferenceNo = bill.EBill.ReferenceNo,
                        EBillIssueDate = bill.EBill.EBillIssueDate
                    } : null
                });
        }

        public static BillViewModel GetViewModel(this Bill bill)
        {
            return new BillViewModel()
            {
                Accountant = bill.AppUser,
                AccountantID = bill.AccountantID,
                Subscription = new SubscriptionListDisplayViewModel()
                {
                    ID = bill.SubscriptionID,
                    Name = bill.Subscription.Customer.CorporateCustomerInfo != null ? bill.Subscription.Customer.CorporateCustomerInfo.Title : bill.Subscription.Customer.FirstName + " " + bill.Subscription.Customer.LastName
                },
                ServicePeriodLength = (short?)null,
                SubscriptionID = bill.SubscriptionID,
                ID = bill.ID,
                IssueDate = bill.IssueDate,
                DueDate = bill.DueDate,
                PayDate = bill.PayDate,
                PeriodStart = bill.PeriodStart,
                PeriodEnd = bill.PeriodEnd,
                StateID = bill.BillStatusID,
                PaymentTypeID = bill.PaymentTypeID,
                PaymentGatewayID = (bill.ExternalPayment != null) ? bill.ExternalPayment.ExternalUserID ?? bill.ExternalPayment.OfflineUserID : (int?)null,
                PaymentGateway = (bill.ExternalPayment != null && bill.ExternalPayment.ExternalUserID != null) ? bill.ExternalPayment.RadiusRBillingService.Name : bill.PartnerBillPayment != null ? bill.PartnerBillPayment.Partner.Title + (bill.PartnerBillPayment.PartnerSubUser != null ? " (" + bill.PartnerBillPayment.PartnerSubUser.Name + ")" : string.Empty) : (bill.ExternalPayment != null && bill.ExternalPayment.OfflineUserID != null) ? bill.ExternalPayment.OfflinePaymentGateway.Name : null,
                BillFees = bill.BillFees.Select(fee => new BillFeeViewModel()
                {
                    DisplayName = fee.GetDisplayName(),
                    ID = fee.ID,
                    InstallmentCount = fee.InstallmentCount,
                    _currentCost = fee.CurrentCost,
                    _discountAmount = fee.Discount != null ? fee.Discount.Amount : (decimal?)null,
                    StartDate = fee.StartDate,
                    EndDate = fee.EndDate
                }),
                EBillIsSent = bill.EBill != null,
                Source = bill.Source,
                _totalCost = bill.GetTotalCost(),
                _totalDiscount = bill.GetTotalDiscount(),
                EBill = bill.EBill != null ? new EBillViewModel()
                {
                    BillCode = bill.EBill.BillCode,
                    Date = bill.EBill.Date,
                    EBillType = bill.EBill.EBillType,
                    ReferenceNo = bill.EBill.ReferenceNo,
                    EBillIssueDate = bill.EBill.EBillIssueDate
                } : null
            };
        }
    }
}