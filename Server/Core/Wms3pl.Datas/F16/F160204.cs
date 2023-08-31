namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 廠退出貨單資料
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_WMS_NO","RTN_WMS_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F160204")]
  public class F160204 : IAuditInfo
  {

	  /// <summary>
	  /// 廠退出貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RTN_WMS_NO { get; set; }

	  /// <summary>
	  /// 廠退出貨單序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 RTN_WMS_SEQ { get; set; }

	  /// <summary>
	  /// 廠退出貨單建立日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime RTN_WMS_DATE { get; set; }

    /// <summary>
    /// 派車單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string DISTR_CAR_NO { get; set; }

	  /// <summary>
	  /// 廠退單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RTN_VNR_NO { get; set; }

	  /// <summary>
	  /// 廠退單序號
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 RTN_VNR_SEQ { get; set; }

	  /// <summary>
	  /// 廠商編號F1908
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 廠退出貨數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 RTN_WMS_QTY { get; set; }

	  /// <summary>
	  /// 異動類型 F000903
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string ORD_PROP { get; set; }

	  /// <summary>
	  /// 自取(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SELF_TAKE { get; set; }

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
	  /// 配送方式 0: 自取  1: 專車  2: 派車
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string DISTR_CAR { get; set; }

    /// <summary>
    /// 配送商
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string ALL_ID { get; set; }

		/// <summary>
		/// 配送方式 (0: 自取 1:宅配)
		/// </summary>
    [Column(TypeName = "char(1)")]
    public string DELIVERY_WAY { get; set; }

    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 出貨倉別
    /// </summary>
    [Column(TypeName = "varchar(8)")]
    public string TYPE_ID { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// 處理狀態 (0: 待處理 1:已配庫 2: 已包裝 3: 已出貨 )
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string PROC_FLAG { get; set; }
    /// <summary>
    /// 宅配單數 (若選擇自取，則設為0)
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? SHEET_NUM { get; set; }
    /// <summary>
    /// 備註/自取訊息
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string MEMO { get; set; }
	}
}
        