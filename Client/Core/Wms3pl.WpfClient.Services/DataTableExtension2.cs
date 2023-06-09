using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Wms3pl.WpfClient.Services
{
  public static class DataTableExtension2
  {
    public static void RenderDataTableToExcel2007(this DataTable sourceTable, string exportFile)
    {
      ExcelDocument excelDocument = new ExcelDocument();
      excelDocument.CreatePackage(exportFile);

      using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(exportFile, true))
      {
        WorkbookPart workbook = spreadsheet.WorkbookPart;
        WorksheetPart worksheet = workbook.WorksheetParts.Last();
        SheetData data = worksheet.Worksheet.GetFirstChild<SheetData>();
        //if (bestFit)
          CalculateAutoFitWidth(sourceTable, worksheet);
        Row header = new Row();
        header.RowIndex = (UInt32)1;


        for (int i = 0; i < sourceTable.Columns.Count; i++)
        {
          DataColumn column = sourceTable.Columns[i];
          Cell headerCell = CreateTextCell(sourceTable.Columns.IndexOf(column) + 1,
              1, column.ColumnName);
          header.AppendChild(headerCell);
        }
        data.AppendChild(header);

        for (int i = 0; i < sourceTable.Rows.Count; i++)
        {
          DataRow contentRow = sourceTable.Rows[i];
          data.AppendChild(CreateContentRow(contentRow, i + 2));
        }

        
      }
    }

    private static void CalculateAutoFitWidth(DataTable sourceTable, WorksheetPart worksheet)
    {
      double[] fitWidths = new double[sourceTable.Columns.Count];
      for (int i = 0; i < sourceTable.Columns.Count; i++)
      {
        DataColumn column = sourceTable.Columns[i];
        var width = GetWidth("Calibri", 11, column.ColumnName);
        fitWidths[i] = Math.Max(width, fitWidths[i]);
      }

      for (int i = 0; i < sourceTable.Rows.Count; i++)
      {
        DataRow contentRow = sourceTable.Rows[i];
        for (int j = 0; j < sourceTable.Columns.Count; j++)
        {
          if (contentRow[j] != null)
          {
            var text = contentRow[j].ToString();
            var width = GetWidth("Calibri", 11, text);
            fitWidths[j] = Math.Max(width, fitWidths[j]);
          }
        }
      }

      Columns columns = worksheet.Worksheet.GetFirstChild<Columns>();
      
      for (int i= 0; i < fitWidths.Length; i++)
      {
        var width = fitWidths[i];
        var unit = (UInt32Value) (1U*(i + 1));
        var column = new Column { Min = unit, Max = unit, Width = width, CustomWidth = true, BestFit = true };
        columns.Append(column);
      }
    }


    private static Cell CreateTextCell(int columnIndex, int rowIndex, object cellValue)
    {
      Cell cell = new Cell();

      cell.DataType = CellValues.InlineString;
      cell.CellReference = GetColumnName(columnIndex) + rowIndex;

      InlineString inlineString = new InlineString();
      Text t = new Text();

      t.Text = cellValue.ToString();
      inlineString.AppendChild(t);
      cell.AppendChild(inlineString);

      return cell;
    }

    private static Row CreateContentRow(DataRow dataRow, int rowIndex)
    {
      Row row = new Row { RowIndex = (UInt32)rowIndex };

      for (int i = 0; i < dataRow.Table.Columns.Count; i++)
      {
        Cell dataCell = CreateTextCell(i + 1, rowIndex, dataRow[i]);
        row.AppendChild(dataCell);
      }
      return row;
    }

    private static string GetColumnName(int columnIndex)
    {
      int dividend = columnIndex;
      string columnName = String.Empty;
      int modifier;

      while (dividend > 0)
      {
        modifier = (dividend - 1) % 26;
        columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
        dividend = (int)((dividend - modifier) / 26);
      }

      return columnName;
    }

    private static double GetWidth(string font, int fontSize, string text)
    {
      System.Drawing.Font stringFont = new System.Drawing.Font(font, fontSize);
      return GetWidth(stringFont, text);
    }

    private static double GetWidth(System.Drawing.Font stringFont, string text)
    {
      // This formula is based on this article plus a nudge ( + 0.2M )
      // http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet.column.width.aspx
      // Truncate(((256 * Solve_For_This + Truncate(128 / 7)) / 256) * 7) = DeterminePixelsOfString

      Size textSize = TextRenderer.MeasureText(text, stringFont);
      double width = (double)(((textSize.Width / (double)7) * 256) - (128 / 7)) / 256;
      width = (double)decimal.Round((decimal)width + 0.2M, 2);

      return width;
    }

    public static string ExportDataAsExcelString(this DataTable dataTable)
    {
      StringBuilder sb = new StringBuilder();

      List<string> headers = new List<string>();
      for (int i = 0; i < dataTable.Columns.Count; i++)
      {
        DataColumn column = dataTable.Columns[i];
        headers.Add(column.ColumnName);
      }
      sb.AppendLine(string.Join("\t", headers.ToArray()));


      for (int i = 0; i < dataTable.Rows.Count; i++)
      {
        DataRow row = dataTable.Rows[i];
        var s = string.Join("\t", row.ItemArray);
        sb.AppendLine(s);
      }
      return sb.ToString();
    }

		public static void ExportDataAsCSVString(this DataTable dataTable, string exportFile)
    {
        var sb = new StringBuilder();

        List<string> headers = new List<string>();
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            DataColumn column = dataTable.Columns[i];
            headers.Add(column.ColumnName);
        }
        sb.AppendLine(string.Join(",", headers.ToArray()));

        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            DataRow row = dataTable.Rows[i];
            var s = string.Join(",", row.ItemArray);
            sb.AppendLine(s);
        }

				System.IO.File.WriteAllText(exportFile, sb.ToString(), Encoding.GetEncoding(950));
    }

    public static void ExportDataAsCSVString(this DataTable dataTable, string exportFile, Encoding encoding)
    {
        StringBuilder sb = new StringBuilder();

        List<string> headers = new List<string>();
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            DataColumn column = dataTable.Columns[i];
            headers.Add(column.ColumnName);
        }
        sb.AppendLine(string.Join(",", headers.ToArray()));


        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            DataRow row = dataTable.Rows[i];
            var s = string.Join(",", row.ItemArray);
            sb.AppendLine(s.ToString());
        }

        System.IO.File.WriteAllText(exportFile, sb.ToString(), encoding);
    }


    public static void ExportDataAsExcelString(this DataTable dataTable, string exportFile, Encoding encoding)
    {
        StringBuilder sb = new StringBuilder();

        List<string> headers = new List<string>();
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            DataColumn column = dataTable.Columns[i];
            headers.Add(column.ColumnName);
        }
        sb.AppendLine(string.Join("\t", headers.ToArray()));


        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            DataRow row = dataTable.Rows[i];
            var s = string.Join("\t", row.ItemArray);
            sb.AppendLine(s.ToString());
        }

        System.IO.File.WriteAllText(exportFile, sb.ToString(), encoding);
    }

  }
}
