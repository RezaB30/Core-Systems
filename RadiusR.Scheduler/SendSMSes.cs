//using NLog;
//using RadiusR.DB;
//using RadiusR.DB.Enums;
//using RadiusR.SMS;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.Entity;
//using RadiusR.DB.Utilities.Billing;
//using RadiusR.Scheduler.SMS;
//using System.Data.SqlClient;
//using System.Linq.Expressions;
//using RadiusR.DB.ModelExtentions;

//namespace RadiusR.Scheduler
//{
//    public static partial class Scheduler
//    {
//        private static bool _isScheduledSMSSenderRunning = false;
//        private static DateTime _lastSuccessfulScheduledSMSSending = DateTime.MinValue;

//        private static Logger scheduledSMSLogger = LogManager.GetLogger("scheduled_sms");

//        public static void SendSMSes()
//        {
//            try
//            {
//                // sms client
//                var smsClient = new SMSService();
//                var today = DateTime.Today;
//                // set for first batch
//                long minID = 0;
//                while (true)
//                {
//                    using (RadiusREntities db = new RadiusREntities())
//                    {
//                        db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
//                        // base query
//                        var baseQuery = db.ScheduledSMS
//                            .PrepareForSMS()
//                            //.Include(ss => ss.Bill.Service)
//                            .Include(ss => ss.Bill.BillFees.Select(bf => bf.Discount))
//                            .OrderBy(ss => ss.ID)
//                            .Where(ss => !ss.SendTime.HasValue && (!ss.ExpirationDate.HasValue || ss.ExpirationDate > today))
//                            .Where(ss => ss.ID > minID);
//                        // get the batch
//                        var currentBatch = baseQuery.Take(batchSize).ToArray();
//                        // finish if nothing left
//                        if (currentBatch.Count() == 0)
//                            break;
//                        // set for the next batch
//                        minID = currentBatch.Max(ss => ss.ID);
//                        // for fast saving save in batch
//                        var sentSMSArchives = new List<SMSArchive>();
//                        // iterate scheduled SMSes
//                        foreach (var scheduledSMS in currentBatch)
//                        {
//                            try
//                            {
//                                Dictionary<string, object> extraParameters;
//                                switch ((SMSType)scheduledSMS.SMSType)
//                                {
//                                    case SMSType.NewBill:
//                                        extraParameters = new Dictionary<string, object>
//                                    {
//                                        { SMSParamaterRepository.SMSParameterNameCollection.BillIssueDate, scheduledSMS.Bill.IssueDate },
//                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() },
//                                        { SMSParamaterRepository.SMSParameterNameCollection.LastPaymentDay, scheduledSMS.Bill.DueDate }
//                                    };
//                                        break;
//                                    case SMSType.PaymentReminder:
//                                        extraParameters = new Dictionary<string, object>()
//                                    {
//                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() },
//                                        { SMSParamaterRepository.SMSParameterNameCollection.LastPaymentDay, scheduledSMS.Bill.DueDate }
//                                    };
//                                        break;
//                                    case SMSType.PaymentDone:
//                                        extraParameters = new Dictionary<string, object>()
//                                    {
//                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() }
//                                    };
//                                        break;
//                                    case SMSType.PrePaidExpiration:
//                                        extraParameters = null;
//                                        break;
//                                    case SMSType.FailedAutomaticPayment:
//                                        extraParameters = new Dictionary<string, object>()
//                                    {
//                                        { SMSParamaterRepository.SMSParameterNameCollection.BillTotal, scheduledSMS.Bill.GetPayableCost() },
//                                        { SMSParamaterRepository.SMSParameterNameCollection.BillIssueDate, scheduledSMS.Bill.IssueDate },
//                                        { SMSParamaterRepository.SMSParameterNameCollection.ErrorMessage, scheduledSMS.CalculatedParameters }
//                                    };
//                                        break;
//                                    default:
//                                        continue;
//                                }

//                                sentSMSArchives.Add(smsClient.SendSubscriberSMS(scheduledSMS.Subscription, (SMSType)scheduledSMS.SMSType, extraParameters));
                                
//                            }
//                            catch (Exception ex)
//                            {
//                                scheduledSMSLogger.Error(ex, "Error sending scheduled SMS with id [{0}]", scheduledSMS.ID);
//                            }
//                        }

//                        using (var transaction = db.Database.BeginTransaction())
//                        {
//                            try
//                            {
//                                // this is here to notify of any changes in the table in database---------------------
//                                var toPreventChangesGoUnnoticed = new object[] { new ScheduledSMS(), new ScheduledSMS().SendTime, new ScheduledSMS().ID };
//                                // CHANGE THIS ON ERROR---------------------------------------------------------------
//                                var sqlCommand = string.Format(@"UPDATE {0} SET {1} = @now WHERE {2} in ({3});", "ScheduledSMS", "SendTime", "ID", string.Join(",", currentBatch.Select(ss => ss.ID).ToArray()));
//                                // -----------------------------------------------------------------------------------
//                                var now = DateTime.Now;
//                                db.Database.ExecuteSqlCommand(sqlCommand, new[] { new SqlParameter("@now", now)});
                                
//                                db.SMSArchives.AddRangeSafely(sentSMSArchives);

//                                db.SaveChanges();
//                                transaction.Commit();
//                            }
//                            catch
//                            {
//                                transaction.Rollback();
//                                throw;
//                            }
//                        }
//                    }
//                }
//                _lastSuccessfulScheduledSMSSending = DateTime.Now;
//                logger.Trace("Scheduled SMS sending done!");
//                try
//                {
//                    using (RadiusREntities db = new RadiusREntities())
//                    {
//                        db.ScheduledSMS.RemoveRange(db.ScheduledSMS.Where(ss => (ss.SMSType == (short)SMSType.FailedAutomaticPayment) && (ss.SendTime.HasValue || ss.ExpirationDate <= DateTime.Today)));
//                        db.SaveChanges();
//                        logger.Trace("Removed expired & sent scheduled SMSes.");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    scheduledSMSLogger.Error(ex, "Error removing expired & sent SMSes.");
//                }
//            }
//            catch (Exception ex)
//            {
//                scheduledSMSLogger.Fatal(ex, "Error sending scheduled SMSes.");
//            }
//            finally
//            {
//                _isScheduledSMSSenderRunning = false;
//            }
//        }
//    }
//}
