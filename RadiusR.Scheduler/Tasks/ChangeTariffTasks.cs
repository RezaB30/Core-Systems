using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using NLog;
using RadiusR.DB;
using System.Data.Entity;
using RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TariffChanges;

namespace RadiusR.Scheduler.Tasks
{
    class ChangeTariffTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("change-tariff-tasks");

        public override bool Run()
        {
            try
            {
                logger.Info("State change scheduler started.");
                using (RadiusREntities db = new RadiusREntities())
                {
                    var today = DateTime.Today;
                    var todaysTasks = db.ChangeServiceTypeTasks.Include(cst => cst.SchedulerTask).Where(cst => cst.SchedulerTask.ExecuteDate <= today).ToArray();
                    foreach (var task in todaysTasks)
                    {
                        // check abort
                        if (_isAborted)
                        {
                            logger.Debug("Aborted by scheduler.");
                            return false;
                        }
                        try
                        {
                            // change subscription tariff
                            var results = db.ChangeSubscriptionTariff(new TariffChangeOptions()
                            {
                                ForceNow = false,
                                NewBillingPeriod = task.NewBillingPeriod,
                                SubscriptionID = task.SubscriptionID,
                                TariffID = task.NewServiceID,
                                Gateway = new GatewayOptions()
                                {
                                    InterfaceType = DB.Enums.SystemLogInterface.RadiusRScheduler,
                                    InterfaceUsername = null,
                                    UserID = null
                                }
                            });
                            if (results != TariffChangeResult.TariffChanged)
                            {
                                logger.Warn($"Change tariff task for subscription id: {task.SubscriptionID} returned with code: {results:G}");
                            }
                            else
                            {
                                // remove the task
                                db.SchedulerTasks.Remove(task.SchedulerTask);
                                db.ChangeServiceTypeTasks.Remove(task);
                                db.SaveChanges();
                            }
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
