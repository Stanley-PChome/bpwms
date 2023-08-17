namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// WebApi訂單池
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F05000101")]
  public class F05000101 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }

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
	  /// 通路
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CHANNEL { get; set; }

	  /// <summary>
	  /// 對外單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string FOREIGN_ORDNO { get; set; }

    /// <summary>
    /// 訂單資料
    /// </summary>
    [Column(TypeName = "nvarchar(MAX)")]  
    public string ORDDATA { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [Column(TypeName = "nvarchar(MAX)")]
    public string ERRMSG { get; set; }

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
        