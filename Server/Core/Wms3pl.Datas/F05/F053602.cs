﻿namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 跨庫調撥揀貨容器人員分貨刷讀記錄檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F053602")]
	public class F053602 : IAuditInfo
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
		/// 人員刷讀商品條碼
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(50)")]
    public string SCAN_CODE { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    [Column(TypeName = "int")]
    public int? QTY { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

		/// <summary>
		/// 是否成功(0=否; 1=是)
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string IS_PASS { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string MESSAGE { get; set; }

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
