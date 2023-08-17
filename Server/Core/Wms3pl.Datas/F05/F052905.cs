namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	[Serializable]
	[DataServiceKey("ID")]
	[Table("F052905")]
	public class F052905 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
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
		/// 批次日期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime DELV_DATE { get; set; }
    /// <summary>
    /// 批次時段
    /// </summary>
    [Column(TypeName = "varchar(8)")]
    public string PICK_TIME { get; set; }
		/// <summary>
		/// 跨庫調撥的目的地名稱
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string MOVE_OUT_TARGET { get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CONTAINER_CODE { get; set; }
		/// <summary>
		/// 播種類型(01:正常出貨,02:訂單取消)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string SOW_TYPE { get; set; }
		/// <summary>
		/// 容器箱序
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int CONTAINER_SEQ { get; set; }
		/// <summary>
		/// 狀態(0:開箱;1:關箱;2:扣帳)
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

    [Column(TypeName = "varchar(3)")]
    public string PACKAGE_BOX_SEQ { get; set; }
  }
}
