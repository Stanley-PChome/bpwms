namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨檢驗身檔明細
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","RETURN_NO","RETURN_AUDIT_SEQ","RTN_DTL_SEQ")]
  [Table("F16140201")]
  public class F16140201 : IAuditInfo
  {

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
	  /// 退貨單號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string RETURN_NO { get; set; }

	  /// <summary>
	  /// 退貨檢驗序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 RETURN_AUDIT_SEQ { get; set; }

	  /// <summary>
	  /// 退貨明細序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 RTN_DTL_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 應退貨數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 RTN_QTY { get; set; }

	  /// <summary>
	  /// 實際退貨數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 AUDIT_QTY { get; set; }

	  /// <summary>
	  /// 實際損失數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 MISS_QTY { get; set; }

	  /// <summary>
	  /// 實際不良品數量
	  /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 BAD_QTY { get; set; }

    /// <summary>
    /// 加工組合成品編號(實體組合)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string BOM_ITEM_CODE { get; set; }

    /// <summary>
    /// 商品在每一個加工成品組合數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? BOM_QTY { get; set; }

	  /// <summary>
	  /// 商品總件數(一般商品固定為1,組合商品由BOM表計算取得)
	  /// </summary>
    [Required]
		[Column(TypeName = "int")]
		public Int32 ITEM_QTY { get; set; }

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
        