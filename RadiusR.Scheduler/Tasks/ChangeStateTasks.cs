using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.Billing;
using System.Data.Entity;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges;

namespace RadiusR.Scheduler.Tasks
{
    public class ChangeStateTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("change-state-tasks");

        public override bool Run()
        {
            try
            {
                logger.Info("State change scheduler started.");
                using (RadiusREntities db = new RadiusREntities())
                {
                    var today = DateTime.Today;
                    var todaysTasks = db.ChangeStateTasks.Include(cst => cst.SchedulerTask).Where(cst => cst.SchedulerTask.ExecuteDate <= today).ToArray();
                    foreach (var task in todaysTasks)
                    {
                        try
                        {
                            // check abort
                            if (_isAborted)
                            {
                                logger.Debug("Aborted by scheduler.");
                                return false;
                            }
                            // check for each state change type
                            switch ((CustomerState)task.NewState)
                            {
                                case CustomerState.Active:
                                    StateChangeUtilities.ChangeSubscriptionState(task.SubscriptionID, new ActivateSubscriptionOptions()
                                    {
                                        AppUserID = null,
                                        ForceUnfreeze = false,
                                        LogInterface = SystemLogInterface.RadiusRScheduler,
                                        LogInterfaceUsername = null,
                                        ScheduleSMSes = true
                                    });
                                    break;
                                // invalid changes (cannot be done by the scheduler)
                                case CustomerState.Disabled:
                                case CustomerState.Cancelled:
                                case CustomerState.Registered:
                                case CustomerState.Reserved:
                                case CustomerState.PreRegisterd:
                                default:
                                    logger.Warn($"New state is invalid for subscription id: {task.SubscriptionID}, state code: {task.NewState}");
                                    break;
                            }
                            // remove the task
                            db.SchedulerTasks.Remove(task.SchedulerTask);
                            db.ChangeStateTasks.Remove(task);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            // log exception
                            logger.Error(ex, "Exception occured.");
                        }

                    }
                }
                logger.Trace("Scheduled change state tasks done.");
                return true;
            }
            catch (Exception ex)
            {
                // log exceptions
                logger.Fatal(ex, "Error running scheduled tasks.");
                return false;
            }
        }
    }
}
