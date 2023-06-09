using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    /// <summary>
    /// 特殊結構出貨包裝刷讀紀錄
    /// </summary>
    [Serializable]
	[DataServiceKey("ID")]
	[Table("F05500103")]
	public class F05500103 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public Int64 ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
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
        /// 集貨揀貨單號
        /// </summary>
        [Required]
        public string PICK_ORD_NO { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
		public string ITEM_CODE { get; set; }
        /// <summary>
        /// 商品序號
        /// </summary>
        public string SERIAL_NO { get; set; }
        /// <summary>
        /// 人員刷讀商品條碼
        /// </summary>
        public string SCAN_CODE { get; set; }
        /// <summary>
        /// 是否通過
        /// </summary>
        [Required]
        public string ISPASS { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string MESSAGE { get; set; }
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
