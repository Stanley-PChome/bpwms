namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 下位系統的庫存原始總表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060601")]
	public class F060601 : IAuditInfo
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
		/// 對接系統(0: Geek、1: 盟立)
		/// </summary>
		[Required]
		public string SRC_SYSTEM { get; set; }
		/// <summary>
		/// 對帳時間(yyyy/MM/dd HH:ii:ss)
		/// </summary>
		[Required]
		public string AUDIT_DATE { get; set; }
		/// <summary>
		/// 備份日期
		/// </summary>
		[Required]
		public string CAL_DATE { get; set; }
		/// <summary>
		/// 總頁數
		/// </summary>
		[Required]
		public int TOTAL_PAGE { get; set; }
		/// <summary>
		/// 當前頁
		/// </summary>
		[Required]
		public int CURRENT_PAGE { get; set; }
		/// <summary>
		/// 一頁筆數
		/// </summary>
		[Required]
		public int PAGE_SIZE { get; set; }
		/// <summary>
		/// 處理狀態 (0: 待處理 1: 處理中 2: 已匯出 9: 重複匯入取消)
		/// </summary>
		[Required]
		public string PROC_FLAG { get; set; }
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
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
