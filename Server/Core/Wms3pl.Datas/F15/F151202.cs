namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 彙總調撥明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("BATCH_ALLOC_NO","BATCH_ALLOC_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F151202")]
  public class F151202 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總調撥單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BATCH_ALLOC_NO { get; set; }

	  /// <summary>
	  /// 彙總調撥單序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal BATCH_ALLOC_SEQ { get; set; }

	  /// <summary>
	  /// 調撥單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ALLOCATION_NO { get; set; }

	  /// <summary>
	  /// 調撥單序號
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal ALLOCATION_SEQ { get; set; }

	  /// <summary>
	  /// 貨架編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string SHELF_NO { get; set; }

	  /// <summary>
	  /// 預計作業儲位
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(14)")]
    public string B_LOC_CODE { get; set; }

    /// <summary>
    /// 實際作業儲位
    /// </summary>
    [Column(TypeName = "varchar(14)")]
    public string A_LOC_CODE { get; set; }

	  /// <summary>
	  /// 預計作業數量
	  /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal B_QTY { get; set; }

    /// <summary>
    /// 實際作業數量
    /// </summary>
    [Column(TypeName = "decimal(18, 0)")]
    public Decimal? A_QTY { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 效期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 入庫日
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }

	  /// <summary>
	  /// 作業狀態(0: 待處理 1: 呼叫貨架中 2: 作業完成 9: 取消
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 商品條碼2
    /// </summary>
    [Column(TypeName = "varchar(26)")]
    public string EAN_CODE2 { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主(0:共用)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主(0:共用)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 原呼叫AGV儲位
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(14)")]
    public string O_LOC_CODE { get; set; }
  }
}
        