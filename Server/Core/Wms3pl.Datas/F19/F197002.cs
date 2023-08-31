namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 板箱標籤主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("YEAR")]
	[Table("F197002")]
	public class F197002 : IAuditInfo
	{

		/// <summary>
		/// 年度
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string YEAR { get; set; }

		/// <summary>
		/// 箱號
		/// </summary>
		[Column(TypeName = "varchar(12)")]
		public string BOX_NO { get; set; }

		/// <summary>
		/// 板號
		/// </summary>
		[Column(TypeName = "varchar(10)")]
		public string PALLET_NO { get; set; }

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
		/// 異動人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

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
