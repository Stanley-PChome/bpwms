using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleUtility.Helpers;
using Wms3pl.WpfClient.ExDataServices.S99WcfService;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.ScheduleModule.Consoles.MessageSender
{
	class Program
	{
		private static AppConfig _appConfig;
		static void Main(string[] args)
		{
#if DEBUG
			args = new string[] { "-SchemaName=BPWMS_MSSQL" };
#endif
			SetAppConfig();
			SetArgConfig(args);
			AppDomain.CurrentDomain.UnhandledException += ConsoleHelper.CurrentDomain_UnhandledException;
			SendMessages();
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

		private static void SendMessages()
		{
			var wcf = new S99WcfServiceClient();
			var loMessages = WcfServiceHelper.ExecuteForConsoleBySchemaName<WMSMessage[]>(wcf.InnerChannel, () => wcf.GetWmsMessages(), false,_appConfig.SchemaName, "GetWmsMessages").ToList();
			var sentMessages = new List<WMSMessage>();
			try
			{
				foreach (var loMessage in loMessages)
				{
					try
					{
						if (loMessage.IsMail)
						{
							SendMail(string.Join(";", loMessage.ReceiverMails), loMessage.MSG_SUBJECT,
								string.Format("訊息時間:「{0}」\n訊息編號:「{1}」\n{2}", loMessage.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"), loMessage.MESSAGE_ID, loMessage.MESSAGE_CONTENT));
						}
						if (loMessage.IsSms)
						{
							SendSms(loMessage.MESSAGE_CONTENT, loMessage.ReceiverMobiles);
						}
						sentMessages.Add(loMessage);
					}
					catch (Exception ex)
					{
						ExceptionPolicy.HandleException(ex, "Default Policy");
					}
				}
			}
			finally
			{
				WcfServiceHelper.ExecuteForConsoleBySchemaName(wcf.InnerChannel, () => wcf.SetMessageStatus(sentMessages.ToArray()), false,_appConfig.SchemaName, "SetMessageStatus");
			}
		}

		#region SendMail
		private static void SendMail(string mailTo, string mailSubject, string mailBody)
		{
			var sm = new SendMessage.MailHelper();
			if (!string.IsNullOrEmpty(mailBody))
			{
				mailBody = mailBody.Replace("\n", "<br>");
			}
			sm.SendMail(mailTo, mailSubject, mailBody);
		}
		#endregion

		#region SendSms
		public static void SendSms(string msg, string[] phoneNumbers)
		{
			SendMessage.SmsHelper ss = new SendMessage.SmsHelper();


#if !DEBUG
				ss.SendSms(msg, phoneNumbers);			
#endif

		}
		#endregion
	}
}
