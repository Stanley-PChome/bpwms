namespace Wms3pl.Datas.F02
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 驗收批號的流水號紀錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ITEM_CODE", "RT_DATE")]
	[Table("F020203")]
	public class F020203 : IAuditInfo
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

		/// <summary>
		/// 業主
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

		/// <summary>
		/// 驗收日期
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime RT_DATE { get; set; }

		/// <summary>
		/// 目前已使用流水號
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public Int32 RT_SEQ { get; set; }

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
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
	}
}
