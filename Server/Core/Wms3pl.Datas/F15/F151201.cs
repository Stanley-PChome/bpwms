namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總調撥主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_ALLOC_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F151201")]
  public class F151201 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總調撥單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BATCH_ALLOC_NO { get; set; }

	  /// <summary>
	  /// 彙總日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime BATCH_DATE { get; set; }

	  /// <summary>
	  /// 單據狀態(0: 待處理  1: 作業處理中 2: 作業已完成  9:取消)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

	  /// <summary>
	  /// 指定工作站
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string STATION_NO { get; set; }

	  /// <summary>
	  /// 貨架數
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal SHELF_CNT { get; set; }

	  /// <summary>
	  /// 品項數
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal ITEM_CNT { get; set; }

	  /// <summary>
	  /// 總數量
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal TOTAL_QTY { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主(0:共用)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主(0:共用)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
        