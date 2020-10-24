using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Controllers.Shared
{
    public class CalendarController : BaseController
    {
        // GET: Calendar
        public ActionResult CalendarData(int year, int month, int? day)
        {
            ViewBag.Year = year;
            ViewBag.MonthNo = month;
            ViewBag.Day = day;

            var datetimeInfo = DateTimeFormatInfo.GetInstance(Thread.CurrentThread.CurrentCulture);
            var currentDay = day ?? 1;
            var sampleDate = new DateTime(year, month, 1);
            ViewBag.Month = datetimeInfo.GetMonthName(month);
            Calendar calendar = Thread.CurrentThread.CurrentCulture.Calendar;
            var totalDays = calendar.GetDaysInMonth(year, month);
            ViewBag.TotalDays = totalDays;
            var startingDayOfWeek = calendar.GetDayOfWeek(sampleDate);
            ViewBag.StartingDay = startingDayOfWeek;
            var dayNames = new List<string>();
            int i = 0;
            ViewBag.DayNames = new string[7];
            foreach (var dayofweek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var currentDayName = datetimeInfo.GetAbbreviatedDayName((DayOfWeek)dayofweek);
                ViewBag.DayNames[i] = currentDayName;
                i++;
            }

            return View();
        }
    }
}