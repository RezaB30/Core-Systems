using RezaB.Web.Helpers.ColorPallete.ColorPallete;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class PieChartHelper
    {
        private const decimal Margin = 5m;

        /// <summary>
        /// Draws a chart diagram
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="radius">Radius of the chart.</param>
        /// <param name="chartData">A list of data to be shown.</param>
        /// <param name="formatString">Format string for data weight.</param>
        /// <param name="formatCulture">The culture to show the weight in it's format.</param>
        /// <returns></returns>
        public static MvcHtmlString PieChart(this HtmlHelper helper, uint radius, List<PieChartData> chartData, Func<decimal, string> DataFormatFunction)
        {
            var diagCulture = CultureInfo.CreateSpecificCulture("en-US");
            var colorPallete = ColorsElements.GetSortedEntries();

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("diagram-container");
            wrapper.AddCssClass("pie-diagram");
            TagBuilder tooltip = new TagBuilder("div");
            tooltip.AddCssClass("diagram-tooltip");
            wrapper.InnerHtml += tooltip.ToString(TagRenderMode.Normal);

            TagBuilder svg = new TagBuilder("svg");
            svg.MergeAttribute("xmlns", "http://www.w3.org/2000/svg");
            svg.MergeAttribute("width", (radius + Margin).ToString("0"));
            svg.MergeAttribute("height", (radius + Margin).ToString("0"));
            svg.MergeAttribute("viewBox", "-1 -1 2 2");
            svg.MergeAttribute("style", "transform: rotate(-90deg);");
            svg.AddCssClass("svg-diagram");

            var percentages = CalculatePercentages(chartData);

            var cumulativePercentage = 0m;
            var colorIndex = 0;
            foreach (var data in chartData)
            {
                var title = string.Format("{0}: {1}({2})",
                    data.Title,
                    DataFormatFunction.Invoke(data.Weight),
                    percentages[data.Title].ToString("P"));

                if (percentages[data.Title] == 1m)
                {
                    TagBuilder circle = new TagBuilder("circle");
                    circle.AddCssClass("chart-slice");
                    circle.MergeAttribute("cx", "0");
                    circle.MergeAttribute("cy", "0");
                    circle.MergeAttribute("r", "1");
                    circle.MergeAttribute("fill", colorPallete[colorIndex % colorPallete.Length].Value);
                    circle.MergeAttribute("title", title);

                    svg.InnerHtml += circle.ToString(TagRenderMode.SelfClosing);
                }
                else
                {
                    var startingCoords = GetCoords(cumulativePercentage);
                    cumulativePercentage += percentages[data.Title];
                    var DestinationCoords = GetCoords(cumulativePercentage);
                    var largeArcFlag = percentages[data.Title] > 0.5m ? 1 : 0;

                    var pathData = string.Format("M{0} {1} A1 1 0 {2} 1 {3} {4} L0 0",
                        startingCoords.x.ToString("0.##", diagCulture),
                        startingCoords.y.ToString("0.##", diagCulture),
                        largeArcFlag.ToString("0"),
                        DestinationCoords.x.ToString("0.##", diagCulture),
                        DestinationCoords.y.ToString("0.##", diagCulture));

                    TagBuilder path = new TagBuilder("path");
                    path.MergeAttribute("d", pathData);
                    path.MergeAttribute("title", title);
                    path.AddCssClass("chart-slice");
                    path.MergeAttribute("fill", colorPallete[colorIndex % colorPallete.Length].Value);

                    svg.InnerHtml += path.ToString(TagRenderMode.SelfClosing);
                }
                colorIndex++;

            }

            if (chartData.Count() <= 0)
            {
                wrapper.InnerHtml += RadiusR.Localization.Helpers.Common.NoData;
            }
            else
            {
                wrapper.InnerHtml += svg.ToString(TagRenderMode.Normal);
                wrapper.InnerHtml += GenerateDiagramLegends(chartData.Select(d => d.Title).ToArray());
            }
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        private static Dictionary<string, decimal> CalculatePercentages(List<PieChartData> chartData)
        {
            var totalWeight = chartData.Sum(cd => cd.Weight);
            if (totalWeight == 0)
                totalWeight = 1;
            var dictionary = new Dictionary<string, decimal>();
            foreach (var data in chartData)
            {
                dictionary.Add(data.Title, data.Weight / totalWeight);
            }

            return dictionary;
        }

        private static Coords GetCoords(decimal percentage)
        {
            Coords coords = new Coords();
            coords.x = (decimal)Math.Cos(((double)percentage) * 2 * Math.PI);
            coords.y = (decimal)Math.Sin(((double)percentage) * 2 * Math.PI);

            return coords;
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
    }

    /// <summary>
    /// Data to show in a pie chart
    /// </summary>
    public class PieChartData
    {
        /// <summary>
        /// Chart section title, must be unique
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Chart section value
        /// </summary>
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Defines 2d coordinates.
    /// </summary>
    internal class Coords
    {
        /// <summary>
        /// The x element.
        /// </summary>
        public decimal x { get; set; }
        /// <summary>
        /// The y element.
        /// </summary>
        public decimal y { get; set; }
        /// <summary>
        /// Creates a 2d coordinates.
        /// </summary>
        /// <param name="x">X element</param>
        /// <param name="y">Y element</param>
        public Coords(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Creates a 2d coordinates.
        /// </summary>
        public Coords() { }
    }
}