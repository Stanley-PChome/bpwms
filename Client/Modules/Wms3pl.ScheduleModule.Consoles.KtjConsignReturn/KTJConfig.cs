using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.ScheduleModule.Consoles.KtjConsignReturn
{
	public class AppConfig
	{
		/// <summary>
		/// Ftp Ip 位址
		/// </summary>
		public string FtpIp { get; set; }
		/// <summary>
		/// Ftp上傳路徑
		/// </summary>
		public string FtpUploadPath { get; set; }
		/// <summary>
		/// 本機備份路徑
		/// </summary>
		public string LocalBackupPath { get; set; }
		/// <summary>
		/// Ftp登入帳號
		/// </summary>
		public string FtpAccount { get; set; }
		/// <summary>
		/// Ftp登入密碼
		/// </summary>
		public string FtpPassword { get; set; }
		/// <summary>
		/// Log路徑
		/// </summary>
		public string LogPath { get; set; }
		/// <summary>
		/// 產生檔案預設暫存路徑
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// 上傳後發送郵件地址
		/// </summary>
		public string UploadMail { get; set; }
		/// <summary>
		/// 上傳後發送郵件CC地址
		/// </summary>
		public string UploadMailCC { get; set; }
		/// <summary>
		/// 上船後發送郵件主旨
		/// </summary>
		public string UploadSubject { get; set; }
		/// <summary>
		/// 壓縮檔密碼
		/// </summary>
		public string ZipPassword { get; set; }

		/// <summary>
		/// 連線資料庫名稱
		/// </summary>
		public string SchemaName { get; set; }
	}

	#region SendMail 列舉
	public enum SendMailType
	{
		FtpUpload = 1,
		Error = 2
	}
	#endregion
}
