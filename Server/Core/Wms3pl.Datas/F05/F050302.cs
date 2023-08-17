namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 貨主單據身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_NO","ORD_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F050302")]
  public class F050302 : IAuditInfo
  {

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

	  /// <summary>
	  /// 訂單序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string ORD_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 訂貨數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ORD_QTY { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
	  /// ***********待刪除***********
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string WELCOME_LETTER { get; set; }

    /// <summary>
    /// 商品單價
    /// </summary>
    [Column(TypeName = "decimal(13, 2)")]
    public decimal? PRICE { get; set; }

    /// <summary>
    /// 商品金額
    /// </summary>
    [Column(TypeName = "decimal(13, 2)")]
    public decimal? AMOUNT { get; set; }

	  /// <summary>
	  /// 設定不出貨(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string NO_DELV { get; set; }

    /// <summary>
    /// 指定廠商編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }
    /// <summary>
    /// 指定出貨批號
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }
	}
}
        