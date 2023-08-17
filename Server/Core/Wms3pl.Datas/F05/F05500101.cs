namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 包裝刷驗紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("WMS_ORD_NO","LOG_SEQ","DC_CODE","GUP_CODE","CUST_CODE","PACKAGE_BOX_NO")]
  [Table("F05500101")]
  public class F05500101 : IAuditInfo
  {

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 包裝人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PACKAGE_STAFF { get; set; }

	  /// <summary>
	  /// 包裝人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string PACKAGE_NAME { get; set; }

	  /// <summary>
	  /// 紀錄序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 LOG_SEQ { get; set; }

    /// <summary>
    /// 商品品號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

    /// <summary>
    /// 序號狀態
    /// </summary>
    [Column(TypeName = "char(2)")]
    public string STATUS { get; set; }

	  /// <summary>
	  /// 是否通過
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPASS { get; set; }

    /// <summary>
    /// 刷驗訊息
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string MESSAGE { get; set; }

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
	  /// 包裝箱號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "smallint")]
    public Int16 PACKAGE_BOX_NO { get; set; }

    /// <summary>
    /// 人員刷讀商品條碼
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SCAN_CODE { get; set; }

    /// <summary>
    /// 紀錄狀態(0:正常包裝 9:取消包裝)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FLAG { get; set; }

    /// <summary>
    /// 紀律序號原始單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ORG_SERIAL_WMS_NO { get; set; }

  }
}
        