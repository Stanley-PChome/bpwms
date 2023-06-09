namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 下位系統的庫存原始明細資料
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060701")]
	public class F060701 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 物流單號
		/// </summary>
		[Required]
		public string SHIP_CODE { get; set; }
		/// <summary>
		/// 物流商編號
		/// </summary>
		[Required]
		public string SHIP_PROVIDER { get; set; }
		/// <summary>
		/// 分揀機編號
		/// </summary>
		[Required]
		public string SORTER_CODE { get; set; }
		/// <summary>
		/// 目的流道口
		/// </summary>
		[Required]
		public string PORT_NO { get; set; }
		/// <summary>
		/// 紙箱型號
		/// </summary>
		public string BOX_CODE { get; set; }
		/// <summary>
		/// 紙箱長度
		/// </summary>
		public Decimal? BOX_LENGTH { get; set; }
		/// <summary>
		/// 紙箱寬度
		/// </summary>
		public Decimal? BOX_WIDTH { get;set; }
		/// <summary>
		/// 紙箱高度
		/// </summary>
		public Decimal? BOX_HEIGHT { get;set; }
		/// <summary>
		/// 紙箱重量
		/// </summary>
		public Decimal? BOX_WEIGHT { get;set; }
		/// <summary>
		/// 紀錄時間
		/// </summary>
		public DateTime CREATE_TIME { get; set; }
		/// <summary>
		/// 處理狀態(0:待處理;1:處理中;2:已完成)
		/// </summary>
		public string STATUS { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }
		
		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string MSG_CONTENT { get; set; }
	}
}
