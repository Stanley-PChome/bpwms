namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 進倉驗收上架結果表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F050305")]
	public class F050305 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int64 ID { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 訂單單號
		/// </summary>
		public string ORD_NO { get; set; }

		/// <summary>
		/// 單據類別
		/// </summary>
		public string SOURCE_TYPE { get; set; }

		/// <summary>
		/// 來源單號
		/// </summary>
		public string SOURCE_NO { get; set; }

		/// <summary>
		/// 歷程狀態(0: 單據成立、1: 開始揀貨、2: 開始包裝、3: 包裝完成、5: 出貨完成、9: 單據取消)
		/// </summary>
		[Required]
		public string STATUS { get; set; }

		/// <summary>
		/// 明細資料處理狀態(0:待處理 1:已處理)
		/// </summary>
		[Required]
		public string PROC_FLAG { get; set; }

		/// <summary>
		/// 紀錄轉出時間
		/// </summary>
		public DateTime? TRANS_DATE { get; set; }

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
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WORKSTATION_CODE { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WMS_ORD_NO { get; set; }
	}
}
