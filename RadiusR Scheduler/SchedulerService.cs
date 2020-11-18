using RadiusR.Scheduler;
using RezaB.Scheduling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Scheduler
{
    public partial class SchedulerService : ServiceBase
    {
        Scheduler _scheduler;
        public SchedulerService()
        {
            InitializeComponent();
            CanPauseAndContinue = false;
            CanShutdown = false;
        }

        protected override void OnStart(string[] args)
        {
            _scheduler = RadiusR.Scheduler.SchedulerInitializer.GetScheduler(TimeSpan.FromMinutes(1), "radiusr-scheduler");
            _scheduler.Start();
        }

        protected override void OnStop()
        {
            _scheduler.Stop();
        }
    }
}
