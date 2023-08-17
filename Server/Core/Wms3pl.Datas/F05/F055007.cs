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
	[Table("F055007")]
	public class F055007 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
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
        /// 出貨單號
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_ORD_NO { get; set; }
        /// <summary>
        /// 出貨箱序
        /// </summary>
        [Required]
    [Column(TypeName = "smallint")]
    public Int16 PACKAGE_BOX_NO { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string CUST_ORD_NO { get; set; }
        /// <summary>
        /// 報表編號(01=箱明細, 02=一般出貨小白標 ,03=廠退出貨小白標, 04=LMS 檔案)
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(2)")]
    public string REPORT_CODE { get; set; }
    /// <summary>
    /// 報表名稱
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string LMS_NAME { get; set; }
    /// <summary>
    /// LMS提供的URL
    /// </summary>
    [Column(TypeName = "varchar(300)")]
    public string LMS_URL { get; set; }
        /// <summary>
        /// 印表機編號(1=印表機1;2=印表機2;3=快速標籤機
        /// </summary>
        [Required]
    [Column(TypeName = "char(1)")]
    public string PRINTER_NO { get; set; }
        /// <summary>
        /// 檔案項次
        /// </summary>
        [Required]
    [Column(TypeName = "int")]
    public int REPORT_SEQ { get; set; }
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
    /// 是否列印(0:否 1:是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ISPRINTED { get; set; }
    /// <summary>
    /// 列印時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PRINT_TIME { get; set; }

  }
}
