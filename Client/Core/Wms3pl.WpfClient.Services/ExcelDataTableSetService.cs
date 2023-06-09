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
	public class ImportExcelClass
	{
		public string ClassColName { get; set; }
		public string ExcelColName { get; set; }
	}

	public partial class ExcelDataTableSetService
	{
		List<ImportExcelClass> exportList = new List<ImportExcelClass>();
		/// <summary>
		/// 欄位設定
		/// </summary>
		/// <param name="sourceCol">資料來源: Class Value</param>
		/// <param name="setCol">轉換Excel Title</param>
		/// <returns></returns>
		public ExcelDataTableSetService SetCol(string sourceCol, string setCol = null)
		{
			if (!string.IsNullOrEmpty(sourceCol))
			{
				ImportExcelClass exportClass = new ImportExcelClass();
				exportClass.ClassColName = sourceCol;
				exportClass.ExcelColName = setCol == null || string.IsNullOrEmpty(setCol) ? sourceCol : setCol;
				exportList.Add(exportClass);
			}
			return this;
		}

		public DataTable SetDataTable(DataTable dataSource)
		{
			var tmpData = dataSource.Copy();
			//remove 沒有設定的欄位
			foreach (DataColumn column in tmpData.Columns)
			{
				var haveCol = exportList.Where(o => o.ClassColName == column.ColumnName).FirstOrDefault();
				if (haveCol == null)
				{
					dataSource.Columns.Remove(column.ColumnName);
				}
			}
			//依設定順序將Datable 做排序
			var colIndex = 0;
			foreach (var col in exportList)
			{
				dataSource.Columns[col.ClassColName].SetOrdinal(colIndex);
				dataSource.Columns[col.ClassColName].ColumnName = col.ExcelColName;
				colIndex++;
			}
			return dataSource;
		}
	}
}
