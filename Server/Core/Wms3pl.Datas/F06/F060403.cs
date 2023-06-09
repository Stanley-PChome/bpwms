namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 盤點完成結果回傳明細
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060403")]
	public class F060403 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }
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
		/// 任務單號
		/// </summary>
		[Required]
		public string DOC_ID { get; set; }
		/// <summary>
		/// 原WMS單號
		/// </summary>
		[Required]
		public string WMS_NO { get; set; }
		/// <summary>
		/// 倉庫代碼
		/// </summary>
		[Required]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		[Required]
		public string SKUCODE { get; set; }
		/// <summary>
		/// 系統記錄數量
		/// </summary>
		[Required]
		public int SKUSYSQTY { get; set; }
		/// <summary>
		/// 實際盤點數量
		/// </summary>
		[Required]
		public int SKUQTY { get; set; }
		/// <summary>
		/// 盤點人員(工號)
		/// </summary>
		[Required]
		public string OPERATOR { get; set; }
		/// <summary>
		/// 盤點時間
		/// </summary>
		[Required]
		public string OPERATORTIME { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品)
		/// </summary>
		[Required]
		public int SKULEVEL { get; set; }
		/// <summary>
		/// 商品效期(yyyy/mm/dd)
		/// </summary>
		public string EXPIRYDATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string OUTBATCHCODE { get; set; }
		/// <summary>
		/// 貨架編號
		/// </summary>
		public string SHELFCODE { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BINCODE { get; set; }
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
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
