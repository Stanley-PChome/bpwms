namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("WMS_ORD_NO","WMS_ORD_SEQ","GUP_CODE","CUST_CODE","DC_CODE")]
  [Table("F050802")]
  public class F050802 : IAuditInfo
  {

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 出貨單序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string WMS_ORD_SEQ { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 訂貨數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? ORD_QTY { get; set; }

    /// <summary>
    /// 預計出貨數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? B_DELV_QTY { get; set; }

    /// <summary>
    /// 實際出貨數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? A_DELV_QTY { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }
  }
}
        