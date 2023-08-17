namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 郵遞區號與區域對應
  /// </summary>
  [Serializable]
  [DataServiceKey("ZIP_CODE","REGION_CODE","GUP_CODE","CUST_CODE")]
  [Table("F050007")]
  public class F050007 : IAuditInfo
  {

	  /// <summary>
	  /// 郵遞區號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string ZIP_CODE { get; set; }

	  /// <summary>
	  /// 區域代碼，E:東部 S:中南部 N:北部
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "char(1)")]
    public string REGION_CODE { get; set; }

	  /// <summary>
	  /// 業主代號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主代號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人員名稱
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(16)")]
    public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "varchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
  }
}
        