namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調撥缺貨檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LACK_SEQ")]
  [Table("F151003")]
  public class F151003 : IAuditInfo
  {

	  /// <summary>
	  /// 缺貨檔序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 LACK_SEQ { get; set; }

	  /// <summary>
	  /// 調撥單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ALLOCATION_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 調撥數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 MOVE_QTY { get; set; }

	  /// <summary>
	  /// 缺貨數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 LACK_QTY { get; set; }

	  /// <summary>
	  /// 缺貨原因(F1951 WHERE UCT_ID = ‘MV’)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string REASON { get; set; }

    /// <summary>
    /// 缺貨原因備註
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MEMO { get; set; }

	  /// <summary>
	  /// 缺貨處理狀態(0缺貨待確認 2結案 9刪除)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

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
	  /// 調撥單序號
	  /// </summary>
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 ALLOCATION_SEQ { get; set; }

    /// <summary>
    /// 缺貨類型 (0:調撥下架、1:調撥上架)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string LACK_TYPE { get; set; }
  }
}
        