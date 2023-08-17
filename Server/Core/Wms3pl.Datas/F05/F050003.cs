namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨單特殊商品批次維護表
  /// </summary>
  [Serializable]
  [DataServiceKey("SEQ_NO")]
  [Table("F050003")]
  public class F050003 : IAuditInfo
  {

	  /// <summary>
	  /// 序號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "int")]
    public Int32 SEQ_NO { get; set; }

	  /// <summary>
	  /// 單據類型 F190001 Where TicketType=O
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal TICKET_ID { get; set; }

	  /// <summary>
	  /// 出貨日期(起)
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime BEGIN_DELV_DATE { get; set; }

	  /// <summary>
	  /// 出貨日期(迄)
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime END_DELV_DATE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建檔人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 設定狀態(9:刪除)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
  }
}
        