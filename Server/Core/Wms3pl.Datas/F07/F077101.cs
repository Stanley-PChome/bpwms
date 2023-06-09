namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 進倉人員作業紀錄
    /// </summary>
    [Serializable]
	[DataServiceKey("ID")]
	[Table("F077101")]
	public class F077101 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }

        /// <summary>
        /// 人員帳號
        /// </summary>
        [Required]
		public string EMP_ID { get; set; }

        /// <summary>
        /// 作業功能(1: 商品複驗、2: 上架移動)
        /// </summary>
        [Required]
		public string WORK_TYPE { get; set; }

        /// <summary>
        /// 參考資料表的ID
        /// 當 work_type=1，ref_id= F020502.ID
        /// 當 work_type=2，ref_id= F020501.ID
        /// </summary>
        [Required]
        public long REF_ID { get; set; }

        /// <summary>
        /// 工作站號碼
        /// </summary>
        public string WORKSTATION_CODE { get; set; }

        /// <summary>
        /// 作業時間
        /// </summary>
        public DateTime? WORKING_TIME { get; set; }

        /// <summary>
        /// 狀態(0: 開始、1: 完成)
        /// </summary>
        [Required]
        public string STATUS { get; set; }

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
