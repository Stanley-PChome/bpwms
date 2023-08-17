namespace Wms3pl.Datas.F06
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 盤點完成結果回傳
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F060402")]
	public class F060402 : IAuditInfo
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
    [Column(TypeName = "varchar(20)")]
    public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
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
		/// 任務單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string DOC_ID { get; set; }
		/// <summary>
		/// 原WMS單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }
		/// <summary>
		/// 狀態 0: 待處理 1:處理中 2:完成  9:取消
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
    /// <summary>
    /// 執行時間(YYYY/MM/DD hh:mi:ss)
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PROC_DATE { get; set; }
		/// <summary>
		/// 回傳單據狀態中介層回傳之出庫單狀態(3:盤點完成 9:取消)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string M_STATUS { get; set; }
		/// <summary>
		/// 盤點開始時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(19)")]
    public string STARTTIME { get; set; }
		/// <summary>
		/// 盤點完成時間(yyyy/MM/dd HH:mm:ss)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(19)")]
    public string COMPLETETIME { get; set; }
		/// <summary>
		/// 登錄人員=作業人員
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string OPERATOR { get; set; }
		/// <summary>
		/// 明細數
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
		public int SKUTOTAL { get; set; }
	
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
    public string CRT_STAFF { get; set; }/// <summary>
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
    public string UPD_STAFF { get; set; }/// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
	}
}
