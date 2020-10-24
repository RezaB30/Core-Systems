using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public static class BillExtentions
    {
        /// <summary>
        /// Gets the exact service price for this bill.
        /// </summary>
        /// <param name="bill"></param>
        /// <returns>service price</returns>
        public static decimal GetServicePrice(this Bill bill)
        {
            return bill.BillFees.Any(fee => fee.FeeTypeID == (short)FeeType.Tariff) ? bill.BillFees.FirstOrDefault(fee => fee.FeeTypeID == (short)FeeType.Tariff).CurrentCost : 0m;
        }

        public static decimal GetTotalCost(this Bill bill)
        {
            return bill.BillFees.Select(fee => fee.CurrentCost).DefaultIfEmpty(0m).Sum();
        }

        public static decimal GetTotalDiscount(this Bill bill)
        {
            return bill.BillFees.Sum(fee => fee.Discount != null ? fee.Discount.Amount : 0m);
        }

        public static decimal GetPayableCost(this Bill bill)
        {
            return Math.Max(bill.GetTotalCost() - bill.GetTotalDiscount(), 0m);
        }

        public static decimal GetTotal(this BillReport report)
        {
            return Math.Max(report.Fees.DefaultIfEmpty(0m).Sum() - report.Discounts.DefaultIfEmpty(0m).Sum(), 0m);
        }

        /// <summary>
        /// Gets the bill tax value.
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="taxType">Type of tax to evaluate.</param>
        /// <returns></returns>
        public static decimal GetTaxAmount(this Bill bill, TaxTypeID taxType)
        {
            var total = 0m;
            foreach (var fee in bill.BillFees)
            {
                var feeType = fee.FeeTypeCost ?? fee.Fee.FeeTypeCost;
                total += fee.GetTaxBase() * (feeType.TaxRates.Where(rate => rate.ID == (short)taxType).Select(rate => rate.Rate).DefaultIfEmpty(0m).Sum());
            }
            return total;
        }

        public static decimal GetTaxBase(this Bill bill)
        {
            var total = 0m;
            foreach (var fee in bill.BillFees)
            {
                total += fee.GetTaxBase();
            }
            return total;
        }

        //public static DateTime GetLastPaymentDate(this Bill bill)
        //{
        //    if (!bill.ServiceID.HasValue || bill.Service.PeriodLength.HasValue)
        //        return bill.IssueDate;
        //    return bill.IssueDate.AddDays(AppSettings.PaymentTolerance);
        //}

        public class BillReport
        {
            public DateTime IssueDate { get; set; }

            public short State { get; set; }

            public IEnumerable<decimal> Fees { get; set; }

            public IEnumerable<decimal> Discounts { get; set; }
        }
    }
}
