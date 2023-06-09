using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.ScheduleModule.Consoles.HCTGetReply
{
	public class AppConfig
	{
		/// <summary>
		/// Ftp Ip 位址
		/// </summary>
		public string FtpIp { get; set; }
		/// <summary>
		/// Ftp下載路徑
		/// </summary>
		public string FtpDownloadPath { get; set; }
		/// <summary>
		/// Ftp備份路徑
		/// </summary>
		public string FtpBackupPath { get; set; }
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
		/// 產生SOD檔案路徑
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// 本機存放備份檔路徑
		/// </summary>
		public string LocalBackupPath { get; set; }
		/// <summary>
		/// 連線資料庫名稱
		/// </summary>
		public string SchemaName { get; set; }
	}
}
