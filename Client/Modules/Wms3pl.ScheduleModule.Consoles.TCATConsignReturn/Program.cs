using ConsoleUtility.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Wms3pl.WpfClient.Common.Helpers;
using System.Text;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using Wms3pl.WpfClient.Services;
using ConsoleUtility.Utilities;
using Wms3pl.WpfClient.ExDataServices;
/// <summary>
/// 統一速達(黑貓)託運單回檔
/// </summary>
namespace Wms3pl.ScheduleModule.Consoles.TCATConsignReturn
{
	class Program
	{
		private static AppConfig _appConfig;
		private static wcf.EgsReturnConsignParam _argConfig;
		private static List<wcf.F194715> _settings;
		private static string _scheduleName = "TCATConsignReturn";
		static void Main(string[] args)
		{
			//設定指定參數物件
			SetArgConfig(args);
			_settings = new List<wcf.F194715>();
			//取得客代主檔設定
			GetEgsCustmerSetting();
			foreach(var setting in _settings)
			{
				//設定Ftp物件
				SetAppConfig(setting);
				//指定客代編號
				_argConfig.CustomerId = setting.CUSTOMER_IDk__BackingField;
				//開始執行回檔
				ExecReturnData();
			}
		
		
		}
		/// <summary>
		/// 設定Ftp物件
		/// </summary>
		private static void SetAppConfig(wcf.F194715 setting)
		{
			_appConfig.FtpIp = setting.FTP_IPk__BackingField;
			_appConfig.FtpAccount = setting.FTP_ACCOUNTk__BackingField;
			_appConfig.FtpPassword = setting.FTP_PASSWORDk__BackingField;
			_appConfig.FtpUploadPath = setting.FTP_UPLOADPATHk__BackingField;
			_appConfig.FilePath = string.Format(setting.LOCAL_TEMPPATHk__BackingField, _scheduleName,DateTime.Today.ToString("yyyyMMdd"));
			_appConfig.LogPath = string.Format(setting.LOCAL_LOGPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
			_appConfig.LocalBackupPath = string.Format(setting.LOCAL_BACKUPPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd") );
			_appConfig.ZipPassword = setting.ZIP_PASSWORDk__BackingField;
			_appConfig.UploadMail = setting.MAILTOk__BackingField;
			_appConfig.UploadMailCC = setting.MAILCCk__BackingField;
			_appConfig.UploadSubject = setting.MAILSUBJECTk__BackingField;
			if (!Directory.Exists(_appConfig.LocalBackupPath))
				Directory.CreateDirectory(_appConfig.LocalBackupPath);
		}
		/// <summary>
		/// 設定指定參數物件
		/// </summary>
		/// <param name="args"></param>
		private static void SetArgConfig(string[] args)
		{
			_argConfig = new wcf.EgsReturnConsignParam();
			_appConfig = new AppConfig();
			_argConfig.AllId = "TCAT"; //固定統一速達
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _argConfig);
				//換FTP設定由參數代入
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if (DEBUG)
			//_argConfig.DcCode = "001";
			//_argConfig.GupCode = "01";
			//_argConfig.CustCode = "010001";
			//_argConfig.Channel = "BBIA";
			//_argConfig.DelvTimes = "11";
			//_argConfig.OrdSDate = DateTime.Parse("2017/1/5");
			//_argConfig.OrdSDate = DateTime.Parse("2017/1/5");
			//_argConfig.DelvSDate = DateTime.Parse("2017/4/7");
			//_argConfig.DelvEDate = DateTime.Parse("2017/4/7");
			//_argConfig.CustomerId = "1265635401";
			//_argConfig.DISTR_USE = "01";
			_appConfig.SchemaName = "BPWMS";
#endif
		}
		/// <summary>
		/// 執行回檔
		/// </summary>
		private static void ExecReturnData()
		{
			//寫入DBLog
			var id = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, "0", _scheduleName);

			//客戶代號,物流中心,業主,貨主,通路,配送商,配次,批次日期(起),批次日期(迄),訂單日期(起),訂單日期(迄)
			Log(string.Format("Parameter={0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", _argConfig.CustomerId, _argConfig.DcCode??"0", _argConfig.GupCode ?? "0",_argConfig.CustCode??"0",_argConfig.Channel,_argConfig.AllId, _argConfig.DelvTimes,
				(_argConfig.DelvSDate.HasValue ? _argConfig.DelvSDate.Value.ToString("yyyy/MM/dd") : ""),
				(_argConfig.DelvEDate.HasValue ? _argConfig.DelvEDate.Value.ToString("yyyy/MM/dd") : ""),
				(_argConfig.OrdSDate.HasValue ? _argConfig.OrdSDate.Value.ToString("yyyy/MM/dd") : ""),
				(_argConfig.OrdEDate.HasValue ? _argConfig.OrdEDate.Value.ToString("yyyy/MM/dd") : "")));

			Log("開始回檔");
			Log("取得回檔資料");
			var proxy = new wcf.SharedWcfServiceClient();
			wcf.EgsReturnConsign[] resultData;
			try
			{
				resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																									 , () => proxy.GetEgsReturnConsigns(_argConfig), false, _appConfig.SchemaName);
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				Log(ex.Source);
				Log(ex.StackTrace);
				UpdateDBLogIsSuccess(id, "0", "取得回檔資料失敗");
				return;
			}
			Log("開始產生檔案");
			//查詢結果有資料時 產 txt & 上傳 ftp
			if (resultData == null || !resultData.Any())
			{
				UpdateDBLogIsSuccess(id, "1", "");
				Log("無資料可產生檔案");
				return;
			}

			//產生TXT(EOD檔)
			if (!Directory.Exists(_appConfig.FilePath)) Directory.CreateDirectory(_appConfig.FilePath);
			var di = new DirectoryInfo(_appConfig.FilePath);
			var fileSeq = di.GetFiles("*.eod", SearchOption.AllDirectories).Where(x=> x.Name.ToUpper().Contains(_appConfig.FtpAccount.ToUpper())).Count() + 1;
			var fileName = string.Format("{0}{1}{2}.eod", _appConfig.FtpAccount, fileSeq.ToString().PadLeft(2, '0'), DateTime.Now.ToString("MMdd"));
			var fileFullName = Path.Combine(_appConfig.FilePath, fileName);
			var importResult = ImportTxt(resultData.ToList(), fileFullName);
			if (!importResult)
			{
				UpdateDBLogIsSuccess(id, "0", "產生EOD檔失敗");
				Log("產生EOD檔失敗");
				return;
			}

			Log("開始上傳檔案到FTP");
			//上傳FTP
			var ftp = new FtpUtility(_appConfig.FtpIp, _appConfig.FtpAccount, _appConfig.FtpPassword, _appConfig.FtpUploadPath, 60,true);
			var ftpResult = ftp.Upload(fileFullName);
			if (!ftpResult)
			{
				UpdateDBLogIsSuccess(id, "0", "上傳檔案到FTP失敗");
				Log("上傳檔案到FTP失敗");
				return;
			}

			Log("開始寄送郵件");
			//上傳後寄發Mail
			var mailResult = SendMail(fileFullName, SendMailType.FtpUpload, null);
			if (!mailResult)
			{
				UpdateDBLogIsSuccess(id, "0", "寄送郵件失敗");
				Log("寄送郵件失敗");
				return;
			}

			Log("更新託運單與派車狀態");
			//更新託運單與派車狀態
			var updateResult = UpdateStatus(resultData.ToList());
			if (!updateResult)
			{
				UpdateDBLogIsSuccess(id, "0", "更新託運單與派車狀態失敗");
				Log("更新託運單與派車狀態失敗");
			}
			else
			{
				UpdateDBLogIsSuccess(id, "1", "");
				Log("更新託運單與派車狀態成功");
			}
			Log("壓縮檔案");
			//壓縮檔案至備份路徑
			var zip = new ZipUtilities();
			var fileList = new List<string> { fileFullName };
			Log("壓縮檔案至備份路徑");
			var zipResult = zip.FileZip(fileList, _appConfig.LocalBackupPath, fileName.Replace(".eod", "_" + DateTime.Now.ToString("HHmmss")), _appConfig.ZipPassword);
			if (zipResult)
			{
				Log("壓縮檔案至備份路徑完成");
				//if (File.Exists(fileFullName))
				//	File.Delete(fileFullName);
				//Log("移除EOD檔案");
			}

		}

		#region 寫Log
		private static void Log(string message)
		{
			if (!Directory.Exists(_appConfig.LogPath))
				Directory.CreateDirectory(_appConfig.LogPath);
			var fileFullName = Path.Combine(_appConfig.LogPath, string.Format("Log_{0}.txt", DateTime.Today.ToString("yyyyMMdd")));
			using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
				sw.WriteLine(string.Format("{0}--{1}", DateTime.Now.ToString("HH:mm:ss"), message));
		}
		#endregion

		#region 寫入DB Log
		private static int InsertDBLog(string dcCode,string gupCode,string custCode,string scheduleName)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			return WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																			, () => proxy.InsertDbLog(dcCode??"0", gupCode ?? "0", custCode ?? "0", scheduleName, "0", ""), false, _appConfig.SchemaName);
		}

		public static void UpdateDBLogIsSuccess(int id, string isSuccessful,string message)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																						, () => proxy.UpdateDbLogIsSuccess(id, isSuccessful, message), false, _appConfig.SchemaName);
		}
#endregion

		#region 產生Txt
				private static bool ImportTxt(List<wcf.EgsReturnConsign> datas,string fileFullName)
				{
					try
					{
						using (var sw = new StreamWriter(fileFullName, false, Encoding.GetEncoding(950)))//BIG5
						{
							var sb = new StringBuilder();
							foreach (var item in datas)
							{
								//託運類別|託運單號|訂單編號(客戶端)|契客代號|溫層|	
								sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.CONSIGN_TYPE?.Trim(), item.CONSIGN_NO?.Trim(), item.CUST_ORD_NO?.Trim(), item.CUSTOMER_ID?.Trim(), item.TEMPERATURE?.Trim()));
								//距離|規格|是否代收貨款|代收金額|是否到付|
								sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.DISTANCE?.Trim(), item.SPEC?.Trim(), item.ISCOLLECT?.Trim(), item.COLLECT_AMT??0, item.ARRIVEDPAY?.Trim()));
								//是否付現|收件人姓名|收件人電話|收件人手機|收件人郵遞區號|
								sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.PAYCASH?.Trim(), item.RECEIVER_NAME?.Trim(), item.RECEIVER_PHONE?.Trim(), item.RECEIVER_MOBILE?.Trim(), item.RECEIVER_SUDA5?.Trim()));
								//收件人地址|寄件人姓名|寄件人電話|寄件人手機|寄件人郵遞區號|
								sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.RECEIVER_ADDRESS?.Trim(), item.SENDER_NAME?.Trim(), item.SENDER_TEL?.Trim(), item.SENDER_MOBILE?.Trim(), item.SENDER_SUDA5?.Trim()));
								//寄件人地址|契客出貨日期|預定取件時段|預定配達時段|會員編號|
								sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.SENDER_ADDRESS?.Trim(), item.SHIP_DATE?.Trim(), item.PICKUP_TIMEZONE?.Trim(), item.DELV_TIMEZONE?.Trim(), item.MEMBER_ID?.Trim()));
								//物品名稱|易碎物品|精密儀器|備註|SD路線代碼
								sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}", item.ITEM_NAME?.Trim(), item.ISFRAGILE?.Trim(), item.ISPRECISON_INSTRUMENT?.Trim(), item.MEMO?.Trim(), item.SD_ROUTE?.Trim()));
								sw.WriteLine(sb.ToString());
								sb.Clear();
							}
						}
						return true;
					}
					catch(Exception ex)
					{
						return false;
					}
				}
		#endregion

		#region SendMail
				public static bool SendMail(string filePath, SendMailType sendType, string body = null)
				{
					try
					{
						string mailTo = null;
						string mailCC = null;
						string mailSubject = null;
						string mailBody = null;
						string mailFile = filePath;
						mailBody = "請參閱附件";
						if (SendMailType.FtpUpload == sendType)
						{
							mailTo = _appConfig.UploadMail;
							mailCC = _appConfig.UploadMailCC;
							mailSubject = string.Format(_appConfig.UploadSubject,DateTime.Today.ToString("yyyy/MM/dd"),"EOD","已上傳");
						}
						SendMessage.MailHelper sm = new SendMessage.MailHelper();
						sm.SendMail(null
												, mailTo
												, mailCC
												, null
												, mailFile
												, mailSubject
												, mailBody
												, null);
						return true;
					}
					catch(Exception ex)
					{
						return false;
					}

				}
		#endregion

		#region 更新託運單與派車狀態
				private static bool UpdateStatus(List<wcf.EgsReturnConsign> datas)
				{
					var proxy = new wcf.SharedWcfServiceClient();
					var itemData = ExDataMapper.MapCollection<wcf.EgsReturnConsign, wcf.EgsReturnConsign>(datas).ToArray();
					var updateResult = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																								, () => proxy.UpdateStatusForTCAT(itemData), false, _appConfig.SchemaName);
					if (updateResult.IsSuccessed)
						return true;

					return false;
				}

		#endregion

		#region 取得客代主檔設定

		private static void GetEgsCustmerSetting()
		{
			var proxy = new wcf.SharedWcfServiceClient();
			_settings = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																									 , () => proxy.GetEgsCustomerSetting(_argConfig.CustomerId), false, _appConfig.SchemaName).ToList();
		}

		#endregion
	}



}
