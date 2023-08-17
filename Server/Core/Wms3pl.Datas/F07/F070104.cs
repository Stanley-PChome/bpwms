namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 混和型容器明細檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F070104")]
	public class F070104 : IAuditInfo
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
		/// 業主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }

		/// <summary>
		/// 品號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

		/// <summary>
		/// 效期
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SOURCE_NO { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string ITEM_SEQ { get; set; }

		/// <summary>
		/// 效期
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? VALID_DATE { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }

		/// <summary>
		/// 效期
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int QTY { get; set; }

    /// <summary>
    /// 效期，2023/3/24 預計將此欄位廢止，改用F07010401
    /// </summary>
    [Column(TypeName = "varchar(MAX)")]
    public string SERIAL_NO_LIST { get; set; }

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
    /// 狀態(0:待處理 1:驗收完成 2:上架完成)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
	}
}
