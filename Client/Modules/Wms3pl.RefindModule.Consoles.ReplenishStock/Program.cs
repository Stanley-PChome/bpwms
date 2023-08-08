using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.ExDataServices.R01WcfService;
using Wms3pl.WpfClient.Services;
using ConsoleUtility.Helpers;
using System.Configuration;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using r01Wcf = Wms3pl.WpfClient.ExDataServices.R01WcfService;
using System.IO;
using Newtonsoft.Json;
using Wms3pl.Datas.Shared.ApiEntities;

namespace Wms3pl.RefineModule.Consoles.ReplenishStock
{
	class Program
	{
		private static AppConfig _appConfig;
		private static r01Wcf.BatchTransApiOrdersReq param;
		static void Main(string[] args)
		{
			SetAppConfig();
			SetArgConfig(args);
			Init();
			Execute();
		}

		/// <summary>
		/// 設定指定參數物件
		/// </summary>
		/// <param name="args"></param>
		private static void SetArgConfig(string[] args)
		{
			param = new r01Wcf.BatchTransApiOrdersReq();
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, param);
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if (DEBUG)
			_appConfig.SchemaName = "PHWMS_PDT";
			param.DcCode = "12";
			param.GupCode = "10";
			param.CustCode = "010001";
#endif
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

		/// <summary>
		/// 起始
		/// </summary>
		private static void Init()
		{
			if (!Directory.Exists(_appConfig.FilePath))
				Directory.CreateDirectory(_appConfig.FilePath);
		}

		private static void Execute()
		{
			ConsoleHelper.FilePath = _appConfig.FilePath;
			ConsoleHelper.Log("*** 補貨排程處理 Start ***");
			var id = InsertDBLog(param.DcCode, param.GupCode, param.CustCode, "ReplenishStock");

			#region 建立Wms訂單
			try
			{
				var wcf = new R01WcfServiceClient();
				var result = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel, () => wcf.GetReplensihStock(param), false, _appConfig.SchemaName, "ReplenishStocks");

				if (result.IsSuccessed)
				{
					ConsoleHelper.Log($"[補貨建立調撥單] 處理狀況：成功");

					var apiDataList = result.Data != null ? JsonConvert.DeserializeObject<List<ApiResponse>>(result.Data.ToString()) : null;
					if (apiDataList != null && apiDataList.Any())
					{
						ConsoleHelper.Log($"[補貨建立調撥單] 處理明細：");

						apiDataList.ForEach(o =>
						{
							ConsoleHelper.Log($"物流中心服務貨主：[{o.No}]");
                            var data = JsonConvert.DeserializeObject<List<ApiResponse>>(o.MsgContent);
							ConsoleHelper.Log($"處理結果：");
							if(data!=null)
								ConsoleHelper.Log(string.Join("\r", data.Select(x => x.MsgContent??"")), false);
             });
					}
				}
				else
				{
					ConsoleHelper.Log($"[補貨建立調撥單] 處理狀況：失敗");
					ConsoleHelper.Log($"[補貨建立調撥單] 錯誤訊息：{result.MsgContent}");
				}
			}
			catch (Exception ex)
			{
				ConsoleHelper.Log($"[補貨建立調撥單] 發生錯誤：{ex.ToString()}");
			}
			#endregion

			ConsoleHelper.Log("*** 補貨排程處理 End ***");
			ConsoleHelper.Log(string.Empty, false);

			UpdateDBLogIsSuccess(id, "1", "");
		}

		/// <summary>
		/// 紀錄Log至BatchTransApiOrders.txt
		/// </summary>
		/// <param name="message"></param>
		//private static void Log(string message, bool isShowDatetime = true)
		//{
		//	var fileFullName = Path.Combine(_appConfig.FilePath, "BatchTransApi_Order.txt");
		//	using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
		//		sw.WriteLine(isShowDatetime ? $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {message}" : message);
		//}

		#region 寫入DB Log
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
