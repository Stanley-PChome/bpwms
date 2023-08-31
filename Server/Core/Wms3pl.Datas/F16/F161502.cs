namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨彙總表身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("GATHER_NO","GATHER_SEQ","DC_CODE")]
  [Table("F161502")]
  public class F161502 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總單號
	  /// </summary>
    [Key]
    [Required]
		[Column(TypeName = "varchar(20)")]
		public string GATHER_NO { get; set; }

	  /// <summary>
	  /// 彙總單序號(項次,箱號)
	  /// </summary>
    [Key]
    [Required]
		[Column(TypeName = "varchar(5)")]
		public string GATHER_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
		[Column(TypeName = "varchar(20)")]
		public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Required]
		[Column(TypeName = "varchar(4)")]
		public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
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
    [Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 商品名稱(匯入彙總表品名)
		/// </summary>
		[Column(TypeName = "nvarchar(300)")]
		public string ITEM_NAME { get; set; }

		/// <summary>
		/// 退貨數量(匯入彙總表退貨數)
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string RTN_QTY { get; set; }
  }
}
        