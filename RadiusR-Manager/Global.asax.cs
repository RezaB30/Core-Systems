﻿using RadiusR_Manager.Binders;
using RezaB.Web.Helpers.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RadiusR_Manager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            ModelBinders.Binders[typeof(DateTime?)] = new DateBinder();
            ModelBinders.Binders[typeof(DateTime)] = new DateBinder();
            ModelBinders.Binders[typeof(DateWithTime)] = new DateWithTimeBinder();
            ModelBinders.Binders[typeof(bool)] = new BooleanBinder();
            ModelBinders.Binders[typeof(bool?)] = new BooleanBinder();
            ModelBinders.Binders[typeof(Models.ViewModels.PDFTemplates.InvariantDecimal)] = new InvariantCultureDecimalBinder();
        }
    }
}
