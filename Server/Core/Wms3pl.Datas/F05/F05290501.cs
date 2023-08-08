namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	[Serializable]
	[DataServiceKey("ID")]
	[Table("F05290501")]
	public class F05290501 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }
		/// <summary>
		/// F05290401.ID
		/// </summary>
		[Required]
		public long REF_ID { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[Required]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨項次
		/// </summary>
		[Required]
		public string PICK_ORD_SEQ { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[Required]
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 出貨項次
		/// </summary>
		[Required]
		public string WMS_ORD_SEQ { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		public string SERIAL_NO { get; set; }
		/// <summary>
		/// 實際播種數量
		/// </summary>
		[Required]
		public int A_SET_QTY { get; set; }
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
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員編號
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
