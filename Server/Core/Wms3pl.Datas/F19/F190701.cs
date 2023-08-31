namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 查詢轉檔設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("QID")]
	[Table("F190701")]
	public class F190701 : IAuditInfo
	{

		/// <summary>
		/// 查詢編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal QID { get; set; }

		/// <summary>
		/// 查詢名稱
		/// </summary>
		[Column(TypeName = "nvarchar(40)")]
		public string NAME { get; set; }

		/// <summary>
		/// 查詢詳細描述
		/// </summary>
		[Column(TypeName = "nvarchar(60)")]
		public string DESCRIPT { get; set; }

		/// <summary>
		/// 查詢語法設定F190702
		/// </summary>
		[Column(TypeName = "int")]
		public Int32? FUN_ID { get; set; }

		/// <summary>
		/// 平均花費時間
		/// </summary>
		[Column(TypeName = "decimal(18,0)")]
		public Decimal? AVG_SPEND_TIME { get; set; }

		/// <summary>
		/// 欄位清單名稱
		/// </summary>
		[Column(TypeName = "nvarchar(600)")]
		public string FIELD_LIST_NAME { get; set; }

		/// <summary>
		/// 類別名稱
		/// </summary>
		[Column(TypeName = "nvarchar(40)")]
		public string CLASS_NAME { get; set; }

		/// <summary>
		/// 查詢群組
		/// </summary>
		[Column(TypeName = "varchar(4)")]
		public string QGROUP { get; set; }

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
