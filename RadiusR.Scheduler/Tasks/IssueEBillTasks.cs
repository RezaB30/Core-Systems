using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using NLog;
using RezaB.NetInvoice.RadiusRDBAdapter;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RezaB.NetInvoice.RadiusRDBAdapter.DBExtentions;
using RadiusR.DB.Utilities.Billing;
using RezaB.NetInvoice.Wrapper;

namespace RadiusR.Scheduler.Tasks
{
    public class IssueEBillTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("issue-ebill-tasks");
        private const int batchSize = 1000;

        public override bool Run()
        {
            // issue e-bills
            logger.Info("E-bill tasks started.");
            var ebillDefaults = new EBillDefaults();
            if (ebillDefaults.IsActive)
            {
                // update e-bill companies
                logger.Info("Updating e-bill companies...");
                try
                {
                    Adapter.UpdateEBillCompanies();
                    logger.Info("E-bill companies updated.");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error updating e-bill companies");
                }
                // abort by flag
                if (_isAborted)
                {
                    logger.Debug("Aborted by the scheduler.");
                    return false;
                }
                // issue e-bills
                logger.Trace("Issuing e-bills...");
                try
                {
                    // create default values
                    long baseBillID = -1;
                    // fetch date info for threshold
                    var softThreshold = DateTime.Today.AddDays(-1 * AppSettings.ReviewDelay);
                    var hardThreshold = DateTime.Today.AddDays(-1 * AppSettings.EBillsThreshold);
                    // get bill years to create e-bills correctly
                    var validYears = new int[0];
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        var afterThresholdBillsWithoutEBill = db.Bills.Where(bill => bill.EBill == null).Select(bill => bill.IssueDate).Where(issueDate => issueDate >= hardThreshold).Where(issueDate => issueDate <= softThreshold);
                        validYears = afterThresholdBillsWithoutEBill.GroupBy(issueDate => issueDate.Year).Select(yg => yg.Key).OrderBy(y => y).ToArray();
                    }

                    foreach (var currentYear in validYears)
                    {
                        // create yearly boundries
                        var currentYearStart = new DateTime(currentYear, 1, 1);
                        var currentYearEnd = currentYearStart.AddYears(1);

                        // partitioning data
                        while (true)
                        {
                            using (RadiusREntities db = new RadiusREntities())
                            {
                                // fetch max issued bill and archive ids this year
                                var afterThresholdBillsWithoutEBill = db.Bills.Where(bill => bill.EBill == null && bill.BillStatusID != (short)BillState.Cancelled).Where(bill => bill.IssueDate >= hardThreshold).Where(bill => bill.IssueDate <= softThreshold).Where(bill => bill.IssueDate >= currentYearStart && bill.IssueDate < currentYearEnd);
                                var maxBillID = db.EBills.Where(ebill => ebill.EBillIssueDate >= currentYearStart && ebill.EBillIssueDate < currentYearEnd).Where(ebill => ebill.EBillType == (short)EBillType.EBill).Select(ebill => ebill.InternalSerialNo).DefaultIfEmpty(0).Max();
                                var maxArchiveID = db.EBills.Where(ebill => ebill.EBillIssueDate >= currentYearStart && ebill.EBillIssueDate < currentYearEnd).Where(ebill => ebill.EBillType == (short)EBillType.EArchive).Select(ebill => ebill.InternalSerialNo).DefaultIfEmpty(0).Max();

                                // take batch
                                var rawBillBatch = afterThresholdBillsWithoutEBill.OrderBy(bill => bill.ID)
                                    .Where(bill => bill.ID > baseBillID)
                                    .Take(batchSize)
                                    .PrepareForEBills()
                                    .ToList();
                                var currentBillBatch = rawBillBatch
                                    .Where(b => b.GetPayableCost() > 0)
                                    .SelectInvoiceReadyBills()
                                    .ToList();
                                if (rawBillBatch.Count() <= 0)
                                    break;
                                baseBillID = rawBillBatch.Max(bill => bill.ID);
                                // do batch operations
                                var allClientsTCKorVKN = currentBillBatch.Select(bill => bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo).Distinct();
                                var ebillCompanies = db.EBillRegisteredCompanies.Where(company => allClientsTCKorVKN.Contains(company.TaxNo));
                                var ebillCompaniesTaxNos = ebillCompanies.Select(ecompany => ecompany.TaxNo);
                                // create client
                                NetInvoiceClient client = new NetInvoiceClient(AppSettings.EBillCompanyCode, AppSettings.EBillApiUsername, AppSettings.EBillApiPassword);
                                // issue archives
                                var archiveBills = currentBillBatch.Where(bill => !ebillCompaniesTaxNos.Contains(bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo));
                                foreach (var bill in archiveBills)
                                {
                                    // abort by flag
                                    if (_isAborted)
                                    {
                                        logger.Debug("Aborted by the scheduler.");
                                        return false;
                                    }
                                    try
                                    {
                                        maxArchiveID++;
                                        var currentInvoice = Adapter.CreateInvoice(bill, maxArchiveID, ebillDefaults);

                                        var response = client.SendInvoice(currentInvoice);
                                        if (response.ErrorCode != 0)
                                        {
                                            logger.Error($"Error creating e-archive with ID: {bill.ID} NetInvoice Client Error: {response.ResultDescription}");
                                            maxArchiveID--;
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
                                            EBillIssueDate = bill.IssueDate
                                        });
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex, $"Error creating e-bill with ID: {bill.ID}");
                                        maxArchiveID--;
                                    }
                                }
                                // issue bills
                                var ebillBills = currentBillBatch.Where(bill => ebillCompaniesTaxNos.Contains(bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo));
                                foreach (var bill in ebillBills)
                                {
                                    // abort by flag
                                    if (_isAborted)
                                    {
                                        logger.Debug("Aborted by the scheduler.");
                                        return false;
                                    }
                                    try
                                    {
                                        maxBillID++;
                                        var targetTaxNo = bill.Client.CorporateInfo != null ? bill.Client.CorporateInfo.TaxNo : bill.Client.TCNo;
                                        var ebillCompany = ebillCompanies.FirstOrDefault(company => company.TaxNo == targetTaxNo);
                                        if (ebillCompany == null)
                                        {
                                            logger.Error($"Error creating e-bill with ID: {bill.ID} e-bill company info not found!");
                                            maxBillID--;
                                            continue;
                                        }
                                        var currentInvoice = Adapter.CreateInvoice(bill, maxBillID, ebillDefaults, ebillCompany);

                                        var response = client.SendInvoice(currentInvoice);
                                        if (response.ErrorCode != 0)
                                        {
                                            logger.Error($"Error creating e-bill with ID: {bill.ID} NetInvoice Client Error: {response.ResultDescription}");
                                            maxBillID--;
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
                                            EBillIssueDate = bill.IssueDate
                                        });
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex, $"Error creating e-bill with ID: {bill.ID}");
                                        maxBillID--;
                                    }
                                }
                            }
                        }
                    }

                    logger.Trace("E-bills issued.");
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex, $"General error in e-bill tasks.");
                    return false;
                }
            }
            else
            {
                logger.Trace("Skipped e-bills due to settings...");
            }
            return true;
        }
    }
}
