using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB;
using System.Data.Entity;
using RadiusR.DB.Localization.Bills;

namespace RezaB.NetInvoice.RadiusRDBAdapter.DBExtentions
{
    public static class BillExtentions
    {
        public static IQueryable<Bill> PrepareForEBills(this IQueryable<Bill> query)
        {
            return query
                .Include(b => b.Subscription.Customer.CorporateCustomerInfo)
                .Include(b => b.Subscription.Customer.CustomerIDCard)
                .Include(b => b.Subscription.Customer.BillingAddress)
                .Include(b => b.Subscription.SubscriptionTelekomInfo)
                .Include(b => b.BillFees.Select(bf => bf.Fee.FeeTypeVariant))
                .Include(b => b.BillFees.Select(bf => bf.FeeTypeCost.TaxRates))
                .Include(b => b.BillFees.Select(bf => bf.Fee.FeeTypeCost.TaxRates))
                .Include(b => b.BillFees.Select(bf => bf.Discount));
        }

        public static IEnumerable<InvoiceReadyBill> SelectInvoiceReadyBills(this IEnumerable<Bill> query)
        {
            return query.Select(bill => new InvoiceReadyBill()
            {
                ID = bill.ID,
                IssueDate = bill.IssueDate,
                PeriodStartDate = bill.PeriodStart ?? bill.IssueDate,
                PeriodEndDate = bill.PeriodEnd ?? bill.IssueDate,
                DueDate = bill.DueDate,
                
                Client = new InvoiceReadyBill.InvoiceClient()
                {
                    ClientType = bill.Subscription.Customer.CustomerType,
                    CorporateInfo = bill.Subscription.Customer.CorporateCustomerInfo != null ? new InvoiceReadyBill.InvoiceClient.ClientCorporateInfo()
                    {
                        CompanyTitle = bill.Subscription.Customer.CorporateCustomerInfo.Title,
                        TaxNo = bill.Subscription.Customer.CorporateCustomerInfo.TaxNo,
                        TaxRegion = bill.Subscription.Customer.CorporateCustomerInfo.TaxOffice,
                        CentralSystemNo = bill.Subscription.Customer.CorporateCustomerInfo.CentralSystemNo
                    }
                    : null,
                    FirstName = bill.Subscription.Customer.FirstName,
                    LastName = bill.Subscription.Customer.LastName,
                    TCNo = bill.Subscription.Customer.CustomerIDCard.TCKNo,
                    Phone = bill.Subscription.Customer.ContactPhoneNo,
                    SubscriberNo = bill.Subscription.SubscriberNo,
                    Address = new InvoiceReadyBill.InvoiceAddress() {
                        ApartmentNo = bill.Subscription.Customer.BillingAddress.ApartmentNo,
                        DistrictName = bill.Subscription.Customer.BillingAddress.DistrictName,
                        DoorNo = bill.Subscription.Customer.BillingAddress.DoorNo,
                        FullAddress = bill.Subscription.Customer.BillingAddress.AddressText,
                        NeighborhoodName = bill.Subscription.Customer.BillingAddress.NeighborhoodName,
                        ProvinceName = bill.Subscription.Customer.BillingAddress.ProvinceName,
                        StreetName = bill.Subscription.Customer.BillingAddress.StreetName,
                        PostalCode = bill.Subscription.Customer.BillingAddress.PostalCode.ToString("00000"),
                        BBK = bill.Subscription.Customer.BillingAddress.ApartmentID.ToString()
                    },
                    ActivationDate = bill.Subscription.ActivationDate,
                    Culture = bill.Subscription.Customer.Culture,
                    Email = bill.Subscription.Customer.Email,
                    TTSubscriberNo = bill.Subscription.SubscriptionTelekomInfo != null ? bill.Subscription.SubscriptionTelekomInfo.SubscriptionNo : null
                },
                BillFees = bill.BillFees.Select(billFee => new InvoiceReadyBill.InvoiceBillFee()
                {
                    FeeTypeID = billFee.FeeTypeID,
                    CurrentCost = billFee.CurrentCost,
                    Name = billFee.GetDisplayName(true),
                    Fee = billFee.Fee != null ? new InvoiceReadyBill.InvoiceBillFee.InvoiceFee()
                    {
                        FeeTypeID = billFee.Fee.FeeTypeID,
                        FeeTypeVariant = billFee.Fee.FeeTypeVariant != null ? new InvoiceReadyBill.InvoiceBillFee.InvoiceFee.InvoiceFeeTypeVariant()
                        {
                            Title = billFee.Fee.FeeTypeVariant.Title
                        } : null,
                        FeeDescription = billFee.Fee.Description != null ? new InvoiceReadyBill.InvoiceBillFee.InvoiceFee.InvoiceFeeDescription()
                        {
                            Description = billFee.Fee.Description
                        } : null,
                        FeeTypeCost = new InvoiceReadyBill.InvoiceBillFee.InvoiceFee.InvoiceFeeTypeCost()
                        {
                            TaxRates = billFee.Fee.FeeTypeCost.TaxRates.Select(taxRate => new InvoiceReadyBill.InvoiceBillFee.InvoiceFee.InvoiceFeeTypeCost.InvoiceTaxRate()
                            {
                                ID = taxRate.ID,
                                Rate = taxRate.Rate
                            })
                        }
                    } : null,
                    FeeTypeCost = billFee.FeeTypeCost != null ? new InvoiceReadyBill.InvoiceBillFee.InvoiceFeeTypeCost()
                    {
                        TaxRates = billFee.FeeTypeCost.TaxRates.Select(taxRate => new InvoiceReadyBill.InvoiceBillFee.InvoiceFeeTypeCost.InvoiceTaxRate()
                        {
                            ID = taxRate.ID,
                            Rate = taxRate.Rate
                        })
                    } : null,
                    Discount = billFee.Discount != null ? new InvoiceReadyBill.InvoiceBillFee.InvoiceDiscount()
                    {
                        Amount = billFee.Discount.Amount
                    } : null
                })
            });
        }
    }
}
