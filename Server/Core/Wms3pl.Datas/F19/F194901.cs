namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 車行主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("CARRIAGE_ID")]
	[Table("F194901")]
	public class F194901 : IAuditInfo
	{

		/// <summary>
		/// 車行編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CARRIAGE_ID { get; set; }

		/// <summary>
		/// 車行名稱
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string CARRIAGE_NAME { get; set; }

		/// <summary>
		/// 使用狀態(0: 使用中 1:不使用)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string USAGE { get; set; }

		/// <summary>
		/// 建立人員/盤點人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名/盤點人名
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 建立日期/開始盤點時間
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }
	}
}
