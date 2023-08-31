using ConsoleUtility.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services;

namespace Wms3pl.ScheduleModule.Consoles.WmsImportData
{
	/// <summary>
	/// 
	/// </summary>
	class Program
	{
		private static AppConfig _appConfig;
		private static WcsImportReq param;

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
			param = new WcsImportReq();
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, param);
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if (DEBUG)
			_appConfig.SchemaName = "PHWMS_DEV";
			//param.DcCode = "12";
			//param.GupCode = "10";
			//param.CustCode = "010001";
			param.ScheduleNo = "08";
			//param.StockCompareDate = DateTime.Today;
#endif
			Schemas.CoreSchema = _appConfig.SchemaName;
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
				case "01":// 執行入庫完成
					ExecuteSchedule("InWarehouseReceipt", "入庫完成結果");
					break;
				case "02":// 執行出庫完成
					ExecuteSchedule("OutWarehouseReceipt", "出庫完成結果");
					break;
				case "03":// 執行盤點完成
					ExecuteSchedule("InventoryReceipt", "盤點完成結果");
					break;
				case "04":// 執行盤點調整完成
					ExecuteSchedule("InventoryAdjustReceipt", "盤點調整完成結果");
					break;
				case "05":// WMS自動倉庫存備份
					ExecuteSchedule("StockSnapshotReceipt", "WMS自動倉庫存備份");
					break;
				case "06":// 庫存比對
					ExecuteSchedule("StockCompareReceipt", "庫存比對");
					break;
				case "07":// 分揀出貨資訊回報更新
					ExecuteSchedule("ShipDebitReceipt", "分揀出貨資訊回報更新");
					break;
				case "08":// 出庫結果回報(按箱)更新函數
					ExecuteSchedule("OutWarehouseContainerReceipt", "出庫結果回報(按箱)更新函數");
					break;
				case "09":// 外部包裝資料處理排程(12)
					param.ProcFlag = "3";
					ExecuteSchedule("OutSidePackage", "外部包裝資料處理排程(12)-取消宅單(3)");
					param.ProcFlag = "8";
					ExecuteSchedule("OutSidePackage", "外部包裝資料處理排程(12)-異常處理(8)");
					param.ProcFlag = "0";
					ExecuteSchedule("OutSidePackage", "外部包裝資料處理排程(12)-待處理(0)");
					break;

				case "":
				case null:// 執行全部
					ExecuteSchedule("InWarehouseReceipt", "入庫完成結果");
					ExecuteSchedule("OutWarehouseReceipt", "出庫完成結果");
					ExecuteSchedule("InventoryReceipt", "盤點完成結果");
					ExecuteSchedule("InventoryAdjustReceipt", "盤點調整完成結果");
					ExecuteSchedule("StockSnapshotReceipt", "WMS自動倉庫存備份");
					ExecuteSchedule("StockCompareReceipt", "庫存比對");
					ExecuteSchedule("ShipDebitReceipt", "分揀出貨資訊回報更新");
					ExecuteSchedule("OutWarehouseContainerReceipt", "出庫結果回報(按箱)更新函數");
					param.ProcFlag = "3";
					ExecuteSchedule("OutSidePackage", "外部包裝資料處理排程(12)-取消宅單(3)");
					param.ProcFlag = "8";
					ExecuteSchedule("OutSidePackage", "外部包裝資料處理排程(12)-異常處理(8)");
					param.ProcFlag = "0";
					ExecuteSchedule("OutSidePackage", "外部包裝資料處理排程(12)-待處理(0)");
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
			ApiResult res = new ApiResult();
			ConsoleHelper.Log($" *** {name}排程Start ***");

			try
			{
				switch (type)
				{
					case "InWarehouseReceipt": // 入庫完成結果
																		 //res = new InboundConfirmService().InboundConfirm(param);
						break;
					case "OutWarehouseReceipt": // 出庫完成結果
						res = new OutboundConfirmService().OutboundConfirm(param);
						break;
					case "InventoryReceipt": // 盤點完成結果
						res = new InventoryConfirmService().InventoryConfirm(param);
						break;
					case "InventoryAdjustReceipt": // 盤點調整完成結果
						res = new InventoryAdjustConfirmService().InventoryAdjustConfirm(param);
						break;
					case "StockSnapshotReceipt": // WMS自動倉庫存備份
						res = new StockSnapshotService().StockSnapshotConfirm(param);
						break;
					case "StockCompareReceipt": // 庫存比對
						res = new StockCompareService().StockCompareConfirm(param);
						break;
					case "ShipDebitReceipt": // 分揀出貨資訊回報更新
						res = new ShipDebitService().ShipDebit(param);
						break;
					case "OutWarehouseContainerReceipt": // 出庫結果回報(按箱)更新函數
						res = new OutWarehouseContainerReceiptService().OutWarehouseContainerReceipt(param);
						break;
					case "OutSidePackage": //外部包裝資料處理排程(12)
						res = new OutSidePackageService().OutSidePackageProcess(param);
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
							if (!string.IsNullOrWhiteSpace(o.No))
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
		}
		#endregion

		//#region Log
		/// <summary>
		/// 紀錄Log至ExportResults.txt
		/// </summary>
		/// <param name="message"></param>
		//private static void Log(string message, bool isShowDatetime = true)
		//{
		//    var fileFullName = Path.Combine(_appConfig.FilePath, "WcsImportResults.txt");
		//    using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
		//        sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
		//}
		//#endregion
	}
}
