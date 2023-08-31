namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 驗收上傳檔案類型
  /// </summary>
  [Serializable]
  [DataServiceKey("UPLOAD_TYPE")]
  [Table("F02020106")]
  public class F02020106 : IAuditInfo
  {

	  /// <summary>
	  /// 上傳檔案類型
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string UPLOAD_TYPE { get; set; }

	  /// <summary>
	  /// 上傳檔案類型名稱
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string UPLOAD_NAME { get; set; }

    /// <summary>
    /// 是否必須上傳
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ISREQUIRED { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

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
        