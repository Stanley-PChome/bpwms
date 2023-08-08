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
    [Table("F060207")]
    public class F060207 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }

		/// <summary>
		///物流中心編號(DCCODE)
		/// </summary>
		[Required]
        public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號(OWNERCODE)
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 倉別編號(ZONECODE)
		/// </summary>
		[Required]
		public string WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 容器編號(周轉箱號)
		/// </summary
		[Required]
		public string CONTAINERCODE { get; set; }

		/// <summary>
		/// 工作站人員(工號)
		/// </summary>
		[Required]
		public string OPERATOR { get; set; }

		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WORKSTATION_NO { get; set; }

		/// <summary>
		/// 播種格口編號
		/// </summary>
		public string SEED_BINCODE { get; set; }

		/// <summary>
		/// 明細筆數
		/// </summary>
		[Required]
		public int SKUTOTAL { get; set; }

		/// <summary>
		/// 處理狀態(0:待處理;1:成功;2:失敗;3:不處理)
		/// </summary>
		[Required]
		public string STATUS { get; set; }

		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string MSG_CONTENT { get; set; }
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
