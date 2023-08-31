namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 程式清單主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("FUN_CODE")]
	[Table("F1954")]
	public class F1954 : IAuditInfo
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
		[Column(TypeName = "nvarchar(30)")]
		public string FUN_NAME { get; set; }

		/// <summary>
		/// 程式類型
		/// </summary>
		[Column(TypeName = "char(1)")]
		public string FUN_TYPE { get; set; }

		/// <summary>
		/// 程式說明
		/// </summary>
		[Column(TypeName = "nvarchar(120)")]
		public string FUN_DESC { get; set; }

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
		/// 目前狀態(0待授權,1已授權)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string STATUS { get; set; }

		/// <summary>
		/// 上傳檔案日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPLOAD_DATE { get; set; }

		/// <summary>
		/// 是否停用
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string DISABLE { get; set; }

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

		/// <summary>
		/// 主功能選單顯示
		/// </summary>
		[Column(TypeName = "varchar(1)")]
		public string MAIN_SHOW { get; set; }

		/// <summary>
		/// 側邊選單顯示
		/// </summary>
		[Column(TypeName = "varchar(1)")]
		public string SIDE_SHOW { get; set; }
	}
}
