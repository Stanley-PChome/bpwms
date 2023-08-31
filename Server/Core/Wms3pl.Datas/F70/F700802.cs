namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 專車單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","DELIVERY_NO","GUP_CODE","CUST_CODE","WMS_NO")]
  [Table("F700802")]
  public class F700802 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 專車單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string DELIVERY_NO { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 物流單據類別
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string ORD_TYPE { get; set; }

    /// <summary>
    /// 客戶代號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RETAIL_CODE { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string CUST_NAME { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string ADDRESS { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    [Column(TypeName = "nvarchar(26)")]
    public string CONTACT { get; set; }

    /// <summary>
    /// 連絡電話
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CONTACT_TEL { get; set; }

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
  }
}
        