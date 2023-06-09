using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Wms3pl.WpfClient.Services
{
	public static class DataTableExtension
	{
		public static Stream RenderDataTableToExcel(this DataTable sourceTable)
		{
			var workbook = new HSSFWorkbook();
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

		public static void RenderDataTableToExcel(this DataTable sourceTable, string fileName)
		{
			MemoryStream ms = RenderDataTableToExcel(sourceTable) as MemoryStream;
			FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
			byte[] data = ms.ToArray();

			fs.Write(data, 0, data.Length);
			fs.Flush();
			fs.Close();

			data = null;
			ms = null;
			fs = null;
		}

		public static DataTable RenderDataTableFromExcel(Stream excelFileStream, string sheetName, int headerRowIndex = 0, int cellCount = -1)
		{
			var workbook = WorkbookFactory.Create(excelFileStream);
			//指定的Sheet
			var sheet = workbook.GetSheet(sheetName);

			var table = ReadDataTableFromSheet(sheet, headerRowIndex, cellCount);

			//建議使用using開啟檔案，或是外部呼叫結束時控制關閉，不要在單純讀取資料的地方關閉
			//excelFileStream.Close();
			workbook = null;
			sheet = null;
			return table;
		}

		/// <summary>
		/// 轉出Excel資料至DataTable
		/// </summary>
		/// <param name="excelFileStream"></param>
		/// <param name="sheetIndex"></param>
		/// <param name="headerRowIndex">標題列位置，-1表示無標題列</param>
		/// <param name="cellCount">欄位數，若有指定標題列，則以標題列的欄位數為主，-1表示不指定欄位數</param>
		/// <returns></returns>
		public static DataTable RenderDataTableFromExcel(Stream excelFileStream, int sheetIndex = 0, int headerRowIndex = 0, int cellCount = -1)
		{
			var workbook = WorkbookFactory.Create(excelFileStream);
			//指定的Sheet
			var sheet = workbook.GetSheetAt(sheetIndex);
			//指定為Header的Row


			var table = ReadDataTableFromSheet(sheet, headerRowIndex, cellCount);
			//建議使用using開啟檔案，或是外部呼叫結束時控制關閉，不要在單純讀取資料的地方關閉
			//excelFileStream.Close(); //hint:
			workbook = null;
			sheet = null;
			return table;
		}

		/// <summary>
		/// 讀取Excel的Sheet轉換為DataTable
		/// </summary>
		private static DataTable ReadDataTableFromSheet(ISheet sheet, int headerRowIndex, int cellCount = -1)
		{
			var table = new DataTable();

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
				if (firstRow == 0)
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
			for (int i = firstRow; i <= sheet.LastRowNum; i++)
			{
				var row = sheet.GetRow(i);
                if (row == null || row.Cells.Count == 0)
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