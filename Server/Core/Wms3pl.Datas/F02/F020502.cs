namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 驗收容器上架明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F020502")]
  public class F020502 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }

    /// <summary>
    /// F020501.ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 F020501_ID { get; set; }

    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 進倉單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string STOCK_NO { get; set; }

    /// <summary>
    /// 進倉項次
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string STOCK_SEQ { get; set; }

    /// <summary>
    /// 驗收單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RT_NO { get; set; }

    /// <summary>
    /// 驗收序號
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string RT_SEQ { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 實際分播數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 QTY { get; set; }

    /// <summary>
    /// 容器條碼
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }

    /// <summary>
    /// 容器分格條碼
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public string BIN_CODE { get; set; }

    /// <summary>
    /// 狀態(0: 待複驗、1: 不須複驗、2: 複驗完成、3:複驗失敗、4: 上架完成)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 複驗原因代碼
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string RECHECK_CAUSE { get; set; }

    /// <summary>
    /// 複驗備註
    /// </summary>
    [Column(TypeName = "nvarchar(255)")]
    public string RECHECK_MEMO { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人名
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
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 驗收註記
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string RCV_MEMO { get; set; }
  }
}
