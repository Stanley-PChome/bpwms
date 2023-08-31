namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 人員密碼修改歷史紀錄
	/// </summary>
	[Serializable]
	[DataServiceKey("EMP_ID", "CRT_DATE")]
	[Table("F1952_HISTORY")]
	public class F1952_HISTORY
	{

		/// <summary>
		/// 員工編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(16)")]
		public string EMP_ID { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 登入密碼
		/// </summary>
		[Column(TypeName = "varchar(48)")]
		public string PASSWORD { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }
	}
}
