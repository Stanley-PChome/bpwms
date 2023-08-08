namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 複驗異常處理-序號記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F02050402")]
  public class F02050402 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    public long ID { get; set; }
    /// <summary>
    /// F020504.ID
    /// </summary>
    [Required]
    public long F020504_ID { get; set; }
    /// <summary>
    /// F020501.ID
    /// </summary>
    [Required]
    public long F020501_ID { get; set; }
    /// <summary>
    /// F020502.ID
    /// </summary>
    [Required]
    public long F020502_ID{ get; set; }
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
    /// 進倉單號
    /// </summary>
    [Required]
    public string STOCK_NO { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    [Required]
    public string RT_NO { get; set; }
    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    public string ITEM_CODE { get; set; }
    /// <summary>
    /// 商品序號
    /// </summary>
    [Required]
    public string SERIAL_NO { get; set; }
    /// <summary>
    /// 處理方式 1:移除數量 2:轉不良品
    /// </summary>
    [Required]
    public string PROC_TYPE { get; set; }
    /// <summary>
    /// 狀態 0: 待處理 1:處理中 2:完成  F:處理失敗 T: TimeOut 9:取消 如果PROC_TYPE=2(轉不良品)，STATUS直接為2
    /// </summary>
    [Required]
    public string STATUS { get; set; }
    /// <summary>
    /// 傳送時間
    /// </summary>
    public DateTime? PROC_DATE { get; set; }
    /// <summary>
    /// 訊息
    /// </summary>
    public string MESSAGE { get; set; }
    /// <summary>
    /// 已派送次數
    /// </summary>
    public int RESENT_CNT { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }
    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    public String CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    public String CRT_NAME { get; set; }
    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// 異動人員
    /// </summary>
    public String UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人名
    /// </summary>
    public String UPD_NAME { get; set; }

  }
}
