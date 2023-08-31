namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 生產線設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PRODUCE_NO","DC_CODE")]
  [Table("F910004")]
  public class F910004 : IAuditInfo
  {

	  /// <summary>
	  /// 生產線編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string PRODUCE_NO { get; set; }

	  /// <summary>
	  /// 生產線名稱
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string PRODUCE_NAME { get; set; }

    /// <summary>
    /// 生產線說明
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string PRODUCE_DESC { get; set; }

	  /// <summary>
	  /// 生產線IP
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string PRODUCE_IP { get; set; }

	  /// <summary>
	  /// 生產線狀態(0:使用中 9刪除)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

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
	  /// 建立日期
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
    /// 異動日期
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
        