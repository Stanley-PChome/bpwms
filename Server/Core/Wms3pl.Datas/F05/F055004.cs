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
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

		/// <summary>
		/// 訂單編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

		/// <summary>
		/// 訂單序號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string ORD_SEQ { get; set; }

		/// <summary>
		/// 出貨單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

		/// <summary>
		/// 箱數編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string BOX_NO { get; set; }

    /// <summary>
    /// 紙箱編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string BOX_NUM { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

		/// <summary>
		/// 數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int QTY { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string MAKE_NO { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? VALID_DATE { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }
  }
}
