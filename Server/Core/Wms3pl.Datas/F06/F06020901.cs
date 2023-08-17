namespace Wms3pl.Datas.F06
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  ///  外部出貨包裝紀錄頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F06020901")]
  public class F06020901 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

    /// <summary>
    /// 原任務單號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string DOC_ID { get; set; }

    /// <summary>
    /// 箱序
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int BOX_SEQ { get; set; }

    /// <summary>
    /// 紙箱編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BOX_NO { get; set; }

    /// <summary>
    /// 列印箱明細時間
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(19)")]
    public string PRINT_BOX_TIME { get; set; }

    /// <summary>
    /// 物流商編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string TRANSPORT_PROVIDER { get; set; }

    /// <summary>
    /// 宅配單號
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public string CONSIGN_NO { get; set; }

    /// <summary>
    /// 作業狀態 (0: 待處理、1: 已取宅配單、2: 取宅單失敗、3: 宅單取消成功、4: 宅單取消失敗、9: 取消)
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int PROC_FLAG { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
