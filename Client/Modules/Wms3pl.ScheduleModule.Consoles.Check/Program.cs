using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleUtility;
using ConsoleUtility.Helpers;
using ConsoleUtility.Model;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.Services;
using wcfS03 = Wms3pl.WpfClient.ExDataServices.S03WcfService;

namespace Wms3pl.ScheduleModule.Consoles.Check
{
	/// <summary>
	/// 群組設定(排程用群組，手動執行才用CheckType，以方便維護)
	/// </summary>
	public enum CheckTypeGroup
	{
		None,
		/// <summary>
		/// Check相關排程
		/// </summary>
		CheckGroup,
		/// <summary>
		/// 行事曆相關排程
		/// </summary>
		CalendarGroup
	}
	/// <summary>
	/// 數值來源F190004的Check，非檢查相關從1000號開始起跳用以識別
	/// </summary>
	public enum CheckType
	{
		None,
		/// <summary>
		/// 檢查出貨單的頭一箱箱子與建議箱，是否連續三次皆不同
		/// </summary>
		CheckShipperSuggestionBox = 1,
		/// <summary>
		/// 訂單狀態有問題
		/// </summary>
		OrderProblem = 2,
		/// <summary>
		/// 超過設定的撿貨時間限制
		/// </summary>
		ExceedPickFinishTime = 4,
		/// <summary>
		/// 倉務管理至行事歷
		/// </summary>
		StockToSchedule = 1001,
		/// <summary>
		/// 行事歷至 Lo 訊息池
		/// </summary>
		ScheduleToLoMessage = 1002,

	}

	public class CheckArguments : BaseSystemScheduleArguments
	{
		public string CheckTypeName { get; set; }//單獨指定排程
		public string CheckTypeGroup { get; set; }//群組設定(多排程)

	}


	class Program
	{
		private static AppConfig _appConfig;
		#region Test Data
		static CheckArguments GetTestCheckData(CheckType checkType)
		{
			//因此專案有各種不同的檢查排程共用
			var argData = new CheckArguments();
			switch (checkType)
			{
				case CheckType.StockToSchedule:
					argData.CheckTypeName = "StockToSchedule";
					break;
				case CheckType.ScheduleToLoMessage:
					argData.CheckTypeName = "ScheduleToLoMessage";
					break;
				case CheckType.CheckShipperSuggestionBox:
					argData.CheckTypeName = "CheckShipperSuggestionBox";
					break;
				case CheckType.OrderProblem:
					argData.CheckTypeName = "OrderProblem";
					break;
				case CheckType.ExceedPickFinishTime:
					argData.CheckTypeName = "ExceedPickFinishTime";
					break;
				default:
					throw new ArgumentOutOfRangeException("checkType", checkType, null);
			}
			return argData;
		}

		static CheckArguments GetTestCheckData(CheckTypeGroup checkTypeGroup)
		{
			//因此專案有各種不同的檢查排程共用
			var argData = new CheckArguments();
			switch (checkTypeGroup)
			{
				case CheckTypeGroup.CheckGroup:
					argData.CheckTypeGroup = "CheckGroup";
					break;
				case CheckTypeGroup.CalendarGroup:
					argData.CheckTypeGroup = "CalendarGroup";
					break;
				default:
					throw new ArgumentOutOfRangeException("checkTypeGroup", checkTypeGroup, null);
			}
			return argData;
		}

		#endregion

		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += ConsoleHelper.CurrentDomain_UnhandledException;
#if DEBUG
			args = new string[] { "-SchemaName=BPWMS_MSSQL","-CheckTypeGroup=CheckGroup" };
#endif
			SetAppConfig();
			SetArgConfig(args);

			var argData = new CheckArguments();
			ConsoleHelper.ArgumentsTransform(args, argData);
			var checkTypeGroup = CheckTypeGroup.None;
			var checkType = CheckType.None;

#if (DEBUG)
			//請用此測試
			//argData = GetTestCheckData(CheckType.OrderProblem);
			argData = GetTestCheckData(CheckTypeGroup.CheckGroup);
#endif


			#region 檢查
			if (string.IsNullOrEmpty(argData.CheckTypeGroup) || !Enum.TryParse(argData.CheckTypeGroup, out checkTypeGroup))
			{
				if (string.IsNullOrEmpty(argData.CheckTypeName) || !Enum.TryParse(argData.CheckTypeName, out checkType))
				{
					throw new Exception("找不到相關 Enum Function");
				}
			}
			#endregion


			var selectDateTime = string.IsNullOrWhiteSpace(argData.SelectDate)
				? DateTime.Now
				: DateTimeHelper.ConversionDate(argData.SelectDate.Replace("T", " "));

			if (checkTypeGroup != CheckTypeGroup.None)//有設定使用群組
			{
				switch (checkTypeGroup)
				{
					case CheckTypeGroup.CheckGroup:
						CheckShipperSuggestionBox(GetCheckNo(CheckType.CheckShipperSuggestionBox), selectDateTime);
						CheckOrderIsProblem(GetCheckNo(CheckType.OrderProblem), selectDateTime);
						CheckExceedPickFinishTime(GetCheckNo(CheckType.ExceedPickFinishTime), selectDateTime);
						break;
					case CheckTypeGroup.CalendarGroup:
						StockToSchedule();
						//ScheduleToLoMessage();
						break;
				}
			}
			else
			{
				var checkNo = GetCheckNo(checkType);
				switch (checkType)
				{
					case CheckType.StockToSchedule:
						StockToSchedule();
						break;
					case CheckType.ScheduleToLoMessage:
						//ScheduleToLoMessage();
						break;
					case CheckType.CheckShipperSuggestionBox:
						CheckShipperSuggestionBox(checkNo, selectDateTime);
						break;
					case CheckType.OrderProblem:
						CheckOrderIsProblem(checkNo, selectDateTime);
						break;
					case CheckType.ExceedPickFinishTime:
						CheckExceedPickFinishTime(checkNo, selectDateTime);
						break;
				}
			}

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

		#region 檢查出貨單的頭一箱箱子與建議箱，是否連續三次皆不同

		static void CheckShipperSuggestionBox(string checkNo, DateTime selectDate)
		{
			//argData = InitArgumentsDataByShipperSuggestionBox(argData);

			var result = CheckBoxNumIsProblem(selectDate, checkNo);

			if (!result.Any()) return;

			var f0020Data = GetF0020("CKM00001");
			var subject = f0020Data.MSG_SUBJECTk__BackingField;
			var formatContent = f0020Data.MSG_CONTENTk__BackingField;

			foreach (var r in result.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }))
			{
				var dcCode = r.Select(x => x.DC_CODE).FirstOrDefault();
				var settingData = GetF190103(dcCode, checkNo);
				if (settingData == null) continue;

				var cycleTime = settingData.CYCLE_MINk__BackingField;
				if (CheckTimeUp(cycleTime)) //cycleTime是根據DC設定，
				{

					var dcName = r.Select(x => x.DC_NAME).FirstOrDefault();
					var gupName = r.Select(x => x.GUP_NAME).FirstOrDefault();
					var custName = r.Select(x => x.CUST_NAME).FirstOrDefault();
					var f1924Data = GetF1924DataBySchedule(dcCode, checkNo);
					if (f1924Data != null)
					{
						var content = GetMailContentByShipperSuggestionBox(formatContent, dcName, gupName, custName, r.ToList());
						foreach (var f1924 in f1924Data)
						{
							if (VerificationHelper.CheckEmailFormat(f1924.EMAIL))
							{
								SendMail(f1924.EMAIL, subject, content);
							}
						}
					}
				}
			}
		}

		private static string GetMailContentByShipperSuggestionBox(string content, string dcName, string gupName, string custName, List<wcfS03.F055001QueryItem> data)
		{
			var contentStart = string.Format(content, dcName, gupName, custName);
			var fommatText = @"出貨單號:{0}, 電腦名稱:{1}";
			var detailText = string.Empty;
			foreach (var d in data)
			{
				detailText += string.Format(fommatText, d.WMS_ORD_NO, d.CLIENT_PC, ConsoleHelper.MailWrap);
			}
			var result = string.Format("{1}{0}{2}{0}", Environment.NewLine, contentStart, detailText);
			return result;
		}


		#endregion

		#region 檢查進倉、退貨、出貨、調撥、盤點單，是否有單據預設狀態為預設值，且超過一小時

		private static void CheckOrderIsProblem(string checkNo, DateTime selectDate)
		{
			var result = GetOrderIsProblem(selectDate);

			if (!result.Any()) return;

			var f0020Data = GetF0020("CKM00002");

			var subject = f0020Data.MSG_SUBJECTk__BackingField;
			var formatContent = f0020Data.MSG_CONTENTk__BackingField;

			foreach (var r in result.GroupBy(x => new { x.TYPE, x.DC_CODE, x.GUP_CODE, x.CUST_CODE }))
			{
				var dcCode = r.Select(x => x.DC_CODE).FirstOrDefault();
				var settingData = GetF190103(dcCode, checkNo);
				if (settingData == null) continue;

				var cycleTime = settingData.CYCLE_MINk__BackingField;
				if (CheckTimeUp(cycleTime))//cycleTime是根據DC設定，
				{
					var dcName = r.Select(x => x.DC_NAME).FirstOrDefault();
					var gupName = r.Select(x => x.GUP_NAME).FirstOrDefault();
					var custName = r.Select(x => x.CUST_NAME).FirstOrDefault();
					var f1924Data = GetF1924DataBySchedule(dcCode, checkNo);
					if (f1924Data != null)
					{
						var content = GetMailContentByOrderIsProblem(formatContent, dcName, gupName, custName, r.ToList());
						foreach (var f1924 in f1924Data)
						{
							if (VerificationHelper.CheckEmailFormat(f1924.EMAIL))
							{
								SendMail(f1924.EMAIL, subject, content);
							}
						}
					}
				}
			}

		}

		static bool CheckTimeUp(int cycleTime)
		{
#if DEBUG
			return true;
#endif
			var hourMinute = 60;//1小時 = 60分鐘
			var checkMinute = 5;//設定前後時差
			var now = DateTime.Now;
			for (int i = 0; i < hourMinute; i += cycleTime)
			{
				if (Math.Abs(now.Minute - i) < checkMinute)
				{
					return true;
				}
			}
			return false;
		}

		private static string GetMailContentByOrderIsProblem(string content, string dcName, string gupName, string custName, List<wcfS03.OrderIsProblem> data)
		{
			var type = data.Select(x => x.TYPE).FirstOrDefault();
			var nos = string.Join(",", data.Select(x => x.NO));
			var contentStart = string.Format(content, dcName, gupName, custName, type);
			var fommatText = @"{0}單號:{1} {2}";
			var detailText = string.Empty;
			detailText += string.Format(fommatText, type, nos, ConsoleHelper.MailWrap);

			var result = string.Format("{1}{0}{2}{0}", Environment.NewLine, contentStart, detailText);
			return result;
		}

		#endregion

		#region 撿貨時間過長(超過F0003的PickFinishTime設定)

		private static void CheckExceedPickFinishTime(string checkNo, DateTime selectDate)
		{
			var result = GetExceedPickFinishTimeDatas(selectDate);

			if (!result.Any()) return;

			var f0020Data = GetF0020("CKM00003");
			var subject = f0020Data.MSG_SUBJECTk__BackingField;
			var formatContent = f0020Data.MSG_CONTENTk__BackingField;

			foreach (var r in result.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }))
			{
				var dcCode = r.Select(x => x.DC_CODE).FirstOrDefault();
				var settingData = GetF190103(dcCode, checkNo);
				if (settingData == null) continue;

				var cycleTime = settingData.CYCLE_MINk__BackingField;
				if (CheckTimeUp(cycleTime)) //cycleTime是根據DC設定，
				{
					var dcName = r.Select(x => x.DC_NAME).FirstOrDefault();
					var gupName = r.Select(x => x.GUP_NAME).FirstOrDefault();
					var custName = r.Select(x => x.CUST_NAME).FirstOrDefault();
					var f1924Data = GetF1924DataBySchedule(dcCode, checkNo);
					if (f1924Data != null)
					{
						var content = GetMailContentByExceedPickFinishTime(formatContent, dcName, gupName, custName, r.ToList());
						foreach (var f1924 in f1924Data)
						{
							if (VerificationHelper.CheckEmailFormat(f1924.EMAIL))
							{
								SendMail(f1924.EMAIL, subject, content);
							}
						}
					}
				}
			}

		}

		private static string GetMailContentByExceedPickFinishTime(string content, string dcName, string gupName, string custName, List<wcfS03.ExceedPickFinishTime> data)
		{
			var nos = string.Join(",", data.Select(x => x.PICK_ORD_NO));
			var contentStart = string.Format(content, dcName, gupName, custName);
			var fommatText = @"揀貨單號:{0} {1}";
			var detailText = string.Empty;
			detailText += string.Format(fommatText, nos, ConsoleHelper.MailWrap);

			var result = string.Format("{1}{0}{2}{0}", Environment.NewLine, contentStart, detailText);
			return result;
		}


		#endregion

		#region 倉務管理寫入至行事歷
		private static void StockToSchedule()
		{
			InsertSchedule();
		}
		#endregion

		//#region 行事歷寫入Lo訊息池
		//private static void ScheduleToLoMessage()
		//{
		//	InsertSchLoMessage();
		//}
		//#endregion

		#region Common
		private static void SendMail(string mailTo, string mailSubject, string mailBody)
		{
			var sm = new SendMessage.MailHelper();
			sm.SendMail(mailTo, mailSubject, mailBody);
		}
		static string GetCheckNo(CheckType checkType)
		{
			return string.Format("{0:00}", (int)checkType);
		}
		#endregion

		#region Services

		#region 檢查出貨單的頭一箱箱子與建議箱，是否連續三次皆不同
		static List<wcfS03.F055001QueryItem> CheckBoxNumIsProblem(DateTime selectDate, string checkNo)
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.GetBoxNumIsProblem(selectDate, checkNo), false,_appConfig.SchemaName, "CheckBoxNumIsProblem");
			return resultData.ToList();
		}

		static wcfS03.F0020 GetF0020(string msgNo)
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.GetF0020Data(msgNo), false,_appConfig.SchemaName, "GetF0020");
			return resultData;
		}

		static wcfS03.F190103 GetF190103(string dcCode, string checkNo)
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.GetF190103BySchedule(dcCode, checkNo), false,_appConfig.SchemaName, "GetF190103");
			return resultData;
		}

		static List<wcfS03.F1924QueryData> GetF1924DataBySchedule(string dcCode, string checkNo)
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.GetF1924DataBySchedule(dcCode, checkNo), false, _appConfig.SchemaName, "GetF1924DataBySchedule");
			return resultData.ToList();
		}
		#endregion

		#region 檢查進倉、退貨、出貨、調撥、盤點單，是否有單據預設狀態為預設值，且超過一小時

		static List<wcfS03.OrderIsProblem> GetOrderIsProblem(DateTime selectDate)
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.GetOrderIsProblem(selectDate), false, _appConfig.SchemaName, "GetOrderIsProblem");
			return resultData.ToList();
		}

		#endregion

		#region 撿貨時間過長(超過F0003的PickFinishTime設定)
		static List<wcfS03.ExceedPickFinishTime> GetExceedPickFinishTimeDatas(DateTime selectDate)
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.GetExceedPickFinishTimeDatas(selectDate), false, _appConfig.SchemaName, "GetExceedPickFinishTimeDatas");
			return resultData.ToList();
		}
		#endregion

		#region 倉務管理寫入至行事歷
		static void InsertSchedule()
		{
			var wcf = new wcfS03.S03WcfServiceClient();
			var resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
				, () => wcf.InsertSchedule(), false, _appConfig.SchemaName, "InsertSchedule");
		}
		#endregion

		//#region 行事歷寫入Lo訊息池
		//static void InsertSchLoMessage()
		//{
		//	var wcf = new wcfS03.S03WcfServiceClient();
		//	var resultData = WcfServiceHelper.ExecuteForConsole(wcf.InnerChannel
		//		, () => wcf.InsertSchLoMessage(), false);
		//}
		//#endregion



		#endregion
	}
}
