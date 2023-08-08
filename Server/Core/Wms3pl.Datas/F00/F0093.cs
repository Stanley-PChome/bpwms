using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  /// <summary>
  /// LMS系統異常通知
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F0093")]
  public class F0093 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Int64 ID { get; set; }

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
    /// 單號
    /// </summary>
    public string WMS_NO { get; set; }

    /// <summary>
    /// 單據類型(C=驗收單, T=調撥單, O=出貨單, P=揀貨單)
    /// </summary>
    public string WMS_TYPE { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 0: 待處理 1:處理中 2:完成 F:處理失敗 T: TimeOut 9:取消
    /// </summary>
    [Required]
    public string STATUS { get; set; }

    /// <summary>
    /// 訊息代號
    /// </summary>
    [Required]
    public string MSG_NO { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    public string MSG_CONTENT { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 已派送次數
    /// </summary>
    [Required]
    public int RESENT_CNT { get; set; }

  }
}
