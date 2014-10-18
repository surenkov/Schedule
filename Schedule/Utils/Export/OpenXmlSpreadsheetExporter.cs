using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using Schedule.Controls.Slices;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils.ValueConverters;

namespace Schedule.Utils.Export
{
    internal class OpenXmlSpreadsheetExporter : IExporter
    {
        public string FormatString()
        {
            return "Microsoft Excel (*.xlsx)|*.xlsx";
        }

        public Type SourceType()
        {
            return typeof(SliceView);
        }

        public void Save(string path, object source)
        {
            var workbook = new XLWorkbook();
            var view = source as SliceView;

            if (view != null)
            {
                SaveSchedule(workbook, view.ItemsSource as IEnumerable<SliceRowViewModel>);
                SaveFilters(workbook, view.Filters);
            }

            try
            {
                workbook.SaveAs(path);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, "Excel export");
            }
        }

        private void SaveFilters(XLWorkbook workbook, IEnumerable<Controls.Editors.Filter> filters)
        {
            if (filters == null || workbook == null) return;

            var filtersWorksheet = workbook.Worksheets.Add("Filters");
            DataTable filtersTable = new DataTable();
            PropertyToStringConverter converter = new PropertyToStringConverter();

            filtersTable.Columns.Add("Property", typeof(string));
            filtersTable.Columns.Add("#", typeof(string));
            filtersTable.Columns.Add("Value", typeof(object));

            foreach (var filter in filters)
            {
                string propName = (string)converter.Convert(filter.PropertiesBox.SelectedItem,
                    typeof(string), null, CultureInfo.CurrentCulture);
                string comparer = filter.ConditionsBox.SelectedItem.ToString();
                object value = filter.Value;
                filtersTable.Rows.Add(propName, comparer, value);
            }

            var ssTable = filtersWorksheet.Cell(2, 2).InsertTable(filtersTable);
            filtersWorksheet.Column(2).Width = 15;
            filtersWorksheet.Column(3).Width = 3;
            filtersWorksheet.Column(4).Width = 30;
            filtersWorksheet.Column(4).Style.Alignment.WrapText = true;

            ssTable.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ssTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        }

        private void SaveSchedule(XLWorkbook workbook, IEnumerable<SliceRowViewModel> rows)
        {
            if (rows == null || workbook == null) return;

            const int cellWidth = 2, cellHeight = 3;
            const int startX = 3, startY = 3;

            int maxY = startY;
            int maxX = startX;

            var scheduleWorksheet = workbook.Worksheets.Add("Schedule");
            foreach (var row in rows)
            {
                int cellCount = Math.Max(row.Items.Max(p => p.Items.Count()), 1);
                maxY += cellHeight * cellCount;
            }

            int currentX = 0;
            var firsRow = rows.FirstOrDefault();
            if (firsRow != null)
            {
                maxX = cellWidth * firsRow.Items.Count();
                foreach (var cell in firsRow.Items)
                {
                    var hHeader =
                        scheduleWorksheet.Range(startY - 1, currentX + startX, startY - 1, currentX + startX + cellWidth - 1);
                    hHeader.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    hHeader.Style.Fill.BackgroundColor = XLColor.LightGray;
                    hHeader.Merge();

                    hHeader.Cell(1, 1).Value = cell.HorizontalValue;
                    hHeader.Cell(1, 1).Style.Alignment.WrapText = true;

                    currentX += cellWidth;
                }
            }

            var tableRange = scheduleWorksheet.Range(startY, startX, maxY - 1, maxX + startX - 1);

            int currentY = currentX = maxY = 1;
            foreach (var row in rows)
            {
                currentX = 1;
                foreach (var cell in row.Items)
                {
                    currentY = maxY;
                    tableRange.Column(currentX).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    foreach (var card in cell.Items)
                    {
                        using (var cellRange = tableRange.Range(currentY, currentX, currentY + cellHeight - 1, currentX + cellWidth - 1))
                        {
                            cellRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            scheduleWorksheet.Column(startX + currentX - 1).Width = 17.0;

                            cellRange.Cell(1, 1).Style.Alignment.WrapText = true;
                            cellRange.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            cellRange.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            cellRange.Cell(1, 1).Style.Font.Bold = true;
                            cellRange.Cell(1, 1).Value = card.Item.Course;

                            cellRange.Cell(1, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            cellRange.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cellRange.Cell(1, 2).Value = card.Item.Type;

                            cellRange.Range(2, 1, 2, 2).Merge();
                            cellRange.Cell(2, 1).Style.Alignment.WrapText = true;
                            cellRange.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cellRange.Cell(2, 1).Style.Font.FontSize = 9.0;
                            cellRange.Cell(2, 1).Value = card.Item.Teacher;

                            cellRange.Cell(3, 1).Value = card.Item.Group;
                            cellRange.Cell(3, 2).Value = card.Item.Class;
                            cellRange.Cell(3, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            cellRange.Cell(3, 2).Style.Font.Bold = true;
                        }
                        currentY += cellHeight;
                    }
                    currentX += cellWidth;
                }
                maxY += cellHeight * Math.Max(row.Items.Max(p => p.Items.Count()), 1);
            }

            currentY = maxY = 0;
            foreach (var row in rows)
            {
                maxY += cellHeight * Math.Max(row.Items.Max(p => p.Items.Count()), 1);
                tableRange.Row(maxY).Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                var vHeader = scheduleWorksheet.Range(currentY + startY, startX - 1,
                    maxY + startY - 1, startX - 1);
                vHeader.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                vHeader.Style.Fill.BackgroundColor = XLColor.LightGray;
                vHeader.Merge();

                var cell = vHeader.Cell(1, 1);
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.TextRotation = 90;
                cell.Style.Alignment.WrapText = true;
                cell.WorksheetColumn().Width = 2.3;
                cell.Value = row.Header;

                currentY = maxY;
            }

            var hCell = scheduleWorksheet.Cell(startY - 1, startX - 1);
            hCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            hCell.Style.Fill.BackgroundColor = XLColor.LightGray;

            tableRange.Style.Fill.BackgroundColor = XLColor.White;
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        }
    }
}