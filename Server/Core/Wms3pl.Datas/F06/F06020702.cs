namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 出貨明細人員明細紀錄資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F06020702")]
    public class F06020702 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }

		/// <summary>
		/// F060207流水號
		/// </summary>
		[Required]
		public long F060207_ID { get; set; }

		/// <summary>
		/// 貨架編號
		/// </summary>
		public string SHELF_CODE { get; set; }

		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BIN_CODE { get; set; }

		/// <summary>
		/// 揀出數量
		/// </summary>
		[Required]
		public int SKUQTY { get; set; }

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
