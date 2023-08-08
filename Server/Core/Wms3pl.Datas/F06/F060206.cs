namespace Wms3pl.Datas.F06
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出庫容器明細資料檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F060206")]
  public class F060206 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    public long ID { get; set; }

    /// <summary>
    /// 任務單號
    /// </summary>
    [Required]
    public string DOC_ID { get; set; }

    /// <summary>
    /// 容器編號
    /// </summary>
    public string CONTAINERCODE { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    public string SKUCODE { get; set; }

    /// <summary>
    /// 裝箱數量
    /// </summary>
    [Required]
    public int SKUQTY { get; set; }

    /// <summary>
    /// 商品序號清單 (紀錄本箱中的序號)
    /// </summary>
    public string SERIALNUMLIST { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }

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

    /// <summary>
    /// 容器分隔編號
    /// </summary>
    public string BIN_CODE { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    public string SERIAL_NO { get; set; }


  }
}
