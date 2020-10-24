using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using RezaB.Web.Helpers;
using RezaB.Data.Localization;

namespace RadiusR_Manager.Helpers
{
    public static class RoleEditorHelper
    {
        public static MvcHtmlString RoleEditor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return helper.RoleEditor(expression, fullName);
        }

        public static MvcHtmlString RoleEditor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string id)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            int? value = metadata.Model as int?;
            object _enumType;
            object _resourceType;
            Type enumType;
            Type resourceType;
            if (metadata.AdditionalValues.TryGetValue("EnumType", out _enumType) && metadata.AdditionalValues.TryGetValue("EnumResourceType", out _resourceType))
            {
                enumType = _enumType as Type;
                resourceType = _resourceType as Type;
                var type = typeof(LocalizedList<,>).MakeGenericType(enumType, resourceType);
                var genericList = Activator.CreateInstance(type) as LocalizedList;

                var items = genericList.GenericList.Select(obj=> new ListItem { ID = (int)obj.GetType().GetProperty("ID").GetValue(obj), Name = obj.GetType().GetProperty("Name").GetValue(obj) as string });
                RadiusR.DB.Role currentRole = null;
                using (RadiusR.DB.RadiusREntities db = new RadiusR.DB.RadiusREntities())
                {
                    var predefined = items.Select(v=>v.ID).ToArray();
                    var allDbRoles = db.Roles.ToArray();
                    currentRole = allDbRoles.FirstOrDefault(role => role.ID == value);

                    var acceptableItems = allDbRoles.Where(role => role.CanBeManuallyAssigned).Select(role => new ListItem { ID = role.ID, Name = role.Name }).ToArray();
                    for (int i = 0; i < acceptableItems.Length; i++)
                    {
                        var matchedItem = items.FirstOrDefault(item => item.ID == acceptableItems[i].ID);
                        if (matchedItem != null)
                        {
                            acceptableItems[i].Name = matchedItem.Name;
                        }
                    }

                    items = acceptableItems;
                }
                if ((value.HasValue && value.Value > 0) && !items.Select(item=>item.ID).Contains(value.Value))
                {
                    var tempModel = new RadiusR_Manager.Models.RadiusViewModels.AppUserViewModel()
                    {
                        Role = currentRole
                    };
                    return helper.RoleDisplay(role => tempModel.Role);
                }
                var selectList = new SelectList(items, "ID", "Name", value > 0 ? value : null);

                return helper.Select(id, selectList, RadiusR.Localization.Helpers.Common.Select);
            }

            return new MvcHtmlString(metadata.Model.ToString());
        }

        private class ListItem
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }
    }
}