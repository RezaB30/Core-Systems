using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Data.Entity.Infrastructure;
using RadiusR_Manager.Properties;
using System.Web.Routing;
using RadiusR.DB;
using NLog;
using RezaB.Web;

namespace RadiusR_Manager.Controllers
{
    public class BaseController : Controller
    {
        protected static Logger logger = LogManager.GetLogger("main");
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //Localization in Base controller:

            string lang = CookieTools.getCulture(Request.Cookies);

            var routeData = RouteData.Values;
            var routeCulture = routeData.Where(r => r.Key == "lang").FirstOrDefault();
            if (string.IsNullOrEmpty((string)routeCulture.Value))
            {
                routeData.Remove("lang");
                routeData.Add("lang", lang);

                Thread.CurrentThread.CurrentUICulture =
                Thread.CurrentThread.CurrentCulture =
                CultureInfo.GetCultureInfo(lang);

                Response.RedirectToRoute(routeData);
            }
            else
            {
                lang = (string)RouteData.Values["lang"];

                Thread.CurrentThread.CurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                    CultureInfo.GetCultureInfo(lang);
            }

            ViewBag.Version = Settings.Default.Version;
            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.Exception is System.Web.Mvc.HttpAntiForgeryException))
            {
                if (filterContext.Exception is System.Data.Entity.Validation.DbEntityValidationException)
                {
                    var entityException = filterContext.Exception as System.Data.Entity.Validation.DbEntityValidationException;
                    logger.Error(filterContext.Exception, string.Join(Environment.NewLine, entityException.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(se => se.PropertyName + "->" + se.ErrorMessage))));
                }
                else
                {
                    logger.Error(filterContext.Exception);
                }
            }
            //var error = ErrorHandler.GetMessage(filterContext.Exception, Request.IsLocal);
            //filterContext.ExceptionHandled = true;
            ////filterContext.HttpContext.Response.Clear();
            //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ////filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            //filterContext.Result = Error(error.Message, error.Details);
        }

        [AllowAnonymous]
        [HttpGet, ActionName("Language")]
        public virtual ActionResult Language(string culture, string sender)
        {
            CookieTools.SetCultureInfo(Response.Cookies, culture);

            Dictionary<string, object> responseParams = new Dictionary<string, object>();
            Request.QueryString.CopyTo(responseParams);
            responseParams.Add("lang", culture);

            return RedirectToAction(sender, new RouteValueDictionary(responseParams));
        }

        public virtual ActionResult Error(string message, string details)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new { Code = 1, Message = message, Details = details }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.Message = message;
            ViewBag.Details = details;
            return View("ErrorDialogBox");
        }

        protected void SetupPages<T>(int? page, ref IQueryable<T> viewResults)
        {
            var totalCount = viewResults.Count();
            var pagesCount = Math.Ceiling((float)totalCount / (float)AppSettings.TableRows);
            ViewBag.PageCount = pagesCount;
            ViewBag.PageTotalCount = totalCount;

            if (!page.HasValue)
            {
                page = 0;
            }

            viewResults = viewResults.Skip(page.Value * AppSettings.TableRows).Take(AppSettings.TableRows);
        }

        protected string ViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        //protected bool IsValidForDealer(Subscription subscription)
        //{
        //    var dealerId = User.GiveUserDealerId();
        //    if (!dealerId.HasValue)
        //        return true;
        //    if (User.IsInRole("cashier") && !subscription.DealerID.HasValue)
        //        return true;
        //    if (subscription.DealerID == dealerId)
        //        return true;

        //    return false;
        //}

        //protected bool IsValidForDealer(Service service)
        //{
        //    var dealerId = User.GiveUserDealerId();
        //    if (!dealerId.HasValue)
        //        return true;
        //    if (service.Dealers.Select(dealer => dealer.ID).Contains(dealerId.Value))
        //        return true;

        //    return false;
        //}
    }
}