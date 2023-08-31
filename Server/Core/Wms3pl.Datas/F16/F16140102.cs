namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨檢驗刷驗紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_ID")]
  [Table("F16140102")]
  public class F16140102 : IAuditInfo
  {

	  /// <summary>
	  /// 紀錄序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
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
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 退貨單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RETURN_NO { get; set; }

    /// <summary>
    /// 商品品號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

    /// <summary>
    /// 檢驗數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? AUDIT_QTY { get; set; }

	  /// <summary>
	  /// 檢驗人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string AUDIT_STAFF { get; set; }

	  /// <summary>
	  /// 檢驗人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string AUDIT_NAME { get; set; }

	  /// <summary>
	  /// 是否通過
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPASS { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MESSAGE { get; set; }

    /// <summary>
    /// 電腦名稱
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CLIENT_PC { get; set; }

    /// <summary>
    /// 錄影機台
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string VIDEO_NO { get; set; }

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
	  /// 建立日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
  }
}
        