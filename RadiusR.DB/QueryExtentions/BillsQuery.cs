using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.QueryExtentions
{
    public static class BillsQuery
    {
        public static IQueryable<Bill> RemoveCashierPayments(this IQueryable<Bill> query)
        {
            return query.Where(bill => !bill.AccountantID.HasValue || bill.AppUser.Role.Name != "cashier");
        }

        public static decimal GetTotalPayableAmount(this IQueryable<Bill> query)
        {
            return query.Where(bill => bill.BillStatusID != (short)Enums.BillState.Cancelled).Include(bill => bill.BillFees.Select(billFee => billFee.Discount)).Select(bill => bill.BillFees.Select(billFee => billFee.CurrentCost).DefaultIfEmpty(0m).Sum() - bill.BillFees.Select(billFee => billFee.Discount != null ? billFee.Discount.Amount : 0m).DefaultIfEmpty(0m).Sum()).DefaultIfEmpty(0m).Sum();
        }

        public static decimal GetTotalAmount(this IQueryable<Bill> query)
        {
            return query.Include(bill => bill.BillFees.Select(billFee => billFee.Discount)).Select(bill => bill.BillFees.Select(billFee => billFee.CurrentCost).DefaultIfEmpty(0m).Sum()).DefaultIfEmpty(0m).Sum();
        }

        public static decimal GetTotalDiscountAmount(this IQueryable<Bill> query)
        {
            return query.Include(bill => bill.BillFees.Select(billFee => billFee.Discount)).Select(bill => bill.BillFees.Select(billFee => bill.BillStatusID == (short)Enums.BillState.Cancelled ? billFee.CurrentCost : billFee.Discount != null ? billFee.Discount.Amount : 0m).DefaultIfEmpty(0m).Sum()).DefaultIfEmpty(0m).Sum();
        }
    }
}
