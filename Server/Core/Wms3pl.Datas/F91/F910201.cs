namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 流通加工單頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PROCESS_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F910201")]
  public class F910201 : IAuditInfo
  {

	  /// <summary>
	  /// 加工單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 加工來源(0貨主1物流中心)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROCESS_SOURCE { get; set; }

	  /// <summary>
	  /// 委外商F1928
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string OUTSOURCE_ID { get; set; }

	  /// <summary>
	  /// 完工日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime FINISH_DATE { get; set; }

    /// <summary>
    /// 一般、同質性商品加工單-成品編號;快速加工單-原料編號(F1903)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 組合編號F910101.BOM_NO
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE_BOM { get; set; }

	  /// <summary>
	  /// 加工數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 PROCESS_QTY { get; set; }

	  /// <summary>
	  /// 實際加工數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 A_PROCESS_QTY { get; set; }

	  /// <summary>
	  /// 損壞數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 BREAK_QTY { get; set; }

	  /// <summary>
	  /// 最小盒裝數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 BOX_QTY { get; set; }

	  /// <summary>
	  /// 最小箱裝數
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 CASE_QTY { get; set; }

    /// <summary>
    /// 進倉單號F020201
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ORDER_NO { get; set; }

	  /// <summary>
	  /// 單據狀態(0待處理 1加工中 2加工完成 3結案 9取消)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

	  /// <summary>
	  /// 作業狀態(0建立加工單 1開立揀料單 2預選標籤 3選擇生產線 4啟動加工 5加工完成)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_STATUS { get; set; }

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
	  /// 報價單編號(項目編號)F910401
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string QUOTE_NO { get; set; }

	  /// <summary>
	  /// 完工時間
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string FINISH_TIME { get; set; }

	  /// <summary>
	  /// 加工單類別，0:一般加工單，1:同質性商品加工單，2:快速加工單
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_TYPE { get; set; }

    /// <summary>
    /// 快速加工單-加工項目 F000904 TOPIC=F910201 SUBTOPIC=PROCESS_ITEM
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string PROCESS_ITEM { get; set; }

    /// <summary>
    /// 快速加工單-成品編號F1903
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string GOOD_CODE { get; set; }

    /// <summary>
    /// 快速加工單-分裝數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? PACK_QTY { get; set; }

    /// <summary>
    /// 快速加工單-加工開始時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PROC_BEGIN_DATE { get; set; }

    /// <summary>
    /// 快速加工單-加工完成時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PROC_END_DATE { get; set; }
  }
}
        