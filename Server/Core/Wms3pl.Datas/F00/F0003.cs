namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 系統設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("AP_NAME","CUST_CODE","GUP_CODE","DC_CODE")]
  [Table("F0003")]
  public class F0003 : IAuditInfo
  {

	  /// <summary>
	  /// 設定名稱
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string AP_NAME { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 設定值
	  /// </summary>
    [Column(TypeName = "varchar(120)")]
    public string SYS_PATH { get; set; }

	  /// <summary>
	  /// 設定檔名
	  /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string FILENAME { get; set; }

	  /// <summary>
	  /// 設定類型
	  /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string FILETYPE { get; set; }

	  /// <summary>
	  /// 設定描述
	  /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string DESCRIPT { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
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
	  /// 異動時間
	  /// </summary>
    [Column(TypeName = "datetime2(0)")]
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
    [Column(TypeName = "nvarchar(16)")]
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
	  public string CRT_NAME { get; set; }
  }
}
        