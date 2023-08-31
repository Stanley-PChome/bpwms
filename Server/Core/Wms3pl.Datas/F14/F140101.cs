namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 盤點單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F140101")]
  public class F140101 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 盤點單日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime INVENTORY_DATE { get; set; }

	  /// <summary>
	  /// 計費(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISCHARGE { get; set; }

    /// <summary>
    /// 費用/次
    /// </summary>
    [Column(TypeName = "decimal(9, 2)")]
    public decimal? FEE { get; set; }

	  /// <summary>
	  /// 盤點類型(0商品抽盤,1循環盤,2異動盤,3全盤,4半年盤, 5儲位抽盤)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string INVENTORY_TYPE { get; set; }

    /// <summary>
    /// 盤點週期
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? INVENTORY_CYCLE { get; set; }

    /// <summary>
    /// 盤點年份
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? INVENTORY_YEAR { get; set; }

    /// <summary>
    /// 盤點月份
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? INVENTORY_MON { get; set; }

    /// <summary>
    /// 循環盤次數
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? CYCLE_TIMES { get; set; }

	  /// <summary>
	  /// 盤點時顯示盤點數(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SHOW_CNT { get; set; }

	  /// <summary>
	  /// 盤點單狀態(0待處理1初盤2複盤3已確認 5過帳 9取消)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

	  /// <summary>
	  /// 品項數
	  /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ITEM_CNT { get; set; }

	  /// <summary>
	  /// 商品件數
	  /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ITEM_QTY { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MEMO { get; set; }

    /// <summary>
    /// 列印盤點單時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PRINT_DATE { get; set; }

    /// <summary>
    /// 過帳時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? POSTING_DATE { get; set; }

	  /// <summary>
	  /// 是否有產生複盤資料(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISSECOND { get; set; }

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
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
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
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 盤點工具 0:人工 1:AGV 2:SHUTTLE
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string CHECK_TOOL { get; set; }
  }
}
        