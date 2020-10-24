using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.NetInvoice.Wrapper;
using System.Globalization;
using RadiusR.DB.Localization;
using NLog;
using System.Data.Entity;
using RadiusR.DB.Enums;
using RezaB.NetInvoice.RadiusRDBAdapter.DBExtentions;

namespace RezaB.NetInvoice.RadiusRDBAdapter
{
    public static class Adapter
    {
        private static Logger EBillIssueLogger = LogManager.GetLogger("issue_ebill_exceptions");
        private static bool IsStopped = false;
        private static bool IsRunning = false;
        /// <summary>
        /// Creates invoice for NetInvoice client from database.
        /// </summary>
        /// <param name="dbBill">Database bill.</param>
        /// <param name="billNo">Invoice serial No. (Unique per year)</param>
        /// <param name="defaults">Invoice defaults</param>
        /// <param name="ebillCompany">Company e-bill info if exists.</param>
        /// <returns></returns>
        public static Invoice CreateInvoice(InvoiceReadyBill dbBill, int billNo, EBillDefaults defaults, EBillRegisteredCompany ebillCompany = null)
        {
            //var culture = CultureInfo.CreateSpecificCulture("tr-tr");
            var type = InvoiceType.EArchive;
            if (ebillCompany != null)
                type = InvoiceType.EBill;
            // create the bill
            var result = new Invoice()
            {
                InvoiceInfo = new Wrapper.InvoiceInfo.InvoiceInfo()
                {
                    CurrencyCode = Enums.CurrencyCodes.TRY,
                    DueDate = dbBill.DueDate,
                    IssueDate = dbBill.IssueDate,
                    InvoiceEndDate = dbBill.IssueDate,
                    InvoiceStartDate = dbBill.PeriodStartDate,
                    Type = type,
                    InvoiceIDPrefix = type == InvoiceType.EArchive ? defaults.InvoiceArchiveIDPrefix : defaults.InvoiceBillIDPrefix,
                    InvoiceInternalID = billNo,
                    Items = dbBill.BillFees.Select(fee => new Wrapper.InvoiceInfo.ItemInfo()
                    {
                        //Name = fee.GetDisplayName(dbBill, true),
                        Name = fee.Name,
                        Total = fee.CurrentCost,
                        Taxes = (fee.FeeTypeCost != null) ? (fee.FeeTypeCost.TaxRates.Select(tax => new Wrapper.InvoiceInfo.TaxInfo()
                        {
                            Percentage = tax.Rate,
                            Type = ConvertTaxType((TaxTypeID)tax.ID)
                        }).ToList()) : (fee.Fee.FeeTypeCost.TaxRates.Select(tax => new Wrapper.InvoiceInfo.TaxInfo()
                        {
                            Percentage = tax.Rate,
                            Type = ConvertTaxType((TaxTypeID)tax.ID)
                        }).ToList()),
                        Discount = fee.Discount == null ? null : new Wrapper.InvoiceInfo.DiscountInfo()
                        {
                            Amount = fee.Discount.Amount,
                            Type = Wrapper.InvoiceInfo.DiscountTypes.Fixed
                        }
                    }).ToList()
                },
                SenderInfo = new Wrapper.ClientInfo.CompanyInfo()
                {
                    CentralSystemNo = defaults.SenderCentralSystemNo,
                    CityName = defaults.SenderCityName,
                    CountryName = defaults.SenderCountryName,
                    ProvinceName = defaults.SenderProvinceName,
                    CompanyTaxRegion = defaults.SenderCompanyTaxRegion,
                    CompanyTitle = defaults.SenderCompanyTitle,
                    Email = defaults.SenderEmail,
                    PhoneNo = defaults.SenderPhoneNo,
                    RegistrationNo = defaults.SenderRegistrationNo,
                    VKN = defaults.SenderTaxNo,
                    FaxNo = defaults.SenderFaxNo
                }
            };
            // setup receipent info
            if (dbBill.Client.CorporateInfo != null)
            {
                result.ReceipentInfo = new Wrapper.ClientInfo.CompanyInfo()
                {
                    CountryName = defaults.SenderCountryName,
                    CompanyTitle = dbBill.Client.CorporateInfo.CompanyTitle,
                    CompanyTaxRegion = dbBill.Client.CorporateInfo.TaxRegion,
                    PhoneNo = dbBill.Client.Phone,
                    VKN = dbBill.Client.CorporateInfo.TaxNo,
                    SubscriberNo = dbBill.Client.SubscriberNo,
                    TTsubscriberNo = dbBill.Client.TTSubscriberNo,
                    ProvinceName = dbBill.Client.Address.ProvinceName,
                    CityName = dbBill.Client.Address.DistrictName,
                    StreetName = dbBill.Client.Address.StreetName,
                    BuildingNumber = dbBill.Client.Address.DoorNo,
                    Room = dbBill.Client.Address.ApartmentNo,
                    PostalCode = dbBill.Client.Address.PostalCode,
                    CentralSystemNo = dbBill.Client.CorporateInfo.CentralSystemNo,
                    NeighbourHoodName = dbBill.Client.Address.NeighborhoodName,
                    
                };
            }
            else
            {
                result.ReceipentInfo = new Wrapper.ClientInfo.PersonalInfo()
                {
                    CountryName = defaults.SenderCountryName,
                    PhoneNo = dbBill.Client.Phone,
                    FirstName = dbBill.Client.FirstName,
                    LastName = dbBill.Client.LastName,
                    TCKN = dbBill.Client.TCNo,
                    SubscriberNo = dbBill.Client.SubscriberNo,
                    TTsubscriberNo = dbBill.Client.TTSubscriberNo,
                    ProvinceName = dbBill.Client.Address.ProvinceName,
                    CityName = dbBill.Client.Address.DistrictName,
                    StreetName = dbBill.Client.Address.StreetName,
                    BuildingNumber = dbBill.Client.Address.DoorNo,
                    Room = dbBill.Client.Address.ApartmentNo,
                    PostalCode = dbBill.Client.Address.PostalCode,
                    NeighbourHoodName = dbBill.Client.Address.NeighborhoodName,
                    BBK = dbBill.Client.Address.BBK
                };
            }
            // set optional or conditional parameters
            if (!string.IsNullOrEmpty(dbBill.Client.Email))
                result.ReceipentInfo.Email = dbBill.Client.Email;
            if (type == InvoiceType.EBill)
                result.InvoiceInfo.EBillMailUrn = ebillCompany.MailUrn;
            if (defaults.PastDueFlatPenalty > 0m)
                result.InvoiceInfo.PastDueFlatPenalty = defaults.PastDueFlatPenalty;
            if (defaults.PastDuePenaltyPercentage > 0m)
                result.InvoiceInfo.PastDuePenaltyPercentage = defaults.PastDuePenaltyPercentage;

            return result;
        }
        /// <summary>
        /// Converts db tax type to invoice tax type.
        /// </summary>
        /// <param name="dbTaxType">db tax type.</param>
        /// <returns>invoice tax type.</returns>
        private static Enums.TaxTypeCodes ConvertTaxType(TaxTypeID dbTaxType)
        {
            switch (dbTaxType)
            {
                case TaxTypeID.VAT:
                    return Enums.TaxTypeCodes.KDV;
                case TaxTypeID.SCT:
                    return Enums.TaxTypeCodes.OIV;
                default:
                    return Enums.TaxTypeCodes.KDV;
            }
        }
        /// <summary>
        /// Updates database with new e-bill companies from web service.
        /// (does not handle exceptions)
        /// </summary>
        public static void UpdateEBillCompanies()
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                // find last updated company
                var startDate = db.EBillRegisteredCompanies.Select(company => company.RegistrationDate).DefaultIfEmpty(new DateTime(1753, 1, 1)).Max();
                // send request
                var client = new NetInvoiceClient(AppSettings.EBillCompanyCode, AppSettings.EBillApiUsername, AppSettings.EBillApiPassword);
                var response = client.GetECompaniesList(startDate);
                var validCompanies = response.CompanyList.Where(company => company.RegistrationDate > startDate);
                // add to database
                var toAddEntities = validCompanies.Select(company => new EBillRegisteredCompany()
                {
                    MailUrn = company.MailUrn,
                    Name = company.Name,
                    RegistrationDate = company.RegistrationDate,
                    TaxNo = company.TaxNo
                }).ToList();
                db.EBillRegisteredCompanies.AddRange(toAddEntities);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Sends e-bills in batch.
        /// </summary>
        /// <param name="billIds">To send bill ids.</param>
        /// <returns>The batch results</returns>
        public static EBillBatchResults SendBatch(IEnumerable<long> billIds, DateTime? issueDate = null)
        {
            var results = new EBillBatchResults();
            results.TotalCount = billIds.Count();
            if (IsRunning)
            {
                results.ErrorCode = EBillBatchResults.ResultType.CuncurrencyDetected;
                return results;
            }
            IsStopped = false;
            try
            {
                IsRunning = true;
                var years = new int[0];
                if (!issueDate.HasValue)
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        var billsWithoutEbill = db.Bills.Where(bill => billIds.Contains(bill.ID)).Where(bill => bill.EBill == null);
                        years = billsWithoutEbill.GroupBy(bill => bill.IssueDate.Year).Select(yg => yg.Key).OrderBy(y => y).ToArray();
                    }
                }
                else
                {
                    years = new int[1] { issueDate.Value.Year };
                }
                foreach (var currentYear in years)
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.Database.Log = log => System.Diagnostics.Debug.WriteLine(log);
                        // create default values
                        EBillDefaults defaults = new EBillDefaults();
                        // fetch max issued bill and archive ids this year
                        var currentYearStart = new DateTime(currentYear, 1, 1);
                        var currentYearEnd = currentYearStart.AddYears(1);
                        var maxBillID = db.EBills.Where(ebill => ebill.EBillIssueDate >= currentYearStart && ebill.EBillIssueDate < currentYearEnd).Where(ebill => ebill.EBillType == (short)EBillType.EBill).Select(ebill => ebill.InternalSerialNo).DefaultIfEmpty(0).Max();
                        var maxArchiveID = db.EBills.Where(ebill => ebill.EBillIssueDate >= currentYearStart && ebill.EBillIssueDate < currentYearEnd).Where(ebill => ebill.EBillType == (short)EBillType.EArchive).Select(ebill => ebill.InternalSerialNo).DefaultIfEmpty(0).Max();
                        // find valid bills
                        var billsWithoutEbill = db.Bills.Where(bill => billIds.Contains(bill.ID)).Where(bill => bill.EBill == null)
                            .PrepareForEBills()
                            .ToList()
                            .SelectInvoiceReadyBills()
                            .ToList();
                        var allClientsTCKorVKN = billsWithoutEbill.Select(bill => bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo).Distinct();
                        var ebillCompanies = db.EBillRegisteredCompanies.Where(company => allClientsTCKorVKN.Contains(company.TaxNo));
                        var ebillCompaniesTaxNos = ebillCompanies.Select(ecompany => ecompany.TaxNo);
                        // create client
                        NetInvoiceClient client = new NetInvoiceClient(AppSettings.EBillCompanyCode, AppSettings.EBillApiUsername, AppSettings.EBillApiPassword);
                        // issue archives
                        var archiveBills = billsWithoutEbill.Where(bill => !ebillCompaniesTaxNos.Contains(bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo));
                        foreach (var bill in archiveBills)
                        {
                            if (IsStopped)
                                return results;
                            try
                            {
                                maxArchiveID++;
                                var currentInvoice = CreateInvoice(bill, maxArchiveID, defaults);
                                if (issueDate.HasValue)
                                    currentInvoice.InvoiceInfo.IssueDate = issueDate.Value;

                                var response = client.SendInvoice(currentInvoice);
                                if (response.ErrorCode != 0)
                                {
                                    EBillIssueLogger.Error("Error creating e-archive with ID: " + bill.ID + " NetInvoice Client Error: " + response.ResultDescription);
                                    maxArchiveID--;
                                    results.UnsuccessfulCount++;
                                    continue;
                                }
                                // add to entities
                                db.EBills.Add(new EBill()
                                {
                                    BillID = bill.ID,
                                    BillCode = currentInvoice.InvoiceID,
                                    Date = DateTime.Now,
                                    EBillType = (short)EBillType.EArchive,
                                    InternalSerialNo = maxArchiveID,
                                    ReferenceNo = currentInvoice.ReferenceNo,
                                    EBillIssueDate = issueDate ?? bill.IssueDate
                                });
                                db.SaveChanges();
                                results.SuccessfulCount++;
                            }
                            catch (Exception ex)
                            {
                                EBillIssueLogger.Error(ex, "Error creating e-bill with ID: " + bill.ID);
                                maxArchiveID--;
                                results.UnsuccessfulCount++;
                                results.ErrorCode = EBillBatchResults.ResultType.PartialError;
                            }
                        }
                        // issue bills
                        var ebillBills = billsWithoutEbill.Where(bill => ebillCompaniesTaxNos.Contains(bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo));
                        foreach (var bill in ebillBills)
                        {
                            if (IsStopped)
                                return results;
                            try
                            {
                                maxBillID++;
                                var ebillCompany = ebillCompanies.FirstOrDefault(company => company.TaxNo == (bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo));
                                if (ebillCompany == null)
                                {
                                    EBillIssueLogger.Error("Error creating e-bill with ID: " + bill.ID + " e-bill company info not found!");
                                    maxBillID--;
                                    continue;
                                }
                                var currentInvoice = CreateInvoice(bill, maxBillID, defaults, ebillCompany);
                                if (issueDate.HasValue)
                                    currentInvoice.InvoiceInfo.IssueDate = issueDate.Value;

                                var response = client.SendInvoice(currentInvoice);
                                if (response.ErrorCode != 0)
                                {
                                    EBillIssueLogger.Error("Error creating e-bill with ID: " + bill.ID + " NetInvoice Client Error: " + response.ResultDescription);
                                    maxBillID--;
                                    results.UnsuccessfulCount++;
                                    continue;
                                }
                                // add to entities
                                db.EBills.Add(new EBill()
                                {
                                    BillID = bill.ID,
                                    BillCode = currentInvoice.InvoiceID,
                                    Date = DateTime.Now,
                                    EBillType = (short)EBillType.EBill,
                                    InternalSerialNo = maxBillID,
                                    ReferenceNo = response.ID,
                                    EBillIssueDate = issueDate ?? bill.IssueDate
                                });
                                db.SaveChanges();
                                results.SuccessfulCount++;
                            }
                            catch (Exception ex)
                            {
                                EBillIssueLogger.Error(ex, "Error creating e-bill with ID: " + bill.ID);
                                maxBillID--;
                                results.UnsuccessfulCount++;
                                results.ErrorCode = EBillBatchResults.ResultType.PartialError;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EBillIssueLogger.Error(ex);
                results.ErrorCode = EBillBatchResults.ResultType.FatalError;
            }
            finally
            {
                IsRunning = false;
            }
            return results;
        }
        /// <summary>
        /// Stops running batch.
        /// </summary>
        public static void StopBatch()
        {
            IsStopped = true;
        }

        public static CancellationResult CancelEBill(EBill dbEBill)
        {
            if (dbEBill.Bill.BillStatusID == (short)BillState.Paid)
                return new CancellationResult(CancellationResultType.InvalidBillState);
            var client = new NetInvoiceClient(AppSettings.EBillCompanyCode, AppSettings.EBillApiUsername, AppSettings.EBillApiPassword);
            if(dbEBill.EBillType == (short)EBillType.EBill)
            {
                var response = client.CancelEBill(dbEBill.ReferenceNo);
                if (response.ErrorCode != 0)
                    return new CancellationResult(CancellationResultType.WebServiceError, response.ResultDescription);
                return new CancellationResult(CancellationResultType.Success);
            }
            if(dbEBill.EBillType == (short)EBillType.EArchive)
            {
                var response = client.CancelEArchive(dbEBill.ReferenceNo,dbEBill.Bill.GetTaxBase());
                if (response.ErrorCode != 0)
                    return new CancellationResult(CancellationResultType.WebServiceError, response.ResultDescription);
                return new CancellationResult(CancellationResultType.Success);
            }

            return new CancellationResult(CancellationResultType.InvalidEBillType);
        }
    }
}
