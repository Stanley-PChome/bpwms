using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	/// <summary>
	/// 出貨包裝箱列印報表
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F0501_HISTORY")]
	public class F0501_HISTORY : IAuditInfo
	{
		[Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }
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
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }
		/// <summary>
		/// 紙箱編號
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string IS_LOCK { get; set; }

    /// <summary>
    /// 配庫批次號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ALLOT_BATCH_NO { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔人員名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 建檔日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
  }
}
