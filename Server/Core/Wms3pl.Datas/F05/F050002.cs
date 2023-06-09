namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 訂單池明細
	/// </summary>
	[Serializable]
	[DataServiceKey("ORD_NO", "ORD_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F050002")]
	public class F050002 : IAuditInfo
	{

		/// <summary>
		/// 訂單編號
		/// </summary>
		[Key]
		[Required]
		public string ORD_NO { get; set; }

		/// <summary>
		/// 訂單序號
		/// </summary>
		[Key]
		[Required]
		public string ORD_SEQ { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 訂貨數
		/// </summary>
		[Required]
		public Int32 ORD_QTY { get; set; }

		/// <summary>
		/// 商品序號
		/// </summary>
		public string SERIAL_NO { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// ***********待刪除***********
		/// </summary>
		[Required]
		public string WELCOME_LETTER { get; set; }

		/// <summary>
		/// 商品單價
		/// </summary>
		public decimal? PRICE { get; set; }

		/// <summary>
		/// 商品金額
		/// </summary>
		public decimal? AMOUNT { get; set; }

		/// <summary>
		/// 是否已檢查過同質性商品，0:未檢查 1:已檢查
		/// </summary>
		public string CHECKED_SAMEITEM { get; set; }

		/// <summary>
		/// 設定不出貨(0否1是)
		/// </summary>
		[Required]
		public string NO_DELV { get; set; }

		/// <summary>
		/// 指定廠商編號
		/// </summary>
		public string VNR_CODE { get; set; }
		/// <summary>
		/// 指定商品驗收批號
		/// </summary>
		public string MAKE_NO { get; set; }
	}
}