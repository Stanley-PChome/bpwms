namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 配送商主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ALL_ID", "DC_CODE")]
	[Table("F1947")]
	public class F1947 : IAuditInfo
	{

		/// <summary>
		/// 配送商編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(10)")]
		public string ALL_ID { get; set; }

		/// <summary>
		/// 配送商名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(30)")]
		public string ALL_COMP { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

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

		/// <summary>
		/// 配送碼頭
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string PIER_CODE { get; set; }

		/// <summary>
		/// 配送路線取得URL
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string DELIVERYROUTE_URL { get; set; }

		/// <summary>
		/// 託運單報表檔名(不含副檔名)
		/// </summary>
		[Column(TypeName = "varchar(30)")]
		public string CONSIGN_REPORT { get; set; }

		/// <summary>
		/// 結算日(2碼數字)
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string ACC_KIND { get; set; }

		/// <summary>
		/// 擴增預留欄位A
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_A { get; set; }

		/// <summary>
		/// 擴增預留欄位B
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_B { get; set; }

		/// <summary>
		/// 擴增預留欄位C
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_C { get; set; }

		/// <summary>
		/// 擴增預留欄位D
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_D { get; set; }

		/// <summary>
		/// 擴增預留欄位E
		/// </summary>
		[Column(TypeName = "varchar(50)")]
		public string EXTENSION_E { get; set; }

		/// <summary>
		/// 是否檢查配送路線F194705
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string CHECK_ROUTE { get; set; }

		/// <summary>
		/// 配送商類別 0:一般配送商,1:超取配送商
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string TYPE { get; set; }

		/// <summary>
		/// 允許來回件(0:否;1:是)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ALLOW_ROUND_PIECE { get; set; }
	}
}
