namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	///  檔案路徑設定檔
	/// </summary>
	[Serializable]
	[DataServiceKey("PATH_NO")]
	[Table("F190907")]
	public class F190907 : IAuditInfo
	{

		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		[Column(TypeName = "decimal(18,0)")]
		public Decimal PATH_NO { get; set; }

		/// <summary>
		/// 檔案類型
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(40)")]
		public string PATH_KEY { get; set; }

		/// <summary>
		/// 檔案名稱
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string PATH_NAME { get; set; }

		/// <summary>
		/// 檔案路徑
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(500)")]
		public string PATH_ROOT { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(3)")]
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(2)")]
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(6)")]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2(0)")]
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		[Column(TypeName = "varchar(20)")]
		public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		[Column(TypeName = "nvarchar(16)")]
		public string CRT_NAME { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		[Column(TypeName = "datetime2(0)")]
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		[Column(TypeName = "nvarchar(20)")]
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		[Column(TypeName = "nvarchar(16)")]
		public string UPD_NAME { get; set; }

		/// <summary>
		/// 備註
		/// </summary>
		[Column(TypeName = "nvarchar(100)")]
		public string NOTE { get; set; }

		/// <summary>
		/// 網路磁碟路徑
		/// </summary>
		[Column(TypeName = "nvarchar(200)")]
		public string NETWORKPATH { get; set; }

		/// <summary>
		/// 網路磁碟帳號
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string NETWORKACCOUNT { get; set; }

		/// <summary>
		/// 網路磁碟密碼
		/// </summary>
		[Column(TypeName = "nvarchar(50)")]
		public string NETWORKPASSWORD { get; set; }

		/// <summary>
		/// 存放檔案類型(0:本機檔案;1:網路磁碟機)
		/// </summary>
		[Required]
		[Column(TypeName = "char(1)")]
		public string FILESOURCE_TYPE { get; set; }
	}
}
