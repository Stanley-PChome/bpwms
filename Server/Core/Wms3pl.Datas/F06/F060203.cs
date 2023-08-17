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
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

    /// <summary>
    /// 任務單號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string DOC_ID { get; set; }

    /// <summary>
    /// WMS單據項次
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int ROWNUM { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string SKUCODE { get; set; }

    /// <summary>
    /// 預計出庫數量=預計數量
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int SKUPLANQTY { get; set; }

    /// <summary>
    /// 實際出庫數量=播種完成數量
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int SKUQTY { get; set; }

    /// <summary>
    /// 商品等級(0=殘品/客退品, 1=正品/新品)
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int SKULEVEL { get; set; }

    /// <summary>
    /// 商品效期(yyyy/mm/dd)
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string EXPIRYDATE { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string OUTBATCHCODE { get; set; }

    /// <summary>
    /// 商品序號清單 (不再使用)
    /// </summary>
    [Column(TypeName = "varchar(MAX)")]
    public string SERIALNUMLIST { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

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
