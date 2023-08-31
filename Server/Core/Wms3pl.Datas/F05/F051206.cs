namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 揀貨缺貨檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LACK_SEQ")]
  [Table("F051206")]
  public class F051206 : IAuditInfo
  {

	  /// <summary>
	  /// 缺貨檔序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 LACK_SEQ { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

    /// <summary>
    /// 揀貨單單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PICK_ORD_NO { get; set; }

    /// <summary>
    /// 揀貨單序號
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string PICK_ORD_SEQ { get; set; }

    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 缺貨數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? LACK_QTY { get; set; }

    /// <summary>
    /// 缺貨原因(F1951 WHERE UCT_ID = ‘PK’)
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string REASON { get; set; }

    /// <summary>
    /// 缺貨原因備註
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MEMO { get; set; }

    /// <summary>
    /// 缺貨處理狀態(0缺貨待確認 1貨主待確認 2已確認)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 缺貨處理結果(1:缺品出貨2取消訂單)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string RETURN_FLAG { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
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
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 是否刪除(0否 1是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ISDELETED { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 缺貨儲位
    /// </summary>
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 轉移狀態(0:未轉移;1:已轉移)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string TRANS_FLAG { get; set; }

    /// <summary>
    /// 轉移日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? TRANS_DATE { get; set; }

    /// <summary>
    /// 轉移人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string TRANS_STAFF { get; set; }

    /// <summary>
    /// 轉移人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string TRANS_NAME { get; set; }

    /// <summary>
    /// 調撥單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ALLOCATION_NO { get; set; }

    /// <summary>
    /// 調撥單序號
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? ALLOCATION_SEQ { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }
  }
}
