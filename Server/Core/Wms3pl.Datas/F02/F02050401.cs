namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 複驗異常處理歷程記錄
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F02050401")]

  public class F02050401 : IAuditInfo
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
    public Int64 F020504_ID { get; set; }
    /// <summary>
    /// 項目說明
    /// </summary>
    [Required]
    public String PROC_DESC { get; set; }
    /// <summary>
    /// 複驗原因代碼
    /// </summary>
    public String RECHECK_CAUSE { get; set; }
    /// <summary>
    /// 備註
    /// </summary>
    public String MEMO { get; set; }
    /// <summary>
    /// 處理人員
    /// </summary>
    [Required]
    public String PROC_STAFF { get; set; }
    /// <summary>
    /// 處理人名
    /// </summary>
    [Required]
    public String PROC_NAME { get; set; }
    /// <summary>
    /// 處理時間
    /// </summary>
    [Required]
    public DateTime PROC_TIME { get; set; }
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
