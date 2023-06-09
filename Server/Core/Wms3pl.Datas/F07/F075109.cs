namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 外部出貨包裝單據紀錄表
  /// </summary>
  [Serializable]
	[DataServiceKey("DC_CODE", "DOC_ID")]
  [Table("F075109")]
	public class F075109 : IAuditInfo
	{

		/// <summary>
		/// 物流中心編號
		/// </summary>
    [Key]
		[Required]
		public string DC_CODE { get; set; }

    /// <summary>
    /// 任務單號
    /// </summary>
    [Key]
    [Required]
		public string DOC_ID { get; set; }

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
