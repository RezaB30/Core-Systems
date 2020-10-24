using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using RadiusR.DB;
using RezaB.Data;
using RezaB.Web.Helpers.ColorPallete.ColorPallete;

namespace RadiusR_Manager.Helpers
{
    public static class LinearDiagramHelper
    {
        private const decimal Margin = 5m;
        private const decimal BarTitleSize = 20m;
        private const decimal BarTitleOffset = 20m;

        private static decimal GraphAreaMargin
        {
            get
            {
                return Margin + BarTitleSize;
            }
        }

        /// <summary>
        /// Draws a line diagram.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="width">Width of diagram in pixels.</param>
        /// <param name="height">Height of diagram in pixels.</param>
        /// <param name="dataArray">Diagram data array.</param>
        /// <param name="tags">Diagram vertical and horizontal bars tags.</param>
        /// <returns></returns>
        public static MvcHtmlString LinearDiagram(this HtmlHelper helper, uint width, uint height, List<LinearDiagramDataArray> dataArray, LinearDiagramTags tags)
        {
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("diagram-container");
            wrapper.AddCssClass("linear-diagram");
            TagBuilder tooltip = new TagBuilder("div");
            tooltip.AddCssClass("diagram-tooltip");
            wrapper.InnerHtml += tooltip.ToString(TagRenderMode.Normal); 

            TagBuilder svg = new TagBuilder("svg");
            svg.MergeAttribute("xmlns", "http://www.w3.org/2000/svg");
            svg.MergeAttribute("width", width.ToString());
            svg.MergeAttribute("height", height.ToString());
            svg.AddCssClass("svg-diagram");

            AddBackgroundPattern(ref svg);
            AddDiagramBars(ref svg, tags.HorizontalBarTitle, tags.VerticalBarTitle);

            if (dataArray.Max(da => da.Data.Count()) > 0)
            {
                BalanceArrayLengths(dataArray);

                var stepCount = CalculateStepCount(dataArray.Max(da => da.Data.Count()));
                for (int i = 0; i < dataArray.Count(); i++)
                {
                    dataArray[i] = new LinearDiagramDataArray(dataArray[i].Title, dataArray[i].Data.Chuncks(stepCount).Select(c => c.Sum()).ToArray(), dataArray[i].DataFormatFunction);
                }
                var stepJump = CalculateStepJump(dataArray.Max(da => da.Data.Count()), width);
                var heigthFactor = CalculateHeightFactor(dataArray.Max(da => da.Data.Max()), height);

                var ColorPallet = ColorsElements.GetSortedEntries();
                int colorIndex = 0;
                foreach (var dataRow in dataArray)
                {
                    var currentColor = ColorPallet[colorIndex % ColorPallet.Length].Value;

                    TagBuilder currentPath = new TagBuilder("path");
                    currentPath.AddCssClass("diagram-path");
                    currentPath.MergeAttribute("stroke", currentColor);
                    currentPath.MergeAttribute("title", dataRow.Title);

                    var pathDirections = new List<string>();
                    var pathPins = new List<TagBuilder>();
                    var diagramHorizontal = GraphAreaMargin;
                    for (int i = 0; i < dataRow.Data.Count(); i++)
                    {
                        var sumValue = dataRow.Data.ToArray()[i];
                        var diagramHeight = (sumValue * heigthFactor) + GraphAreaMargin;
                        var diagramVertical = height - diagramHeight;

                        pathDirections.Add("L" +
                            diagramHorizontal.ToString("0.##", CultureInfo.GetCultureInfo("en-US")) +
                            " " +
                            diagramVertical.ToString("0.##", CultureInfo.GetCultureInfo("en-US")));

                        pathPins.Add(GeneratePin(diagramHorizontal, diagramVertical, 3m, currentColor));
                        pathPins.LastOrDefault().MergeAttribute("title", dataRow.DataFormatFunction.Invoke(sumValue));

                        diagramHorizontal += stepJump;
                    }
                    pathDirections[0] = pathDirections[0].Replace('L', 'M');
                    currentPath.MergeAttribute("d", string.Join(" ", pathDirections.ToArray()));

                    svg.InnerHtml += currentPath.ToString(TagRenderMode.SelfClosing);
                    svg.InnerHtml += string.Join(" ", pathPins.Select(p => p.ToString(TagRenderMode.SelfClosing)));

                    colorIndex++;
                }
            }

            wrapper.InnerHtml += svg.ToString(TagRenderMode.Normal);
            wrapper.InnerHtml += GenerateDiagramLegends(dataArray.Select(da => da.Title).ToArray()).ToString(TagRenderMode.Normal);
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        private static decimal CalculateStepJump(int count, uint width)
        {
            if (count > AppSettings.ChartMaxSteps)
                count = AppSettings.ChartMaxSteps;

            return (width - (GraphAreaMargin * 2m)) / ((count == 0)? 1 : count);
        }

        private static decimal CalculateHeightFactor(decimal max, uint height)
        {
            if (max == 0)
            {
                return 0;
            }
            return (height - (GraphAreaMargin * 2m)) / max;
        }

        private static int CalculateStepCount(int maxArrayLength)
        {
            return Convert.ToInt32(Math.Floor((decimal)maxArrayLength / AppSettings.ChartMaxSteps) + 1);
        }

        private static void AddBackgroundPattern(ref TagBuilder svg)
        {
            TagBuilder defs = new TagBuilder("defs");

            TagBuilder pattern = new TagBuilder("pattern");
            pattern.MergeAttribute("id", "graph-back");
            pattern.MergeAttribute("width", "15");
            pattern.MergeAttribute("height", "15");
            pattern.MergeAttribute("patternUnits", "userSpaceOnUse");

            TagBuilder patternPath = new TagBuilder("path");
            patternPath.AddCssClass("diagram-back");
            patternPath.MergeAttribute("d", "M0 0 L14 0 L14 14");

            pattern.InnerHtml += patternPath.ToString(TagRenderMode.SelfClosing);
            defs.InnerHtml += pattern.ToString(TagRenderMode.Normal);

            TagBuilder workArea = new TagBuilder("rect");
            workArea.MergeAttribute("x", GraphAreaMargin.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")));
            workArea.MergeAttribute("y", GraphAreaMargin.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")));
            workArea.MergeAttribute("width", (int.Parse(svg.Attributes["width"]) - (2m * GraphAreaMargin)).ToString("0.##", CultureInfo.GetCultureInfo("en-US")));
            workArea.MergeAttribute("height", (int.Parse(svg.Attributes["height"]) - (2m * GraphAreaMargin)).ToString("0.##", CultureInfo.GetCultureInfo("en-US")));
            workArea.MergeAttribute("fill", "url(#graph-back)");

            svg.InnerHtml += defs.ToString(TagRenderMode.Normal);
            svg.InnerHtml += workArea.ToString(TagRenderMode.SelfClosing);
        }

        private static void AddDiagramBars(ref TagBuilder svg, string horizontalBarTag, string verticalBarTag)
        {
            TagBuilder barPath = new TagBuilder("path");
            barPath.AddCssClass("diagram-bar");
            barPath.MergeAttribute("d", "M" +
                GraphAreaMargin.ToString("0.##", CultureInfo.GetCultureInfo("en-US")) +
                " " +
                GraphAreaMargin.ToString("0.##", CultureInfo.GetCultureInfo("en-US")) +
                " L" +
                GraphAreaMargin.ToString("0.##", CultureInfo.GetCultureInfo("en-US")) +
                " " +
                (int.Parse(svg.Attributes["height"]) - GraphAreaMargin).ToString("0.##", CultureInfo.GetCultureInfo("en-US")) +
                "L" +
                (int.Parse(svg.Attributes["width"]) - GraphAreaMargin).ToString("0.##", CultureInfo.GetCultureInfo("en-US")) +
                " " +
                (int.Parse(svg.Attributes["height"]) - GraphAreaMargin).ToString("0.##", CultureInfo.GetCultureInfo("en-US")));
            svg.InnerHtml += barPath.ToString(TagRenderMode.SelfClosing);

            TagBuilder horizontalBarText = new TagBuilder("text");
            horizontalBarText.MergeAttribute("x", (Margin + BarTitleOffset).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")));
            horizontalBarText.MergeAttribute("y", ((int.Parse(svg.Attributes["height"]) - Margin).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US"))));
            horizontalBarText.AddCssClass("diagram-tag");
            horizontalBarText.InnerHtml += horizontalBarTag;
            svg.InnerHtml += horizontalBarText.ToString(TagRenderMode.Normal);

            TagBuilder verticalBarText = new TagBuilder("text");
            verticalBarText.MergeAttribute("x", BarTitleOffset.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")));
            verticalBarText.MergeAttribute("y", ((int.Parse(svg.Attributes["height"]) - (Margin + BarTitleSize)).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US"))));
            verticalBarText.MergeAttribute("transform", "rotate(270 " + verticalBarText.Attributes["x"] + "," + verticalBarText.Attributes["y"] + ")");
            verticalBarText.AddCssClass("diagram-tag");
            verticalBarText.InnerHtml += verticalBarTag;
            svg.InnerHtml += verticalBarText.ToString(TagRenderMode.Normal);
        }

        private static TagBuilder GeneratePin(decimal x, decimal y, decimal radius, string color)
        {
            TagBuilder circle = new TagBuilder("circle");
            circle.MergeAttribute("cx", x.ToString("0.##", CultureInfo.GetCultureInfo("en-US")));
            circle.MergeAttribute("cy", y.ToString("0.##", CultureInfo.GetCultureInfo("en-US")));
            circle.MergeAttribute("r", radius.ToString("0.##", CultureInfo.GetCultureInfo("en-US")));
            circle.MergeAttribute("stroke", color);
            circle.MergeAttribute("fill", color);
            circle.AddCssClass("diagram-pin");

            return circle;
        }

        private static TagBuilder GenerateDiagramLegends(string[] titles)
        {
            TagBuilder legends = new TagBuilder("div");
            legends.AddCssClass("diagram-legends");

            var colors = ColorsElements.GetSortedEntries();
            for (int i = 0; i < titles.Count(); i++)
            {
                TagBuilder row = new TagBuilder("div");

                TagBuilder icon = new TagBuilder("div");
                icon.AddCssClass("diagram-legend-icon");
                icon.MergeAttribute("style", "background-color: " + colors[i % colors.Length].Value + ";");

                row.InnerHtml = icon.ToString(TagRenderMode.Normal);
                row.InnerHtml += "&nbsp;" + titles[i];
                legends.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            return legends;
        }

        private static void BalanceArrayLengths(List<LinearDiagramDataArray> dataArray)
        {
            var maxLength = dataArray.Max(da => da.Data.Count());
            for (int i = 0; i < dataArray.Count; i++)
            {
                var currentCount = dataArray[i].Data.Count();
                if (currentCount < maxLength)
                {
                    dataArray[i].Data = dataArray[i].Data.Concat(new decimal[maxLength - currentCount]);
                    for (int j = currentCount; j < maxLength; j++)
                    {
                        dataArray[i].Data.ToArray()[j] = 0m;
                    }
                }
            }
        }
    }

    /// <summary>
    /// An array for diagram data.
    /// </summary>
    public class LinearDiagramDataArray
    {
        /// <summary>
        /// Title to be used in diagram for this data.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// An array of contigus diagram data.
        /// </summary>
        public IEnumerable<decimal> Data { get; set; }

        /// <summary>
        /// Format string for showing data on diagram.
        /// </summary>
        public Func<decimal,string> DataFormatFunction { get; private set; }

        /// <summary>
        /// Creates a data array for a linear diagram.
        /// </summary>
        /// <param name="title">Data title in diagram</param>
        /// <param name="data">Data array to be shown in diagram</param>
        /// <param name="dataFormatString">Format string for showing data on diagram</param>
        /// <param name="culture">Culture for format string</param>
        public LinearDiagramDataArray(string title, IEnumerable<decimal> data, Func<decimal, string> dataFormatFunction)
        {
            Title = title;
            Data = data;
            DataFormatFunction = dataFormatFunction;
        }
    }

    /// <summary>
    /// Includes a linear diagram tag strings.
    /// </summary>
    public class LinearDiagramTags
    {
        /// <summary>
        /// Title on horizontal data bar.
        /// </summary>
        public string HorizontalBarTitle { get; set; }
        /// <summary>
        /// Title on vertical data bar.
        /// </summary>
        public string VerticalBarTitle { get; set; }
    }
}