namespace Wms3pl.Datas.F20
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調整單(訂單調整)刪除派車單明細備份
  /// </summary>
  [Serializable]
  [DataServiceKey("DISTR_CAR_NO", "DISTR_CAR_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
  [Table("F20010201")]
  public class F20010201 : IAuditInfo
  {

    /// <summary>
    /// 派車單號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string DISTR_CAR_NO { get; set; }

    /// <summary>
    /// 派車單序號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 DISTR_CAR_SEQ { get; set; }

    /// <summary>
    /// 客戶代號F1910
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RETAIL_CODE { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(200)")]
    public string CUST_NAME { get; set; }

    /// <summary>
    /// 委託單位
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string ENTRUST_DEPT { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(200)")]
    public string ADDRESS { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string CONTACT { get; set; }

    /// <summary>
    /// 連絡電話
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CONTACT_TEL { get; set; }

    /// <summary>
    /// 取件時間(F194701)
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string TAKE_TIME { get; set; }

    /// <summary>
    /// 商品數量
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ITEM_QTY { get; set; }

    /// <summary>
    /// 派車用途(02取件(逆物流),01送件(正物流))F000904
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string DISTR_USE { get; set; }

    /// <summary>
    /// 物流單據類別F000901
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string ORD_TYPE { get; set; }

    /// <summary>
    /// 物流單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string MEMO { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 裝車車號A
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string CAR_NO_A { get; set; }

    /// <summary>
    /// 裝車車號B
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string CAR_NO_B { get; set; }

    /// <summary>
    /// 裝車車號C
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string CAR_NO_C { get; set; }

    /// <summary>
    /// 是否已上傳封條(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISSEAL { get; set; }

    /// <summary>
    /// 配送效率F190102
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string DELV_EFFIC { get; set; }

    /// <summary>
    /// 是否快速到貨(0非當配1當配)F000904 Where Topic=F194701
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(1)")]
    public string CAN_FAST { get; set; }

    /// <summary>
    /// 配送溫層(A常溫 B低溫)F000904 Where Topic=F194701
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string DELV_TMPR { get; set; }

    /// <summary>
    /// 郵遞區號F1934
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string ZIP_CODE { get; set; }

    /// <summary>
    /// 材積
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public decimal? VOLUMN { get; set; }
  }
}
