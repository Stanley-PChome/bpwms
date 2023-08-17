using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F055005")]
	public class F055005: IAuditInfo
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
    [Column(TypeName = "varchar(50)")]
    public string PAST_NO { get; set; }

		/// <summary>
		/// 單據編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 明細資料處理狀態(0:待處理 1:已處理)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string PROC_FLAG { get; set; }

    /// <summary>
    /// 紀錄轉出時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? TRANS_DATE { get; set; }
		
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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

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
	}
}
