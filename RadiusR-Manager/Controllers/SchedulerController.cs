using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers
{
    public class SchedulerController : BaseController
    {
        [AuthorizePermission(Permissions = "Scheduler Settings")]
        [HttpGet]
        // GET: Scheduler/Settings
        public ActionResult Settings()
        {
            var settings = new SchedulerSettingsViewModel()
            {
                _schedulerRetryDelay = SchedulerSettings.SchedulerRetryDelay,
                _schedulerStartTime = SchedulerSettings.SchedulerStartTime,
                _schedulerStopTime = SchedulerSettings.SchedulerStopTime,
                _smsSchedulerStartTime = SchedulerSettings.SMSSchedulerStartTime,
                _smsSchedulerStopTime = SchedulerSettings.SMSSchedulerStopTime,
                _dailyDisconnectionTime = SchedulerSettings.DailyDisconnectionTime,
                SMSSchedulerPaymentReminderThreshold = SchedulerSettings.SMSSchedulerPaymentReminderThreshold,
                SMSSchedulerPrepaidReminderThreshold = SchedulerSettings.SMSSchedulerPrepaidReminderThreshold,
                SchedulerBillingType = SchedulerSettings.SchedulerBillingType
            };
            return View(settings);
        }

        [AuthorizePermission(Permissions = "Scheduler Settings")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        // POST: Scheduler/Settings
        public ActionResult Settings(SchedulerSettingsViewModel settings)
        {
            if (ModelState.IsValid)
            {
                SchedulerSettings.Update(settings);
                return RedirectToAction("Settings", new { errorMessage = 0 });
            }

            return View(settings);
        }
    }
}