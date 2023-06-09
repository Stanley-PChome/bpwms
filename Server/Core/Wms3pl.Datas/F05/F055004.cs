namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出貨包裝商品拆批號紀錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F055004")]
	public class F055004 : IAuditInfo
	{

		/// <summary>
		/// 資料Id
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }

		/// <summary>
		/// 物流中心
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
		/// 訂單編號
		/// </summary>
		[Required]
		public string ORD_NO { get; set; }

		/// <summary>
		/// 訂單序號
		/// </summary>
		[Required]
		public string ORD_SEQ { get; set; }

		/// <summary>
		/// 出貨單號
		/// </summary>
		[Required]
		public string WMS_NO { get; set; }

		/// <summary>
		/// 箱數編號
		/// </summary>
		[Required]
		public string BOX_NO { get; set; }

		/// <summary>
		/// 紙箱編號
		/// </summary>
		public string BOX_NUM { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 數量
		/// </summary>
		[Required]
		public int QTY { get; set; }

		/// <summary>
		/// 批號
		/// </summary>
		public string MAKE_NO { get; set; }

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

        /// <summary>
        /// 效期
        /// </summary>
        public DateTime? VALID_DATE { get; set; }
    }
}
