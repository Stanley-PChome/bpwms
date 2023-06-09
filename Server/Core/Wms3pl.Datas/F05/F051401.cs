﻿namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	[Serializable]
	[DataServiceKey("DC_CODE", "COLLECTION_CODE", "CELL_CODE")]
	[Table("F051401")]
	public class F051401: IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 集貨場編號
		/// </summary>
		[Key]
		[Required]
		public string COLLECTION_CODE { get; set; }
		/// <summary>
		/// 集貨格編號
		/// </summary>
		[Key]
		[Required]
		public string CELL_CODE { get; set; }
		/// <summary>
		/// 集貨格類型 (F194501)
		/// </summary>
		[Required]
		public string CELL_TYPE { get; set; }
		/// <summary>
		/// 容器條碼
		/// </summary>
		public string CONTAINER_CODE { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 狀態(0: 空儲位、1: 已安排、2: 已放入、3: 已到齊)
		/// </summary>
		[Required]
		public string STATUS { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員編號
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CUST_CODE { get; set; }
	}
}