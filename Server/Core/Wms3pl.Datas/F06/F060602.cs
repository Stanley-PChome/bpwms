namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 下位系統的庫存原始明細資料
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060602")]
	public class F060602 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
		/// <summary>
		/// F060601的流水ID
		/// </summary>
		[Required]
    [Column(TypeName = "bigint")]
    public long F060601_ID { get; set; }
		/// <summary>
		/// 倉庫代碼
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }
    /// <summary>
    /// 貨架編號
    /// </summary>
    [Column(TypeName = "varchar(16)")]
    public string SHELF_CODE { get; set; }
    /// <summary>
    /// 儲位編號
    /// </summary>
    [Column(TypeName = "varchar(24)")]
    public string BIN_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string CUST_CODE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }
		/// <summary>
		/// 商品等級(0=殘品、1=正品)
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int SKU_LEVLE { get; set; }
		/// <summary>
		/// 效期(yyyy/mm/dd)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string VALID_DATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int QTY { get; set; }
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
