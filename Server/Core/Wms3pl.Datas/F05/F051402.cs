namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;

	[Serializable]
	[DataServiceKey("ID")]
	[Table("F051402")]
	public class F051402
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public int ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 集貨場編號
		/// </summary>
		[Required]
		public string COLLECTION_CODE { get; set; }
		/// <summary>
		/// 集貨格編號
		/// </summary>
		[Required]
		public string CELL_CODE { get; set; }
		/// <summary>
		/// 容器條碼
		/// </summary>
		[Required]
		public string CONTAINER_CODE { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 動作狀態(0: 第一箱容器放入 1: 第一箱容器取出 2: 放入後即釋放)
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
        /// <summary>
        /// 集貨場名稱
        /// </summary>
        public string COLLECTION_NAME { get; set; }

    }
}
