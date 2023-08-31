namespace Wms3pl.Datas.F20
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單(訂單調整)刪除託運單備份
  /// </summary>
  [Serializable]
  [DataServiceKey("CONSIGN_ID")]
  [Table("F20010202")]
  public class F20010202 : IAuditInfo
  {

    /// <summary>
    /// 託運單 ID
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal CONSIGN_ID { get; set; }

    /// <summary>
    /// 託運單號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(50)")]
    public string CONSIGN_NO { get; set; }

    /// <summary>
    /// 物流單號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 路線代碼
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string ROUTE_CODE { get; set; }

    /// <summary>
    /// 業主代碼
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主代碼
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 物流中心代碼
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔人員名稱
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "varchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 新竹貨運到著站編號
    /// </summary>
[Column(TypeName = "varchar(10)")]
    public string ERST_NO { get; set; }

    /// <summary>
    /// 路線(ex.當日配北區or2A)
    /// </summary>
[Column(TypeName = "nvarchar(30)")]
    public string ROUTE { get; set; }

    /// <summary>
    /// 配次
    /// </summary>
[Column(TypeName = "varchar(10)")]
    public string DELV_TIMES { get; set; }

    /// <summary>
    /// 配送商回覆的配達時間
    /// </summary>
[Column(TypeName = "datetime2(0)")]
    public DateTime? PAST_DATE { get; set; }

    /// <summary>
    /// 配送狀態(0未配送,2配送中,3已配達)
    /// </summary>
    [Required]
[Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// To貨主配送回檔狀態(0未回檔1已回檔)
    /// </summary>
    [Required]
[Column(TypeName = "char(1)")]
    public string CUST_EDI_STATUS { get; set; }

    /// <summary>
    /// To配送商回檔狀態(0未回檔1已回檔)
    /// </summary>
    [Required]
[Column(TypeName = "char(1)")]
    public string DISTR_EDI_STATUS { get; set; }

    /// <summary>
    /// 配送回覆結果
    /// </summary>
[Column(TypeName = "nvarchar(500)")]
    public string RESULT { get; set; }

    /// <summary>
    /// To貨主配送回檔次數
    /// </summary>
    [Required]
[Column(TypeName = "int")]
    public Int32 CUST_EDI_QTY { get; set; }

    /// <summary>
    /// 配送商回覆的發送(商品)日期
    /// </summary>
[Column(TypeName = "datetime2(0)")]
    public DateTime? SEND_DATE { get; set; }
  }
}
