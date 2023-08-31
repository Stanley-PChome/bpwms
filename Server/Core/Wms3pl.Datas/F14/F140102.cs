namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 盤點單設定倉別
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","WAREHOUSE_ID","DC_CODE","GUP_CODE","CUST_CODE","AREA_CODE")]
  [Table("F140102")]
  public class F140102 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 倉別編號(F1980)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 通道起
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string CHANNEL_BEGIN { get; set; }

    /// <summary>
    /// 通道迄
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string CHANNEL_END { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
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
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 樓層起
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FLOOR_BEGIN { get; set; }

    /// <summary>
    /// 樓層迄
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FLOOR_END { get; set; }

    /// <summary>
    /// 座起
    /// </summary>
    [Column(TypeName = "char(2)")]
    public string PLAIN_BEGIN { get; set; }

    /// <summary>
    /// 座迄
    /// </summary>
    [Column(TypeName = "char(2)")]
    public string PLAIN_END { get; set; }

	  /// <summary>
	  /// 儲區
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string AREA_CODE { get; set; }
  }
}
        