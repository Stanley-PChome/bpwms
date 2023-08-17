namespace Wms3pl.Datas.F06
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  ///  外部出貨包裝紀錄明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F06020902")]
  public class F06020902 : IAuditInfo
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
    /// 庫內品號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string SKU_CODE { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int SKU_QTY { get; set; }

    /// <summary>
    /// 商品序號清單 (紀錄本箱中的序號)
    /// </summary>
    [Column(TypeName = "varchar(MAX)")]
    public string SERIAL_NO_LIST { get; set; }

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
