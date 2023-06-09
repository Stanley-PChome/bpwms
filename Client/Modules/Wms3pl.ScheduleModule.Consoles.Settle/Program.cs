using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleUtility.Helpers;
using Wms3pl.WpfClient.ExDataServices.S00WcfService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.ScheduleModule.Consoles.Settle
{
	class Program
	{
		private static AppConfig _appConfig;
		public static ArgClass ArgList = new ArgClass();
		private static void Main(string[] args)
		{
#if DEBUG
			args = new string[] { "-SchemaName=PHWMS_DEV", "-SettleType=DailyFeeSettle" };
#endif

			SetAppConfig();
			//設定指定參數物件
			SetArgConfig(args);
			AppDomain.CurrentDomain.UnhandledException += ConsoleHelper.CurrentDomain_UnhandledException;

			var settleTypeList = new List<SettleTypeEnum>()
			{
				SettleTypeEnum.DailyStockSettle,
				SettleTypeEnum.DailyFeeSettle,
				SettleTypeEnum.SettleReport,
				SettleTypeEnum.DailyShipFeeSettle
			};

			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, ArgList);

				if (!string.IsNullOrEmpty(ArgList.SettleType))
				{
					SettleTypeEnum settleType;
					if (!Enum.TryParse(ArgList.SettleType, out settleType))
					{
						throw new Exception("SettleType參數設定不正確(0:庫存日結,1:費用日結,2:報表結算,3:運費日結)");
					}
					settleTypeList = new List<SettleTypeEnum>() { settleType };
				}
			}

			//起始日期
			DateTime settleDate;
			if (string.IsNullOrEmpty(ArgList.SelectDate) || !DateTime.TryParse(ArgList.SelectDate, out settleDate))
			{
				settleDate = DateTime.Today;
			}
			//結束日期
			DateTime settleEndDate;
			if (string.IsNullOrEmpty(ArgList.SelectEndDate) || !DateTime.TryParse(ArgList.SelectEndDate, out settleEndDate))
			{
				settleEndDate = settleDate;
			}
			//依照日期區間結算
			var span = settleEndDate.Subtract(settleDate);
			var dayDiff = span.Days + 1;
			for (int i = 0; i < dayDiff; i++)
			{
				foreach (var settleType in settleTypeList)
				{
					ConsoleHelper.TypeName = settleType.ToString();
					ConsoleHelper.FilePath = _appConfig.FilePath;
					switch (settleType)
					{
						case SettleTypeEnum.DailyStockSettle:
							InsertStockSettle(settleDate);
							break;
						case SettleTypeEnum.DailyFeeSettle:
							DailyFeeSettle(settleDate);
							break;
						case SettleTypeEnum.SettleReport:
							CalcWorkPerformanceDaily(settleDate);
							CalcSettleReportDaily(settleDate);
							break;
						case SettleTypeEnum.DailyShipFeeSettle:
							DailyShipFeeSettle(settleDate);
							break;
					}
				}
				settleDate = settleDate.AddDays(1);
			}
		}

		/// <summary>
		/// 設定app設定值
		/// </summary>
		private static void SetAppConfig()
		{
			_appConfig = new AppConfig()
			{
				FilePath = string.Format(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd")),
			};
		}
		/// <summary>
		/// 設定指定參數物件
		/// </summary>
		/// <param name="args"></param>
		private static void SetArgConfig(string[] args)
		{
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
		}

		public enum SettleTypeEnum
		{
			/// <summary>
			/// 每日庫存結算
			/// </summary>
			DailyStockSettle,
			/// <summary>
			/// 每日費用結算
			/// </summary>
			DailyFeeSettle,
			/// <summary>
			/// 報表結算
			/// </summary>
			SettleReport,
			/// <summary>
			/// 每日運費結算
			/// </summary>
			DailyShipFeeSettle,
		}

		public class ArgClass
		{
			public string SettleType { get; set; }
			public string SelectDate { get; set; }
			public string SelectEndDate { get; set; }
		}

		/// <summary>
		/// 庫存數據結算
		/// </summary>
		/// <param name="settleDate"></param>
		private static void InsertStockSettle(DateTime settleDate)
		{
			ConsoleHelper.Log($" *** InsertStockSettle排程Start ***");
			var wcf = new S00WcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
				() => wcf.DailyStockBackup(settleDate), false,_appConfig.SchemaName, "Settle.DailyStockBackup");
			WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
				() => wcf.InsertStockSettle(settleDate), false, _appConfig.SchemaName, "Settle.InsertStockSettle");

			ConsoleHelper.Log($"*** InsertStockSettle排程 End ***");
			ConsoleHelper.Log(string.Empty, false);
		}

		private static void DailyFeeSettle(DateTime settleDate)
		{
			ConsoleHelper.Log($" *** DailyFeeSettle排程Start ***");
			var wcf = new S00WcfServiceClient();
			//取得各貨主合約資料
			var contractData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
				() => wcf.GetContractSettleDatas(settleDate), false, _appConfig.SchemaName, "Settle.GetContractSettleDatas");

			//將合約內報價單分類
			var contractType = contractData.GroupBy(n => new { n.DC_CODE, n.GUP_CODE, n.CUST_CODE, n.ITEM_TYPE }).ToList();
			foreach (var itemType in contractType)
			{
				//by貨主+報價單類型給Server端計算(避免處理太久Timeout)
				var type = itemType.Key;
				var quotes =
					contractData.Where(
						n =>
							n.DC_CODE == type.DC_CODE && n.GUP_CODE == type.GUP_CODE && n.CUST_CODE == type.CUST_CODE &&
							n.ITEM_TYPE == type.ITEM_TYPE).ToList();
				if (!quotes.Any())
					continue;

				WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
					() =>
						wcf.SettleDaily(settleDate, type.DC_CODE, type.GUP_CODE, type.CUST_CODE, quotes.First().CONTRACT_NO,
							type.ITEM_TYPE, quotes.Select(n => n.QUOTE_NO).ToArray()), false, _appConfig.SchemaName, "Settle.SettleDaily");
			}
			//配送商費用結算(不在合約內)
			WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel, () => wcf.SettleDaily(settleDate, "", "", "","", "008", null),
				false, _appConfig.SchemaName, "Settle.SettleDaily");

			ConsoleHelper.Log($"*** DailyFeeSettle排程 End ***");
			ConsoleHelper.Log(string.Empty, false);
		}

		private static void SettleReport(DateTime settleDate)
		{
			ConsoleHelper.Log($" *** SettleReport排程Start ***");
			CalcWorkPerformanceDaily(settleDate);
			CalcSettleReportDaily(settleDate);
			ConsoleHelper.Log($"*** SettleReport排程 End ***");
			ConsoleHelper.Log(string.Empty, false);
		}

		/// <summary>
		/// 計算作業績效報表
		/// </summary>
		/// <param name="settleDate">計算這個日期的前一日</param>
		private static void CalcWorkPerformanceDaily(DateTime settleDate)
		{
			var wcf = new S00WcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
				() => wcf.CalcWorkPerformanceDaily(settleDate), false, _appConfig.SchemaName, "Settle.CalcWorkPerformanceDaily");
		}

		/// <summary>
		/// 寫入結算報表資料
		/// </summary>
		/// <param name="settleDate"></param>
		private static void CalcSettleReportDaily(DateTime settleDate)
		{

			var wcf = new S00WcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
				() => wcf.CalcSettleReportDaily(settleDate), false, _appConfig.SchemaName, "Settle.CalcSettleReportDaily");
		}

		private static void DailyShipFeeSettle(DateTime settleDate)
		{
			ConsoleHelper.Log($" *** DailyShipFeeSettle排程Start ***");
			var wcf = new S00WcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel,
				() => wcf.CalculateShipFee(settleDate), false, _appConfig.SchemaName, "Settle.CalculateShipFee");
			ConsoleHelper.Log($"*** DailyShipFeeSettle排程 End ***");
			ConsoleHelper.Log(string.Empty, false);
		}
	}
}
