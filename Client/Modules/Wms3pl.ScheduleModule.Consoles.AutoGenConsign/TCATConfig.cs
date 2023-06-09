using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.ScheduleModule.Consoles.AutoGenConsign
{
	public class AppConfig
	{
		/// <summary>
		/// 產生檔案預設暫存路徑
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// 連線資料庫名稱
		/// </summary>
		public string SchemaName { get; set; }
	}
}
