namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 集貨格設定
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "CELL_TYPE")]
	[Table("F194501")]
	public class F194501: IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 集貨格料架類型
		/// </summary>
		[Key]
		[Required]
		public string CELL_TYPE { get; set; }
		/// <summary>
		/// 集貨格料架名稱
		/// </summary>
		[Required]
		public string CELL_NAME { get; set; }
		/// <summary>
		/// 長
		/// </summary>
		[Required]
		public int LENGTH { get; set; }
		/// <summary>
		/// 寬
		/// </summary>
		[Required]
		public int DEPTH { get; set; }
		/// <summary>
		/// 高
		/// </summary>
		[Required]
		public int HEIGHT { get; set; }
		/// <summary>
		/// 容積率
		/// </summary>
		[Required]
		public decimal VOLUME_RATE { get; set; }
		/// <summary>
		/// 單個集貨格最大可放容量(已乘容積率)
		/// </summary>
		[Required]
		public Int64 MAX_VOLUME { get; set; }
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
