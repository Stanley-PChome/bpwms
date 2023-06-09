using ConsoleUtility.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using exportWcf = Wms3pl.WpfClient.ExDataServices.ExportWcfService;
using Wms3pl.Datas.Shared.ApiEntities;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wms3pl.ScheduleModule.Consoles.ExportData
{
	/// <summary>
	/// API資料轉入WMS排程
	/// </summary>
	class Program
	{
		private static AppConfig _appConfig;
		private static exportWcf.ExportResultReq param;

		static void Main(string[] args)
		{
			SetAppConfig();
			SetArgConfig(args);
			Init();
			Execute();
		}

		private static void SetAppConfig()
		{
			_appConfig = new AppConfig()
			{
				FilePath = string.Format(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd"))
			};
		}

		/// <summary>
		/// 設定指定參數物件
		/// </summary>
		/// <param name="args"></param>
		private static void SetArgConfig(string[] args)
		{
			param = new exportWcf.ExportResultReq();
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, param);
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if (DEBUG)
            _appConfig.SchemaName = "PHWMS_DEV";
            //param.DcCode = "001";
            //param.GupCode = "01";
            //param.CustCode = "";
            param.ScheduleNo = "01";
#endif
		}

		/// <summary>
		/// 起始
		/// </summary>
		private static void Init()
		{
			if (!Directory.Exists(_appConfig.FilePath))
				Directory.CreateDirectory(_appConfig.FilePath);
		}

		#region 排程
		private static void Execute()
		{
			switch (param.ScheduleNo)
			{
				case "01":// 進倉回檔
					ExecuteSchedule("ExportWarehouseInResults", "商品進倉結果回傳");
					break;
				case "02":// 訂單回檔
					ExecuteSchedule("ExportOrderResults", "客戶訂單結果回傳");
					break;
				case "03":// 廠退回檔
					ExecuteSchedule("ExportVendorReturnResults", "廠退出貨結果回傳");
					break;
				case "04":// 庫存警示回報
					ExecuteSchedule("ExpStockAlertResults", "庫存警示回報排程");
					break;
				case "05":// 單箱出貨回檔
					ExecuteSchedule("ExportHomeDeliveryResults", "單箱出貨扣帳結果回傳");
					break;
				case "06":// 庫存異動回報
					ExecuteSchedule("ExportStockMovementResults", "庫存異動回報");
					break;
                case "07":// LMS系統異常通知
                    ExecuteSchedule("ExportSysErrorNotifyResults", "LMS系統異常通知");
                    break;
                case null:
					ExecuteSchedule("ExportWarehouseInResults", "商品進倉結果回傳");
					ExecuteSchedule("ExportOrderResults", "客戶訂單結果回傳");
					ExecuteSchedule("ExportVendorReturnResults", "廠退出貨結果回傳");
					ExecuteSchedule("ExpStockAlertResults", "庫存警示回報排程");
					ExecuteSchedule("ExportHomeDeliveryResults", "單箱出貨扣帳結果回傳");
					ExecuteSchedule("ExportStockMovementResults", "庫存異動回報");
                    ExecuteSchedule("ExportSysErrorNotifyResults", "LMS系統異常通知");
                    break;
			}
		}

		/// <summary>
		/// 排程執行
		/// </summary>
		private static void ExecuteSchedule(string type, string name)
		{
			ConsoleHelper.TypeName = type;
			ConsoleHelper.FilePath = _appConfig.FilePath;
			exportWcf.ApiResult res = new exportWcf.ApiResult();
			var proxy = new exportWcf.ExportWcfServiceClient();
			ConsoleHelper.Log($" *** {name}排程Start ***");
			var id = InsertDBLog(param.DcCode, param.GupCode, param.CustCode, type);

			try
			{
				switch (type)
				{
					case "ExportWarehouseInResults": // 商品進倉結果回傳排程
						res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
				, () => proxy.ExportWarehouseInResults(param), false, _appConfig.SchemaName);
						break;
					case "ExportOrderResults": // 客戶訂單結果回傳排程
						res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
				, () => proxy.ExportOrderResults(param), false, _appConfig.SchemaName);
						break;
					case "ExportVendorReturnResults": // 廠退出貨結果回傳排程
						res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
				, () => proxy.ExportVendorReturnResults(param), false, _appConfig.SchemaName);
						break;
					case "ExpStockAlertResults": // 庫存警示回報排程
						res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
					, () => proxy.ExpStockAlertResults(param), false, _appConfig.SchemaName);
						break;
					case "ExportHomeDeliveryResults": // 單箱出貨扣帳回檔排程
						res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
					, () => proxy.ExportHomeDeliveryResults(param), false, _appConfig.SchemaName);
						break;
					case "ExportStockMovementResults": // 庫存異動回報
						res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
					, () => proxy.ExportStockMovementResults(param), false, _appConfig.SchemaName);
						break;
                    case "ExportSysErrorNotifyResults": // LMS系統異常通知
                        res = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
                    , () => proxy.ExportSysErrorNotifyResults(param), false, _appConfig.SchemaName);
                        break;
                }

				if (res.IsSuccessed)
				{
					ConsoleHelper.Log($"處理狀況：成功");

					var apiDataList = res.Data != null ? JsonConvert.DeserializeObject<List<ApiResponse>>(res.Data.ToString()) : null;
					if (apiDataList != null && apiDataList.Any())
					{
						ConsoleHelper.Log($"處理明細：");

						apiDataList.ForEach(o =>
						{
							ConsoleHelper.Log($"物流中心服務貨主：[{o.No}]");
							ConsoleHelper.Log($"處理結果：{o.MsgContent}");
						});
					}
				}
				else
				{
					ConsoleHelper.Log($"處理狀況：失敗");
					ConsoleHelper.Log($"錯誤訊息：[{res.MsgCode}]{res.MsgContent}");
				}
			}
			catch (Exception ex)
			{
				ConsoleHelper.Log($"發生錯誤：{ex.ToString()}");
			}

			ConsoleHelper.Log($"*** {name}排程 End ***");
			ConsoleHelper.Log(string.Empty, false);

			UpdateDBLogIsSuccess(id, "1", "");

		}
		#endregion

		#region Log
		/// <summary>
		/// 紀錄Log至ExportResults.txt
		/// </summary>
		/// <param name="message"></param>
		//private static void Log(string message, bool isShowDatetime = true)
		//{
		//	var fileFullName = Path.Combine(_appConfig.FilePath, "ExportResults.txt");
		//	using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
		//		sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
		//}

		private static int InsertDBLog(string dcCode, string gupCode, string custCode, string scheduleName)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			return WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																																			, () => proxy.InsertDbLog(dcCode ?? "0", gupCode ?? "0", custCode ?? "0", scheduleName, "0", ""), false, _appConfig.SchemaName);
		}

		public static void UpdateDBLogIsSuccess(int id, string isSuccessful, string message)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																																									, () => proxy.UpdateDbLogIsSuccess(id, isSuccessful, message), false, _appConfig.SchemaName);
		}
		#endregion
	}
}
