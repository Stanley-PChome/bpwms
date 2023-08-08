namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	[Serializable]
	[DataServiceKey("PICK_ORD_NO", "WMS_ORD_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F052903")]
	public class F052903 : IAuditInfo
	{
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[Key]
		[Required]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[Key]
		[Required]
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨箱號位置
		/// </summary>
		public int PICK_LOC_NO { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		public string CONTAINER_CODE { get; set; }
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
		/// 下一站類型(2: 集貨場、3: 包裝站)
		/// </summary>
		[Required]
		public string NEXT_STEP { get; set; }
		/// <summary>
		/// 集貨場編號
		/// </summary>
		public string COLLECTION_CODE { get; set; }
		/// <summary>
		/// 狀態(0: 待處理(揀貨批次寫入) 1:開始分貨、2: 分貨完成、3: 缺貨)
		/// </summary>
		[Required]
		public string STATUS { get; set; }
		/// <summary>
		/// 建立日期(開始分貨)
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
		/// 異動日期(分貨完成)
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動日期(分貨完成)
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
