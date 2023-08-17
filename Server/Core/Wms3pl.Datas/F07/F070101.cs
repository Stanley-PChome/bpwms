namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 容器頭檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F070101")]
	public class F070101 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

		/// <summary>
		/// F0701的流水ID
		/// </summary>
		[Required]
    [Column(TypeName = "bigint")]
    public long F0701_ID { get; set; }

		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

		/// <summary>
		/// 倉庫代碼
		/// </summary>
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 單據編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 單據類型(所有WMS單據都可以)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string WMS_TYPE { get; set; }

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

    /// <summary>
    /// 揀貨單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PICK_ORD_NO { get; set; }

    }
}
