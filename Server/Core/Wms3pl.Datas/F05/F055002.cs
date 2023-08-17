namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨包裝身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("WMS_ORD_NO","PACKAGE_BOX_NO","PACKAGE_BOX_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F055002")]
  public class F055002 : IAuditInfo
  {

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 包裝箱號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 PACKAGE_BOX_NO { get; set; }

	  /// <summary>
	  /// 包裝箱序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 PACKAGE_BOX_SEQ { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 出貨數*****待刪除*******
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? DELV_QTY { get; set; }

    /// <summary>
    /// 包裝數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? PACKAGE_QTY { get; set; }

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
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
    /// 電腦名稱
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CLIENT_PC { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }
    /// <summary>
    /// 訂單序號
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string ORD_SEQ { get; set; }
    /// <summary>
    /// 工作站編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WORKSTATION_CODE { get; set; }
  }
}