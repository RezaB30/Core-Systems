using RadiusR.DB;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RezaB.NetInvoice.RadiusRDBAdapter.DBExtentions
{
    public class InvoiceReadyBill
    {
        public long ID { get; set; }

        public DateTime IssueDate { get; set; }

        //public decimal PartialPercentage { get; set; }

        public InvoiceService Service { get; set; }

        public InvoiceClient Client { get; set; }

        public IEnumerable<InvoiceBillFee> BillFees { get; set; }


        public class InvoiceService
        {
            public short? PeriodLength { get; set; }

            public string Name { get; set; }
        }

        public class InvoiceClient
        {
            public DateTime? ActivationDate { get; set; }

            public string Culture { get; set; }

            public short ClientType { get; set; }

            public string Phone { get; set; }

            public string SubscriberNo { get; set; }

            public string TTSubscriberNo { get; set; }

            public InvoiceAddress Address { get; set; }

            public string Email { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string TCNo { get; set; }

            public ClientCorporateInfo CorporateInfo { get; set; }

            public class ClientCorporateInfo
            {
                public string CompanyTitle { get; set; }

                public string TaxRegion { get; set; }

                public string TaxNo { get; set; }

                public string CentralSystemNo { get; set; }
            }
        }

        public class InvoiceBillFee
        {
            public short? FeeTypeID { get; set; }

            public decimal CurrentCost { get; set; }

            public InvoiceFee Fee { get; set; }

            public InvoiceFeeTypeCost FeeTypeCost { get; set; }

            public InvoiceDiscount Discount { get; set; }

            public string Name { get; set; }

            public class InvoiceFee
            {
                public short FeeTypeID { get; set; }

                public InvoiceFeeTypeVariant FeeTypeVariant { get; set; }

                public InvoiceFeeDescription FeeDescription { get; set; }

                public InvoiceFeeTypeCost FeeTypeCost { get; set; }

                public class InvoiceFeeTypeVariant
                {
                    public string Title { get; set; }
                }

                public class InvoiceFeeDescription
                {
                    public string Description { get; set; }
                }

                public class InvoiceFeeTypeCost
                {
                    public IEnumerable<InvoiceTaxRate> TaxRates { get; set; }

                    public class InvoiceTaxRate
                    {
                        public short ID { get; set; }

                        public decimal Rate { get; set; }
                    }
                }
            }

            public class InvoiceFeeTypeCost
            {
                public IEnumerable<InvoiceTaxRate> TaxRates { get; set; }

                public class InvoiceTaxRate
                {
                    public short ID { get; set; }

                    public decimal Rate { get; set; }
                }
            }

            public class InvoiceDiscount
            {
                public decimal Amount { get; set; }
            }

            //public string GetDisplayName(InvoiceReadyBill parent, bool useClientCulture = false)
            //{
            //    var culture = useClientCulture ? CultureInfo.CreateSpecificCulture(parent.Client.Culture) : Thread.CurrentThread.CurrentCulture;
            //    if (FeeTypeID == (short?)FeeType.Tariff)
            //    {
            //        var results = parent.Service.Name;
            //        if (parent.PartialPercentage < 1m)
            //            results += " (" + Math.Round(30 * parent.PartialPercentage).ToString() + " " + RadiusR.Localization.Pages.Common.ResourceManager.GetString("Days", culture) + ")";
            //        if (parent.Service != null && parent.Service.PeriodLength.HasValue)
            //            results += " (" + parent.PartialPercentage.ToString("##0") + "X)";

            //        return results;
            //    }
            //    else
            //    {
            //        var results = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(FeeTypeID ?? Fee.FeeTypeID, culture);
            //        if (Fee != null && Fee.FeeTypeVariant != null)
            //            results += " (" + Fee.FeeTypeVariant.Title + ")";
            //        if (Fee != null && Fee.FeeDescription != null)
            //            results += " (" + Fee.FeeDescription.Description + ")";

            //        return results;
            //    }
            //}
        }

        public DateTime DueDate { get; set; }

        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }
        //{
        //    get
        //    {
        //        if (!ServiceID.HasValue || PartialPercentage == 0m)
        //        {
        //            return IssueDate;
        //        }
        //        DateTime startDate;
        //        if (Service.PeriodLength.HasValue)
        //        {
        //            startDate = IssueDate.AddDays(-1 * Service.PeriodLength.Value);
        //        }
        //        else
        //        {
        //            startDate = IssueDate.AddMonths(-1);
        //        }

        //        if (Client.ActivationDate.HasValue && Client.ActivationDate > startDate)
        //        {
        //            startDate = Client.ActivationDate.Value;
        //        }

        //        return startDate;
        //    }
        //}

        public class InvoiceAddress
        {
            public string ProvinceName { get; set; }

            public string DistrictName { get; set; }

            public string NeighborhoodName { get; set; }

            public string StreetName { get; set; }

            public string DoorNo { get; set; }

            public string ApartmentNo { get; set; }

            public string PostalCode { get; set; }

            public string BBK { get; set; }

            public string FullAddress { get; set; }
        }
    }
}
