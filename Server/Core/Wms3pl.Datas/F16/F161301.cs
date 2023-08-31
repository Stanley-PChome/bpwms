namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨點收頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_CHECK_NO","DC_CODE")]
  [Table("F161301")]
  public class F161301 : IAuditInfo
  {

	  /// <summary>
	  /// 退貨點收編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RTN_CHECK_NO { get; set; }

	  /// <summary>
	  /// 收件人
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CONSIGNEE { get; set; }

	  /// <summary>
	  /// 收貨日期(年月日+時分)
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime RECEIPT_DATE { get; set; }

	  /// <summary>
	  /// 運輸公司
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string TRANSPORT { get; set; }

	  /// <summary>
	  /// 車號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string CAR_NO { get; set; }

	  /// <summary>
	  /// 點收狀態
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
        