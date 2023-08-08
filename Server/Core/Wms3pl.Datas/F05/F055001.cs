namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出貨包裝頭檔
	/// </summary>
	[Serializable]
	[DataServiceKey("WMS_ORD_NO", "PACKAGE_BOX_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F055001")]
	public class F055001 : IAuditInfo
	{

		/// <summary>
		/// 出貨單號
		/// </summary>
		[Key]
		[Required]
		public string WMS_ORD_NO { get; set; }

		/// <summary>
		/// 包裝箱號
		/// </summary>
		[Key]
		[Required]
		public Int16 PACKAGE_BOX_NO { get; set; }

		/// <summary>
		/// 批次日期
		/// </summary>
		[Required]
		public DateTime DELV_DATE { get; set; }

		/// <summary>
		/// 批次時段
		/// </summary>
		[Required]
		public string PICK_TIME { get; set; }

		/// <summary>
		/// 列印註記
		/// </summary>
		public Decimal? PRINT_FLAG { get; set; }

		/// <summary>
		/// 列印日期
		/// </summary>
		public DateTime? PRINT_DATE { get; set; }

		/// <summary>
		/// 包裝紙箱編號(F1903.ITEM_CODE)
		/// </summary>
		public string BOX_NUM { get; set; }

		/// <summary>
		/// 託運單號
		/// </summary>
		public string PAST_NO { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 包裝人員
		/// </summary>
		public string PACKAGE_STAFF { get; set; }

		/// <summary>
		/// 包裝人名
		/// </summary>
		public string PACKAGE_NAME { get; set; }

		/// <summary>
		/// 0未稽核 1已稽核
		/// </summary>
		[Required]
		public string STATUS { get; set; }

		/// <summary>
		/// 稽核時間
		/// </summary>
		public DateTime? AUDIT_DATE { get; set; }

		/// <summary>
		/// 包裝重量
		/// </summary>
		public decimal? WEIGHT { get; set; }

		/// <summary>
		/// 稽核人員
		/// </summary>
		public string AUDIT_STAFF { get; set; }

		/// <summary>
		/// 稽核人名
		/// </summary>
		public string AUDIT_NAME { get; set; }

		/// <summary>
		/// 配送商
		/// </summary>
		public string ALL_ID { get; set; }
		/// <summary>
		/// 小白標條碼
		/// </summary>
		public string BOX_DOC { get; set; }

		/// <summary>
		/// 是否關箱(0:否;1:是)
		/// </summary>
		public string IS_CLOSED { get; set; }

		/// <summary>
		/// 是否原箱(0:否:1:是)
		/// </summary>
		public string IS_ORIBOX { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WORKSTATION_CODE { get; set; }
    /// <summary>
    /// 封箱工作站編號
    /// </summary>
    public string CLOSEBOX_WORKSTATION_CODE { get; set; }
    /// <summary>
    /// 分揀機編號
    /// </summary>
    public string SORTER_CODE { get; set; }
    /// <summary>
    /// 配箱電腦名稱
    /// </summary>
    public string PACK_CLIENT_PC { get; set; }
    /// <summary>
    /// 扣帳電腦名稱
    /// </summary>
    public string AUDIT_CLIENT_PC { get; set; }
    /// <summary>
    /// 配箱站與封箱站是否分開(0:否 1:是)
    /// </summary>
    public string NO_SPEC_REPROTS { get; set; }
    /// <summary>
    /// 是否刷讀紙箱關箱(0:否 1:是)
    /// </summary>
    public string CLOSE_BY_BOXNO { get; set; }
    /// <summary>
    /// 人員關箱時間
    /// </summary>
    public DateTime? CLOSEBOX_TIME { get; set; }
    /// <summary>
    /// 物流商編號
    /// </summary>
    public string LOGISTIC_CODE { get; set; }
    /// <summary>
    /// 原包裝時申請物流商編號
    /// </summary>
    public string ORG_LOGISTIC_CODE { get; set; }
    /// <summary>
    /// 原包裝時申請宅配單號
    /// </summary>
    public string ORG_PAST_NO { get; set; }
    /// <summary>
    /// 原包裝時紙箱編號
    /// </summary>
    public string ORG_BOX_NUM { get; set; }
  }
}