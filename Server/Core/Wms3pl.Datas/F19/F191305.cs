namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 批次快速移轉庫存紀錄表
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "TRANSACTION_NO")]
	[Table("F191305")]
	public class F191305 : IAuditInfo
	{

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 交易編號
		/// </summary>
		[Key]
		[Required]
		public string TRANSACTION_NO { get; set; }

		/// <summary>
		/// 快速移轉庫存調整單的json資料
		/// </summary>
		[Required]
		public string SEND_DATA { get; set; }
		/// <summary>
		/// 處理狀態 (0: 待處理、1:已處理、2: 失敗)
		/// </summary>
		[Required]
		public string STATUS { get; set; }


		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

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
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
	}
}
