using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using NLog;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.Billing;

namespace RadiusR.Scheduler.Tasks
{
    public class IssueBillTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("issue-bill-tasks");
        private const int batchSize = 1000;

        public override bool Run()
        {
            try
            {
                logger.Info("Issue bill tasks started.");
                var minClientID = (long)0;
                var processedCount = 0;
                while (true)
                {
                    // abort by flag
                    if (_isAborted)
                    {
                        logger.Debug("Aborted by the scheduler.");
                        return false;
                    }

                    using (RadiusREntities db = new RadiusREntities())
                    {
                        // ---------------- ID Batching -------------------
                        var baseQuery = GetValidClientsForBilling(db.Subscriptions
                            .OrderBy(subscription => subscription.ID))
                            .Where(client => client.ID >= minClientID);
                        var maxClientID = baseQuery.Select(s => s.ID).Take(batchSize).DefaultIfEmpty(0).Max();
                        if (maxClientID == 0)
                            break;

                        var currentBatch = db.PrepareForBilling(baseQuery.Where(client => client.ID <= maxClientID)).ToArray();
                        // ------------------------------------------------

                        // for each item in batch
                        foreach (var batchItem in currentBatch)
                        {
                            // abort by flag
                            if (_isAborted)
                            {
                                logger.Debug("Aborted by the scheduler.");
                                return false;
                            }
                            // issue subscription bills
                            try
                            {
                                batchItem.IssueBill();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, $"Error issuing bill for client: id={batchItem.Subscription.ID}");
                            }
                        }
                        // save to database
                        db.SaveChanges();
                        minClientID = currentBatch.Max(client => client.Subscription.ID) + 1;
                        processedCount += batchSize;
                    }
                }

                logger.Info("Bills Issued.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Error running issue bill tasks.");
                return false;
            }
        }

        private IQueryable<Subscription> GetValidClientsForBilling(IQueryable<Subscription> subscriptions)
        {
            return subscriptions.Where(subscription => (subscription.State == (short)CustomerState.Active || subscription.State == (short)CustomerState.Reserved) && subscription.ActivationDate.HasValue && subscription.Service.BillingType != (short)ServiceBillingType.PrePaid);
        }
    }
}
