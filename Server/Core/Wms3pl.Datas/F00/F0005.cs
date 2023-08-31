namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心系統設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("SET_NAME","DC_CODE")]
  [Table("F0005")]
  public class F0005 : IAuditInfo
  {

	  /// <summary>
	  /// 設定名稱
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string SET_NAME { get; set; }

	  /// <summary>
	  /// 設定值
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(120)")]
    public string SET_VALUE { get; set; }

	  /// <summary>
	  /// 設定描述
	  /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string DESCRIPT { get; set; }

	  /// <summary>
	  /// 物流中心編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
	  public string DC_CODE { get; set; }

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
	  /// 建立時間
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
	  /// 異動時間
	  /// </summary>
    [Column(TypeName = "datetime2(0)")]
	  public DateTime? UPD_DATE { get; set; }
  }
}
        