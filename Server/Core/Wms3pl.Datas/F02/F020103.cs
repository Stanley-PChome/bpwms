namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進場管理主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ARRIVE_DATE","PURCHASE_NO","DC_CODE","GUP_CODE","CUST_CODE","SERIAL_NO")]
  [Table("F020103")]
  public class F020103 : IAuditInfo
  {

	  /// <summary>
	  /// 進場日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ARRIVE_DATE { get; set; }

    /// <summary>
    /// 進場時段(0:上午1:下午2:夜間 9:不分)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ARRIVE_TIME { get; set; }

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PURCHASE_NO { get; set; }

	  /// <summary>
	  /// 碼頭編號
	  /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string PIER_CODE { get; set; }

	  /// <summary>
	  /// 廠商編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 車號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string CAR_NUMBER { get; set; }

    /// <summary>
    /// 預計進場時間
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string BOOK_INTIME { get; set; }

    /// <summary>
    /// 進場時間
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string INTIME { get; set; }

    /// <summary>
    /// 離場時間
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string OUTTIME { get; set; }

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
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
    /// 總品項數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? ITEM_QTY { get; set; }

    /// <summary>
    /// 總件數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? ORDER_QTY { get; set; }

    /// <summary>
    /// 總體積(立方公分)
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? ORDER_VOLUME { get; set; }

	  /// <summary>
	  /// 進場管理序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 SERIAL_NO { get; set; }

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
        