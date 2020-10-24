using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Scheduler
{
    public static class ScheduledTask
    {
        /// <summary>
        /// Removes change state scheduled tasks for a customer.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="clientId">ID of a customer</param>
        public static void RemoveChangeStateTask(this RadiusREntities entities, long clientId)
        {
            var changeStateTasks = entities.SchedulerTasks.Where(task => task.ChangeStateTasks.Any(stateTask => stateTask.SubscriptionID == clientId)).ToList();
            foreach (var task in changeStateTasks)
            {
                entities.ChangeStateTasks.RemoveRange(task.ChangeStateTasks);
            }
            entities.SchedulerTasks.RemoveRange(changeStateTasks);
        }
    }
}
