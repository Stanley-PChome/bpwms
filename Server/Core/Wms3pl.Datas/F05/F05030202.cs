namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配庫後訂單明細與出貨明細對照表
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F05030202")]
  public class F05030202 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal ID { get; set; }

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
	  [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

	  /// <summary>
	  /// 訂單序號
	  /// </summary>
    [Required]
   
    [Column(TypeName = "varchar(6)")]
    public string ORD_SEQ { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 出貨序號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string WMS_ORD_SEQ { get; set; }

	  /// <summary>
	  /// 預計出貨數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 B_DELV_QTY { get; set; }

    /// <summary>
    /// 實際出貨數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? A_DELV_QTY { get; set; }

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
        