namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;

	[Serializable]
	[DataServiceKey("ID")]
	[Table("F051402")]
	public class F051402
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "int")]
    public int ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
		/// <summary>
		/// 集貨場編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string COLLECTION_CODE { get; set; }
		/// <summary>
		/// 集貨格編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string CELL_CODE { get; set; }
		/// <summary>
		/// 容器條碼
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }
    /// <summary>
    /// 出貨單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 動作狀態(0: 第一箱容器放入 1: 第一箱容器取出 2: 放入後即釋放)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
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
    /// 異動人員編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
    /// <summary>
    /// 業主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 集貨場名稱
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string COLLECTION_NAME { get; set; }

    }
}
