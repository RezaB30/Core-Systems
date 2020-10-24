using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "BTK Logs")]
    public class BTKLogsController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
 
        // GET: BTKLogs/IPBlocks
        public ActionResult IPBlocks(int? page)
        {
            var results = db.BTKIPBlocks.OrderByDescending(block => block.ID).Select(block => new BTKIPBlockViewModel()
            {
                AllocationDate = block.AllocationDate,
                BlockType = block.BlockType,
                DeallocationDate = block.DeallocationDate,
                EndingIP = block.EndingIP,
                ID = block.ID,
                OperatorName = block.OperatorName,
                ServiceType = block.ServiceType,
                StartingIP = block.StartingIP,
                UseLocation = block.UseLocation,
                UsesNAT = block.UsesNAT
            });

            SetupPages(page, ref results);

            return View(results);
        }

        [HttpGet]
        // GET: BTKLogs/AddIPBlock
        public ActionResult AddIPBlock()
        {
            return View(new BTKIPBlockViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BTKLogs/AddIPBlock
        public ActionResult AddIPBlock(BTKIPBlockViewModel IPBlock)
        {
            if (ModelState.IsValid)
            {
                db.BTKIPBlocks.Add(new BTKIPBlock()
                {
                    AllocationDate = IPBlock.AllocationDate.Value,
                    BlockType = IPBlock.BlockType.Value,
                    DeallocationDate = IPBlock.DeallocationDate,
                    EndingIP = IPBlock.EndingIP,
                    OperatorName = IPBlock.OperatorName,
                    ServiceType = IPBlock.ServiceType.Value,
                    StartingIP = IPBlock.StartingIP,
                    UseLocation = IPBlock.UseLocation,
                    UsesNAT = IPBlock.UsesNAT
                });

                db.SaveChanges();

                return RedirectToAction("IPBlocks", new { errorMessage = 0 });
            }

            return View(IPBlock);
        }

        [HttpGet]
        // GET: BTKLogs/EditIPBlock
        public ActionResult EditIPBlock(int id)
        {
            var IPBlock = db.BTKIPBlocks.Find(id);
            if (IPBlock == null)
            {
                return RedirectToAction("IPBlocks", new { errorMessage = 9 });
            }

            var results = new BTKIPBlockViewModel()
            {
                AllocationDate = IPBlock.AllocationDate,
                BlockType = IPBlock.BlockType,
                DeallocationDate = IPBlock.DeallocationDate,
                EndingIP = IPBlock.EndingIP,
                ID = IPBlock.ID,
                OperatorName = IPBlock.OperatorName,
                ServiceType = IPBlock.ServiceType,
                StartingIP = IPBlock.StartingIP,
                UseLocation = IPBlock.UseLocation,
                UsesNAT = IPBlock.UsesNAT
            };

            return View(viewName: "AddIPBlock", model: results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BTKLogs/EditIPBlock
        public ActionResult EditIPBlock(int id, BTKIPBlockViewModel IPBlock)
        {
            var dbIPBlock = db.BTKIPBlocks.Find(id);
            if (dbIPBlock == null)
            {
                return RedirectToAction("IPBlocks", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                dbIPBlock.AllocationDate = IPBlock.AllocationDate.Value;
                dbIPBlock.BlockType = IPBlock.BlockType.Value;
                dbIPBlock.DeallocationDate = IPBlock.DeallocationDate;
                dbIPBlock.EndingIP = IPBlock.EndingIP;
                dbIPBlock.OperatorName = IPBlock.OperatorName;
                dbIPBlock.ServiceType = IPBlock.ServiceType.Value;
                dbIPBlock.StartingIP = IPBlock.StartingIP;
                dbIPBlock.UseLocation = IPBlock.UseLocation;
                dbIPBlock.UsesNAT = IPBlock.UsesNAT;

                db.SaveChanges();

                return RedirectToAction("IPBlocks", new { errorMessage = 0 });
            }

            return View(IPBlock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BTKLogs/RemoveIPBlock
        public ActionResult RemoveIPBlock(int id)
        {
            var dbIPBlock = db.BTKIPBlocks.Find(id);
            if (dbIPBlock == null)
            {
                return RedirectToAction("IPBlocks", new { errorMessage = 9 });
            }

            db.BTKIPBlocks.Remove(dbIPBlock);
            db.SaveChanges();

            return RedirectToAction("IPBlocks", new { errorMessage = 0 });
        }

        [HttpGet]
        // GET: BTKLogs/SchedulerSettings
        public ActionResult SchedulerSettings()
        {
            var allSettings = db.BTKSchedulerSettings.ToArray().Select(set => new BTKSchedulerSettingsViewModel()
            {
                FTPFolder = set.FTPFolder,
                FTPPassword = set.FTPPassword,
                FTPUsername = set.FTPUsername,
                LogType = set.LogType,
                SchedulerActiveTime = TimeSpan.FromTicks(set.SchedulerActiveTime).ToString(@"hh\:mm\:ss"),
                SchedulerStartDay = set.SchedulerStartDay,
                SchedulerStartTime = TimeSpan.FromTicks(set.SchedulerStartTime).ToString(@"hh\:mm\:ss"),
                SchedulerWorkPeriod = set.SchedulerWorkPeriod,
                PartitionFiles = set.PartitionFiles,
                IsActive = set.IsActive
            });

            return View(allSettings);
        }

        [HttpGet]
        // GET: BTKLogs/EditSchedulerSettings
        public ActionResult EditSchedulerSettings(short id)
        {
            var dbSettings = db.BTKSchedulerSettings.Find(id);
            if (dbSettings == null)
            {
                return RedirectToAction("SchedulerSettings", new { errorMessage = 9 });
            }

            return View(new BTKSchedulerSettingsViewModel()
            {
                FTPFolder = dbSettings.FTPFolder,
                FTPPassword = dbSettings.FTPPassword,
                FTPUsername = dbSettings.FTPUsername,
                LogType = dbSettings.LogType,
                SchedulerActiveTime = new TimeSpan(dbSettings.SchedulerActiveTime).ToString(@"hh\:mm\:ss"),
                SchedulerStartDay = dbSettings.SchedulerStartDay,
                SchedulerStartTime = new TimeSpan(dbSettings.SchedulerStartTime).ToString(@"hh\:mm\:ss"),
                SchedulerWorkPeriod = dbSettings.SchedulerWorkPeriod,
                PartitionFiles = dbSettings.PartitionFiles
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BTKLogs/EditSchedulerSettings
        public ActionResult EditSchedulerSettings(short id, BTKSchedulerSettingsViewModel settings)
        {
            var startDayRequiredPeriods = new[]
            {
                (short)SchedulerWorkPeriods.Weekly,
                (short)SchedulerWorkPeriods.Monthly
            };
            var startTimeRequirements = new[]
            {
                (short)SchedulerWorkPeriods.Daily,
                (short)SchedulerWorkPeriods.Weekly,
                (short)SchedulerWorkPeriods.Monthly
            };
            if (!startDayRequiredPeriods.Contains(settings.SchedulerWorkPeriod))
            {
                settings.SchedulerStartDay = null;
                ModelState.Remove("SchedulerStartDay");
                ModelState.Remove("StartingDaysOfWeek");
            }
            else if (settings.SchedulerWorkPeriod != (short)SchedulerWorkPeriods.Weekly)
            {
                ModelState.Remove("StartingDaysOfWeek");
            }
            if (!startTimeRequirements.Contains(settings.SchedulerWorkPeriod))
            {
                settings.SchedulerStartTime = "00:00:00";
                ModelState.Remove("SchedulerStartTime");
            }

            if (ModelState.IsValid)
            {
                var dbSettings = db.BTKSchedulerSettings.Find(id);
                if (dbSettings == null)
                {
                    return RedirectToAction("SchedulerSettings", new { errorMessage = 9 });
                }

                dbSettings.FTPFolder = settings.FTPFolder;
                dbSettings.FTPPassword = settings.FTPPassword;
                dbSettings.FTPUsername = settings.FTPUsername;
                dbSettings.SchedulerActiveTime = TimeSpan.Parse(settings.SchedulerActiveTime).Ticks;
                dbSettings.SchedulerStartDay = settings.SchedulerStartDay;
                dbSettings.SchedulerStartTime = TimeSpan.Parse(settings.SchedulerStartTime).Ticks;
                dbSettings.SchedulerWorkPeriod = settings.SchedulerWorkPeriod;
                dbSettings.PartitionFiles = settings.PartitionFiles;

                db.SaveChanges();

                return RedirectToAction("SchedulerSettings", new { errorMessage = 0 });
            }

            settings.LogType = id;
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BTKLogs/ChangeLoggerState
        public ActionResult ChangeLoggerState(short logType)
        {
            if (!Enum.IsDefined(typeof(BTKLogTypes), (int)logType))
            {
                return RedirectToAction("SchedulerSettings", new { errorMessage = 9 });
            }

            var loggerSettings = db.BTKSchedulerSettings.Find(logType);
            if (loggerSettings == null)
            {
                return RedirectToAction("SchedulerSettings", new { errorMessage = 9 });
            }

            loggerSettings.IsActive = !loggerSettings.IsActive;
            db.SaveChanges();

            return RedirectToAction("SchedulerSettings", new { errorMessage = 0 });
        }
    }
}