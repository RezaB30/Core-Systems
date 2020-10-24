using RadiusR.DB;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.BTKLogging
{
    public class SchedulerSettings
    {
        public BTKLogTypes LogType { get; set; }
        public TimeSpan SchedulerStartTime { get; set; }
        public TimeSpan SchedulerActiveTime { get; set; }
        public SchedulerWorkPeriods SchedulerWorkPeriod { get; set; }
        public string SchedulerStartDay { get; set; }
        public bool PartitionFiles { get; set; }
        public string FTPFolder { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastOperationTime { get; set; }
        public DateTime CurrentOperationTime { get; private set; }
        public DateTime? NextOperationTime { get; private set; }

        public SchedulerSettings(BTKSchedulerSetting dbSetting)
        {
            LogType = (BTKLogTypes)dbSetting.LogType;
            SchedulerStartTime = new TimeSpan(dbSetting.SchedulerStartTime);
            SchedulerActiveTime = new TimeSpan(dbSetting.SchedulerActiveTime);
            SchedulerWorkPeriod = (SchedulerWorkPeriods)dbSetting.SchedulerWorkPeriod;
            SchedulerStartDay = dbSetting.SchedulerStartDay;
            PartitionFiles = dbSetting.PartitionFiles;
            FTPFolder = dbSetting.FTPFolder;
            FTPUsername = dbSetting.FTPUsername;
            FTPPassword = dbSetting.FTPPassword;
            IsActive = dbSetting.IsActive;
            LastOperationTime = dbSetting.LastOperationTime ?? DateTime.Now.AddMonths(-1);

            CurrentOperationTime = DateTime.Now;
            NextOperationTime = GetNextDate();
        }

        public bool IsActTime()
        {
            if (!IsActive)
                return false;

            return NextOperationTime.HasValue;

            var checkTime = LastOperationTime;
            switch (SchedulerWorkPeriod)
            {
                case SchedulerWorkPeriods.Hourly:
                    return DateTime.Now.Day > checkTime.Day || DateTime.Now.Hour > checkTime.Hour;

                case SchedulerWorkPeriods.Daily:
                    return DateTime.Now.Day > checkTime.Day && DateTime.Now.TimeOfDay > SchedulerStartTime;

                case SchedulerWorkPeriods.Weekly:
                    return DateTime.Now.Day > checkTime.Day && StartDays.Contains((int)DateTime.Now.DayOfWeek + 1) && DateTime.Now.TimeOfDay > SchedulerStartTime;

                case SchedulerWorkPeriods.Monthly:
                    return DateTime.Now.Day > checkTime.Day && StartDays.Contains(DateTime.Now.Day) && DateTime.Now.TimeOfDay > SchedulerStartTime;

                default:
                    return false;
            }
        }

        public IEnumerable<int> StartDays
        {
            get
            {
                return SchedulerStartDay.Split(',').Select(s => int.Parse(s));
            }
        }

        private DateTime? GetNextDate()
        {
            switch (SchedulerWorkPeriod)
            {
                case SchedulerWorkPeriods.Hourly:
                    {
                        var tempTime = LastOperationTime.AddHours(1);
                        var nextOperationTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day, tempTime.Hour, 0, 0);
                        return (nextOperationTime < CurrentOperationTime) ? nextOperationTime : (DateTime?)null;
                    }
                case SchedulerWorkPeriods.Daily:
                    {
                        var tempTime = LastOperationTime.AddDays(1);
                        var nextOperationTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day, 0, 0, 0);
                        return (nextOperationTime < CurrentOperationTime) ? nextOperationTime : (DateTime?)null;
                    }
                case SchedulerWorkPeriods.Weekly:
                    {
                        if (StartDays.Count() < 1)
                            return null;
                        var tempTime = LastOperationTime.AddDays(1);
                        while (true)
                        {
                            if (StartDays.Contains((int)tempTime.DayOfWeek))
                            {
                                var nextOperationTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day, 0, 0, 0);
                                return nextOperationTime < CurrentOperationTime ? nextOperationTime : (DateTime?)null;
                            }
                            tempTime = tempTime.AddDays(1);
                        }
                    }
                case SchedulerWorkPeriods.Monthly:
                    {
                        if (StartDays.Count() < 1)
                            return null;
                        var tempTime = LastOperationTime.AddDays(1);
                        while (true)
                        {
                            if (StartDays.Contains(tempTime.Day))
                            {
                                var nextOperationTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day, 0, 0, 0);
                                return nextOperationTime < CurrentOperationTime ? nextOperationTime : (DateTime?)null;
                            }
                            tempTime = tempTime.AddDays(1);
                        }
                    }
                default:
                    return null;
            }
        }
    }
}
