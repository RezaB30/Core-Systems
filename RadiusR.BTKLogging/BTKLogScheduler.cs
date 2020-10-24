using NLog;
using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.BTKLogging
{
    public static class BTKLogScheduler
    {
        private static bool IsStopped = false;
        private static Thread runThread = null;
        private static TimeSpan _checkPeriod = TimeSpan.FromSeconds(1);
        private static Logger logger = LogManager.GetLogger("BTK Log Scheduler");
        public static void Start()
        {
            IsStopped = false;
            runThread = new Thread(new ThreadStart(Run));
            runThread.IsBackground = true;
            runThread.Start();
        }

        public static void Stop()
        {
            IsStopped = true;
            if (runThread != null)
                runThread.Join();
        }

        private static void Run()
        {
            while (!IsStopped)
            {
                try
                {
                    var settings = SettingsCache.Get();
                    for (int i = 0; i < settings.Length; i++)
                    {
                        if (settings[i].IsActTime())
                        {
                            logger.Trace("Started logging for {0}.", settings[i].LogType.ToString());

                            while (settings[i].NextOperationTime.HasValue)
                            {
                                try
                                {
                                    BTKLogManager.CreateLogs(settings[i]);
                                    using (RadiusREntities db = new RadiusREntities())
                                    {
                                        var dbSettings = db.BTKSchedulerSettings.Find((short)settings[i].LogType);
                                        dbSettings.LastOperationTime = settings[i].NextOperationTime;
                                        db.SaveChanges();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex, "Error creating {0} logs.", settings[i].LogType.ToString());
                                    break;
                                }
                                SettingsCache.Load();
                                settings = SettingsCache.Get();
                            }

                            logger.Trace("{0} logs done.", settings[i].LogType.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex, "Error in main thread.");
                }

                Thread.Sleep(_checkPeriod);
            }

        }
    }
}
