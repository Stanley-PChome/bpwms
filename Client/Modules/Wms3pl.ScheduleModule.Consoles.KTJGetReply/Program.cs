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

namespace Wms3pl.ScheduleModule.Consoles.KTJGetReply
{
	class Program
	{
		private static AppConfig _appConfig;
		private static wcf.EgsReturnConsignParam _argConfig;
		private static List<wcf.F194714> _statusList;
		private static List<wcf.F194715> _settings;
		private static string _scheduleName = "KTJGetReply";
		private static List<KeyValuePair<string, string>> _f050901StatusList;
		static void Main(string[] args)
		{
			SetArgConfig(args);
			_settings = new List<wcf.F194715>();
			GetKtjCustomerSetting();
			//取得貨態表
			GetKTJStatusList();
			//取得F050901 Status對照表
			SetF050901StatusList();
			foreach (var setting in _settings)
			{
				//設定Ftp物件
				SetAppConfig(setting);
				//指定客代編號
				_argConfig.CustomerId = setting.CUSTOMER_IDk__BackingField;
				ExecGetReplyData();
			}

			//如果暫存資料夾沒檔案就刪除資料夾
			if (Directory.GetFiles(_appConfig.FilePath).Length == 0)
				Directory.Delete(_appConfig.FilePath);
		}

		private static void SetArgConfig(string[] args)
		{
			_argConfig = new wcf.EgsReturnConsignParam();
			_appConfig = new AppConfig();
			_argConfig.AllId = "KTJ"; //固定大榮物流
			if (args.Any())
			{
				ConsoleHelper.ArgumentsTransform(args, _argConfig);
				ConsoleHelper.ArgumentsTransform(args, _appConfig);
			}
#if (DEBUG)
			//_argConfig.CustomerId = "";
			_appConfig.SchemaName = "BPWMS";
#endif
		}

		#region 取得客代主檔設定
		private static void GetKtjCustomerSetting()
		{
			var proxy = new wcf.SharedWcfServiceClient();
			_settings = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel
																									 , () => proxy.GetKtjCustomerSetting(_argConfig.CustomerId), false, _appConfig.SchemaName).ToList();
		}
		#endregion

		#region 取得貨態
		private static bool GetKTJStatusList()
		{
			var proxy = new wcf.SharedWcfServiceClient();
			#region 取得狀態項目資料
			try
			{
				_statusList = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel, () => proxy.GetStatusListByAllId(_argConfig.AllId), false, _appConfig.SchemaName).ToList();
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				Log(ex.Source);
				Log(ex.StackTrace);
				Log("取得狀態項目資料失敗");
				return false;
			}
			return true;
			#endregion
		}
		#endregion

		private static void SetF050901StatusList()
		{
			_f050901StatusList = new List<KeyValuePair<string, string>>();
			_f050901StatusList.Add(new KeyValuePair<string, string>("0", "未配送"));
			_f050901StatusList.Add(new KeyValuePair<string, string>("2", "配送中"));
			_f050901StatusList.Add(new KeyValuePair<string, string>("3", "已配達"));
			_f050901StatusList.Add(new KeyValuePair<string, string>("4", "退貨"));
			_f050901StatusList.Add(new KeyValuePair<string, string>("5", "異常"));

		}

		/// <summary>
		/// 設定Ftp物件
		/// </summary>
		private static void SetAppConfig(wcf.F194715 setting)
		{
			_appConfig.FtpIp = setting.FTP_IPk__BackingField;
			_appConfig.FtpAccount = setting.FTP_ACCOUNTk__BackingField;
			_appConfig.FtpPassword = setting.FTP_PASSWORDk__BackingField;
			_appConfig.FtpDownloadPath = setting.FTP_DOWNLOADPATHk__BackingField;
			_appConfig.FilePath = string.Format(setting.LOCAL_TEMPPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
			_appConfig.LogPath = string.Format(setting.LOCAL_LOGPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
			_appConfig.LocalBackupPath = string.Format(setting.LOCAL_BACKUPPATHk__BackingField, _scheduleName, DateTime.Today.ToString("yyyyMMdd"));
			if (!Directory.Exists(_appConfig.LocalBackupPath))
				Directory.CreateDirectory(_appConfig.LocalBackupPath);
		}

		private static void ExecGetReplyData()
		{
			if (!Directory.Exists(_appConfig.FilePath))
				Directory.CreateDirectory(_appConfig.FilePath);
			Log("開始");
			Log("取得回檔資料");
			#region FTP 檔案下載
			var ftp = new FtpUtility(_appConfig.FtpIp, _appConfig.FtpAccount, _appConfig.FtpPassword, _appConfig.FtpDownloadPath, 60, true);
			var ftpFiles = ftp.GetFileList();
			if (ftpFiles != null && ftpFiles.Any())
			{
				ftpFiles = ftpFiles.Where(x => x.ToUpper().Contains(".CSV") && x.ToUpper().Contains(string.Format("SD_{0}", DateTime.Today.ToString("yyyyMMdd")))).ToList();
				foreach (var fileName in ftpFiles)
				{
					var id = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, _argConfig.CustCode, _scheduleName);
					if (ftp.Download(fileName, _appConfig.FilePath))
					{
						Log(string.Format("客戶代號:{0} 下載FTP檔案{1}成功!!", _argConfig.CustomerId, fileName));
						if (ftp.DeleteFile(fileName))
						{
							Log(string.Format("客戶代號:{0} 刪除FTP檔案{1}成功!!", _argConfig.CustomerId, fileName));
							UpdateDBLogIsSuccess(id, "1", string.Format("客戶代號:{0} 下載FTP檔案{1}與刪除FTP檔案{1}成功!!", _argConfig.CustomerId, fileName));
						}
						else
						{
							UpdateDBLogIsSuccess(id, "0", string.Format("客戶代號:{0} 刪除FTP檔案{1}失敗!!", _argConfig.CustomerId, fileName));
							Log(string.Format("客戶代號:{0} 刪除FTP檔案{1}失敗!!", _argConfig.CustomerId, fileName));
						}
					}
					else
					{
						UpdateDBLogIsSuccess(id, "0", string.Format("客戶代號:{0} 下載FTP檔案{1}失敗!!", _argConfig.CustomerId, fileName));
						Log(string.Format("客戶代號:{0} 下載FTP檔案{1}失敗!!", _argConfig.CustomerId, fileName));
					}
				}
			}
			else
			{
				var id = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, _argConfig.CustCode, _scheduleName);
				if (ftpFiles == null)
				{
					UpdateDBLogIsSuccess(id, "0", ftp.Logger.ToString());
					Log(ftp.Logger.ToString());
				}
				else
				{
					UpdateDBLogIsSuccess(id, "1", "FTP上無CSV檔案!!");
					Log("FTP上無CSV檔案!!");
				}
			}
			#endregion
			ProcessLocalFileData();
			Log("結束");
		}

		private static void ProcessLocalFileData()
		{
			var id = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, _argConfig.CustCode, _scheduleName);
			var files = Directory.GetFiles(_appConfig.FilePath).Select(x => Path.GetFileName(x)).Where(x => x.ToUpper().Contains(".CSV") && x.ToUpper().Contains(string.Format("SD_{0}", DateTime.Today.ToString("yyyyMMdd")))).ToList();
			var bkfiles = Directory.GetFiles(_appConfig.LocalBackupPath).Select(x => Path.GetFileName(x)).Where(x => x.ToUpper().Contains(".CSV") && x.ToUpper().Contains(string.Format("SD_{0}", DateTime.Today.ToString("yyyyMMdd")))).ToList();
			if (files != null && files.Any())
			{
				foreach (string fname in files)
				{
					#region 備份檔案

					var source = Path.Combine(_appConfig.FilePath, fname);
					var dest = Path.Combine(_appConfig.LocalBackupPath, fname);
					var exist = (from e in bkfiles where e.Contains(fname) select e).Any();
					var upFile = fname;
					if (exist)
					{
						upFile = fname.Replace(Path.GetExtension(fname), string.Format("_{0}{1}", DateTime.Now.ToString("HHmmss"), Path.GetExtension(fname)));
						dest = Path.Combine(_appConfig.LocalBackupPath, upFile);
					}
					File.Copy(source, dest);

					#endregion

					Log(string.Format("讀取{0}，開始更新託運單狀態", source));
					UpdateF050901Data(upFile, id);
					File.Delete(source);
				}
			}
			else
			{
				UpdateDBLogIsSuccess(id, "1", string.Format("路徑：{0} 無CSV檔案!!", _appConfig.FilePath));
				Log(string.Format("路徑：{0} 無CSV檔案!!", _appConfig.FilePath));
			}
		}

		private static void UpdateF050901Data(string fileName, int id)
		{
			var proxy = new wcf.SharedWcfServiceClient();

			var updates = new List<wcf.F050901>();
			var fileData = new List<KTJGetReply>();
			#region 取得檔案內容
			string line;
			StreamReader file = new StreamReader(Path.Combine(_appConfig.LocalBackupPath, fileName), Encoding.GetEncoding(950));
			while ((line = file.ReadLine()) != null)
			{
				string[] str = line.Split('|');
				if (str.Length != 8)
				{
					Log(string.Format("客戶代號:{0} 檔案:{1} 資料格式錯誤:{2}", _argConfig.CustomerId, fileName, line));
					continue;
				}

				var data = new KTJGetReply();
				data.CONSIGN_NO = str[0];
				data.CUST_ID = str[4];
				data.STATUS_ID = str[5];
				data.IN_DATE = str[3];
				data.RESULT = str[6];
				fileData.Add(data);
			}
			file.Close();
			if (fileData.Count == 0)
			{
				UpdateDBLogIsSuccess(id, "0", string.Format("客戶代號:{0} 檔案{1}無資料!!", _argConfig.CustomerId, fileName));
				Log(string.Format("客戶代號:{0} 檔案{1}無資料!!", _argConfig.CustomerId, fileName));
				return;
			}
			#endregion

			#region 處理檔案內容排除非此客戶代號託運單
			var notNowCustIdData = fileData.Where(x => x.CUST_ID != _argConfig.CustomerId).ToList();
			if (notNowCustIdData.Any())
			{
				foreach (var item in notNowCustIdData)
					Log(string.Format("客戶代號:{0} 檔案:{1} 託運單號:{2}的客戶代號{3}與目前客戶代號不同", _argConfig.CustomerId, fileName, item.CONSIGN_NO, item.CUST_ID));
			}
			#endregion

			#region 取得對應FTP檔案的託運單資料
			var consignNos = (from o in fileData
							  where o.CUST_ID == _argConfig.CustomerId
							  select o.CONSIGN_NO).Distinct().ToList();

			if (!consignNos.Any())
			{
				Log(string.Format("客戶代號:{0} 檔案:{1} 無此客戶代號的託運單號資料", _argConfig.CustomerId, fileName));
				UpdateDBLogIsSuccess(id, "1", string.Format("客戶代號:{0} 檔案:{1} 無此客戶代號的託運單號資料", _argConfig.CustomerId, fileName));
				return;
			}

			wcf.F050901[] oldDatas;
			try
			{
				Log(string.Format("客戶代號:{0} 檔案:{1}中有{2}筆資料(託運單號數共{3}張)!!", _argConfig.CustomerId, fileName, fileData.Count, consignNos.Count));

				oldDatas = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel, () => proxy.GetUpdateF050901DataForLogId(_argConfig.CustomerId, "TOMORROWDELIVID", consignNos.ToArray()), false, _appConfig.SchemaName);
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				Log(ex.Source);
				Log(ex.StackTrace);
				UpdateDBLogIsSuccess(id, "0", "取得託運單資料失敗");
				return;
			}
			#endregion

			#region 更新前資料處理
			_statusList.ForEach(o => o.STATUS_IDk__BackingField = o.STATUS_IDk__BackingField.PadLeft(2, '0'));

			var joindata = from o in fileData
						   join st in _statusList
						   on o.STATUS_ID equals st.STATUS_IDk__BackingField into ost
						   from st in ost.DefaultIfEmpty()
						   join f591 in oldDatas
						   on o.CONSIGN_NO equals f591.CONSIGN_NOk__BackingField into of591
						   from f591 in of591.DefaultIfEmpty()
						   where o.CUST_ID == _argConfig.CustomerId
						   select new { Consign = o, Status = st, f050901 = f591 };

			var groupData = from o in joindata
							group o by new { o.Consign.CONSIGN_NO, o.f050901 } into g
							select g;
			var successCount = 0;
			foreach (var consign in groupData)
			{
				if (consign.Key.f050901 == null)
				{
					var id2 = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, _argConfig.CustCode, _scheduleName);
					UpdateDBLogIsSuccess(id2, "0", string.Format("客戶代號:{0} 託運單號{1}不存在於DB!!", _argConfig.CustomerId, consign.Key.CONSIGN_NO));
					Log(string.Format("客戶代號:{0} 託運單號：{1}不存在於DB!!", _argConfig.CustomerId, consign.Key.CONSIGN_NO));
					continue;
				}
				var noMapStatusList = consign.Where(x => x.Status == null);
				if (noMapStatusList.Any())
				{
					foreach (var noMapItem in noMapStatusList)
					{
						var id2 = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, _argConfig.CustCode, _scheduleName);
						UpdateDBLogIsSuccess(id2, "0", string.Format("客戶代號:{0} 託運單號{1}的狀態ID{2}找不到對應資料!!", _argConfig.CustomerId, noMapItem.Consign.CONSIGN_NO, noMapItem.Consign.STATUS_ID));
						Log(string.Format("客戶代號:{0} 託運單號：{1} 的狀態ID {2} 找不到對應資料!!", _argConfig.CustomerId, noMapItem.Consign.CONSIGN_NO, noMapItem.Consign.STATUS_ID));
					}
				}
				var UpdateConsignList = consign.Where(x => x.Status != null);
				var isOk = false;
				foreach (var mapItem in UpdateConsignList)
				{
					if ((consign.Key.f050901.STATUSk__BackingField == "3" && mapItem.Status.STATUSk__BackingField != "3") ||
							(consign.Key.f050901.STATUSk__BackingField == "2" && mapItem.Status.STATUSk__BackingField == "0"))
					{
						var statusDesc1 = string.Empty;
						if (consign.Key.f050901.RESULTk__BackingField != null)
							statusDesc1 = _statusList.FirstOrDefault(o => o.STATUS_IDk__BackingField == consign.Key.f050901.RESULTk__BackingField)?.STATUS_DESCk__BackingField;
						else
							statusDesc1 = _f050901StatusList.FirstOrDefault(x => x.Key == consign.Key.f050901.STATUSk__BackingField).Value;

						var id2 = InsertDBLog(_argConfig.DcCode, _argConfig.GupCode, _argConfig.CustCode, _scheduleName);
						UpdateDBLogIsSuccess(id2, "0", string.Format("客戶代號:{0} 託運單號：{1} 狀態已經是{2}，不再更新為{3}!!", _argConfig.CustomerId, mapItem.Consign.CONSIGN_NO, statusDesc1, mapItem.Status.STATUS_DESCk__BackingField));
						Log(string.Format("客戶代號:{0} 託運單號：{1} 狀態已經是{2}，不再更新為{3}!!", _argConfig.CustomerId, mapItem.Consign.CONSIGN_NO, statusDesc1, mapItem.Status.STATUS_DESCk__BackingField));
						continue;
					}
					consign.Key.f050901.STATUSk__BackingField = mapItem.Status.STATUSk__BackingField;
					consign.Key.f050901.RESULTk__BackingField = mapItem.Consign.RESULT;
					var inDate = GetDate(mapItem.Consign.IN_DATE);
					if (consign.Key.f050901.STATUSk__BackingField != "3")
					{
						if (consign.Key.f050901.SEND_DATEk__BackingField == null && consign.Key.f050901.STATUSk__BackingField != "4")
							consign.Key.f050901.SEND_DATEk__BackingField = inDate;
					}
					else
					{
						if (consign.Key.f050901.PAST_DATEk__BackingField == null)
							consign.Key.f050901.PAST_DATEk__BackingField = inDate;
					}
					isOk = true;
					successCount++;
				}
				if (isOk)
					updates.Add(consign.Key.f050901);

				#region 更新託運單資料
				var result = UpdateStatus(updates);

				if (result)
				{
					UpdateDBLogIsSuccess(id, "1", string.Format("客戶代號:{0} 檔案:{1} 更新託運單成功 (總筆數：{2} 成功筆數：{3} 已更新託運單{4}張)", _argConfig.CustomerId, fileName, fileData.Count, successCount, updates.Count));
					Log(string.Format("客戶代號:{0} 檔案:{1} 更新託運單成功 (總筆數：{2} 成功筆數：{3} 已更新託運單{4}張)", _argConfig.CustomerId, fileName, fileData.Count, successCount, updates.Count));
				}
				else
				{
					UpdateDBLogIsSuccess(id, "0", "更新託運單失敗");
					Log("更新託運單失敗");
				}
				#endregion
			}
			#endregion
		}

		private static bool UpdateStatus(List<wcf.F050901> datas)
		{
			if (datas.Any())
			{
				var proxy = new wcf.SharedWcfServiceClient();
				var updateResult = WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel, () => proxy.UpdateStatusSOD(datas.ToArray()), false, _appConfig.SchemaName);
				if (!updateResult.IsSuccessed)
					return false;
			}
			return true;
		}

		private static DateTime GetDate(string inDate)
		{
			var year = Convert.ToInt32(inDate.Substring(0, 4));
			var month = Convert.ToInt32(inDate.Substring(4, 2));
			var day = Convert.ToInt32(inDate.Substring(6, 2));
			var hour = Convert.ToInt32(inDate.Substring(8, 2));
			var minute = Convert.ToInt32(inDate.Substring(10, 2));
			var second = Convert.ToInt32(inDate.Substring(12, 2));
			return new DateTime(year, month, day, hour, minute, second);
		}

		#region Log
		private static void Log(string message)
		{
			if (!Directory.Exists(_appConfig.LogPath))
				Directory.CreateDirectory(_appConfig.LogPath);
			var fileFullName = Path.Combine(_appConfig.LogPath, string.Format("Log_{0}.txt", DateTime.Today.ToString("yyyyMMdd")));
			using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
				sw.WriteLine(string.Format("{0}--{1}", DateTime.Now.ToString("HH:mm:ss"), message));
		}
		#endregion

		#region DB Log
		private static int InsertDBLog(string dcCode, string gupCode, string custCode, string scheduleName)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			return WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel, () => proxy.InsertDbLog(dcCode ?? "0", gupCode ?? "0", custCode ?? "0", scheduleName, "0", ""), false, _appConfig.SchemaName);
		}

		public static void UpdateDBLogIsSuccess(int id, string isSuccessful, string message)
		{
			var proxy = new wcf.SharedWcfServiceClient();
			WcfServiceHelper.ExecuteForConsoleBySchemaName(proxy.InnerChannel, () => proxy.UpdateDbLogIsSuccess(id, isSuccessful, message), false, _appConfig.SchemaName);
		}
		#endregion
	}

	public class KTJGetReply
	{
		public string CUST_ID { get; set; }
		public string CONSIGN_NO { get; set; }
		public string STATUS_ID { get; set; }
		public string IN_DATE { get; set; }
		public string RESULT { get; set; }
	}
}
