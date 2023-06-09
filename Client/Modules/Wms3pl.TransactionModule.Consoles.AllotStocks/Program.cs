using ConsoleUtility.Helpers;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.T05WcfService;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
namespace Wms3pl.RefineModule.Consoles.AllotStocks
{
	class Program
	{
		private static AppConfig _appConfig;

		static void Main(string[] args)
		{
#if DEBUG
			args = new string[] { "-SchemaName=PHWMS_PDT" };
#endif
			SetAppConfig();
			SetArgConfig(args);
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			AutoAllotStocks();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ExceptionPolicy.HandleException(e.ExceptionObject as Exception, "Default Policy");
			Environment.Exit(1);
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
			if(args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
		}

		private static void AutoAllotStocks()
		{
			Log("開始執行自動配庫");
			//寫入DBLog
			var id = InsertDBLog("0","0","0", "AutoAllotStocks");
			var wcfT05 = new T05WcfServiceClient();
			try
			{
				WcfServiceHelper.ExecuteForConsoleBySchemaName(wcfT05.InnerChannel, () => wcfT05.AutoAllotStocks(), false, _appConfig.SchemaName, "AutoAllotStocks");
				UpdateDBLogIsSuccess(id,"1","");

			}
			catch (Exception ex)
			{
				Log(ex.Message);
				Log(ex.Source);
				Log(ex.StackTrace);
				UpdateDBLogIsSuccess(id, "0", "執行自動配庫失敗");
				return;
			}
			Log("結束執行自動配庫");
		}
		#region 寫Log
		private static void Log(string message)
		{
			if (!Directory.Exists(_appConfig.FilePath))
				Directory.CreateDirectory(_appConfig.FilePath);
			var fileFullName = Path.Combine(_appConfig.FilePath, string.Format("Log_{0}.txt", DateTime.Today.ToString("yyyyMMdd")));
			using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
				sw.WriteLine(string.Format("{0}--{1}", DateTime.Now.ToString("HH:mm:ss"), message));
		}
		#endregion
		#region 寫入DB Log
		private static int InsertDBLog(string dcCode, string gupCode, string custCode, string scheduleName)
		{
			if (string.IsNullOrWhiteSpace(dcCode))
				dcCode = "0";
			if (string.IsNullOrWhiteSpace(gupCode))
				gupCode = "0";
			if (string.IsNullOrWhiteSpace(custCode))
				custCode = "0";
			var proxy = new wcf.SharedWcfServiceClient();
			return WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																			, () => proxy.InsertDbLog(dcCode, gupCode, custCode, scheduleName, "0", ""), false, _appConfig.SchemaName);
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
