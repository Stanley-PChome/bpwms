namespace Wms3pl.Datas.F00
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 跨庫物流中心主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("CROSS_CODE")]
	[Table("F0001")]
	public class F0001 : IAuditInfo
	{
		/// <summary>
		/// 跨庫物流中心代碼
		/// </summary>
		[Key]
		[Required]
		public string CROSS_CODE { get; set; }
		/// <summary>
		/// 跨庫物流中心名稱
		/// </summary>
		[Required]
		public string CROSS_NAME { get; set; }
		/// <summary>
		/// 出貨類別(01:跨庫調撥出 02:訂單出)
		/// </summary>
		[Required]
		public string CROSS_TYPE { get; set; }
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
	}
}
