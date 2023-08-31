using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  /// <summary>
  /// 程式功能與LOG對應表
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F0006")]
  public class F0006 : IAuditInfo
  {
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string DC_CODE { get; set; }
    /// <summary>
    /// 介接系統編號 (0: LMS系統、1: WCS系統、2: WCSPR系統、3: PDA系統)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string EXTERNAL_SYSTEM { get; set; }
    /// <summary>
    /// 顯示名稱(非程式名稱，方便人員看得懂的名稱)
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(40)")]
    public string SHOW_NAME { get; set; }
    /// <summary>
    /// 排程名稱
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string PROG_NAME { get; set; }
    /// <summary>
    /// 排程資料表
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PROG_LOG_TABLE { get; set; }
    /// <summary>
    /// API名稱
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string API_NAME { get; set; }
    /// <summary>
    /// API資料表
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string API_LOG_TABLE { get; set; }
    /// <summary>
    /// 功能說明
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string MEMO { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }
    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人員名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// 異動人員編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
    /// <summary>
    /// 篩選規則
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string FILTER_RULE { get; set; }
  }
}
