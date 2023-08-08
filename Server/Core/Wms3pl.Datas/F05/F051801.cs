namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 便利倉儲位主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "CONVENIENT_CODE", "CELL_CODE")]
	[Table("F051801")]
	public class F051801 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 便利倉編號
		/// </summary>
		[Key]
		[Required]
		public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// 儲格編號
		/// </summary>
		[Key]
		[Required]
		public string CELL_CODE { get; set; }
		/// <summary>
		/// 廠商編號(F1908)
		/// </summary>
		public string VNR_CODE { get; set; }
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
		/// 狀態(0: 空儲位、1: 已安排、2: 已放入)
		/// </summary>
		[Required]
		public string STATUS { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員編號
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
