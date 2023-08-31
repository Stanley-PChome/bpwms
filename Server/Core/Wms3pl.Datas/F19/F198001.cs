namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 倉別型態主檔
	/// </summary>
	[Serializable]
	[DataServiceKey("TYPE_ID")]
	[Table("F198001")]
	public class F198001 : IAuditInfo
	{

		/// <summary>
		/// 倉別型態編號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "varchar(8)")]
		public string TYPE_ID { get; set; }

		/// <summary>
		/// 倉別型態名稱
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(80)")]
		public string TYPE_NAME { get; set; }

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
		/// 是否計算材積
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string CALVOLUMN { get; set; }

		/// <summary>
		/// 商品主檔維護的上架倉別是否顯示
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string ITEM_PICK_WARE { get; set; }

		/// <summary>
		/// 儲位是否必須放入相同的貨主商品
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string LOC_MUSTSAME_NOWCUSTCODE { get; set; }
	}
}
