using ConsoleUtility.Helpers;
using ConsoleUtility.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;

namespace Wms3pl.ScheduleModule.Consoles.HCTConsignReturn
{
	class Program
	{
		private static AppConfig _appConfig;
		private static wcf.HctShipReturnParam _argConfig;
		private static List<wcf.F194715> _settings;
		private static string _scheduleName = "HCTConsignReturn";

		static void Main(string[] args)
		{
			//設定指定參數物件
			SetArgConfig(args);
			_settings = new List<wcf.F194715>();
			//取得客代主檔設定
			GetHctCustmerSetting();
			foreach (var setting in _settings)
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
			_appConfig.FilePath = string.Format(setting.LOCAL_TEMPPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
			_appConfig.LogPath = string.Format(setting.LOCAL_LOGPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
			_appConfig.LocalBackupPath = string.Format(setting.LOCAL_BACKUPPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
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
			_argConfig = new wcf.HctShipReturnParam();
			_appConfig = new AppConfig();
			_argConfig.AllId = "HCT"; //固定新竹貨運
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
			//_argConfig.CustomerId = "04665460018";
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
			Log(string.Format("Parameter={0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", _argConfig.CustomerId, _argConfig.DcCode ?? "0", _argConfig.GupCode ?? "0", _argConfig.CustCode ?? "0", _argConfig.Channel, _argConfig.AllId, _argConfig.DelvTimes,
				(_argConfig.DelvSDate.HasValue ? _argConfig.DelvSDate.Value.ToString("yyyy/MM/dd") : ""),
				(_argConfig.DelvEDate.HasValue ? _argConfig.DelvEDate.Value.ToString("yyyy/MM/dd") : ""),
				(_argConfig.OrdSDate.HasValue ? _argConfig.OrdSDate.Value.ToString("yyyy/MM/dd") : ""),
				(_argConfig.OrdEDate.HasValue ? _argConfig.OrdEDate.Value.ToString("yyyy/MM/dd") : "")));

			Log("開始回檔");
			Log("取得回檔資料");
			var proxy = new wcf.SharedWcfServiceClient();
			wcf.HctShipReturn[] resultData;
			try
			{
				resultData = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																									 , () => proxy.GetHctShipReturns(_argConfig), false, _appConfig.SchemaName);
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

			//產生TXT
			if (!Directory.Exists(_appConfig.FilePath)) Directory.CreateDirectory(_appConfig.FilePath);
			var di = new DirectoryInfo(_appConfig.FilePath);
			var fileName = string.Format("{0}.txt",  DateTime.Now.ToString("yyyyMMddHHmmss"));
			var fileFullName = Path.Combine(_appConfig.FilePath, fileName);
			var importResult = ImportTxt(resultData.ToList(), fileFullName);
			if (!importResult)
			{
				UpdateDBLogIsSuccess(id, "0", "產生txt檔失敗");
				Log("產生txt檔失敗");
				return;
			}

			Log("開始上傳檔案到FTP");
			//上傳FTP
			var ftp = new FtpUtility(_appConfig.FtpIp, _appConfig.FtpAccount, _appConfig.FtpPassword, _appConfig.FtpUploadPath, 60, true);
			var ftpResult = ftp.Upload(fileFullName);
			if (!ftpResult)
			{
				UpdateDBLogIsSuccess(id, "0", "上傳檔案到FTP失敗");
				Log("上傳檔案到FTP失敗");
				return;
			}
			else
				Log("上傳檔案到FTP成功");


			Log("開始寄送郵件");
			//上傳後寄發Mail
			var mailResult = SendMail(fileFullName, SendMailType.FtpUpload, null);
			if (!mailResult)
			{
				UpdateDBLogIsSuccess(id, "0", "寄送郵件失敗");
				Log("寄送郵件失敗");
				return;
			}
			else
				Log("寄送郵件成功");

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
			var zipResult = zip.FileZip(fileList, _appConfig.LocalBackupPath, fileName, _appConfig.ZipPassword);
			if (zipResult)
			{
				Log("壓縮檔案至備份路徑完成");
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

		#region 產生Txt
		private static bool ImportTxt(List<wcf.HctShipReturn> datas, string fileFullName)
		{
			try
			{
				using (var sw = new StreamWriter(fileFullName, false, Encoding.GetEncoding(950)))//BIG5
				{
					var sb = new StringBuilder();
					foreach (var item in datas)
					{
						//查貨號碼|清單編號|客戶代號|收貨人代號|收貨人名稱|	
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.CONSIGN_NO?.Trim(), item.CUST_ORD_NO?.Trim(), item.CONTRACT_CUST_NO?.Trim(), item.RECEIVER_CODE?.Trim(), item.RECEIVER_NAME?.Trim()));
						//收貨人電話1|收貨人電話2|收貨人地址|代收貨款|egamt|
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.RECEIVER_PHONE?.Trim(), item.RECEIVER_MOBILE?.Trim(), item.RECEIVER_ADDRESS?.Trim(), item.COLLECT_AMT, item.EGAMT?.Trim()));
						//發送日期|發送站代號|到著站代號|ekamt|件數|
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.SEND_DATE?.Trim(), item.SEND_CODE?.Trim(), item.ARRIVAL_CODE?.Trim(), item.EKAMT?.Trim(), item.PIECES?.Trim()));
						//追加件數|重量|ebamt|eramt|esamt|
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.ADD_PIECES?.Trim(), item.WEIGHT?.Trim(), item.EBAMT?.Trim(), item.ERAMT?.Trim(), item.ESAMT?.Trim()));
						//edamt|elamt|傳票區分|商品種類|商品區分|
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.EDAMT?.Trim(), item.ELAMT?.Trim(), item.SUMMON_TYPE?.Trim(), item.ITEM_KIND?.Trim(), item.ITEM_TYPE?.Trim()));
						//指定日期|指定時間|供貨人代號|供貨人名稱|供貨人電話1|
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.ASSIGN_DATE?.Trim(), item.ASSIGN_TIME?.Trim(), item.SUPPLIER_CODE?.Trim(), item.SUPPLIER_NAME?.Trim(), item.SUPPLIER_PHONE?.Trim()));
						//供貨人電話2|供貨人地址|備註|esel|eprint|
						sb.Append(string.Format("{0}|{1}|{2}|{3}|{4}|", item.SUPPLIER_MOBILE?.Trim(), item.SUPPLIER_ADDRESS?.Trim(), item.MEMO?.Trim(), item.ESEL?.Trim(), item.EPRINT?.Trim()));
						//郵遞區號
						sb.Append(string.Format("{0}", item.RECEIVER_ZIP_CODE ));
						sw.WriteLine(sb.ToString());
						sb.Clear();
					}
				}
				return true;
			}
			catch (Exception ex)
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
					mailSubject = string.Format(_appConfig.UploadSubject, DateTime.Today.ToString("yyyy/MM/dd"), "HCT出貨託運檔", "已上傳");
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
			catch (Exception ex)
			{
				return false;
			}

		}
		#endregion

		#region 更新託運單與派車狀態
		private static bool UpdateStatus(List<wcf.HctShipReturn> datas)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			var itemData = ExDataMapper.MapCollection<wcf.HctShipReturn, wcf.HctShipReturn>(datas).ToArray();
			var updateResult = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																						, () => proxy.UpdateStatusForHCT(itemData), false, _appConfig.SchemaName);
			if (updateResult.IsSuccessed)
				return true;

			return false;
		}

		#endregion

		#region 取得客代主檔設定

		private static void GetHctCustmerSetting()
		{
			var proxy = new wcf.SharedWcfServiceClient();
			_settings = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																									 , () => proxy.GetHctCustomerSetting(_argConfig.CustomerId), false, _appConfig.SchemaName).ToList();
		}

		#endregion
	}
}
