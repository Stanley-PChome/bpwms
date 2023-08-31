namespace Wms3pl.Datas.F01
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 進倉單主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("STOCK_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F010201")]
	public class F010201 : IAuditInfo
	{

		/// <summary>
		/// 進倉單號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string STOCK_NO { get; set; }

		/// <summary>
		/// 建立進倉單日期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime STOCK_DATE { get; set; }

    /// <summary>
    /// 採購日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? SHOP_DATE { get; set; }

		/// <summary>
		/// 預定進貨日
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime DELIVER_DATE { get; set; }

    /// <summary>
    /// 來源單據 F000902
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string SOURCE_TYPE { get; set; }

    /// <summary>
    /// 來源單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SOURCE_NO { get; set; }

		/// <summary>
		/// 單據性質(異動類型)F000903
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string ORD_PROP { get; set; }

		/// <summary>
		/// 廠商編號 F1908
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(25)")]
    public string CUST_ORD_NO { get; set; }

    /// <summary>
    /// 貨主成本中心
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string CUST_COST { get; set; }

		/// <summary>
		/// 單據狀態(0待處理1驗收中2結案8異常9取消) F000904
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
    /// 採購單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SHOP_NO { get; set; }

		/// <summary>
		/// 是否為EDI來源的單據
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string EDI_FLAG { get; set; }

    /// <summary>
    /// 驗證碼
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string CHECK_CODE { get; set; }

		[Required]
    [Column(TypeName = "char(1)")]
    public string CHECKCODE_EDI_STATUS { get; set; }

    /// <summary>
    /// 出貨驗證碼(ex: 報關單號)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string DELV_CHECKCODE { get; set; }

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
    /// 快速通關分類
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FAST_PASS_TYPE { get; set; }
    /// <summary>
    /// 預定進倉時段
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string BOOKING_IN_PERIOD { get; set; }

		/// <summary>
		/// 是否使用者強制結案 (0: 否、1: 是)
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string USER_CLOSED { get; set; }

		/// <summary>
		/// 使用者強制結案備註原因
		/// </summary>
		[Column(TypeName = "nvarchar(200)")]
		public string USER_CLOSED_MEMO { get; set; }
	}
}
