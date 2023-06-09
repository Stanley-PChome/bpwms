using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F70DataService;
using wcfR01 = Wms3pl.WpfClient.ExDataServices.R01WcfService;
using System.Runtime.Serialization;
using Wms3pl.WpfClient.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Configuration;
using ConsoleUtility.Helpers;

namespace Wms3pl.RefineModule.Consoles.MoveGoldLoc
{
	class Program
	{
		private static AppConfig _appConfig;


		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#if DEBUG
			args = new string[] { "-SchemaName=PHWMS_DEV" };
#endif
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			SetAppConfig();
			SetArgConfig(args);
			MoveGoldLocs();
		}
		/// <summary>
		/// 設定Ftp物件
		/// </summary>
		private static void SetAppConfig()
		{
			_appConfig = new AppConfig()
			{
				FilePath = string.Format(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd")),
			};
		}
		private static void SetArgConfig(string[] args)
		{
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ExceptionPolicy.HandleException(e.ExceptionObject as Exception, "Default Policy");
			Environment.Exit(1);
		}


		private static void MoveGoldLocs()
		{
			/// <param name="schRunDay">(排程為一週執行一次 , 故設 7)</param>
			/// <param name="baseDay">過去 x 個月以來，平均出貨間隔天數<=3天</param>
			/// <param name="avgOrders">間隔天數<=3天。 </param>
			ConsoleHelper.FilePath = _appConfig.FilePath;
			ConsoleHelper.Log($" *** MoveGoldLocs排程Start ***");
			var wcf = new wcfR01.R01WcfServiceClient();
			var result = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
														, () => wcf.GetSchOrderNormalData(7, 90, 3), false, _appConfig.SchemaName, "MoveGoldLocs_GetSchOrderNormalData");

			

			string fileName = "RefineMoveGoldLoc.xlsx";
			//var fileName = string.Format(_fileName, DateTime.Now.Year
			//											, DateTime.Now.Month.ToString().PadLeft(2, '0')
			//											, DateTime.Now.Day.ToString().PadLeft(2, '0'));

			//Group by dc / gup /cust
			var groupData = result.GroupBy(a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE })
											.Select(g => new
											{
												DC_CODE = g.Key.DC_CODE,
												GUP_CODE = g.Key.GUP_CODE,
												CUST_CODE = g.Key.CUST_CODE
											}).ToList();
			foreach (var item in groupData)
			{
				var importData = result.Where(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE);

				//產生Excel
				string filePath;
				var resultExcel = ImportExcel(importData.ToList(), fileName, out filePath);
				if (resultExcel)
				{
					//新增行事曆
					F700501 f700501 = new F700501
					{
						SCHEDULE_DATE = DateTime.Today,
						SCHEDULE_TIME = "10:00",
						IMPORTANCE = "1", //重要性(0低1一般2高)
						SCHEDULE_TYPE = "S",
						SUBJECT = "一般揀貨轉黃金揀貨區",
						CONTENT = "一般揀貨轉黃金揀貨區",
						DC_CODE = item.DC_CODE,
						FILE_NAME = filePath
					};

					var wcf700501 = ExDataMapper.Map<F700501, wcfR01.F700501>(f700501);
					var resultP70 = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel, () => wcf.InsertF700501(wcf700501), false, _appConfig.SchemaName, "MoveGoldLocs_InsertF700501");

				}
			}
			ConsoleHelper.Log($"*** MoveGoldLocs排程 End ***");
			ConsoleHelper.Log(string.Empty, false);
		}


		private static bool ImportExcel(List<wcfR01.SchF700501Data> importData, string fileName,out string filePath)
		{

			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.DefaultExt = ".xlsx";
				saveFileDialog.Filter = "Excel (.xlsx)|*.xlsx";
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.OverwritePrompt = true;

				var excelExportService = new ExcelExportService();
				excelExportService.CreateNewSheet("移入黃金揀貨區名單");
				var excelReportDataSource = new ExcelExportReportSource();
				var detailDataTable = importData.ToDataTable("");

				#region 更名
				detailDataTable.Columns.Remove("ExtensionData");
				detailDataTable.Columns.Remove("AREA_CODE");
				detailDataTable.Columns.Remove("A_DELV_QTY");
				detailDataTable.Columns.Remove("WAREHOUSE_ID");
				detailDataTable.Columns.Remove("ORDER_COUNT");
				detailDataTable.Columns.Remove("DC_CODE");
				detailDataTable.Columns.Remove("GUP_CODE");
				detailDataTable.Columns.Remove("CUST_CODE");
				detailDataTable.Columns.Remove("DELV_DATE");

				detailDataTable.Columns["DC_NAME"].ColumnName = "物流中心";
				detailDataTable.Columns["GUP_NAME"].ColumnName = "業主";
				detailDataTable.Columns["CUST_NAME"].ColumnName = "貨主";
				detailDataTable.Columns["ITEM_CODE"].ColumnName = "品號";
				detailDataTable.Columns["ITEM_NAME"].ColumnName = "品名";
				detailDataTable.Columns["ITEM_COLOR"].ColumnName = "顏色";
				detailDataTable.Columns["ITEM_SIZE"].ColumnName = "尺寸";
				detailDataTable.Columns["ITEM_SPEC"].ColumnName = "規格";
				detailDataTable.Columns["MEMO"].ColumnName = "條件";
				detailDataTable.Columns["MEMO1"].ColumnName = "建議動作";
				#endregion

				excelReportDataSource.Data = detailDataTable;
				excelExportService.AddExportReportSource(excelReportDataSource);
				//var filepath = ConfigurationManager.AppSettings["FilePath"];

				var filepath = $"{ConsoleHelper.FilePath}{importData.First().DC_CODE}_{importData.First().GUP_CODE}_{importData.First().CUST_CODE}";

				if (!Directory.Exists(filepath))
					Directory.CreateDirectory(filepath);

				filePath = Path.Combine(filepath, fileName);
				return excelExportService.Export(filepath, fileName);

			}
			return false;
		}


	}
}
