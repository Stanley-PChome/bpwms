namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉驗收檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RT_NO", "RT_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
  [Table("F020201")]
  public class F020201 : IAuditInfo
  {

    /// <summary>
    /// 驗收單號碼
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RT_NO { get; set; }

    /// <summary>
    /// 驗收單序號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string RT_SEQ { get; set; }

    /// <summary>
    /// 進倉單號F010201
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PURCHASE_NO { get; set; }

    /// <summary>
    /// 進倉單序號F010202
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string PURCHASE_SEQ { get; set; }

    /// <summary>
    /// 廠編
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 驗收日
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? RECE_DATE { get; set; }

    /// <summary>
    /// 商品有效日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? VALI_DATE { get; set; }

    /// <summary>
    /// 商品製造日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? MADE_DATE { get; set; }

    /// <summary>
    /// 訂貨數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? ORDER_QTY { get; set; }

    /// <summary>
    /// 驗收數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? RECV_QTY { get; set; }

    /// <summary>
    /// 抽驗數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? CHECK_QTY { get; set; }

    /// <summary>
    /// 檢驗狀態(1待上傳,2已上傳,3綁容器)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 已抽驗商品
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CHECK_ITEM { get; set; }

    /// <summary>
    /// 已抽驗序號
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CHECK_SERIAL { get; set; }

    /// <summary>
    /// 是否列印箱板標
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPRINT { get; set; }

    /// <summary>
    /// 是否上傳圖檔
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISUPLOAD { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(40)")]
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
    /// 特採原因
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string SPECIAL_DESC { get; set; }

    /// <summary>
    /// 特採原因編碼
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string SPECIAL_CODE { get; set; }

    /// <summary>
    /// 是否特採
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISSPECIAL { get; set; }

    /// <summary>
    /// 到貨時間(年月日時間)
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? IN_DATE { get; set; }

    /// <summary>
    /// 指定出貨倉別
    /// </summary>
    [Column(TypeName = "varchar(8)")]
    public string TARWAREHOUSE_ID { get; set; }

    /// <summary>
    /// 快驗
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string QUICK_CHECK { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }

    /// <summary>
    /// 驗收模式(0=單據綁定驗收,1=容器綁定驗收)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string RT_MODE { get; set; }

    /// <summary>
    /// 驗收工具(0: 舊版資料、1: 電腦版、2:PDA)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string DEVICE_MODE { get; set; }
    /// <summary>
    /// 棧板容器編號
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string PALLET_LOCATION { get; set; }
    /// <summary>
    /// 是否列印商品ID標
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IS_PRINT_ITEM_ID { get; set; }
    /// <summary>
    /// 是否列印驗收單
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IS_PRINT_RECVNOTE { get; set; }
    /// <summary>
    /// 列印狀態(0: 不需列印、1: 待列印、2: 已列印)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string PRINT_MODE { get; set; }
    /// <summary>
    /// 列印時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PRINT_TIME { get; set; }
    /// <summary>
    /// 列印人員編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PRINT_STAFF { get; set; }
    /// <summary>
    /// 列印人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string PRINT_NAME { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(25)")]
    public string CUST_ORD_NO { get; set; }
    /// <summary>
    /// 不良品數
    /// </summary>
    [Column(TypeName = "int")]
    public int? DEFECT_QTY { get; set; }
  }
}
