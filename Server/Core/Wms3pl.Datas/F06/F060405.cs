﻿namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 盤點調整結果回傳
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060405")]
	public class F060405 : IAuditInfo
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
		/// 倉庫代碼
		/// </summary>
		[Required]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 任務單號
		/// </summary>
		[Required]
		public string DOC_ID { get; set; }
		/// <summary>
		/// 原WMS單號
		/// </summary>
		[Required]
		public string WMS_NO { get; set; }
		/// <summary>
		/// 原盤點任務單號
		/// </summary>
		[Required]
		public string CHECK_CODE { get; set; }
		/// <summary>
		/// 狀態 0: 待處理 1:處理中 2:完成  9:取消
		/// </summary>
		[Required]
		public string STATUS { get; set; }
		/// <summary>
		/// 執行時間(YYYY/MM/DD hh:mi:ss)
		/// </summary>
		public DateTime? PROC_DATE { get; set; }
		/// <summary>
		/// 回傳單據狀態 中介層回傳之出庫單狀態(3:盤點完成 9:取消)
		/// </summary>
		[Required]
		public string M_STATUS { get; set; }
		/// <summary>
		/// 明細數
		/// </summary>
		[Required]
		public int SKUTOTAL { get; set; }
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
