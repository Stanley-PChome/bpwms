using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	/// <summary>
	/// 紙箱補貨通知與處理
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F056002")]
	public class F056002 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public Int64 ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		[Required]
		public string WORKSTATION_CODE { get; set; }
		/// <summary>
		/// 紙箱編號
		/// </summary>
		[Required]
		public string BOX_CODE { get; set; }
		/// <summary>
		/// 樓層
		/// </summary>
		[Required]
		public string FLOOR { get; set; }
		/// <summary>
		/// 補貨數量
		/// </summary>
		public int? QTY { get; set; }
		/// <summary>
		/// 狀態(0=未補貨;1=準備中;2:結案)
		/// </summary>
		[Required]
		public string STATUS { get; set; }
		/// <summary>
		/// 補貨人員
		/// </summary>
		public string REPLENISH_STAFF { get; set; }
		/// <summary>
		/// 補貨人名
		/// </summary>
		public string REPLENISH_NAME { get; set; }
		/// <summary>
		/// 補貨開始時間
		/// </summary>
		public DateTime? REPLENISH_STARTTIME { get; set; }
		/// <summary>
		/// 補貨完成時間
		/// </summary>
		public DateTime? REPLENISH_FINISHTIME { get; set; }
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
