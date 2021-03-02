using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using NLog;
using RadiusR.SMS;
using RadiusR.DB;
using System.Data.Entity;
using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.ModelExtentions;
using System.Data.SqlClient;

namespace RadiusR.Scheduler.Tasks
{
    public class ScheduledSMSTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("scheduled-sms-tasks");
        private const int batchSize = 250;

        public override bool Run()
        {
            try
            {
                // sms client
                var smsClient = new SMSService();
                var today = DateTime.Today;
                // set for first batch
                long minID = 0;
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
                        db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                        // base query
                        var baseQuery = db.ScheduledSMS
                            .PrepareForSMS()
                            //.Include(ss => ss.Bill.Service)
                            .Include(ss => ss.Bill.BillFees.Select(bf => bf.Discount))
                            .OrderBy(ss => ss.ID)
                            .Where(ss => !ss.SendTime.HasValue && (!ss.ExpirationDate.HasValue || ss.ExpirationDate > today))
                            .Where(ss => ss.ID > minID);
                        // get the batch
                        var currentBatch = baseQuery.Take(batchSize).ToArray();
                        // finish if nothing left
                        if (currentBatch.Count() == 0)
                            break;
                        // set for the next batch
                        minID = currentBatch.Max(ss => ss.ID);
                        // for fast saving save in batch
                        var sentSMSArchives = new List<SMSArchive>();
                        var sentBatchIDs = new List<long>();
                        // iterate scheduled SMSes
                        foreach (var scheduledSMS in currentBatch)
                        {
                            // abort flag check
                            if (_isAborted)
                            {
                                logger.Info("Abort flag detected...Preparing to wrap up.");
                                break;
                            }
                            try
                            {
                                Dictionary<string, object> extraParameters;
                                switch ((SMSType)scheduledSMS.SMSType)
                                {
                                    case SMSType.NewBill:
                                        extraParameters = new Dictionary<string, object>
                                    {
                                        { SMSParamaterRepository.SMSParameterNameCollection.BillIssueDate, scheduledSMS.Bill.IssueDate },
                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() },
                                        { SMSParamaterRepository.SMSParameterNameCollection.LastPaymentDay, scheduledSMS.Bill.DueDate }
                                    };
                                        break;
                                    case SMSType.PaymentReminder:
                                        extraParameters = new Dictionary<string, object>()
                                    {
                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() },
                                        { SMSParamaterRepository.SMSParameterNameCollection.LastPaymentDay, scheduledSMS.Bill.DueDate }
                                    };
                                        break;
                                    case SMSType.PaymentDone:
                                        extraParameters = new Dictionary<string, object>()
                                    {
                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() }
                                    };
                                        break;
                                    case SMSType.PrePaidExpiration:
                                        extraParameters = null;
                                        break;
                                    case SMSType.FailedAutomaticPayment:
                                        extraParameters = new Dictionary<string, object>()
                                    {
                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() },
                                        { SMSParamaterRepository.SMSParameterNameCollection.BillIssueDate, scheduledSMS.Bill.IssueDate },
                                        { SMSParamaterRepository.SMSParameterNameCollection.ErrorMessage, scheduledSMS.CalculatedParameters }
                                    };
                                        break;
                                    default:
                                        continue;
                                }

                                sentSMSArchives.Add(smsClient.SendSubscriberSMS(scheduledSMS.Subscription, (SMSType)scheduledSMS.SMSType, extraParameters));
                                sentBatchIDs.Add(scheduledSMS.ID);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, $"Error sending scheduled SMS with id [{scheduledSMS.ID}]");
                            }
                        }

                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                // this is here to notify of any changes in the table in database---------------------
                                var toPreventChangesGoUnnoticed = new object[] { new ScheduledSMS(), new ScheduledSMS().SendTime, new ScheduledSMS().ID };
                                // CHANGE THIS ON ERROR---------------------------------------------------------------
                                var sqlCommand = string.Format(@"UPDATE {0} SET {1} = @now WHERE {2} in ({3});", "ScheduledSMS", "SendTime", "ID", string.Join(",", sentBatchIDs.ToArray()));
                                // -----------------------------------------------------------------------------------
                                var now = DateTime.Now;
                                db.Database.ExecuteSqlCommand(sqlCommand, new[] { new SqlParameter("@now", now) });

                                db.SMSArchives.AddRangeSafely(sentSMSArchives);

                                db.SaveChanges();
                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                logger.Info("Scheduled SMS sending done!");
                try
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.ScheduledSMS.RemoveRange(db.ScheduledSMS.Where(ss => (ss.SMSType == (short)SMSType.FailedAutomaticPayment) && (ss.SendTime.HasValue || ss.ExpirationDate <= DateTime.Today)));
                        db.SaveChanges();
                        logger.Info("Removed expired & sent scheduled SMSes.");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error removing expired & sent SMSes.");
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Error sending scheduled SMSes.");
                return false;
            }
        }
    }
}
