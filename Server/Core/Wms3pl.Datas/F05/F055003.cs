namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨箱資料紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("WMS_ORD_NO","ITEM_CODE","PACKAGE_BOX_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F055003")]
  public class F055003 : IAuditInfo
  {

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 類型
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string TYPE { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 出貨箱條碼
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(25)")]
    public string BOX_BARCODE { get; set; }

    /// <summary>
    /// 門市
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RETAIL_CODE { get; set; }

    /// <summary>
    /// 車次
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string DELV_NO { get; set; }

    /// <summary>
    /// 路順
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string DELV_WAY { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 PACKAGE_BOX_NO { get; set; }

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
  }
}
        