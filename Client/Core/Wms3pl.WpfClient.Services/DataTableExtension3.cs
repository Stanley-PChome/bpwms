using System.Data;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

//For Office 2007+

namespace Wms3pl.WpfClient.Services
{
	public static class DataTableExtension3
	{
		public static Stream RenderDataTableToExcelFor2007(this DataTable sourceTable)
		{
			XSSFWorkbook workbook = new XSSFWorkbook();
			MemoryStream ms = new MemoryStream();
			var sheet = workbook.CreateSheet();
			var headerRow = sheet.CreateRow(0);

			// handling header.
			foreach (DataColumn column in sourceTable.Columns)
				headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

			// handling value.
			int rowIndex = 1;

			foreach (DataRow row in sourceTable.Rows)
			{
				var dataRow = sheet.CreateRow(rowIndex);

				foreach (DataColumn column in sourceTable.Columns)
				{
					dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
				}

				rowIndex++;
			}

			workbook.Write(ms);
			ms.Flush();
			ms.Position = 0;

			sheet = null;
			headerRow = null;
			workbook = null;

			return ms;
		}

		public static void RenderDataTableToExcelFor2007(this DataTable sourceTable, string fileName)
		{
			MemoryStream ms = RenderDataTableToExcelFor2007(sourceTable) as MemoryStream;
			FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
			byte[] data = ms.ToArray();

			fs.Write(data, 0, data.Length);
			fs.Flush();
			fs.Close();

			data = null;
			ms = null;
			fs = null;
		}

		public static DataTable RenderDataTableFromExcelFor2007(Stream excelFileStream, string SheetName, int headerRowIndex, int cellCount = -1)
		{
			XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
			var sheet = workbook.GetSheet(SheetName);

			var table = ReadDataTableFromSheet(sheet, headerRowIndex, cellCount);

			excelFileStream.Close();
			workbook = null;
			sheet = null;
			return table;
		}

		public static DataTable RenderDataTableFromExcelFor2007(Stream excelFileStream, int sheetIndex, int headerRowIndex, int cellCount = -1)
		{
			XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
			var sheet = workbook.GetSheetAt(sheetIndex);

			var table = ReadDataTableFromSheet(sheet, headerRowIndex, cellCount);

			excelFileStream.Close();
			workbook = null;
			sheet = null;
			return table;
		}

		private static DataTable ReadDataTableFromSheet(ISheet sheet, int headerRowIndex, int cellCount = -1)
		{
			DataTable table = new DataTable();

			IRow headerRow = null;
			if (headerRowIndex != -1)
				headerRow = sheet.GetRow(headerRowIndex);

			var firstRow = sheet.FirstRowNum + headerRowIndex + 1;
			if (headerRow == null)
			{
				headerRow = sheet.GetRow(0);
				firstRow = sheet.FirstRowNum;
			}
			else
				cellCount = headerRow.LastCellNum;

			if (cellCount == -1)
				cellCount = headerRow.LastCellNum;

			for (int i = headerRow.FirstCellNum; i < cellCount; i++)
			{
				if (firstRow == sheet.FirstRowNum)
				{
					var column = new DataColumn("Col" + i.ToString());
					table.Columns.Add(column);
				}
				else
				{
					var column = new DataColumn(headerRow.GetCell(i).ToString());
					table.Columns.Add(column);
				}
			}

			int rowCount = sheet.LastRowNum;

			for (int i = firstRow; i <= sheet.LastRowNum; i++)
			{
				var row = sheet.GetRow(i);
				if (row == null)
					continue;

				// 是否為空白Row
				bool isEmptyRow = true;
				var dataRow = table.NewRow();
				for (int j = row.FirstCellNum; j < cellCount; j++)
				{
					var cell = row.GetCell(j);
					if (cell != null)
					{
						var content = cell.ToString();
						dataRow[j] = content;

						if (!string.IsNullOrWhiteSpace(content))
							isEmptyRow = false;
					}
				}

				if (!isEmptyRow)
					table.Rows.Add(dataRow);
			}

			return table;
		}


	}
}