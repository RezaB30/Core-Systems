using RadiusR_Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR.Helpers
{
    public static class TreeCheckerHelper
    {
        public static MvcHtmlString TreeChecker<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : IEnumerable<TreeCollection>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model as IEnumerable<TreeCollection>;
            object _resourceType;
            ResourceManager names = null;
            if (metadata.AdditionalValues.TryGetValue("EnumResourceType", out _resourceType))
            {
                names = new ResourceManager(_resourceType as Type);
            }
            // generating the tree
            TagBuilder wrapper = new TagBuilder("ul");
            wrapper.AddCssClass("tree-checker-list");
            wrapper.AddCssClass("tree-checker-wrapper");
            // add wrapper title
            {
                TagBuilder treeItem = new TagBuilder("li");

                TagBuilder title = new TagBuilder("div");
                title.AddCssClass("tree-checker-title");
                title.SetInnerText(RadiusR.Localization.Helpers.Common.All);
                treeItem.InnerHtml += title.ToString(TagRenderMode.Normal);

                // add tree nodes
                foreach (var node in value)
                {
                    treeItem.InnerHtml += _renderNode(node, names);
                }

                wrapper.InnerHtml += treeItem.ToString(TagRenderMode.Normal);
            }

            var Permissions = string.Join(",", value.GetValues());
            wrapper.InnerHtml += helper.HiddenFor(model => Permissions);
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        private static string _renderNode(TreeCollection root, ResourceManager names)
        {
            TagBuilder list = new TagBuilder("ul");
            list.AddCssClass("tree-checker-list");
            list.MergeAttribute("tree-node-value", root.ID.ToString());
            if (root.IsSelected)
                list.AddCssClass("selected");

            TagBuilder treeItem = new TagBuilder("li");

            TagBuilder title = new TagBuilder("div");
            title.AddCssClass("tree-checker-title");
            title.SetInnerText(names != null ? names.GetString(root.Name.Replace(" ", "")) ?? root.Name : root.Name);
            treeItem.InnerHtml += title.ToString(TagRenderMode.Normal);

            foreach (var node in root._sub)
            {
                treeItem.InnerHtml += _renderNode(node, names);
            }

            list.InnerHtml += treeItem.ToString(TagRenderMode.Normal);
            return list.ToString(TagRenderMode.Normal);
        }
    }
}