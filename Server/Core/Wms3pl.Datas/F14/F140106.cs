namespace Wms3pl.Datas.F14
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 盤點單盤點差異
  /// </summary>
  [Serializable]
  [DataServiceKey("INVENTORY_NO","LOC_CODE","ITEM_CODE","VALID_DATE","ENTER_DATE","DC_CODE","GUP_CODE","CUST_CODE","BOX_CTRL_NO","PALLET_CTRL_NO","MAKE_NO")]
  [Table("F140106")]
  public class F140106 : IAuditInfo
  {

	  /// <summary>
	  /// 盤點單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string INVENTORY_NO { get; set; }

	  /// <summary>
	  /// 倉別編號(F1980)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(8)")]
    public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 有效日期
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 入庫日期(YYYY/MM/DD)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 儲位商品庫存數
	  /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 QTY { get; set; }

	  /// <summary>
	  /// 初盤數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 FIRST_QTY { get; set; }

    /// <summary>
    /// 複盤數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? SECOND_QTY { get; set; }

	  /// <summary>
	  /// 初盤差異數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 FIRST_DIFF_QTY { get; set; }

    /// <summary>
    /// 複盤差異數
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? SECOND_DIFF_QTY { get; set; }

	  /// <summary>
	  /// 盤盈是否回沖(0否1是)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string FLUSHBACK { get; set; }

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
    /// 初盤人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string FST_INVENTORY_STAFF { get; set; }

    /// <summary>
    /// 初盤人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string FST_INVENTORY_NAME { get; set; }

    /// <summary>
    /// 初盤日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? FST_INVENTORY_DATE { get; set; }

    /// <summary>
    /// 初盤電腦名稱
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string FST_INVENTORY_PC { get; set; }

    /// <summary>
    /// 複盤人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SEC_INVENTORY_STAFF { get; set; }

    /// <summary>
    /// 複盤人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string SEC_INVENTORY_NAME { get; set; }

    /// <summary>
    /// 複盤日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? SEC_INVENTORY_DATE { get; set; }

    /// <summary>
    /// 複盤電腦名稱
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SEC_INVENTORY_PC { get; set; }

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
	  /// 箱號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }

    /// <summary>
    /// WMS初盤庫差數
    /// </summary>
    [Column(TypeName = "int")]
    public int? FIRST_STOCK_DIFF_QTY { get; set; }

    /// <summary>
    /// WMS複盤庫差數
    /// </summary>
    [Column(TypeName = "int")]
    public int? SECOND_STOCK_DIFF_QTY { get; set; }

    /// <summary>
    /// WMS複盤庫差數
    /// </summary>
    [Column(TypeName = "int")]
    public int? DEVICE_STOCK_QTY { get; set; }

    /// <summary>
    /// WMS未搬動庫存數
    /// </summary>
    [Column(TypeName = "int")]
    public int? UNMOVE_STOCK_QTY { get; set; }

    /// <summary>
    /// 調整狀態(0:未調整1:調整成功;2:調整失敗,3:不調整)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
    /// <summary>
    /// WMS調整狀態(0=待調整,1=調整成功,2=調整失敗,3=不調整)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string WMS_STATUS { get; set; }
    /// <summary>
    /// 人員調整狀態(0=不調整,1=調整)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string PERSON_CONFIRM_STATUS { get; set; }
    /// <summary>
    /// 處理單號(盤盈:調整單號 盤損:調撥單號)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PROC_WMS_NO { get; set; }
	}
}
        