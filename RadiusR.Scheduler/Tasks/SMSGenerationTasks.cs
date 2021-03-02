using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using NLog;
using RadiusR.DB;
using RadiusR.DB.Enums;
using System.Data.Entity;
using RadiusR.Scheduler.SMS;

namespace RadiusR.Scheduler.Tasks
{
    public class SMSGenerationTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("sms-generation-tasks");
        private const int batchSize = 1000;

        public override bool Run()
        {
            var wasSuccessful = false;
            // bill payment reminders
            wasSuccessful = ScheduleBillReminderSMSes();
            // prepaid expiration reminders
            wasSuccessful = wasSuccessful && SchedulePrepaidReminderSMSes();

            if (wasSuccessful)
            {
                logger.Trace("Scheduled SMS creator done.");
            }

            return wasSuccessful;
        }

        private bool ScheduleBillReminderSMSes()
        {
            try
            {
                // set for first batch
                long minBillId = 0;
                var today = DateTime.Today;
                var reminderThreshold = today.AddDays(SchedulerSettings.SMSSchedulerPaymentReminderThreshold);

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
                        var baseQuery = db.Bills
                            .OrderBy(b => b.ID)
                            .Where(b => b.ID > minBillId)
                            .Where(b => b.PaymentTypeID == (short)PaymentType.None)
                            .Where(b => !b.ScheduledSMSes.Any(ss => ss.SMSType == (short)SMSType.PaymentReminder && ss.CreationDate >= DbFunctions.AddDays(b.DueDate, -SchedulerSettings.SMSSchedulerPaymentReminderThreshold)))
                            .Where(b => b.DueDate <= reminderThreshold && b.DueDate > today)
                            // skip automatic payments
                            .Where(b => b.Subscription.MobilExpressAutoPayment == null && b.Subscription.RecurringPaymentSubscription == null)
                            .Include(b => b.Subscription.ScheduledSMSes);
                        // get the current batch
                        var currentBatch = baseQuery.Take(batchSize).ToArray();
                        // finish if nothing left
                        if (currentBatch.Count() == 0)
                            break;
                        // set for next batch
                        minBillId = currentBatch.Max(b => b.ID);
                        // changes in batch
                        var toRemove = new List<ScheduledSMS>();
                        var toAdd = new List<ScheduledSMS>();
                        // iterate
                        foreach (var bill in currentBatch)
                        {
                            // abort by flag
                            if (_isAborted)
                            {
                                logger.Debug("Aborted by the scheduler.");
                                return false;
                            }
                            // add scheduled SMS
                            var results = bill.AddPaymentReminderSMS();
                            if (results.ToRemove != null)
                                toRemove.Add(results.ToRemove);
                            if (results.ToAdd != null)
                                toAdd.Add(results.ToAdd);
                        }
                        if (toRemove.Any())
                            db.ScheduledSMS.RemoveRange(toRemove);
                        if (toAdd.Any())
                            db.ScheduledSMS.AddRange(toAdd);
                        db.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Error creating scheduled SMSes.");
                return false;
            }
        }

        private bool SchedulePrepaidReminderSMSes()
        {
            try
            {
                // set for first batch
                long minSubId = 0;
                var today = DateTime.Today;

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
                        var baseQuery = db.Subscriptions
                            .Where(s => s.ID > minSubId)
                            .Where(s => s.Service.BillingType == (short)ServiceBillingType.PrePaid)
                            .Where(s => !s.ScheduledSMSes.Any(ss => ss.SMSType == (short)SMSType.PrePaidExpiration && ss.CreationDate >= DbFunctions.AddDays(s.LastAllowedDate, -1 * SchedulerSettings.SMSSchedulerPrepaidReminderThreshold)))
                            .Where(s => s.LastAllowedDate > today && DbFunctions.AddDays(s.LastAllowedDate, -1 * SchedulerSettings.SMSSchedulerPrepaidReminderThreshold) <= today)
                            .Include(s => s.ScheduledSMSes);
                        // get the current batch
                        var currentBatch = baseQuery.Take(batchSize).ToArray();
                        // finish if nothing left
                        if (currentBatch.Count() == 0)
                            break;
                        // set for next batch
                        minSubId = currentBatch.Max(s => s.ID);
                        // changes in batch
                        var toRemove = new List<ScheduledSMS>();
                        var toAdd = new List<ScheduledSMS>();
                        // iterate
                        foreach (var subscription in currentBatch)
                        {
                            // abort by flag
                            if (_isAborted)
                            {
                                logger.Debug("Aborted by the scheduler.");
                                return false;
                            }
                            // add scheduled SMS
                            var results = subscription.AddPrepaidReminderSMS();
                            if (results.ToRemove != null)
                                toRemove.Add(results.ToRemove);
                            if (results.ToAdd != null)
                                toAdd.Add(results.ToAdd);
                        }

                        if (toRemove.Any())
                            db.ScheduledSMS.RemoveRange(toRemove);
                        if (toAdd.Any())
                            db.ScheduledSMS.AddRange(toAdd);
                        db.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Error creating scheduled SMSes.");
                return false;
            }
        }
    }
}
