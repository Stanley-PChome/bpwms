namespace Wms3pl.Datas.F16
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 廠退單主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("RTN_VNR_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F160201")]
	public class F160201 : IAuditInfo
	{

		/// <summary>
		/// 廠退單號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string RTN_VNR_NO { get; set; }

		/// <summary>
		/// 廠退日期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime RTN_VNR_DATE { get; set; }

		/// <summary>
		/// 單據狀態(0:待處理 1:處理中 2:結案  9:取消)F000904
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

		/// <summary>
		/// 異動類型F000903
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string ORD_PROP { get; set; }

		/// <summary>
		/// 廠退類型 F160203
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(5)")]
    public string RTN_VNR_TYPE_ID { get; set; }

		/// <summary>
		/// 廠退原因(F1951.UCC_CODE WHERE UCT_ID=RV)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string RTN_VNR_CAUSE { get; set; }

		/// <summary>
		/// 自取(0否1是)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string SELF_TAKE { get; set; }

		/// <summary>
		/// 廠商編號F1908
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 成本中心
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string COST_CENTER { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column(TypeName = "nvarchar(300)")]
    public string MEMO { get; set; }

    /// <summary>
    /// 過帳日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? POSTING_DATE { get; set; }

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
    [Column(TypeName = "varchar(12)")]
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
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 對外單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string FOREIGN_WMSNO { get; set; }

    /// <summary>
    /// 對外客戶編號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string FOREIGN_CUSTCODE { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BATCH_NO { get; set; }
    /// <summary>
    /// 0:貨主單據建立 1:貨主單據更正 2:貨主單據取消
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IMPORT_FLAG { get; set; }
		/// <summary>
		/// 貨物聯絡人電話
		/// </summary>
		[Column(TypeName = "varchar(40)")]
		public string ITEM_TEL { get; set; }
		/// <summary>
		/// 地址
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string ADDRESS { get; set; }
    /// <summary>
    /// 貨物聯絡人
    /// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string ITEM_CONTACT { get; set; }
    /// <summary>
    /// 配送方式 (0: 自取 1:宅配) 
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string DELIVERY_WAY { get; set; }
		/// <summary>
		/// 出貨倉別類型 (F198001)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(8)")]
    public string TYPE_ID { get; set; }
	}
}
