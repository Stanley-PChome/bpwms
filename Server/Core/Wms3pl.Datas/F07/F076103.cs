using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
  /// <summary>
  /// 貨主訂單單號取消訂單鎖定
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE", "CUST_ORD_NO")]
  [Table("F076103")]
  public class F076103 : IAuditInfo
  {
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 貨主單據編號
    /// </summary>
    [Required]
    public string CUST_ORD_NO { get; set; }

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
  }
}
