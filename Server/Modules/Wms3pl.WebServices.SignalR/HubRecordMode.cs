using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.SignalR
{
	/// <summary>
	/// 紀錄模式
	/// </summary>
	public enum HubRecordMode
	{
		/// <summary>
		/// Server記憶體
		/// </summary>
		Memory=1,
		/// <summary>
		/// 資料庫(F0070)
		/// </summary>
		DataBase = 2
	}
}
