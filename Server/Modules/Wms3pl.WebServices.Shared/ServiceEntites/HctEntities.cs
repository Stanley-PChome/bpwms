using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
	public class HctStation
	{
		/// <summary>
		/// 群組(固定1)
		/// </summary>
		public string Group { get; set; }
		/// <summary>
		/// 站所四碼代號
		/// </summary>
		public string PutData_m { get; set; }
		/// <summary>
		/// 站所簡碼
		/// </summary>
		public string PutData_s { get; set; }
		/// <summary>
		/// 站所名稱
		/// </summary>
		public string PutDataName { get; set; }

		/// <summary>
		/// 郵遞區號
		/// </summary>
		public string PutDataZip { get; set; }

		/// <summary>
		/// 優勢困難配送
		/// </summary>
		public string PutAo_Flag { get; set; }

	}
}
