namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 碼頭期間設定
  /// </summary>
  [Serializable]
  [DataServiceKey("BEGIN_DATE","END_DATE","PIER_CODE","DC_CODE")]
  [Table("F020104")]
  public class F020104 : IAuditInfo
  {

	  /// <summary>
	  /// 設定起始日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime BEGIN_DATE { get; set; }

	  /// <summary>
	  /// 設定結束日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime END_DATE { get; set; }

	  /// <summary>
	  /// 碼頭編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string PIER_CODE { get; set; }

	  /// <summary>
	  /// 暫存區數
	  /// </summary>
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 TEMP_AREA { get; set; }

	  /// <summary>
	  /// 是否允許進場(0:否 1:是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_IN { get; set; }

	  /// <summary>
	  /// 是否允許出場(0:否 1:是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_OUT { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

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
        