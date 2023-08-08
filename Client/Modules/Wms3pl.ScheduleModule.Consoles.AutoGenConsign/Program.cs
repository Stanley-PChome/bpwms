using ConsoleUtility.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Services;
using wcfShared = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
namespace Wms3pl.ScheduleModule.Consoles.AutoGenConsign
{
	class Program
	{
		private static AppConfig _appConfig;
		static void Main(string[] args)
		{
			var param = new wcfShared.AutoGenConsignParam();
			_appConfig = new AppConfig
			{
				FilePath = string.Format(ConfigurationManager.AppSettings["FilePath"], DateTime.Today.ToString("yyyyMMdd")),
			};
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, param);
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if DEBUG
			param.DcCode = "001";
			param.GupCode = "01";
			param.AllId = "TCAT";
			//param.ConsignType = "A";
			param.CustomerId = "1265635401";
			param.IsTest = "1";
			_appConfig.SchemaName = "BPWMS";
#endif
			Log("開始執行產生託運單號!");
			//寫入DBL
			var id = InsertDBLog(param.DcCode, param.GupCode, "0", "AutoGenConsign");
			var wcf = new wcfShared.SharedWcfServiceClient();
			var result = WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel
							, () => wcf.GenerateEgsConsign(param), false, _appConfig.SchemaName);
			Console.WriteLine(result.Message);
			var messages = result.Message.Split(Environment.NewLine.ToCharArray());
			foreach (var message in messages)
			{
				if (message.Length == 0)
					continue;
				Log(message);
			}
			UpdateDBLogIsSuccess(id, "1", "");
			Log("執行產生託運單號完成");
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
			var proxy = new wcfShared.SharedWcfServiceClient();
			return WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																			, () => proxy.InsertDbLog(dcCode??"0", gupCode ?? "0", custCode ?? "0", scheduleName, "0", ""), false, _appConfig.SchemaName);
		}

		public static void UpdateDBLogIsSuccess(int id, string isSuccessful, string message)
		{
			var proxy = new wcfShared.SharedWcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																						, () => proxy.UpdateDbLogIsSuccess(id, isSuccessful, message), false, _appConfig.SchemaName);
		}
		#endregion
	}
}
