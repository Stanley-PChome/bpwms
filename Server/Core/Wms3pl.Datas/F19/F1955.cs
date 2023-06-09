namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 便利倉設定 
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "CONVENIENT_CODE")]
	[Table("F1955")]
	public class F1955: IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 便利場編號
		/// </summary>
		[Key]
		[Required]
		public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// 便利場名稱
		/// </summary>
		[Required]
		public string CONVENIENT_NAME { get; set; }
		/// <summary>
		/// 便利場儲格開頭編號
		/// </summary>
		public string CELL_START_CODE { get; set; }
		/// <summary>
		/// 便利場儲格總數量
		/// </summary>
		[Required]
		public int CELL_NUM { get; set; }
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
