namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訊息池
  /// </summary>
  [Serializable]
  [DataServiceKey("MESSAGE_ID")]
  [Table("F0080")]
  public class F0080 : IAuditInfo
  {

    /// <summary>
    /// 訊息池ID
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal MESSAGE_ID { get; set; }

    /// <summary>
    /// 通知內容：訊息本文
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string MEAAGE_CONTENT { get; set; }

    /// <summary>
    /// 通知時間：訊息實際發送時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? SEND_TIME { get; set; }

    /// <summary>
    /// 預計回覆時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? EXPECT_REPLY_TIME { get; set; }

    /// <summary>
    /// 實際回覆時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? REPLY_TIME { get; set; }

    /// <summary>
    /// 狀態： null:不要求確認,00:要求確認,01:已發送,02:7天已發送,03:14天已發送,44:拒絕,66:同意,99:已處理
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 訊息編號(F0020)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string MSG_NO { get; set; }

    /// <summary>
    /// 發送對象(0物流中心1貨主2配送商)
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string TARGET_TYPE { get; set; }

    /// <summary>
    /// 發送對象:欄位值(物流中心:作業類別(F000905) 貨主:貨主編號 配送商:配送商編號)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string TARGET_CODE { get; set; }

    /// <summary>
    /// 物流中心代碼
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

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
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔人員名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
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
    [Column(TypeName = "varchar(40)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人員名稱
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
