//using RadiusR.DB;
//using RadiusR.DB.Enums;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using NLog;
//using RezaB.NetInvoice.RadiusRDBAdapter;
//using RadiusR.DB.Utilities.Billing;

//namespace RadiusR.Scheduler
//{
//    public static partial class Scheduler
//    {
//        private static bool _isBillingRunning = false;
//        private static DateTime _lastSuccessfulBillIssue = DateTime.MinValue;

//        private static Logger BillIssueLogger = LogManager.GetLogger("issue_bill_exceptions");

//        const int batchSize = 1000;
//        //const int batchBreathTime = 2000;
//        private static void IssueBills()
//        {
//            try
//            {
//                {
//                    var minClientID = (long)0;
//                    var processedCount = 0;
//                    while (true)
//                    {
//                        using (RadiusREntities db = new RadiusREntities())
//                        {
//                            // ---------------- ID Batching -------------------
//                            var baseQuery = db.Subscriptions
//                                .OrderBy(subscription => subscription.ID)
//                                .GetValidClientsForBilling()
//                                .Where(client => client.ID >= minClientID);
//                            var maxClientID = baseQuery.Select(s => s.ID).Take(batchSize).DefaultIfEmpty(0).Max();
//                            if (maxClientID == 0)
//                                break;

//                            var currentBatch = db.PrepareForBilling(baseQuery.Where(client => client.ID <= maxClientID)).ToArray();
//                            // ------------------------------------------------

//                            //if (currentBatch.Count() == 0)
//                            //    break;

//                            // for each item in batch
//                            foreach (var batchItem in currentBatch)
//                            {
//                                if (IsStopped)
//                                    return;
//                                try
//                                {
//                                    batchItem.IssueBill();
//                                }
//                                catch (Exception ex)
//                                {
//                                    LogBillIssueException(ex, "Error issuing bill for client: id=" + batchItem.Subscription.ID);
//                                }
//                            }
//                            // save to database
//                            db.SaveChanges();
//                            minClientID = currentBatch.Max(client => client.Subscription.ID) + 1;
//                            processedCount += batchSize;
//                        }
//                    }

//                    _lastSuccessfulBillIssue = DateTime.Now;
//                    logger.Trace("Bills Issued.");
//                }
//                // issue e-bills
//                var ebillDefaults = new EBillDefaults();
//                if (ebillDefaults.IsActive)
//                {
//                    // update e-bill companies
//                    logger.Trace("Updating e-bill companies...");
//                    try
//                    {
//                        Adapter.UpdateEBillCompanies();
//                        logger.Trace("E-bill companies updated.");
//                    }
//                    catch (Exception ex)
//                    {
//                        EBillIssueLogger.Error(ex, "Error updating e-bill companies");
//                    }
//                    // issue e-bills
//                    logger.Trace("Issuing e-bills...");
//                    IssueEBills();
//                }
//                else
//                {
//                    logger.Trace("Skipped e-bills due to settings...");
//                }
//            }
//            catch (Exception ex)
//            {
//                LogBillIssueException(ex);
//            }
//            finally
//            {
//                _isBillingRunning = false;
//            }
//        }

//        private static IQueryable<Subscription> GetValidClientsForBilling(this IQueryable<Subscription> clients)
//        {
//            return clients.Where(client => (client.State == (short)CustomerState.Active || client.State == (short)CustomerState.Reserved) && client.ActivationDate.HasValue && client.Service.BillingType != (short)ServiceBillingType.PrePaid);
//        }

//        private static void LogBillIssueException(Exception ex, string message = "Error issuing bills!")
//        {
//            BillIssueLogger.Error(ex, message);
//        }
//    }
//}
