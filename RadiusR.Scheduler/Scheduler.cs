//using NLog;
//using RadiusR.DB;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace RadiusR.Scheduler
//{
//    public static partial class Scheduler
//    {
//        private static Logger logger = LogManager.GetLogger("scheduler");

//        //private static bool _isBillSMSRunning = false;
//        private static DateTime _lastSuccessfulScheduledSMSRun = DateTime.MinValue;
//        private static DateTime _lastSuccessfulSMSSendingRun = DateTime.MinValue;

//        private static Thread mainThread = null;
//        private static bool IsStopped = false;
//        private static TimeSpan _checkPeriod = TimeSpan.FromSeconds(1);

//        public static void Start()
//        {
//            mainThread = new Thread(new ThreadStart(Main));
//            mainThread.Start();
//        }

//        public static void Stop()
//        {
//            IsStopped = true;
//            mainThread.Join();
//        }

//        private static void Main()
//        {
//            while (!IsStopped)
//            {
//                try
//                {
//                    var now = DateTime.Now;
//                    // we are in action period
//                    if (now.TimeOfDay >= SchedulerSettings.SchedulerStartTime && now.TimeOfDay < SchedulerSettings.SchedulerStopTime)
//                    {
//                        // if another bill thread is not running and we did not have a successfull bill operation
//                        if (!_isBillingRunning && now.Date > _lastSuccessfulBillIssue.Date)
//                        {
//                            // issue bills
//                            Thread billsThread = new Thread(new ThreadStart(IssueBills));
//                            _isBillingRunning = true;
//                            logger.Trace("Issuing bills...");
//                            billsThread.Start();
//                        }
//                        // if billing is finished
//                        if (!_isBillingRunning && _lastSuccessfulBillIssue.Date <= DateTime.Today)
//                        {
//                            // run the scheduled tasks
//                            if (!_isScheduledTasksRunning && _lastSuccessfulBillIssue.Date >= now.Date && _lastSuccessfulScheduledTaskRun.Date < now.Date)
//                            {
//                                Thread scheduledTasksThread = new Thread(new ThreadStart(RunScheduledTasks));
//                                _isScheduledTasksRunning = true;
//                                logger.Trace("Running scheduled tasks...");
//                                scheduledTasksThread.Start();
//                            }
//                            // run automatic payments
//                            if (MobilExpressSettings.MobilExpressIsActive && !_isAutomaticPaymentRunning && _lastSuccessfulAutomaticPayment.Date < now.Date)
//                            {
//                                Thread automaticPaymentsThread = new Thread(new ThreadStart(AutomaticPayments));
//                                _isAutomaticPaymentRunning = true;
//                                logger.Trace("Running automatic payments...");
//                                automaticPaymentsThread.Start();
//                            }
//                            //// if automatic payments finished
//                            if (!MobilExpressSettings.MobilExpressIsActive || (!_isAutomaticPaymentRunning && _lastSuccessfulAutomaticPayment.Date <= now.Date))
//                            {
//                                // run scheduling SMSes
//                                if (!_isSMSSchedulerRunning && _lastSuccessfulScheduledSMSRun.Date < DateTime.Today)
//                                {
//                                    Thread scheduledSMSCreatorThread = new Thread(new ThreadStart(ScheduleSMSes));
//                                    _isSMSSchedulerRunning = true;
//                                    logger.Trace("Running scheduled SMS creator...");
//                                    scheduledSMSCreatorThread.Start();
//                                }
//                            }
//                        }

//                    }
//                    // run scheduled SMS sending
//                    if (now.TimeOfDay >= SchedulerSettings.SMSSchedulerStartTime && now.TimeOfDay < SchedulerSettings.SMSSchedulerStopTime)
//                    {
//                        if (!_isScheduledSMSSenderRunning && _lastSuccessfulScheduledSMSSending.Date < now.Date)
//                        {
//                            Thread scheduledSMSSenderThread = new Thread(new ThreadStart(SendSMSes));
//                            _isScheduledSMSSenderRunning = true;
//                            logger.Trace("Running scheduled SMS sender...");
//                            scheduledSMSSenderThread.Start();
//                        }
//                    }
//                    // wait for next pass
//                    var lastLoopTime = DateTime.Now;
//                    do
//                    {
//                        Thread.Sleep(_checkPeriod);
//                    } while (!IsStopped && lastLoopTime.Add(SchedulerSettings.SchedulerRetryDelay) > DateTime.Now);
//                }
//                catch (Exception ex)
//                {
//                    logger.Error(ex, "Error in Scheduler!");
//                    Thread.Sleep(TimeSpan.FromMinutes(1));
//                }
//            }
//        }
//    }
//}
