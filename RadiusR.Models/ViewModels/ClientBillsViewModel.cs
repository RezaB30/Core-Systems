using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ClientBillsViewModel
    {
        public long ClientID { get; set; }

        public short State { get; set; }

        public string FullName { set; get; }

        public bool HasBilling { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<BillSelection> BillSelections { get; set; }

        public class BillSelection
        {
            public bool IsSelected { get; set; }

            public long BillID { get; set; }

            public BillViewModel Bill { get; set; }
        }

        public ClientBillsViewModel(Subscription dbSubscription)
        {
            FullName = dbSubscription.ValidDisplayName;
            ClientID = dbSubscription.ID;
            State = dbSubscription.State;
            HasBilling = dbSubscription.HasBilling;
            IsActive = dbSubscription.IsActive;

            BillSelections = dbSubscription.Bills.OrderByDescending(bill => bill.IssueDate).Select(bill => new BillSelection()
            {
                BillID = bill.ID,
                IsSelected = false,
                Bill = bill.GetViewModel()
            });
        }

        public ClientBillsViewModel() { HasBilling = true; }

        public decimal _totalCost
        {
            get
            {
                if (BillSelections == null)
                {
                    return 0m;
                }
                return BillSelections.Where(selection => selection.Bill != null).Sum(selection => selection.Bill._totalCost);
            }
        }

        [UIHint("Currency")]
        public string TotalCost
        {
            get
            {
                return _totalCost.ToString("###,##0.00");
            }
        }
    }
}