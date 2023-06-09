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
    public long ID { get; set; }

    /// <summary>
    /// 原任務單號
    /// </summary>
    [Required]
    public string DOC_ID { get; set; }

    /// <summary>
    /// 箱序
    /// </summary>
    [Required]
    public int BOX_SEQ { get; set; }

    /// <summary>
    /// 庫內品號
    /// </summary>
    [Required]
    public string SKU_CODE { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    [Required]
    public int SKU_QTY { get; set; }

    /// <summary>
    /// 商品序號清單 (紀錄本箱中的序號)
    /// </summary>
    public string SERIAL_NO_LIST { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人名
    /// </summary>
    [Required]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    public string UPD_NAME { get; set; }
  }
}
