using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public static class PrepaidFeePayment
    {
        public static Bill AddPrepaidFeeAndPayTheBill(this RadiusREntities db, Subscription dbSubscription, IEnumerable<Fee> additionalFees,PaymentType paymentType, int? accountantId)
        {
            additionalFees.AsParallel().ForAll(fee => fee.FeeTypeCost = db.FeeTypeCosts.Find(fee.FeeTypeID));
            var billFees = new List<BillFee>();
            foreach (var addedFee in additionalFees)
            {
                addedFee.Subscription = dbSubscription;
                billFees.Add(new BillFee()
                {
                    Fee = addedFee.FeeTypeCost.IsAllTime ? null : addedFee,
                    FeeTypeID = addedFee.FeeTypeCost.IsAllTime ? addedFee.FeeTypeID : (short?)null,
                    CurrentCost = addedFee.Cost ?? addedFee.FeeTypeCost.Cost ?? addedFee.FeeTypeVariant.Price,
                    InstallmentCount = 1
                });
            }

            var addedBill = new Bill()
            {
                AccountantID = accountantId,
                BillFees = billFees,                        
                BillStatusID = (short)BillState.Paid,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now,
                //PartialPercentage = 0m,
                PayDate = DateTime.Now,
                PaymentTypeID = (short)paymentType,
                //ServiceID = dbSubscription.ServiceID
            };

            dbSubscription.Bills.Add(addedBill);

            return addedBill;
        }
    }
}
