namespace Wms3pl.Datas.F19
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨優先權與自動化設備優先權對應檔 
  /// </summary>
  [Serializable]
  [DataServiceKey("PRIORITY_CODE", "DC_CODE", "DEVICE_TYPE")]
  [Table("F195601")]
  public class F195601 : IAuditInfo
  {
    /// <summary>
    /// 出貨優先權代碼
    /// </summary>
    [Key]
    [Required]
    public string PRIORITY_CODE { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Key]
    [Required]
    public string DC_CODE { get; set; }
    /// <summary>
    /// 自動化設備類型(1:AGV, 2:SHUTTLE,3:板進箱出倉)
    /// </summary>
    [Key]
    [Required]
    public string DEVICE_TYPE { get; set; }
    /// <summary>
    /// 自動化設備優先權代碼
    /// </summary>
    [Required]
    public int PRIORITY_VALUE { get; set; }
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
