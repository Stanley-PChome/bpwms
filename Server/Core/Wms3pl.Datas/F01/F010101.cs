namespace Wms3pl.Datas.F01
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 採購單主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("SHOP_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F010101")]
  public class F010101 : IAuditInfo
  {

	  /// <summary>
	  /// 採購單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
	  public string SHOP_NO { get; set; }

	  /// <summary>
	  /// 採購日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
	  public DateTime SHOP_DATE { get; set; }

	  /// <summary>
	  /// 預定進貨日
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
	  public DateTime DELIVER_DATE { get; set; }

	  /// <summary>
	  /// 廠商編號 F1908
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 發票日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime INVOICE_DATE { get; set; }

    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(25)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 連絡電話
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string CONTACT_TEL { get; set; }

	  /// <summary>
	  /// 採購原因 F1951 Where UCT_ID=PO
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string SHOP_CAUSE { get; set; }

	  /// <summary>
	  /// 單據狀態(0待處理1已核准2結案9取消)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
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
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(16)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動類型F000903
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string ORD_PROP { get; set; }
  }
}
        