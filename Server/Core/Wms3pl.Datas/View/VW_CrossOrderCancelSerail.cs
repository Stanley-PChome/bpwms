namespace Wms3pl.Datas.View
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;

  /// <summary>
  /// 跨庫訂單使用序號
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_ORD_NO", "PICK_ORD_NO")]
  [Table("VW_CrossOrderCancelSerail")]
  public class VW_CrossOrderCancelSerail
  {
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
    /// 出貨單號
    /// </summary>
    [Required]
    public string WMS_ORD_NO { get; set; }
    /// <summary>
    /// 揀貨單號
    /// </summary>
    [Required]
    public string PICK_ORD_NO { get; set; }
    /// <summary>
    /// 序號
    /// </summary>
    public string SERIAL_NO { get; set; }
    /// <summary>
    /// 是否為自動倉 0:人工 1:自動
    /// </summary>
    public string DISP_SYSTEM { get; set; }
  }
}
