﻿namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 跨庫調撥揀貨單與揀貨容器綁定
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F0534")]
	public class F0534 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

		/// <summary>
		/// 容器綁定流水號
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
		/// 揀貨單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string PICK_ORD_NO { get; set; }

		/// <summary>
		/// 揀貨容器
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }

		/// <summary>
		/// 跨庫目的地
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string MOVE_OUT_TARGET { get; set; }

		/// <summary>
		/// 狀態(0:待處理;1: 已放入並關閉稽核箱待分配 2:已分配並出貨完成)
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
		/// 裝置類型
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string DEVICE_TYPE { get; set; }

    /// <summary>
		/// 總PCS數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int TOTAL { get; set; }
  }
}
