namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 盤點單設定儲位
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F140111")]
  public class F140111 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(38, 0)")]
    public Decimal SEQ { get; set; }

	  /// <summary>
	  /// 倉別編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(8)")]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 儲區編號
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string AREA_CODE { get; set; }

	  /// <summary>
	  /// 紀錄輸入的儲位編號 (完整或有含遮罩)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE_PART { get; set; }

	  /// <summary>
	  /// 遮罩編號 (0:完整儲位 1: 前3碼 2:前4碼 3:前5碼) 預設為0
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string MASK_CODE { get; set; }

	  /// <summary>
	  /// 輸入方式 (0: 手動 1: 匯入) 預設為 0
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IMPORT_WAY { get; set; }

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
  }
}
        