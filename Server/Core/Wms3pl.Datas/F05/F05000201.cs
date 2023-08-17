namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訂單關聯WelcomeLetter檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CUST_ORD_NO","SERIAL_NO","GUP_CODE","CUST_CODE")]
  [Table("F05000201")]
  public class F05000201 : IAuditInfo
  {

	  /// <summary>
	  /// 貨主單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string CUST_ORD_NO { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string SERIAL_NO { get; set; }

    /// <summary>
    /// 門號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CELL_NUM { get; set; }

    /// <summary>
    /// 生效日期
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string EFFECT_DATE { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Column(TypeName = "varchar(100)")]
    public string CUST_NAME { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

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
        