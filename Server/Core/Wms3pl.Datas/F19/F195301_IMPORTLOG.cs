namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 工作群組程式設定匯入紀錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("LOG_ID")]
	[Table("F195301_IMPORTLOG")]
	public class F195301_IMPORTLOG
	{

		/// <summary>
		/// 紀錄ID
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal LOG_ID { get; set; }

		/// <summary>
		/// 工作群組名稱
		/// </summary>
		[Column(TypeName = "nvarchar(30)")]
		public string GRP_NAME { get; set; }

		/// <summary>
		/// 功能編號
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string FUN_CODE { get; set; }

		/// <summary>
		/// 工作群組是否顯示個資
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string SHOWINFO { get; set; }

		/// <summary>
		/// 錯誤訊息
		/// </summary>
		[Column(TypeName = "nvarchar(1000)")]
		public string ERRMSG { get; set; }

		/// <summary>
		/// 是否匯入成功
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }
	}
}
