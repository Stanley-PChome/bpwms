using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.Services
{
	public static class DataTableHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fullFilePath"></param>
		/// <param name="errorMsg"></param>
		/// <param name="headerRowIndex">-1代表沒有表頭</param>
		/// <returns></returns>
		public static DataTable ReadExcelDataTable(string fullFilePath, ref string errorMsg,int headerRowIndex=0)
		{
			errorMsg = string.Empty;
			var fileExtension = Path.GetExtension(fullFilePath);
			fileExtension = string.IsNullOrWhiteSpace(fileExtension) ? string.Empty : fileExtension.ToLower();

			try
			{
				using (var file = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
				{
					DataTable excelTable = null;
					byte[] bytes = new byte[file.Length];
					file.Read(bytes, 0, (int)file.Length);
					file.Position = 0;

					if (fileExtension == ".xls" || fileExtension == ".xlsx")
					{
						//註記: RenderDataTableFromExcel無法讀取CSV檔案
						excelTable = DataTableExtension.RenderDataTableFromExcel(file, 0, headerRowIndex);
					}
					else
					{
						errorMsg = "不支援的檔案格式!";
						return null;
					}
					return excelTable;
				}
			}
			catch (Exception ex)
			{
				errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, "匯入失敗!", true);
				return null;
			}

		}

	}
}
