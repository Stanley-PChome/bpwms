namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訂單狀況分時統計報表
  /// </summary>
  [Serializable]
  [DataServiceKey("CNT_DATE","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F700702")]
  public class F700702 : IAuditInfo
  {

	  /// <summary>
	  /// 統計日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CNT_DATE { get; set; }

	  /// <summary>
	  /// 0時~1時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT1 { get; set; }

	  /// <summary>
	  /// 1時~2時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT2 { get; set; }

	  /// <summary>
	  /// 2時~3時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT3 { get; set; }

	  /// <summary>
	  /// 3時~4時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT4 { get; set; }

	  /// <summary>
	  /// 4時~5時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT5 { get; set; }

	  /// <summary>
	  /// 5時~6時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT6 { get; set; }

	  /// <summary>
	  /// 6時~7時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT7 { get; set; }

	  /// <summary>
	  /// 7時~8時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT8 { get; set; }

	  /// <summary>
	  /// 8時~9時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT9 { get; set; }

	  /// <summary>
	  /// 9時~10時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT10 { get; set; }

	  /// <summary>
	  /// 10時~11時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT11 { get; set; }

	  /// <summary>
	  /// 11時~12時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT12 { get; set; }

	  /// <summary>
	  /// 12時~13時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT13 { get; set; }

	  /// <summary>
	  /// 13時~14時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT14 { get; set; }

	  /// <summary>
	  /// 14時~15時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT15 { get; set; }

	  /// <summary>
	  /// 15時~16時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT16 { get; set; }

	  /// <summary>
	  /// 16時~17時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT17 { get; set; }

	  /// <summary>
	  /// 17時~18時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT18 { get; set; }

	  /// <summary>
	  /// 18時~19時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT19 { get; set; }

	  /// <summary>
	  /// 19時~20時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT20 { get; set; }

	  /// <summary>
	  /// 20時~21時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT21 { get; set; }

	  /// <summary>
	  /// 21時~22時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT22 { get; set; }

	  /// <summary>
	  /// 22時~23時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT23 { get; set; }

	  /// <summary>
	  /// 23時~24時訂單數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CNT24 { get; set; }

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
	  /// 建立人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立時間
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
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
        