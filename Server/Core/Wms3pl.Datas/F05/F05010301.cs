namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 原始出貨訂單關聯資料身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_NO","ORD_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F05010301")]
  public class F05010301 : IAuditInfo
  {

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 通路品號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CHANNEL_ITEM_CODE { get; set; }

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
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 商品單價
    /// </summary>
    [Column(TypeName = "decimal(15, 4)")]
    public decimal? PRICE { get; set; }

    /// <summary>
    /// 單品詳細
    /// </summary>
    [Column(TypeName = "nvarchar(300)")]
    public string ITEM_DETAIL { get; set; }

    /// <summary>
    /// 單品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_NO { get; set; }

	  /// <summary>
	  /// 訂單序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string ORD_SEQ { get; set; }

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
    /// 規格
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string ITEM_SPEC { get; set; }

    /// <summary>
    /// 贈品量
    /// </summary>
    [Column(TypeName = "smallint")]
    public Int16? GIFTQTY { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    [Column(TypeName = "nvarchar(10)")]
    public string UNIT { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(300)")]
    public string ITEM_DESC { get; set; }

    /// <summary>
    /// 商品未稅金額
    /// </summary>
    [Column(TypeName = "decimal(13, 2)")]
    public decimal? AMOUNT { get; set; }

    /// <summary>
    /// 檔次名稱
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string SPOTNAME { get; set; }

    /// <summary>
    /// 檔次方案
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string SPOTPROPOSAL { get; set; }
  }
}
        