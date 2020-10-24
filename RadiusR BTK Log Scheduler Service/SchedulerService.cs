using RadiusR.BTKLogging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_BTK_Log_Scheduler_Service
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
            BTKLogScheduler.Start();
        }

        protected override void OnStop()
        {
            BTKLogScheduler.Stop();
        }
    }
}
