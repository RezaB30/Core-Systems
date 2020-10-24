using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.Localization;

namespace RadiusR.SystemLogs
{
    public partial class SystemLogProcessor
    {
        private const string ParameterSeparator = "\t";
        private Regex LinkRegex = new Regex(@"[@]link[(].+?[)]");
        private Regex ResourceRegex = new Regex(@"[@]resource[(].+?[)]");
        private Regex ParameterIdentifier = new Regex(@"(\{\d+\})");
        private UrlHelper Url;

        public SystemLogProcessor(UrlHelper Url = null)
        {
            this.Url = Url;
        }

        public string ProcessLog(string rawLog)
        {
            var results = HttpUtility.HtmlDecode(rawLog);
            results = ResourceRegex.Replace(results, find =>
            {
                var core = find.Value.Replace(@"@resource(", "").Replace(")", "");
                var parameters = core.Split(' ');
                var parts = new
                {
                    ResourceTypeName = parameters[0],
                    ResourceKey = parameters.Length > 1 ? parameters[1] : null
                };
                if (parts.ResourceTypeName == null || parts.ResourceKey == null)
                    return find.Value;
                string returnValue = find.Value;
                try
                {
                    returnValue = MasterResourceManager.GetResourceManager(parts.ResourceTypeName).GetString(parts.ResourceKey);
                }
                catch
                {
                    returnValue = parts.ResourceKey;
                }
                return returnValue;
            });
            results = LinkRegex.Replace(results, find =>
            {
                var core = find.Value.Replace(@"@link(", "").Replace(")", "");
                if (Url == null)
                    return "#";
                var parameters = core.Split(' ');
                var parts = new
                {
                    Action = parameters[0],
                    Controller = parameters.Length > 1 ? parameters[1] : null,
                    RouteValues = parameters.Length > 2 ? parameters[2] : null
                };
                if (parts.Action == null)
                    return "#";
                var routeValues = parts.RouteValues != null ? HttpUtility.UrlDecode(parts.RouteValues).Split('&').Select(pair => pair.Split('=')).ToDictionary(pair => pair[0], pair => (pair.Count() > 1 ? pair[1] : null) as object) : null;

                return Url.Action(actionName: parts.Action, controllerName: parts.Controller, routeValues: new RouteValueDictionary(dictionary: routeValues));
            });

            return results;
        }

        //public string InsertResource(PropertyInfo item)
        //{
        //    var displayAttribute = item.GetCustomAttribute<DisplayAttribute>();
        //    if (displayAttribute != null && displayAttribute.ResourceType != null)
        //        return InsertResource(displayAttribute.ResourceType, displayAttribute.Name);
        //    return item.Name;
        //}

        public static string InsertResource(Type resourceType, string resourceKey)
        {
            return "@resource(" + string.Join(" ", new[] { resourceType.FullName, resourceKey }) + ")";
        }

        private static string InsertLink(string action, string controller, object routeValues = null)
        {
            Dictionary<string, string> route = null;
            if (routeValues != null)
            {
                var pairList = new List<string>();
                var properties = routeValues.GetType().GetProperties();
                route = properties.ToDictionary(p => p.Name, p => p.GetValue(routeValues).ToString());
            }
            return InsertLinkWithDictionary(action, controller, route);
        }

        private static string InsertLinkWithDictionary(string action, string controller, Dictionary<string, string> routeValues = null)
        {
            string route = null;
            if (routeValues != null)
            {
                var pairList = new List<string>();
                foreach (var item in routeValues)
                {
                    pairList.Add(item.Key + "=" + item.Value);
                }
                route = string.Join("&", pairList);
            }
            return "@link(" + action + " " + controller + (route != null ? " " + HttpUtility.UrlEncode(route) : null) + ")";
        }

        public string GetParameter(IEnumerable<string> parameters)
        {
            return HttpUtility.HtmlEncode(string.Join(ParameterSeparator, parameters));
        }

        public string TranslateLog(SystemLog log, string rawParameters)
        {
            return TranslateLog((SystemLogTypes)log.LogType, rawParameters);
        }

        public string TranslateLog(SystemLogTypes logType, string rawParameters)
        {
            try
            {
                var rawLog = new ResourceManager(typeof(Localization.LogText)).GetString(logType.ToString());
                var processedParameters = rawParameters != null ? rawParameters.Split(new[] { ParameterSeparator }, StringSplitOptions.RemoveEmptyEntries).Select(p => ProcessLog(p)) : new string[] { };
                var parameterSlots = ParameterIdentifier.Matches(rawLog).Count;
                while (processedParameters.Count() < parameterSlots)
                {
                    processedParameters = processedParameters.Concat(new[] { string.Empty });
                }
                return string.Format(rawLog, args: processedParameters.ToArray());
            }
            catch
            {
                return "<span class='error'>" + Localization.Common.ErrorParsingLog + "</span>";
            }
        }
    }
}
