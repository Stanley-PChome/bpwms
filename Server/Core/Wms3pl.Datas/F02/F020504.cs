namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  ///  複驗異常處理
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F020504")]

  public class F020504 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    public Int64 ID { get; set; }
    /// <summary>
    /// 處理日期
    /// </summary>
    [Required]
    public DateTime PROC_DATE { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    public String DC_CODE { get; set; }
    /// <summary>
    /// 業主編號
    /// </summary>
    [Required]
    public String GUP_CODE { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    public String CUST_CODE { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public String STOCK_NO { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public String STOCK_SEQ { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public String RT_NO { get; set; }
    /// <summary>
    /// 驗收序號
    /// </summary>
    public String RT_SEQ { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    [Required]
    public String ITEM_CODE { get; set; }
    /// <summary>
    /// 原驗收數
    /// </summary>
    [Required]
    public int QTY { get; set; }
    /// <summary>
    /// 容器條碼
    /// </summary>
    [Required]
    public String CONTAINER_CODE { get; set; }
    /// <summary>
    /// 容器分格條碼
    /// </summary>
    public String BIN_CODE { get; set; }
    /// <summary>
    /// 處理方式
    /// </summary>
    [Required]
    public String PROC_CODE { get; set; }
    /// <summary>
    /// 備註
    /// </summary>
    public String MEMO { get; set; }
    /// <summary>
    /// 拒絕驗收數
    /// </summary>
    public int? REMOVE_RECV_QTY { get; set; }
    /// <summary>
    /// 不良品數
    /// </summary>
    public int? NOTGOOD_QTY { get; set; }
    /// <summary>
    /// 處理狀態
    /// </summary>
    [Required]
    public String STATUS { get; set; }
    /// <summary>
    /// F020502.ID
    /// </summary>
    [Required]
    public Int64 F020502_ID { get; set; }
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
