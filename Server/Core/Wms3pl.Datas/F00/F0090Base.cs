using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
	public class F0090Base: IAuditInfo
	{

		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int64 ID { get; set; }

		/// <summary>
		/// API名稱
		/// </summary>
		[Required]
		public string NAME { get; set; }

		/// <summary>
		/// 傳遞資料
		/// </summary>
		public string SEND_DATA { get; set; }

		/// <summary>
		/// 回傳資料
		/// </summary>
		public string RETURN_DATA { get; set; }

		/// <summary>
		/// 狀態(0:失敗,1:成功)
		/// </summary>
		public string STATUS { get; set; }

		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string ERRMSG { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
