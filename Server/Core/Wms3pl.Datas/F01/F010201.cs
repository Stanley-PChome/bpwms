namespace Wms3pl.Datas.F01
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 進倉單主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("STOCK_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F010201")]
	public class F010201 : IAuditInfo
	{

		/// <summary>
		/// 進倉單號
		/// </summary>
		[Key]
		[Required]
		public string STOCK_NO { get; set; }

		/// <summary>
		/// 建立進倉單日期
		/// </summary>
		[Required]
		public DateTime STOCK_DATE { get; set; }

		/// <summary>
		/// 採購日期
		/// </summary>
		public DateTime? SHOP_DATE { get; set; }

		/// <summary>
		/// 預定進貨日
		/// </summary>
		[Required]
		public DateTime DELIVER_DATE { get; set; }

		/// <summary>
		/// 來源單據 F000902
		/// </summary>
		public string SOURCE_TYPE { get; set; }

		/// <summary>
		/// 來源單號
		/// </summary>
		public string SOURCE_NO { get; set; }

		/// <summary>
		/// 單據性質(異動類型)F000903
		/// </summary>
		[Required]
		public string ORD_PROP { get; set; }

		/// <summary>
		/// 廠商編號 F1908
		/// </summary>
		[Required]
		public string VNR_CODE { get; set; }

		/// <summary>
		/// 貨主單號
		/// </summary>
		public string CUST_ORD_NO { get; set; }

		/// <summary>
		/// 貨主成本中心
		/// </summary>
		public string CUST_COST { get; set; }

		/// <summary>
		/// 單據狀態(0待處理1驗收中2結案8異常9取消) F000904
		/// </summary>
		[Required]
		public string STATUS { get; set; }

		/// <summary>
		/// 備註
		/// </summary>
		public string MEMO { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

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
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 採購單號
		/// </summary>
		public string SHOP_NO { get; set; }

		/// <summary>
		/// 是否為EDI來源的單據
		/// </summary>
		[Required]
		public string EDI_FLAG { get; set; }

		/// <summary>
		/// 驗證碼
		/// </summary>
		public string CHECK_CODE { get; set; }
		[Required]
		public string CHECKCODE_EDI_STATUS { get; set; }

		/// <summary>
		/// 出貨驗證碼(ex: 報關單號)
		/// </summary>
		public string DELV_CHECKCODE { get; set; }

		/// <summary>
		/// 對外單號
		/// </summary>
		public string FOREIGN_WMSNO { get; set; }

		/// <summary>
		/// 對外客戶編號
		/// </summary>
		public string FOREIGN_CUSTCODE { get; set; }

		/// <summary>
		/// 批號
		/// </summary>
		public string BATCH_NO { get; set; }

		/// <summary>
		/// 0:貨主單據建立 1:貨主單據更正 2:貨主單據取消
		/// </summary>
		public string IMPORT_FLAG { get; set; }
		/// <summary>
		/// 快速通關分類
		/// </summary>
		public string FAST_PASS_TYPE { get; set; }
		/// <summary>
		/// 預定進倉時段
		/// </summary>
		public string BOOKING_IN_PERIOD { get; set; }
	}
}
