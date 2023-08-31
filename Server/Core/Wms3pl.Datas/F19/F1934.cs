namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 鄉鎮行政區域檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ZIP_CODE")]
	[Table("F1934")]
	public class F1934 : IAuditInfo
	{

		/// <summary>
		/// 鄉鎮區域編號(郵遞區號)
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(5)")]
		public string ZIP_CODE { get; set; }

		/// <summary>
		/// 鄉鎮區域名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(40)")]
		public string ZIP_NAME { get; set; }

		/// <summary>
		/// 縣市行政區編號F1933
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string COUDIV_ID { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立時間
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
		/// 異動時間
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
