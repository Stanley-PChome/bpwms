namespace Wms3pl.Datas.F01
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉棧板貼紙檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","STICKER_NO")]
  [Table("F010203")]
  public class F010203 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心編號
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
	  /// 棧板貼紙編號(進倉單號去掉第一碼英文+棧板編號)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(24)")]
    public string STICKER_NO { get; set; }

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string STOCK_NO { get; set; }

	  /// <summary>
	  /// 棧板編號(流水號四碼)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string PALLET_NO { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 商品條碼(F1903.ENA_CODE1)
    /// </summary>
    [Column(TypeName = "varchar(26)")]
    public string ENA_CODE1 { get; set; }

    /// <summary>
    /// 外箱條碼(F1903.ENA_CODE3)
    /// </summary>
    [Column(TypeName = "varchar(26)")]
    public string ENA_CODE3 { get; set; }

    /// <summary>
    /// 商品箱入數(F190301.計量單位=箱)
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? ITEM_CASE_QTY { get; set; }

    /// <summary>
    /// 商品箱入數(F190301.計量單位=小包裝)
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? ITEM_PACKAGE_QTY { get; set; }

	  /// <summary>
	  /// 棧板每層箱數(F190305.PALLET_LEVEL_CASEQTY)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PALLET_LEVEL_CASEQTY { get; set; }

	  /// <summary>
	  /// 棧板層數(F190305.PALLET_LEVEL_CNT)
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PALLET_LEVEL_CNT { get; set; }

	  /// <summary>
	  /// 訂貨箱數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ORDER_CASE_QTY { get; set; }

	  /// <summary>
	  /// 訂貨零散數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 ORDER_OTHER_QTY { get; set; }

    /// <summary>
    /// 入庫日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? ENTER_DATE { get; set; }

    /// <summary>
    /// 有效期限
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? VALID_DATE { get; set; }

    /// <summary>
    /// 驗收單編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RT_NO { get; set; }

    /// <summary>
    /// 驗收單序號
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string RT_SEQ { get; set; }

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
	  /// 貼紙類型 (1: 驗收前列印 2:驗收後列印) 
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STICKER_TYPE { get; set; }

    /// <summary>
    /// 儲位編號
    /// </summary>
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }

    /// <summary>
    /// 調撥單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ALLOCATION_NO { get; set; }

    /// <summary>
    /// 驗收數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? RECV_QTY { get; set; }

    /// <summary>
    /// 棧板貼紙數參考
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string STICKER_REF { get; set; }

    /// <summary>
    /// 驗收數=驗收箱數*商品箱入數+驗收零散數
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string RECV_QTY_DESC { get; set; }

    /// <summary>
    /// 上架數=上架箱數*商品箱入數+上架零散數
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string TAR_QTY_DESC { get; set; }
  }
}
        