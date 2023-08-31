namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉商品序號刷讀紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PURCHASE_NO","PURCHASE_SEQ","LOG_SEQ","DC_CODE","GUP_CODE","CUST_CODE","RT_NO")]
  [Table("F02020104")]
  public class F02020104 : IAuditInfo
  {

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PURCHASE_NO { get; set; }

	  /// <summary>
	  /// 進倉單序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string PURCHASE_SEQ { get; set; }

	  /// <summary>
	  /// 流水序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 商品品號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
    [Required]
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
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 驗收單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RT_NO { get; set; }

    /// <summary>
    /// 儲值卡盒號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BATCH_NO { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MESSAGE { get; set; }
  }
}
        