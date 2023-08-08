namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 跨庫出貨容器
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F0532")]
	public class F0532 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

		/// <summary>
		/// 跨庫出貨容器使用中流水號
		/// </summary>
		[Required]
    [Column(TypeName = "bigint")]
    public long F0531_ID { get; set; }

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
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

		/// <summary>
		/// 跨庫的容器編號/取消訂單庫內容器條碼
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string OUT_CONTAINER_CODE { get; set; }

		/// <summary>
		/// 跨庫容器箱序/取消訂單庫內容器箱序
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int OUT_CONTAINER_SEQ { get; set; }

    /// <summary>
    /// 跨庫調撥的目的地名稱
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string MOVE_OUT_TARGET { get; set; }

    /// <summary>
    /// 累計PCS數
    /// </summary>
    [Column(TypeName = "int")]
    public int? TOTAL { get; set; }

		/// <summary>
		/// 作業類型(0: 跨庫訂單整箱出庫、1: 新稽核出庫)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string WORK_TYPE { get; set; }

		/// <summary>
		/// 容器類型(0:正常出貨 1: 取消訂單)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string SOW_TYPE { get; set; }

		/// <summary>
		/// 狀態(0:開箱中1:已關箱 2: 已出貨)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 取消訂單容器綁定ID
    /// </summary>
    [Column(TypeName = "bigint")]
    public long? F0701_ID { get; set; }

    /// <summary>
    /// 關箱人員編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CLOSE_STAFF { get; set; }

    /// <summary>
    /// 關箱人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string CLOSE_NAME { get; set; }

    /// <summary>
    /// 人員關箱時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? CLOSE_DATE { get; set; }

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
	}
}
