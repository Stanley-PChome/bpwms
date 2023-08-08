using ConsoleUtility.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.S19WcfService;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using wcfS19 = Wms3pl.WpfClient.ExDataServices.S19WcfService;
namespace Wms3pl.ScheduleModule.Consoles.UpdateLocVolumn
{
	class Program
	{
		private static AppConfig _appConfig;
		private static ExecUpdateLocVolumnParam _argConfig;
		static void Main(string[] args)
		{
			SetAppConfig();
			SetArgConfig(args);
			Exec();
		}
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
			_argConfig = new ExecUpdateLocVolumnParam();
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _argConfig);
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if (DEBUG)
			_appConfig.SchemaName = "BPWMS_MSSQL";
			_argConfig.DcCode = "12";
#endif
		}

		private static void Exec()
		{
			Log("開始執行更新儲位容積排程");
			//寫入DBLog
			var id = InsertDBLog(_argConfig.DcCode, "", "", "UpdateLocVolumn");
			var proxy = new wcfS19.S19WcfServiceClient();
			wcfS19.ApiResult resultData;

			try
			{
				resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																									 , () => proxy.ExecUpdateLocVolumn(_argConfig), false, _appConfig.SchemaName);
				UpdateDBLogIsSuccess(id, resultData.IsSuccessed ? "1" : "0", resultData.MsgContent);
				Log(resultData.MsgContent);
                var locCodeMsg = (string)resultData.Data;
                if(!string.IsNullOrWhiteSpace(locCodeMsg))
                    Log(locCodeMsg);
            }
			catch (Exception ex)
			{
				Log(ex.Message);
				Log(ex.Source);
				Log(ex.StackTrace);
				UpdateDBLogIsSuccess(id, "0", "執行執行更新儲位容積排程失敗");
				return;
			}
			Log("結束執行更新儲位容積排程");
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
