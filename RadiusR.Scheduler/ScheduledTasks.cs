//using NLog;
//using RadiusR.DB;
//using RadiusR.DB.Enums;
//using RadiusR.DB.Utilities.Billing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RadiusR.Scheduler
//{
//    public partial class Scheduler
//    {
//        private static Logger scheduledTasksLogger = LogManager.GetLogger("scheduled_tasks");
//        private static DateTime _lastSuccessfulScheduledTaskRun = DateTime.MinValue;
//        private static bool _isScheduledTasksRunning = false;
//        public static void RunScheduledTasks()
//        {
//            try
//            {
//                using (RadiusREntities db = new RadiusREntities())
//                {
//                    //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
//                    var tasks = db.SchedulerTasks.Where(task => task.ExecuteDate <= DateTime.Now).ToList();
//                    foreach (var task in tasks)
//                    {
//                        if (IsStopped)
//                            return;
//                        var changeStateTask = task.ChangeStateTasks.FirstOrDefault();
//                        //var changeServiceTypeTask = task.ChangeServiceTypeTasks.FirstOrDefault();
//                        if (changeStateTask != null)
//                        {
//                            changeStateTask.Subscription.State = changeStateTask.NewState;
//                            if (changeStateTask.NewState == (short)CustomerState.Active)
//                            {
//                                changeStateTask.Subscription.ActivationDate = DateTime.Now;
//                                //var lastBill = changeStateTask.Subscription.Bills.OrderByDescending(bill => bill.IssueDate).FirstOrDefault();
//                                changeStateTask.Subscription.UpdateLastAllowedDate();
//                            }
//                            db.ChangeStateTasks.Remove(changeStateTask);
//                        }
//                        //if (changeServiceTypeTask != null)
//                        //{
//                        //    changeServiceTypeTask.Subscription.ServiceID = changeServiceTypeTask.NewServiceID;
//                        //    db.ChangeServiceTypeTasks.Remove(changeServiceTypeTask);
//                        //}
//                    }
//                    db.SchedulerTasks.RemoveRange(tasks);

//                    db.SaveChanges();
//                    _lastSuccessfulScheduledTaskRun = DateTime.Now;
//                    logger.Trace("Scheduled tasks done.");
//                }
//            }
//            catch (Exception ex)
//            {
//                // log exceptions
//                scheduledTasksLogger.Error(ex, "Error running scheduled tasks.");
//                return;
//            }
//            finally
//            {
//                _isScheduledTasksRunning = false;
//            }
//        }
//    }
//}
