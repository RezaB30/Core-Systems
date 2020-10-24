using RadiusR.DB;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Extentions
{
    /// <summary>
    /// Scheduled tasks functions.
    /// </summary>
    public static class ScheduledTasks
    {
        /// <summary>
        /// Adds a schedule task for updating client state at a specific date
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="date">Date to perform task</param>
        /// <param name="subscriptionId">Client identity</param>
        /// <param name="state">New state for client</param>
        public static void AddClientStateChange(this RadiusREntities entities, DateTime date, long subscriptionId, CustomerState state)
        {
                var schedulerTask = new SchedulerTask()
                {
                    ExecuteDate = date.Date
                };
                entities.ChangeStateTasks.Add(new ChangeStateTask()
                {
                    SubscriptionID = subscriptionId,
                    NewState = (short)state,
                    SchedulerTask = schedulerTask
                });
        }

        public static void AddClientServiceTypeChange(this RadiusREntities entities, DateTime date, long subscriptionId, int serviceId)
        {
            var schedulerTask = new SchedulerTask()
            {
                ExecuteDate = date.Date
            };
            entities.ChangeServiceTypeTasks.Add(new ChangeServiceTypeTask()
            {
                SubscriptionID = subscriptionId,
                NewServiceID = serviceId,
                SchedulerTask = schedulerTask
            });
        }
    }
}
