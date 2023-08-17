namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 包裝完成訊息檔
  /// </summary>
  [Serializable]
  [DataServiceKey("SOURCE_TYPE","MSG_TYPE")]
  [Table("F05500102")]
  public class F05500102 : IAuditInfo
  {

	  /// <summary>
	  /// 來源單據類型(F000902)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "char(2)")]
    public string SOURCE_TYPE { get; set; }

	  /// <summary>
	  /// 訊息用途(0預設 1稽核 2裝車 3自取 4超取 9裝車稽核)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "char(1)")]
    public string MSG_TYPE { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string MESSAGE { get; set; }

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
  }
}
        