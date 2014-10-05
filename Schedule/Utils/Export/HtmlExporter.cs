using System.IO;
using System.Text;
using System.Collections.Generic;
using Schedule.Controls.Editors;
using Schedule.Controls.Slices;
using Schedule.Models.ViewModels.Slices;
using System.Collections;
using System;
using System.Linq;
using Schedule.Utils.ValueConverters;
using System.Globalization;

namespace Schedule.Utils.Export
{
    class HtmlExporter : IExporter
    {
        public string FormatString()
        {
            return "HTML page (*.html)|*.html;*.htm";
        }

        public void Save(string path, object source, IEnumerable<Filter> filters)
        {
            StringBuilder htmlData = new StringBuilder();

            htmlData.AppendLine("<!DOCTYPE html>");
            htmlData.AppendLine("<html>");
            htmlData.AppendLine("<head>");
            htmlData.AppendLine("<meta charset=\"" + Encoding.UTF8.WebName + "\">");
            htmlData.AppendLine("<title>Schedule</title>");
            htmlData.AppendLine("<style type=\"text/css\">");
            htmlData.AppendLine(Properties.Resources.Html_Styles);
            htmlData.AppendLine("</style>");
            htmlData.AppendLine("</head>");
            htmlData.AppendLine("<body>");

            if (filters != null && filters.Count() > 0)
            {
                PropertyToStringConverter converter = new PropertyToStringConverter();
                htmlData.AppendLine("<table id=\"filters-table\">");
                htmlData.AppendLine("<caption>Applied filters</caption>");
                htmlData.AppendLine(@"<thead><tr>
				            <th>Property</th>
				            <th><div class=""v-header"">&nbsp;</div></th>
                            <th>Value</th>
                        </tr></thead><tbody>");
                foreach (var filter in filters)
                {
                    htmlData.AppendLine("<tr>");
                    htmlData.AppendLine("<td>" + (string)converter.Convert(filter.PropertiesBox.SelectedItem, typeof(string), null, CultureInfo.CurrentCulture) + "</td>");
                    htmlData.AppendLine("<td>" + filter.ConditionsBox.SelectedItem + "</td>");
                    htmlData.AppendLine("<td>" + filter.Value + "</td>");
                    htmlData.AppendLine("</tr>");
                }
                htmlData.AppendLine("</tbody></table>");
            }

            var view = source as SliceView;
            if (view != null)
            {
                var hItems = view.Header as IEnumerable;

                htmlData.AppendLine("<table id=\"schedule-table\"><caption>Schedule</caption><thead><tr>");
                htmlData.AppendLine("<th class=\"empty\">&nbsp;</th>");
                foreach (var item in hItems)
                    htmlData.AppendLine("<th>" + item + "</th>");
                htmlData.AppendLine("</tr></thead><tbody>");

                var rows = view.ItemsSource as IEnumerable<SliceRowViewModel>;
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        htmlData.AppendLine("<tr><th><div class=\"v-header\">" + row.Header + "</div></th>");
                        foreach (var cell in row.Items)
                        {
                            htmlData.AppendLine("<td>");
                            foreach (var card in cell.Items)
                            {
                                htmlData.AppendLine("<div class=\"entity\"><strong class=\"course\">" + card.Item.Course + "</strong>");
                                htmlData.AppendLine("<span class=\"type\">" + card.Item.Type + "</span>");
                                htmlData.AppendLine("<div class=\"clear\"></div>");
                                htmlData.AppendLine("<div class=\"teacher\">" + card.Item.Teacher + "</div>");
                                htmlData.AppendLine("<div class=\"clear\"></div>");
                                htmlData.AppendLine("<span class=\"group\">" + card.Item.Group + "</span><strong class=\"class\">" + card.Item.Class + "</strong>");
                                htmlData.AppendLine("<div class=\"clear\"></div></div>");
                            }
                            htmlData.AppendLine("</td>");
                        }
                        htmlData.AppendLine("</tr>");
                    }
                }
                htmlData.AppendLine("</tbody></table>");
            }

            htmlData.AppendLine(
            @"<script type=""text/javascript"">
                var h = document.getElementsByClassName(""v-header""); 
                for (var i = 0; i < h.length; i++) 
                    h[i].parentNode.setAttribute(""height"", headers[i].scrollWidth);
            </script>");
            htmlData.AppendLine("</body>");
            htmlData.AppendLine("</html>");

            using (FileStream fs = new FileStream(path, FileMode.Truncate))
            {
                using (var stream = new StreamWriter(fs, Encoding.UTF8))
                {
                    stream.Write(htmlData);
                }
            }
        }
    }
}
