using RadiusR.Scheduler;
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
        public SchedulerService()
        {
            InitializeComponent();
            CanPauseAndContinue = false;
            CanShutdown = false;
        }

        protected override void OnStart(string[] args)
        {
            Scheduler.Start();
        }

        protected override void OnStop()
        {
            Scheduler.Stop();
        }
    }
}
