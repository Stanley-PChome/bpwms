﻿namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// WCS任務重新派發記錄表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060703")]
	public class F060703 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public Int64 ID { get; set; }
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
		/// 來源任務單號
		/// </summary>
		[Required]
		public string ORI_DOC_ID { get; set; }
		/// <summary>
		/// 來源資料表

		/// </summary>
		[Required]
		public string ORI_TABLE { get; set; }
		/// <summary>
		/// 來源狀態
		/// </summary>
		[Required]
		public string ORI_STATUS { get; set; }
		/// <summary>
		/// 來源命令類別
		/// </summary>
		[Required]
		public string ORI_CMD_TYPE { get; set; }
		/// <summary>
		/// 來源傳送時間
		/// </summary>
		public DateTime? ORI_PROC_DATE { get; set; }
		/// <summary>
		/// 來源訊息
		/// </summary>
		public string ORI_MESSAGE { get; set; }
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
	}
}