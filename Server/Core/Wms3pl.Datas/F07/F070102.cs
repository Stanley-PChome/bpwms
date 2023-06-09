namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 容器身檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F070102")]
	public class F070102 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }

		/// <summary>
		/// F0701的流水ID
		/// </summary>
		[Required]
		public long F070101_ID { get; set; }

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
		/// 品號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 效期
		/// </summary>
		public DateTime? VALID_DATE { get; set; }

		/// <summary>
		/// 批號
		/// </summary>
		public string MAKE_NO { get; set; }

		/// <summary>
		/// 單據編號
		/// </summary>
		[Required]
		public int QTY { get; set; }

		/// <summary>
		/// 序號
		/// </summary>
		public string SERIAL_NO_LIST { get; set; }

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

        /// <summary>
        /// 容器分隔編號
        /// </summary>
        public string BIN_CODE { get; set; }

        /// <summary>
        /// 原F070101流水號
        /// </summary>
        public long ORG_F070101_ID { get; set; }

        /// <summary>
        /// 揀貨單號
        /// </summary>
        public string PICK_ORD_NO { get; set; }

    }
}
