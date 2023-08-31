namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RETURN_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F161201")]
  public class F161201 : IAuditInfo
  {

	  /// <summary>
	  /// 退貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RETURN_NO { get; set; }

	  /// <summary>
	  /// 退貨建立日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime RETURN_DATE { get; set; }

    /// <summary>
    /// 過帳日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? POSTING_DATE { get; set; }

	  /// <summary>
	  /// 單據狀態(0:待處理 1:已處理 2:結案  9:取消)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 出貨單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

    /// <summary>
    /// 退貨客戶代號(B2B時才需輸入,實際為門市編號 F1910)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RTN_CUST_CODE { get; set; }

	  /// <summary>
	  /// 退貨客戶名稱(可自行輸入,或依照輸入RTN_CUST_CODE帶出門市名稱)
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(200)")]
    public string RTN_CUST_NAME { get; set; }

	  /// <summary>
	  /// 退貨類型(F161203)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string RTN_TYPE_ID { get; set; }

	  /// <summary>
	  /// 退貨原因(F1951.UCC_CODE WHERE UCT_ID=RT)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string RTN_CAUSE { get; set; }

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
	  /// 派車
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string DISTR_CAR { get; set; }

    /// <summary>
    /// 成本中心
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string COST_CENTER { get; set; }

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
    [Column(TypeName = "nvarchar(20)")]
    public string CONTACT { get; set; }

	  /// <summary>
	  /// 電話
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string TEL { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(300)")]
    public string MEMO { get; set; }

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
    [Column(TypeName = "varchar(12)")]
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
	  /// 異動類型F000903
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string ORD_PROP { get; set; }

    /// <summary>
    /// 派車單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string DISTR_CAR_NO { get; set; }

    /// <summary>
    /// GOHAPPY退貨單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string EXCHANGEID { get; set; }

    /// <summary>
    /// 外部單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string FOREIGN_WMSNO { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BATCH_NO { get; set; }

    /// <summary>
    /// 配送方式(1:店取,2:宅配,3:自取,4:派車) F000904 TOPIC=F161201 SUBTOPIC=SHIPWAY
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string SHIPWAY { get; set; }

	  /// <summary>
	  /// 回檔註記(0:未回檔、1:已分配未回檔 2:已回檔)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_FLAG { get; set; }

    /// <summary>
    /// 0:貨主單據建立 1:貨主單據更正 2:貨主單據取消
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IMPORT_FLAG { get; set; }
  }
}
        