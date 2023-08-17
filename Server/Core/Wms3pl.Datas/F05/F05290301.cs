﻿namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	[Serializable]
	[DataServiceKey("PICK_ORD_NO", "PICK_ORD_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	[Table("F05290301")]
	public class F05290301 : IAuditInfo
	{
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 檢貨單序號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string PICK_ORD_SEQ { get; set; }
    /// <summary>
    /// 揀貨箱號位置
    /// </summary>
    [Column(TypeName = "int")]
    public int PICK_LOC_NO { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 出貨單序號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string WMS_ORD_SEQ { get; set; }
    /// <summary>
    /// 容器編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CONTAINER_CODE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }
		/// <summary>
		/// 預計播種數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int B_SET_QTY { get; set; }
    /// <summary>
    /// 實際播種數量
    /// </summary>
    [Column(TypeName = "int")]
    public int A_SET_QTY { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 建立日期(開始分貨)
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
    /// 異動日期(分貨完成)
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// 異動日期(分貨完成)
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
