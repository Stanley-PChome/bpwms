namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 報價單動作明細
  /// </summary>
  [Serializable]
  [DataServiceKey("QUOTE_NO","PROCESS_ID","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F910402")]
  public class F910402 : IAuditInfo
  {

	  /// <summary>
	  /// 報價單項目編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string QUOTE_NO { get; set; }

	  /// <summary>
	  /// 加工動作 F910001
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string PROCESS_ID { get; set; }

	  /// <summary>
	  /// 計量單位 F91000302 Where ItemTypeId=001
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string UNIT_ID { get; set; }

	  /// <summary>
	  /// 標準工時
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 WORK_HOUR { get; set; }

	  /// <summary>
	  /// 動作金額
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(11, 2)")]
    public decimal WORK_COST { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

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

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
  }
}
        