namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 配庫試算缺貨檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F050805")]
	public class F050805 : IAuditInfo
	{

		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal ID { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主
    /// </summary>
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 試算編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CAL_NO { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 揀區總庫存數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? TTL_PICK_STOCK_QTY { get; set; }

    /// <summary>
    /// 總訂購數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? TTL_ORD_QTY { get; set; }

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
    /// 倉別型態編號 F198001
    /// </summary>
    [Column(TypeName = "varchar(8)")]
    public string TYPE_ID { get; set; }

    /// <summary>
    /// 補區總庫存數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? TTL_RESUPPLY_STOCK_QTY { get; set; }

    /// <summary>
    /// 虛擬儲位回復總庫存數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? TTL_VIRTUAL_STOCK_QTY { get; set; }

    /// <summary>
    /// 總缺貨數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? TTL_OUTSTOCK_QTY { get; set; }

    /// <summary>
    /// 建議補貨庫存數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? SUG_RESUPPLY_STOCK_QTY { get; set; }

    /// <summary>
    /// 建議虛擬儲位回復庫存數
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? SUG_VIRTUAL_STOCK_QTY { get; set; }

    /// <summary>
    /// 指定批號
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }

    /// <summary>
    /// 指定序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }
	}
}
