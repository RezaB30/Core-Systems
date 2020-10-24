using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class ClientCreditViewModel
    {
        public long ID { get; set; }

        public string FullName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Total")]
        [UIHint("Currency")]
        public string Total
        {
            get
            {
                return Credits.Sum(credit => credit._amount).ToString("###,##0.00");
            }
        }

        public bool HasBilling { get; set; }

        public IEnumerable<CreditViewModel> Credits { get; set; }

        public ClientCreditViewModel() { }

        public ClientCreditViewModel(RadiusREntities entities, long id)
        {
            var dbSubscription = entities.Subscriptions.Find(id);
            ID = dbSubscription.ID;
            FullName = dbSubscription.Customer.CustomerType == (short)RadiusR.DB.Enums.CustomerType.Individual ? dbSubscription.Customer.FirstName + " " + dbSubscription.Customer.LastName : dbSubscription.Customer.CorporateCustomerInfo != null ? dbSubscription.Customer.CorporateCustomerInfo.Title : "-";
            HasBilling = dbSubscription.HasBilling;
            Credits = dbSubscription.SubscriptionCredits.OrderByDescending(credit => credit.Date).Select(credit => new CreditViewModel()
            {
                ID = credit.ID,
                _amount = credit.Amount,
                SubscriptionID = credit.SubscriptionID,
                Date = credit.Date,
                AccountantName = credit.AccountantID.HasValue? credit.AppUser.Name : "-",
                Subscription = new SubscriptionListDisplayViewModel()
                {
                    ID = credit.SubscriptionID,
                    Name = credit.Subscription.Customer.CorporateCustomerInfo != null ? credit.Subscription.Customer.CorporateCustomerInfo.Title : credit.Subscription.Customer.FirstName + " " + credit.Subscription.Customer.LastName
                },
                Bill = (credit.Bill != null) ? new BillViewModel()
                {
                    ID = credit.BillID.Value,
                    IssueDate = credit.Bill.IssueDate,
                    PayDate = credit.Bill.PayDate
                } : null
            });
        }

        public class CreditViewModel
        {
            public long ID { get; set; }

            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            public long SubscriptionID { get; set; }

            public SubscriptionListDisplayViewModel Subscription { get; set; }

            public Subscription _subscription { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Accountant")]
            public string AccountantName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Amount")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            [UIHint("Currency")]
            public string Amount
            {
                get
                {
                    return _amount.ToString("###,##0.00");
                }
                set
                {
                    decimal parsed;
                    if (decimal.TryParse(value, out parsed))
                        _amount = parsed;
                }
            }

            public decimal _amount { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
            [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
            public DateTime Date { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
            public BillViewModel Bill { get; set; }
        }
    }
}