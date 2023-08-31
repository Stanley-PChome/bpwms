namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 儲位異動LOG
  /// </summary>
  [Serializable]
  [DataServiceKey("SEQ")]
  [Table("F191301")]
  public class F191301 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[Column(TypeName = "bigint")]
		public Int64 SEQ { get; set; }

		/// <summary>
		/// 異動項目(F000904) (topic=F191301, subtopic=action) 新增/調整/刪除
		/// </summary>
		[Column(TypeName = "varchar(30)")]
		public string WH_FIELD { get; set; }

		/// <summary>
		/// 異動原因(F000904) (topic=F191301, subtopic=whmovement)
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string WH_REASON { get; set; }

		/// <summary>
		/// 新值
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string NEW_VALUE { get; set; }

		/// <summary>
		/// 儲位
		/// </summary>
		[Column(TypeName = "varchar(14)")]
		public string LOC_CODE { get; set; }

		/// <summary>
		/// 品號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 數量
		/// </summary>
		[Column(TypeName = "bigint")]
		public Int64? QTY { get; set; }

		/// <summary>
		/// 效期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? VALID_DATE { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 商品序號
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string SERIAL_NO { get; set; }

		/// <summary>
		/// 箱號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string BOX_CTRL_NO { get; set; }

		/// <summary>
		/// 板號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string PALLET_CTRL_NO { get; set; }

		/// <summary>
		/// 單據號碼(WMS_NO)
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string WMS_NO { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 批號
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string MAKE_NO { get; set; }
  }
}
        