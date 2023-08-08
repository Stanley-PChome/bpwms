namespace Wms3pl.Datas.F06
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出庫明細資料檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F060203")]
  public class F060203 : IAuditInfo
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
    /// WMS單據項次
    /// </summary>
    [Required]
    public int ROWNUM { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    public string SKUCODE { get; set; }

    /// <summary>
    /// 預計出庫數量=預計數量
    /// </summary>
    [Required]
    public int SKUPLANQTY { get; set; }

    /// <summary>
    /// 實際出庫數量=播種完成數量
    /// </summary>
    [Required]
    public int SKUQTY { get; set; }

    /// <summary>
    /// 商品等級(0=殘品/客退品, 1=正品/新品)
    /// </summary>
    [Required]
    public int SKULEVEL { get; set; }

    /// <summary>
    /// 商品效期(yyyy/mm/dd)
    /// </summary>
    public string EXPIRYDATE { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    public string OUTBATCHCODE { get; set; }

    /// <summary>
    /// 商品序號清單 (不再使用)
    /// </summary>
    public string SERIALNUMLIST { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    public string SERIAL_NO { get; set; }

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
  }
}
