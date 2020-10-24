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
    [AuthorizePermission(Permissions = "Background Services")]
    public class BackgroundServicesController : BaseController
    {
        protected IEnumerable<System.ServiceProcess.ServiceController> Services
        {
            get
            {
                var validNames = new[]
                {
                    SystemSettings.RadiusRServerServiceName,
                    SystemSettings.RadiusRSchedulerServiceName,
                    SystemSettings.BTKLogSchedulerServiceName
                };

                return System.ServiceProcess.ServiceController.GetServices().Where(sc => validNames.Contains(sc.ServiceName));
            }
        }

        [HttpGet]
        // GET: BackgroundServices
        public ActionResult Index()
        {
            var viewResults = Services.Select(service => new BackgroundServiceViewModel()
            {
                Name = service.ServiceName,
                DisplayName = service.DisplayName,
                State = service.Status == System.ServiceProcess.ServiceControllerStatus.Running ? (short)BackgroundServiceViewModel.ServiceState.Running : (short)BackgroundServiceViewModel.ServiceState.Stopped
            }).ToList();
            ViewBag.ErrorMessage = TempData["Error"];
            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: BackgroundServices
        public ActionResult Index(string Name, short newState)
        {
            BackgroundServiceViewModel.ServiceState NewState;
            try
            {
                NewState = (BackgroundServiceViewModel.ServiceState)newState;
            }
            catch
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            var currentService = Services.FirstOrDefault(service => service.ServiceName == Name);
            if (currentService == null)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }
            if (newState == (short)BackgroundServiceViewModel.ServiceState.Stopped && currentService.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            switch (NewState)
            {
                case BackgroundServiceViewModel.ServiceState.Running:
                    if (currentService.Status != System.ServiceProcess.ServiceControllerStatus.Stopped)
                    {
                        currentService.Stop();
                        currentService.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    }
                    currentService.Start();
                    currentService.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    break;
                case BackgroundServiceViewModel.ServiceState.Stopped:
                    currentService.Stop();
                    currentService.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    break;
                default:
                    return RedirectToAction("Index", new { errorMessage = 9 });
            }

            return RedirectToAction("Index", new { errorMessage = 0 });
        }
    }
}