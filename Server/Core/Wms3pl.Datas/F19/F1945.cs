﻿namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 集貨場設定
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "COLLECTION_CODE")]
	[Table("F1945")]
	public class F1945 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 集貨場編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string COLLECTION_CODE { get; set; }
		/// <summary>
		/// 集貨場名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(20)")]
		public string COLLECTION_NAME { get; set; }
		/// <summary>
		/// 集貨場類型 (0: 訂單集貨 1: 跨庫調撥集貨 2: 廠退集貨)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string COLLECTION_TYPE { get; set; }
		/// <summary>
		/// 集貨格開頭編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string CELL_START_CODE { get; set; }
		/// <summary>
		/// 集貨格類型 (F194501)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string CELL_TYPE { get; set; }
		/// <summary>
		/// 集貨格數量
		/// </summary>
		[Required]
		[Column(TypeName = "int")]
		public int CELL_NUM { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員編號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }
	}
}
