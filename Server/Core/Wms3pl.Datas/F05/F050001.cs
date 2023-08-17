namespace Wms3pl.Datas.F05
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訂單池主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_NO", "GUP_CODE", "CUST_CODE")]
  [Table("F050001")]
  //[IgnoreProperties("EncryptedProperties")]
  public class F050001 : IEncryptable, IAuditInfo
  {

    /// <summary>
    /// 訂單編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName ="varchar(50)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 訂單類型(0:B2B 1:B2C)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]

    public string ORD_TYPE { get; set; }

    /// <summary>
    /// 門市編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RETAIL_CODE { get; set; }

    /// <summary>
    /// 訂貨日
    /// </summary>
    [Required]   
    [Column(TypeName = "datetime2(0)")]
    public DateTime ORD_DATE { get; set; }

    /// <summary>
    /// 處理狀態(0:待處理 1:配庫中)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_FLAG { get; set; }

    /// <summary>
    /// 客戶名稱(B2C用)
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(200)")]
    public string CUST_NAME { get; set; }

    /// <summary>
    /// 自取
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string SELF_TAKE { get; set; }

    /// <summary>
    /// 易碎標籤
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string FRAGILE_LABEL { get; set; }

    /// <summary>
    /// 保證書
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string GUARANTEE { get; set; }

    /// <summary>
    /// 門號申請書
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SA { get; set; }

    /// <summary>
    /// 性別(0:不明1:男2:女)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string GENDER { get; set; }

    /// <summary>
    /// 年紀
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? AGE { get; set; }

    /// <summary>
    /// SA份數
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? SA_QTY { get; set; }

    /// <summary>
    /// 市話
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    [Column(TypeName = "varchar(44)")]
    public string TEL { get; set; }

    /// <summary>
    /// 收件人地址
    /// </summary>
    [Encrypted]
    [SecretPersonalData("ADDR")]
    [Column(TypeName = "nvarchar(280)")]
    public string ADDRESS { get; set; }

    /// <summary>
    /// 配庫順序
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal ORDER_BY { get; set; }

    /// <summary>
    /// 收件人
    /// </summary>
    [Encrypted]
    [SecretPersonalData("NAME")]
    [Column(TypeName = "nvarchar(88)")]
    public string CONSIGNEE { get; set; }

    /// <summary>
    /// 指定到貨日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? ARRIVAL_DATE { get; set; }

    /// <summary>
    /// 異動類型F000903
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string TRAN_CODE { get; set; }

    /// <summary>
    /// 特殊出貨F000904
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string SP_DELV { get; set; }

    /// <summary>
    /// 貨主成本中心
    /// </summary>
    [Column(TypeName = "nvarchar(10)")]
    public string CUST_COST { get; set; }

    /// <summary>
    /// 批次號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BATCH_NO { get; set; }

    /// <summary>
    /// 通路類型(預設為'00',不指定)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CHANNEL { get; set; }

    /// <summary>
    /// POSM包裝量更新
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string POSM { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    [Required]
    [Encrypted]
    [SecretPersonalData("NAME")]
    [Column(TypeName = "nvarchar(88)")]
    public string CONTACT { get; set; }

    /// <summary>
    /// 連絡電話
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    [Column(TypeName = "varchar(44)")]
    public string CONTACT_TEL { get; set; }

    /// <summary>
    /// 收件人連絡電話2
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    [Column(TypeName = "varchar(44)")]
    public string TEL_2 { get; set; }

    /// <summary>
    /// 是否指定專車
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPECIAL_BUS { get; set; }

    /// <summary>
    /// 配送商編號(F1947)
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string ALL_ID { get; set; }

    /// <summary>
    /// 是否代收
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string COLLECT { get; set; }

    /// <summary>
    /// 代收金額
    /// </summary>
    [Column(TypeName = "decimal(11, 2)")]
     public decimal? COLLECT_AMT { get; set; }

    /// <summary>
    /// 注意事項
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MEMO { get; set; }

    /// <summary>
    /// SAP模組
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string SAP_MODULE { get; set; }

    /// <summary>
    /// 來源單據(F000902)
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string SOURCE_TYPE { get; set; }

    /// <summary>
    /// 來源單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SOURCE_NO { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

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
    /// 出貨倉別(F198001)
    /// </summary>
    [Column(TypeName = "varchar(8)")]
    public string TYPE_ID { get; set; }

    /// <summary>
    /// 快速到貨
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CAN_FAST { get; set; }

    /// <summary>
    /// 收件人連絡電話1
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    [Column(TypeName = "varchar(44)")]
    public string TEL_1 { get; set; }

    /// <summary>
    /// 市話區碼
    /// </summary>
    [Encrypted]
    [Column(TypeName = "varchar(24)")]
    public string TEL_AREA { get; set; }

    /// <summary>
    /// 是否列印發票/收據(0否1發票2收據)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PRINT_RECEIPT { get; set; }

    /// <summary>
    /// 指定發票號碼
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RECEIPT_NO { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string ZIP_CODE { get; set; }

    /// <summary>
    /// 單據類別主檔Id
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal TICKET_ID { get; set; }

    /// <summary>
    /// 解析後地址
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string ADDRESS_PARSE { get; set; }

    /// <summary>
    /// 客戶發票統一編號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string UNIFORM { get; set; }

    /// <summary>
    /// 客戶採購單號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SHOP_NO { get; set; }

    /// <summary>
    /// 發票稅別(0免稅1應稅2零稅率)
    /// </summary>
    
    [Column(TypeName = "varchar(1)")]
    public string INVO_TAX_TYPE { get; set; }

    /// <summary>
    /// 銷售總額(未稅)
    /// </summary>
    [Column(TypeName = "decimal(13, 2)")]
    public decimal? SALES_PRICE { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    [Column(TypeName = "decimal(13, 2)")]
    public decimal? TAX { get; set; }

    /// <summary>
    /// 總計金額(應稅)
    /// </summary>
    [Column(TypeName = "decimal(13, 2)")]
    public decimal? TOTAL_AMT { get; set; }

    /// <summary>
    /// 是否已列印發票(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string INVO_PRINTED { get; set; }

    /// <summary>
    /// 發票買受人抬頭
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string UNI_TITLE { get; set; }

    /// <summary>
    /// 發票地址
    /// </summary>
    [Column(TypeName = "nvarchar(280)")]
    public string BILL_TO { get; set; }

    /// <summary>
    /// 1有貨有發票2有貨無發票3無貨有發票
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string HAVE_ITEM_INVO { get; set; }

    /// <summary>
    /// 特別處理標記 (0: 無、1: Apple廠商的商品)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string NP_FLAG { get; set; }

    /// <summary>
    /// 擴增預留欄位A
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_A { get; set; }

    /// <summary>
    /// 擴增預留欄位B
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_B { get; set; }

    /// <summary>
    /// 擴增預留欄位C
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_C { get; set; }

    /// <summary>
    /// 擴增預留欄位D
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_D { get; set; }

    /// <summary>
    /// 擴增預留欄位E
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_E { get; set; }

    /// <summary>
    /// SA檢核表數量
    /// </summary>
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 SA_CHECK_QTY { get; set; }

    /// <summary>
    /// 方便到貨時段
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string DELV_PERIOD { get; set; }

    /// <summary>
    /// 是否超取
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CVS_TAKE { get; set; }

    /// <summary>
    /// 子通路編號(預設為'00',不指定)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SUBCHANNEL { get; set; }

    /// <summary>
    /// 自取驗證碼
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string CHECK_CODE { get; set; }

    /// <summary>
    /// 對外單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string FOREIGN_WMSNO { get; set; }

    /// <summary>
    /// 對外客戶編號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string FOREIGN_CUSTCODE { get; set; }

    /// <summary>
    /// 來回件(0:否;1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ROUND_PIECE { get; set; }

    /// <summary>
    /// 建議紙箱編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SUG_BOX_NO { get; set; }

    /// <summary>
    /// 跨庫調撥的目的地名稱
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string MOVE_OUT_TARGET { get; set; }

    /// <summary>
    /// 優先處理旗標 (1:一般, 2:優先, 3:急件)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FAST_DEAL_TYPE { get; set; }

    /// <summary>
    /// 建議出貨包裝線類型(空白=不指定 PA1=小線 PA2=大線)
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string PACKING_TYPE { get; set; }

    /// <summary>
    /// 是否出貨稽核(0=否 1=是)
    /// </summary>
    [Column(TypeName = "int")]
    public int? ISPACKCHECK { get; set; }
    //[NotMapped]
    //public Dictionary<string, string> EncryptedProperties
    //{
    //    get
    //    {
    //        return new Dictionary<string, string> { { "TEL", "TEL" }, { "ADDRESS", "ADDR" }, { "CONSIGNEE", "NAME" }, { "CONTACT", "NAME" }, { "CONTACT_TEL", "TEL" }, { "TEL_AREA", "NOT" }, { "TEL_1", "TEL" }, { "TEL_2", "TEL" } };
    //    }
    //}

    /// <summary>
    /// 商品處理類別(0:一般 1:含安裝型商品)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ORDER_PROC_TYPE { get; set; }

    /// <summary>
    /// 收貨郵遞區號
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string ORDER_ZIP_CODE { get; set; }

    /// <summary>
    /// 是否北北基訂單(0:否 1:是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IS_NORTH_ORDER { get; set; }

    /// <summary>
    /// 配庫批次號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ALLOT_BATCH_NO { get; set; }

    /// <summary>
    /// 建議物流商編號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string SUG_LOGISTIC_CODE { get; set; }

  }
}
