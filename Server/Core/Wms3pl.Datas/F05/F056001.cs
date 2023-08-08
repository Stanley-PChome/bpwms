using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	/// <summary>
	/// 出貨包裝箱列印報表
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WORKSTATION_CODE", "BOX_CODE")]
	[Table("F056001")]
	public class F056001 : IAuditInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		[Key]
		[Required]
		public string WORKSTATION_CODE { get; set; }
		/// <summary>
		/// 紙箱編號
		/// </summary>
		[Key]
		[Required]
		public string BOX_CODE { get; set; }
		/// <summary>
		/// 樓層
		/// </summary>
		[Required]
		public string FLOOR { get; set; }
		/// <summary>
		/// 安全水位數量
		/// </summary>
		[Required]
		public int SAVE_QTY { get; set; }
		/// <summary>
		/// 目前可使用數量
		/// </summary>
		[Required]
		public int QTY { get; set; }
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
