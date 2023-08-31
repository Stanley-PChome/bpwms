namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 板箱標籤記錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("SEQ_NO")]
	[Table("F19700201")]
	public class F19700201 : IAuditInfo
	{

		/// <summary>
		/// 序號
		/// </summary>
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal SEQ_NO { get; set; }

		/// <summary>
		/// 年度
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(4)")]
		public string YEAR { get; set; }

		/// <summary>
		/// 箱板號起
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(13)")]
		public string START_NO { get; set; }

		/// <summary>
		/// 箱板號迄
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(13)")]
		public string END_NO { get; set; }

		/// <summary>
		/// 列印類型(0:板標 1:箱標)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string LABEL_TYPE { get; set; }

		/// <summary>
		/// 列印張數
		/// </summary>
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal QTY { get; set; }

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
