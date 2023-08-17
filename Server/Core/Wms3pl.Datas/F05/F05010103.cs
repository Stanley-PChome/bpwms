namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 修改超取訂單配送門市Log
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_ID")]
  [Table("F05010103")]
  public class F05010103 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 LOG_ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 異動類型F000904(TOPIC=F05010103 SUBPTOPIC=TYPE)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string TYPE { get; set; }

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 配送門市編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string DELV_RETAILCODE { get; set; }

    /// <summary>
    /// 配送門市名稱
    /// </summary>
    [Column(TypeName = "varchar(150)")]
    public string DELV_RETAILNAME { get; set; }

    /// <summary>
    /// 託運單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CONSIGN_NO { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
        