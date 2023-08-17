namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 容器釋放清單
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060302")]
	public class F060302 : IAuditInfo
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
		/// 物流中心編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CUST_CODE { get; set; }
		/// <summary>
		/// 倉庫代碼
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 容器編碼
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }

		/// <summary>
		/// 狀態 0: 待處理 1:處理中 2:完成  F:處理失敗 T: TimeOut 9:取消
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 傳送時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PROC_DATE { get; set; }

    /// <summary>
    /// 已派送次數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int RESENT_CNT { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [Column(TypeName = "varchar(80)")]
    public string MESSAGE { get; set; }

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
