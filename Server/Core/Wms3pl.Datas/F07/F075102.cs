namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出貨單匯入控管紀錄表
	/// </summary>
	[Serializable]
	[DataServiceKey("CUST_CODE", "CUST_ORD_NO")]
	[Table("F075102")]
	public class F075102 : IAuditInfo
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CUST_CODE { get; set; }

		/// <summary>
		/// 貨主單號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(50)")]
    public string CUST_ORD_NO { get; set; }

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
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
