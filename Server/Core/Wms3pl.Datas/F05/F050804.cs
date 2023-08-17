namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨貼紙主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","STICKER_NO")]
  [Table("F050804")]
  public class F050804 : IAuditInfo
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
	  /// 出貨貼紙編號(出貨單號+箱號)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(24)")]
    public string STICKER_NO { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 箱號(流水號4碼)
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string BOX_NO { get; set; }

    /// <summary>
    /// 揀貨單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PICK_ORD_NO { get; set; }

    /// <summary>
    /// 批次日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? DELV_DATE { get; set; }

    /// <summary>
    /// 批次時間
    /// </summary>
    [Column(TypeName = "varchar(8)")]
    public string PICK_TIME { get; set; }

    /// <summary>
    /// 出車時段
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string CAR_PERIOD { get; set; }

    /// <summary>
    /// 車次/路線
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string DELV_NO { get; set; }

    /// <summary>
    /// 倉庫編號
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 儲區編號
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string AREA_CODE { get; set; }

    /// <summary>
    /// 門市編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RETAIL_CODE { get; set; }

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
  }
}
        