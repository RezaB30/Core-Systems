using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RezaB.Web.Helpers;
using System.Text.RegularExpressions;

namespace RadiusR.Helpers
{
    public static class AddFeeHelper
    {
        private static Regex SampleFinderRegex = new Regex(@"(\.sample)(?=\.)");

        public static MvcHtmlString AddFeeEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, IEnumerable<SubscriberFeesAddViewModel> samples) where TResult : IEnumerable<SubscriberFeesAddViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = metadata.Model as IEnumerable<SubscriberFeesAddViewModel>;
            if (samples == null || !samples.Any())
                return new MvcHtmlString(string.Empty);

            var oldPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            helper.ViewData.TemplateInfo.HtmlFieldPrefix = !string.IsNullOrEmpty(fieldName) ? oldPrefix + "." + fieldName : fullName;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("add-fee-editor-wrapper");

            // available values
            {
                TagBuilder sampleContainer = new TagBuilder("div");
                sampleContainer.MergeAttribute("style", "display: none;");
                sampleContainer.AddCssClass("sample-container");

                var sample = new SubscriberFeesAddViewModel();
                sampleContainer.InnerHtml = helper.AddFeeItem(sample, samples, true);

                wrapper.InnerHtml += sampleContainer.ToString(TagRenderMode.Normal);
            }

            // current values
            {
                TagBuilder ol = new TagBuilder("ol");
                ol.AddCssClass("multiselect-orderedlist");

                for (int i = 0; i < model.Count(); i++)
                {
                    ol.InnerHtml += helper.AddFeeItem(model.ToArray()[i], samples, false, i);
                }
                //foreach (var item in model)
                //{
                //    ol.InnerHtml += helper.AddFeeItem(item, samples, false);
                //}

                wrapper.InnerHtml += ol.ToString(TagRenderMode.Normal);
            }

            // add row
            {
                TagBuilder addRow = new TagBuilder("div");
                addRow.AddCssClass("add-instance-row");

                TagBuilder addButton = new TagBuilder("input");
                addButton.MergeAttribute("type", "button");
                addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                addButton.AddCssClass("link-button iconed-button add-instance-button");

                addRow.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);
                wrapper.InnerHtml += addRow.ToString(TagRenderMode.Normal);
            }

            helper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        private static MvcHtmlString AddFeeCustomFeeFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, bool isAllTime, int? masterIndex = null) where TResult : IEnumerable<SubscriberFeesAddViewModel.CustomFee>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = metadata.Model as IEnumerable<SubscriberFeesAddViewModel.CustomFee>;

            var oldPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            helper.ViewData.TemplateInfo.HtmlFieldPrefix = !string.IsNullOrEmpty(oldPrefix) ? oldPrefix + "." + fieldName : fieldName;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("add-fee-custom-fee-wrapper");

            // sample
            {
                TagBuilder sampleContainer = new TagBuilder("ol");
                sampleContainer.AddCssClass("custom-fee-sample");
                sampleContainer.MergeAttribute("style", "display: none;");

                {
                    var sample = new SubscriberFeesAddViewModel.CustomFee();

                    sampleContainer.InnerHtml += helper.CustomFeeItem(sample, isAllTime);
                }

                wrapper.InnerHtml += sampleContainer.ToString(TagRenderMode.Normal);
            }

            // current values
            {
                TagBuilder ol = new TagBuilder("ol");
                ol.AddCssClass("multiselect-orderedlist");

                for (int i = 0; i < model.Count(); i++)
                {
                    ol.InnerHtml += helper.CustomFeeItem(model.ToArray()[i], isAllTime, masterIndex, i);
                }

                wrapper.InnerHtml += ol.ToString(TagRenderMode.Normal);
            }

            // add row
            {
                TagBuilder addRow = new TagBuilder("div");
                addRow.AddCssClass("add-sub-instance-row");

                TagBuilder addButton = new TagBuilder("input");
                addButton.MergeAttribute("type", "button");
                addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                addButton.AddCssClass("link-button iconed-button add-instance-button");

                addRow.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);
                wrapper.InnerHtml += addRow.ToString(TagRenderMode.Normal);
            }


            helper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        private static string CustomFeeItem<TModel>(this HtmlHelper<TModel> helper, SubscriberFeesAddViewModel.CustomFee sample, bool isAllTime, int? masterIndex = null, int? slaveIndex = null)
        {
            TagBuilder li = new TagBuilder("li");
            li.AddCssClass("multiselect-input-row");

            TagBuilder table = new TagBuilder("table");
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(modelItem => sample.Title);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => sample.Title).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(modelItem => sample.Price);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => sample.Price).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    if (masterIndex.HasValue && slaveIndex.HasValue)
                    {
                        var fieldName = helper.NameFor(modelItem => sample.Title).ToString();
                        if (SampleFinderRegex.IsMatch(fieldName))
                        {
                            var validationName = SampleFinderRegex.Replace(fieldName, "[" + masterIndex + "]", 1);
                            if (SampleFinderRegex.IsMatch(validationName))
                            {
                                validationName = SampleFinderRegex.Replace(validationName, "[" + slaveIndex + "]", 1);
                                if (helper.ViewData.ModelState.ContainsKey(validationName))
                                    cell.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                            }
                        }
                    }
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    if (masterIndex.HasValue && slaveIndex.HasValue)
                    {
                        var fieldName = helper.NameFor(modelItem => sample.Price).ToString();
                        if (SampleFinderRegex.IsMatch(fieldName))
                        {
                            var validationName = SampleFinderRegex.Replace(fieldName, "[" + masterIndex + "]", 1);
                            if (SampleFinderRegex.IsMatch(validationName))
                            {
                                validationName = SampleFinderRegex.Replace(validationName, "[" + slaveIndex + "]", 1);
                                if (helper.ViewData.ModelState.ContainsKey(validationName))
                                    cell.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                            }
                        }
                    }
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                if (!isAllTime)
                {
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.LabelFor(modelItem => sample.Installment);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.Select(modelItem => sample.Installment, RezaB.Web.Helpers.HelperUtilities.CreateNumericSelectList(1, 24, sample.Installment == 0 ? 1 : sample.Installment)).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.MergeAttribute("style", "text-align: right;");
                        TagBuilder removeButton = new TagBuilder("input");
                        removeButton.MergeAttribute("type", "button");
                        removeButton.MergeAttribute("style", "margin-left: 0.5em;");
                        removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                        removeButton.AddCssClass("link-button iconed-button remove-instance-button");
                        cell.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                }
                else
                {
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "4");
                        cell.MergeAttribute("style", "text-align: right;");
                        TagBuilder removeButton = new TagBuilder("input");
                        removeButton.MergeAttribute("type", "button");
                        removeButton.MergeAttribute("style", "margin-left: 0.5em;");
                        removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                        removeButton.AddCssClass("link-button iconed-button remove-instance-button");
                        cell.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if (!isAllTime)
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    if (masterIndex.HasValue && slaveIndex.HasValue)
                    {
                        var fieldName = helper.NameFor(modelItem => sample.Installment).ToString();
                        if (SampleFinderRegex.IsMatch(fieldName))
                        {
                            var validationName = SampleFinderRegex.Replace(fieldName, "[" + masterIndex + "]", 1);
                            if (SampleFinderRegex.IsMatch(validationName))
                            {
                                validationName = SampleFinderRegex.Replace(validationName, "[" + slaveIndex + "]", 1);
                                if (helper.ViewData.ModelState.ContainsKey(validationName))
                                    cell.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                            }
                        }
                    }
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            li.InnerHtml += table.ToString(TagRenderMode.Normal);

            return li.ToString(TagRenderMode.Normal);
        }

        private static string AddFeeItem<TModel>(this HtmlHelper<TModel> helper, SubscriberFeesAddViewModel sampleItem, IEnumerable<SubscriberFeesAddViewModel> samples, bool isSample, int? index = null)
        {
            var results = string.Empty;
            // master select list
            {
                var sample = sampleItem;

                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("multiselect-input-row");

                {
                    TagBuilder row = new TagBuilder("div");
                    row.AddCssClass("type-selection-container");
                    row.InnerHtml = helper.Select(modelItem => sample.FeeTypeID, new SelectList(samples, "FeeTypeID", "FeeTypeName", sample.FeeTypeID), RadiusR.Localization.Helpers.Common.Select).ToHtmlString();

                    TagBuilder removeButton = new TagBuilder("input");
                    removeButton.MergeAttribute("type", "button");
                    removeButton.MergeAttribute("style", "margin-left: 0.5em;");
                    removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                    removeButton.AddCssClass("link-button iconed-button remove-instance-button");
                    row.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);

                    li.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                if(index.HasValue)
                {
                    TagBuilder row = new TagBuilder("div");
                    if (index.HasValue)
                    {
                        var fieldName = helper.NameFor(modelItem => sample.FeeTypeID).ToString();
                        if (SampleFinderRegex.IsMatch(fieldName))
                        {
                            var validationName = SampleFinderRegex.Replace(fieldName, "[" + index + "]", 1);
                            if (helper.ViewData.ModelState.ContainsKey(validationName))
                                row.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                        }
                    }
                    li.InnerHtml += row.ToString(TagRenderMode.Normal);
                }

                TagBuilder subContainer = new TagBuilder("div");
                subContainer.AddCssClass("item-selection-results-container");
                if (!isSample)
                {
                    // fix non sample extra values
                    var sampleRef = samples.FirstOrDefault(s => s.FeeTypeID == sample.FeeTypeID);
                    if (sampleRef != null)
                    {
                        sample.Variants = sampleRef.Variants;
                        sample.IsAllTime = sampleRef.IsAllTime;
                    }
                    else
                    {
                        sample.FeeTypeID = null;
                    }
                    // render item
                    subContainer.InnerHtml = helper.AddFeeSubItem(sample, index);
                }
                li.InnerHtml += subContainer.ToString(TagRenderMode.Normal);

                if (isSample)
                {
                    TagBuilder sampleMasterContainer = new TagBuilder("ol");
                    sampleMasterContainer.AddCssClass("sample-master-container");

                    sampleMasterContainer.InnerHtml = li.ToString(TagRenderMode.Normal);
                    results += sampleMasterContainer.ToString(TagRenderMode.Normal);
                }
                else
                {
                    results += li.ToString(TagRenderMode.Normal);
                }
            }
            // slave select lists
            if (isSample)
            {
                foreach (var sample in samples)
                {
                    TagBuilder sampleItemContainer = new TagBuilder("div");
                    sampleItemContainer.AddCssClass("sample-item");
                    sampleItemContainer.MergeAttribute("value", sample.FeeTypeID.ToString());

                    sampleItemContainer.InnerHtml += helper.AddFeeSubItem(sample);

                    results += sampleItemContainer.ToString(TagRenderMode.Normal);
                }
            }

            return results;
        }

        private static string AddFeeSubItem<TModel>(this HtmlHelper<TModel> helper, SubscriberFeesAddViewModel sample, int? index = null)
        {
            var results = string.Empty;

            if (sample.FeeTypeID.HasValue)
            {
                if (sample.Variants != null)
                {
                    TagBuilder table = new TagBuilder("table");
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.InnerHtml = helper.LabelFor(modelItem => sample.SelectedVariantID).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.InnerHtml = helper.Select(modelItem => sample.SelectedVariantID, new SelectList(sample.Variants, "ID", "Name", sample.SelectedVariantID), Localization.Helpers.Common.Select).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        if (!sample.IsAllTime)
                        {
                            {
                                TagBuilder cell = new TagBuilder("td");
                                cell.InnerHtml = helper.LabelFor(modelItem => sample.InstallmentCount).ToHtmlString();
                                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                            }
                            {
                                TagBuilder cell = new TagBuilder("td");
                                cell.InnerHtml = helper.Select(modelItem => sample.InstallmentCount, HelperUtilities.CreateNumericSelectList(1, 24, sample.InstallmentCount == 0 ? 1 : sample.InstallmentCount)).ToHtmlString();
                                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                            }
                        }
                        table.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("colspan", "2");
                            if (index.HasValue)
                            {
                                var fieldName = helper.NameFor(modelItem => sample.SelectedVariantID).ToString();
                                if (SampleFinderRegex.IsMatch(fieldName))
                                {
                                    var validationName = SampleFinderRegex.Replace(fieldName, "[" + index + "]", 1);
                                    if (helper.ViewData.ModelState.ContainsKey(validationName))
                                        cell.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                                }
                            }
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        if (!sample.IsAllTime)
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("colspan", "2");
                            if (index.HasValue)
                            {
                                var fieldName = helper.NameFor(modelItem => sample.InstallmentCount).ToString();
                                if (SampleFinderRegex.IsMatch(fieldName))
                                {
                                    var validationName = SampleFinderRegex.Replace(fieldName, "[" + index + "]", 1);
                                    if (helper.ViewData.ModelState.ContainsKey(validationName))
                                        cell.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                                }
                            }
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        table.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }
                    results += table.ToString(TagRenderMode.Normal);
                }
                else if (sample.CustomFees != null)
                {
                    results += helper.AddFeeCustomFeeFor(modelItem => sample.CustomFees, sample.IsAllTime, index).ToHtmlString();
                }
                else if (!sample.IsAllTime)
                {
                    TagBuilder table = new TagBuilder("table");
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.InnerHtml = helper.LabelFor(modelItem => sample.InstallmentCount).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.InnerHtml = helper.Select(modelItem => sample.InstallmentCount, HelperUtilities.CreateNumericSelectList(1, 24, sample.InstallmentCount == 0 ? 1 : sample.InstallmentCount)).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }

                        table.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("colspan", "2");
                            if (index.HasValue)
                            {
                                var fieldName = helper.NameFor(modelItem => sample.InstallmentCount).ToString();
                                if (SampleFinderRegex.IsMatch(fieldName))
                                {
                                    var validationName = SampleFinderRegex.Replace(fieldName, "[" + index + "]", 1);
                                    if (helper.ViewData.ModelState.ContainsKey(validationName))
                                        cell.InnerHtml = helper.ValidationMessage(validationName, string.Join(Environment.NewLine, helper.ViewData.ModelState[validationName].Errors.Select(e => e.ErrorMessage))).ToHtmlString();
                                }
                            }
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        table.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }

                    results += table.ToString(TagRenderMode.Normal);
                }
            }
            
            return results;
        }
    }
}
