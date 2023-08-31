namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 廠退類型設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_VNR_TYPE_ID")]
  [Table("F160203")]
  public class F160203 : IAuditInfo
  {

	  /// <summary>
	  /// 廠退類型編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string RTN_VNR_TYPE_ID { get; set; }

	  /// <summary>
	  /// 廠退類型名稱
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(80)")]
    public string RTN_VNR_TYPE_NAME { get; set; }

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
        