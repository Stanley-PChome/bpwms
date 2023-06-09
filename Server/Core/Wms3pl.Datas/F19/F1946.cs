namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 工作站設定
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "WORKSTATION_CODE")]
	[Table("F1946")]
	public class F1946: IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		[Key]
		[Required]
		public string WORKSTATION_CODE { get; set; }
		/// <summary>
		/// 工作站類型 (PA1: 包裝線包裝站、PA2: 單人包裝站)
		/// </summary>
		[Required]
		public string WORKSTATION_TYPE { get; set; }
		/// <summary>
		/// 工作站群組 (0: 出貨包裝)
		/// </summary>
		[Required]
		public string WORKSTATION_GROUP { get; set; }
		/// <summary>
		/// 包裝線號碼
		/// </summary>
		public string PACKING_LINE_NO { get; set; }
    /// <summary>
    /// 工作站狀態(0: 關站、1:開站、2:暫停、3:關站中)
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
	}
}
