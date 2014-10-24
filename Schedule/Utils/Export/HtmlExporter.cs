using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Schedule.Controls.Slices;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils.ValueConverters;

namespace Schedule.Utils.Export
{
    class HtmlExporter : Exporter
    {
        public override string FormatString()
        {
            return "HTML page (*.html)|*.html;*.htm";
        }

        public override Type SourceType()
        {
            return typeof(SliceView);
        }

        public override void Save(string path, object source)
        {
            StringBuilder htmlData = new StringBuilder();

            htmlData.AppendLine("<!DOCTYPE html>");
            htmlData.Append("<html>");
            htmlData.Append("<head>");
            htmlData.Append("<meta charset=\"" + Encoding.UTF8.WebName + "\">");
            htmlData.Append("<title>Schedule</title>");
            htmlData.Append("<style type=\"text/css\">");
            htmlData.Append(Properties.Resources.Html_Styles);
            htmlData.Append("</style>");
            htmlData.Append("</head>");
            htmlData.Append("<body>");

            var view = source as SliceView;
            if (view != null)
            {
                var filters = view.Filters;
                if (filters != null && filters.Count() > 0)
                {
                    PropertyToStringConverter converter = new PropertyToStringConverter();
                    htmlData.Append("<table id=\"filters-table\">");
                    htmlData.Append("<caption>Applied filters</caption>");
                    htmlData.Append(@"<thead><tr><th>Property</th><th><div class=""v-header"">&nbsp;</div></th><th>Value</th</tr></thead><tbody>");
                    foreach (var filter in filters)
                    {
                        htmlData.Append("<tr>");
                        htmlData.Append("<td>" + (string)converter.Convert(filter.PropertiesBox.SelectedItem, typeof(string), null, CultureInfo.CurrentCulture) + "</td>");
                        htmlData.Append("<td>" + filter.ConditionsBox.SelectedItem + "</td>");
                        htmlData.Append("<td>" + filter.Value + "</td>");
                        htmlData.Append("</tr>");
                    }
                    htmlData.Append("</tbody></table>");
                }

                var hItems = view.Header as IEnumerable;
                htmlData.Append("<table id=\"schedule-table\"><caption>Schedule</caption><thead><tr>");
                htmlData.Append("<th class=\"empty\">&nbsp;</th>");
                foreach (var item in hItems)
                    htmlData.Append("<th>" + item + "</th>");
                htmlData.Append("</tr></thead><tbody>");

                var rows = view.ItemsSource as IEnumerable<SliceRowViewModel>;
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        htmlData.Append("<tr><th>" + row.Header + "</th>");
                        foreach (var cell in row.Items)
                        {
                            htmlData.Append("<td>");
                            foreach (var card in cell.Items)
                            {
                                htmlData.Append("<div class=\"entity\"><strong class=\"course\">" + card.Item.Course + "</strong>");
                                htmlData.Append("<span class=\"type\">" + card.Item.Type + "</span>");
                                htmlData.Append("<div class=\"clear\"></div>");
                                htmlData.Append("<div class=\"teacher\">" + card.Item.Teacher + "</div>");
                                htmlData.Append("<div class=\"clear\"></div>");
                                htmlData.Append("<span class=\"group\">" + card.Item.Group + "</span><strong class=\"class\">" + card.Item.Class + "</strong>");
                                htmlData.Append("<div class=\"clear\"></div></div>");
                            }
                            htmlData.Append("</td>");
                        }
                        htmlData.Append("</tr>");
                    }
                }
                htmlData.Append("</tbody></table>");
            }
            htmlData.Append("</body>");
            htmlData.Append("</html>");

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                fs.Flush();
                using (var stream = new StreamWriter(fs, Encoding.UTF8))
                {
                    stream.Write(htmlData);
                }
            }
        }
    }
}
