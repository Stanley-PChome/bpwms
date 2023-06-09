using System;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Wms3pl.WpfClient.Common;
using DataTable = System.Data.DataTable;
using System.Collections.Generic;
namespace Wms3pl.WpfClient.Services
{
	public class ExcelExportReportSource
	{
		/// <summary>
		/// 新增資料表頭上方資訊 如果沒有請設定為null 如果要加入一行空白列請新增null至清單裡
		/// </summary>
		public List<List<NameValuePair<string>>> DataHeaderList { get; set; }

		/// <summary>
		/// 資料表來源
		/// </summary>
		public DataTable Data { get; set; }

		/// <summary>
		/// 設定欄位格式
		/// </summary>
		public List<NameValuePair<string>> DataFormatList { get; set; }
	}

	public partial class ExcelExportService
	{
		private XSSFWorkbook _workbook;
		private ISheet _sheet = null;
		private List<string> _titleList = null;
		private List<ExcelExportReportSource> _exportReportSources = null;
		public ExcelExportService()
		{
			_workbook = new XSSFWorkbook();
			var cellStyle = _workbook.CreateCellStyle();
			var font = _workbook.CreateFont();
			font.FontHeightInPoints = 18;
			cellStyle.SetFont(font);
			cellStyle.Alignment = HorizontalAlignment.Center;
			cellStyle = _workbook.CreateCellStyle();
			font = _workbook.CreateFont();
			font.FontHeightInPoints = 12;
			font.Boldweight = (short)FontBoldWeight.Bold;
			cellStyle.SetFont(font);
			ShowTitle = true;

		}
		public bool ShowTitle { get; set; }
		/// <summary>
		/// 建立一張新的工作表
		/// </summary>
		/// <param name="sheetName"></param>
		public void CreateNewSheet(string sheetName)
		{
			if (_sheet != null)
				BindSheetContent();
			_sheet = _workbook.CreateSheet(sheetName);
			_titleList = null;
			_exportReportSources = null;
		}

		/// <summary>
		/// 新增Sheet表頭資訊
		/// </summary>
		/// <param name="title">表頭資訊 若設null代表空白一列</param>
		public void AddNewTitle(string title)
		{
			if (_titleList == null)
				_titleList = new List<string>();
			_titleList.Add(title);
		}

		/// <summary>
		/// 新增報表資料來源
		/// </summary>
		/// <param name="exportReportSource"></param>
		public void AddExportReportSource(ExcelExportReportSource exportReportSource)
		{
			if (exportReportSource != null)
			{
				if (_exportReportSources == null)
					_exportReportSources = new List<ExcelExportReportSource>();
				_exportReportSources.Add(exportReportSource);
			}
		}


		private void BindSheetContent()
		{

			if (_sheet != null)
			{
				int maxColunm = 1;
				if (_exportReportSources != null)
					maxColunm = _exportReportSources.Where(o => o.Data != null).Max(o => o.Data.Columns.Count);

				int rowIndex = 0;
				if (_titleList != null)
				{
					foreach (var title in _titleList) //產生至中表頭
					{
						var titleRow = _sheet.CreateRow(rowIndex);
						if (title != null)
						{
							_sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, maxColunm - 1));
							titleRow.CreateCell(0).SetCellValue(title);
							titleRow.GetCell(0).CellStyle = _workbook.GetCellStyleAt(1);
						}
						rowIndex++;
					}
				}
				if (_exportReportSources != null)
				{
					foreach (var exportReportSource in _exportReportSources)
					{
						if (exportReportSource.DataHeaderList != null)
						{
							foreach (List<NameValuePair<string>> dataheader in exportReportSource.DataHeaderList)
							{
								var dataHeaderRow = _sheet.CreateRow(rowIndex);
								if (dataheader != null)
								{
									var cellIndex = 0;
									foreach (var nameValuePair in dataheader)
									{
										dataHeaderRow.CreateCell(cellIndex).SetCellValue(nameValuePair.Name);
										dataHeaderRow.GetCell(cellIndex).CellStyle = _workbook.GetCellStyleAt(2);
										dataHeaderRow.CreateCell(cellIndex + 1).SetCellValue(nameValuePair.Value);
										cellIndex += 2;
									}
								}
								rowIndex++;
							}
						}
						if (exportReportSource.Data != null)
						{
							//產生資料表內容
							var sourceTable = exportReportSource.Data;
							//資料表表頭
							if (ShowTitle)
							{
								var headerRow = _sheet.CreateRow(rowIndex);
								foreach (DataColumn column in sourceTable.Columns)
								{
									headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
									headerRow.GetCell(column.Ordinal).CellStyle = _workbook.GetCellStyleAt(2);
									_sheet.AutoSizeColumn(column.Ordinal);
								}
								rowIndex++;
							}
							//資料表內容
							foreach (DataRow row in sourceTable.Rows)
							{
								var dataRow = _sheet.CreateRow(rowIndex);
								foreach (DataColumn column in sourceTable.Columns)
								{
									var cell = dataRow.CreateCell(column.Ordinal);
									if (exportReportSource.DataFormatList != null)
									{
										var format = exportReportSource.DataFormatList.Find(o => o.Name == column.ColumnName);
										if (format != null)
										{
											var currencyCellStyle = _workbook.CreateCellStyle();
											var newDataFormat = _workbook.CreateDataFormat();
											currencyCellStyle.DataFormat = newDataFormat.GetFormat(format.Value);
											cell.CellStyle = currencyCellStyle;
											if (column.DataType == Type.GetType("System.DateTime") && row[column] != null)
											{
												if (format.Value == "yyyy/m/d")
													cell.SetCellValue(DateTime.Parse(row[column].ToString()).ToString("yyyy/MM/dd"));
												else if (format.Value == "yyyy/m/d hh:mm")
													cell.SetCellValue(DateTime.Parse(row[column].ToString()).ToString("yyyy/MM/dd HH:mm"));
												else
													cell.SetCellValue(DateTime.Parse(row[column].ToString()).ToString("yyyy/MM/dd HH:mm:ss"));
											}
											else
												cell.SetCellValue((row[column] ?? "").ToString());
										}
										else
											cell.SetCellValue((row[column] ?? "").ToString());
									}
									else
										cell.SetCellValue((row[column] ?? "").ToString());
								}
								rowIndex++;
							}
						}
						//產生兩列空白列
						_sheet.CreateRow(rowIndex);
						rowIndex++;
						_sheet.CreateRow(rowIndex);
						rowIndex++;
					}
				}
			}
		}

		public bool Export(string filePath, string fileName)
		{
			try
			{
				BindSheetContent();
				var ms = new MemoryStream();
				_workbook.Write(ms);
				var fs = new FileStream(string.Format("{0}\\{1}", filePath, fileName), FileMode.Create, FileAccess.Write);
				byte[] data = ms.ToArray();
				fs.Write(data, 0, data.Length);
				fs.Flush();
				fs.Close();
				ms.Flush();
				ms.Close();
				_workbook = new XSSFWorkbook();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
