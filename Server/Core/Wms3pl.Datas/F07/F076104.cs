using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
  /// <summary>
  /// 集貨場容器進場鎖定
  /// </summary>
  [Serializable]
  [DataServiceKey("CONTAINER_CODE")]
  [Table("F076104")]
  public class F076104 : IAuditInfo
  {
    /// <summary>
    /// 容器條碼
    /// </summary>
    [Required]
    public string CONTAINER_CODE { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人員名稱
    /// </summary>
    [Required]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員編號
    /// </summary>
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人員名稱
    /// </summary>
    public string UPD_NAME { get; set; }
  }
}
