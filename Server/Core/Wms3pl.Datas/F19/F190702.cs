namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 查詢轉檔語法設定
	/// </summary>
	[Serializable]
	[DataServiceKey("FUN_ID")]
	[Table("F190702")]
	public class F190702 : IAuditInfo
	{

		/// <summary>
		/// 查詢設定檔編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "int")]
		public Int32 FUN_ID { get; set; }

		/// <summary>
		/// 查詢設定檔名稱
		/// </summary>
		[Column(TypeName = "nvarchar(40)")]
		public string FUN_NAME { get; set; }

		/// <summary>
		/// 查詢設定檔描述
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string FUN_DESCRIPT { get; set; }

		/// <summary>
		/// 查詢設定檔參數數量
		/// </summary>
		[Column(TypeName = "smallint")]
		public Int16? FUN_PARM_NUM { get; set; }

		/// <summary>
		/// 查詢設定檔參數清單
		/// </summary>
		[Column(TypeName = "varchar(700)")]
		public string FUN_PARM_LIST { get; set; }

		/// <summary>
		/// 查詢語法(外部參數以  '#參數名稱#' 包覆)
		/// </summary>
		[Column(TypeName = "varchar(MAX)")]
		public string FUN_SQL { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 建立時間
		/// </summary>
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
		/// 異動時間
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }
	}
}
