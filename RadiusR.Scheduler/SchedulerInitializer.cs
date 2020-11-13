using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using RezaB.Scheduling.StartParameters;
using RadiusR.Scheduler.Tasks;
using RadiusR.DB;

namespace RadiusR.Scheduler
{
    public static class SchedulerInitializer
    {
        public static RezaB.Scheduling.Scheduler GetScheduler(TimeSpan checkIntervals, string name)
        {
            return new RezaB.Scheduling.Scheduler(new SchedulerOperation[]
            {
                new SchedulerOperation("change-state", new ChangeStateTasks(), new SchedulerTimingOptions(new SchedulerWorkingTimeSpan(SchedulerSettings.SchedulerStartTime, SchedulerSettings.SchedulerStopTime)), 2, new RezaB.Scheduling.SchedulerTask[]
                {
                    new RezaB.Scheduling.SchedulerTask("change-tarif", new ChangeTariffTasks(), 2, new RezaB.Scheduling.SchedulerTask[]
                    {
                        new RezaB.Scheduling.SchedulerTask("issue-bills", new IssueBillTasks(), 3, new RezaB.Scheduling.SchedulerTask[]
                        {
                            new RezaB.Scheduling.SchedulerTask("issue-ebills", new IssueEBillTasks(), 2),
                            new RezaB.Scheduling.SchedulerTask("automatic-payments", new AutomaticPaymentTasks(), 0, new RezaB.Scheduling.SchedulerTask[]
                            {
                                new RezaB.Scheduling.SchedulerTask("sms-generation", new SMSGenerationTasks(), 2)
                            })
                        })
                    }, true)
                }, true),
                new SchedulerOperation("scheduled-smses", new ScheduledSMSTasks(), new SchedulerTimingOptions(new SchedulerWorkingTimeSpan(SchedulerSettings.SMSSchedulerStartTime, SchedulerSettings.SMSSchedulerStopTime)), 0)
            }
            , checkIntervals, name);
        }
    }
}
