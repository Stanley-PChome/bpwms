namespace Wms3pl.Datas.F06
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 入庫任務清單
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F060209")]
  public class F060209 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    public long ID { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Required]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 倉庫代碼
    /// </summary>
    [Required]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 原任務單號
    /// </summary>
    [Required]
    public string DOC_ID { get; set; }

    /// <summary>
    /// WMS單號
    /// </summary>
    [Required]
    public string WMS_NO { get; set; }

    /// <summary>
    /// WMS揀貨單號
    /// </summary>
    [Required]
    public string PICK_NO { get; set; }

    /// <summary>
    /// 登錄人員=作業人員
    /// </summary>
    [Required]
    public string OPERATOR { get; set; }

    /// <summary>
    /// 揀貨開始時間(yyyy/MM/dd HH:mm:ss)
    /// </summary>
    [Required]
    public string START_TIME { get; set; }

    /// <summary>
    /// 揀貨完成時間(yyyy/MM/dd HH:mm:ss)
    /// </summary>
    [Required]
    public string COMPLETE_TIME { get; set; }

    /// <summary>
    /// 作業狀態 (0: 待處理、1: 已取宅配單、2: 已完成包裝、3: 取消宅單失敗、8: 異常、9: 作業取消)
    /// </summary>
    [Required]
    public int PROC_FLAG { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string MSG_CONTENT { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    public string UPD_NAME { get; set; }
  }
}
