namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DataServiceKey("FUN_CODE", "LANG")]
	[Table("F1954_I18N")]
	public class F1954_I18N
	{

		/// <summary>
		/// 程式編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string FUN_CODE { get; set; }

		/// <summary>
		/// 程式名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(300)")]
		public string FUN_NAME { get; set; }

		/// <summary>
		/// 語系
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string LANG { get; set; }

		/// <summary>
		/// 程式說明
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string FUN_DESC { get; set; }
	}
}
