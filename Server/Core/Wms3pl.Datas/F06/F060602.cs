namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 下位系統的庫存原始明細資料
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060602")]
	public class F060602 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }
		/// <summary>
		/// F060601的流水ID
		/// </summary>
		[Required]
		public long F060601_ID { get; set; }
		/// <summary>
		/// 倉庫代碼
		/// </summary>
		[Required]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 貨架編號
		/// </summary>
		public string SHELF_CODE { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BIN_CODE { get; set; }
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
		/// 品號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 商品等級(0=殘品、1=正品)
		/// </summary>
		[Required]
		public int SKU_LEVLE { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		[Required]
		public string VALID_DATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[Required]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		[Required]
		public int QTY { get; set; }
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
