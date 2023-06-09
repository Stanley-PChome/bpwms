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
		public Int64 ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        [Required]
        public string GUP_CODE { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        [Required]
		public string CUST_CODE { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        [Required]
        public string WMS_ORD_NO { get; set; }
        /// <summary>
        /// 出貨箱序
        /// </summary>
        [Required]
		public Int16 PACKAGE_BOX_NO { get; set; }
        /// <summary>
        /// 貨主單號
        /// </summary>
        public string CUST_ORD_NO { get; set; }
        /// <summary>
        /// 報表編號(01=箱明細, 02=一般出貨小白標 ,03=廠退出貨小白標, 04=LMS 檔案)
        /// </summary>
        [Required]
        public string REPORT_CODE { get; set; }
        /// <summary>
        /// 報表名稱
        /// </summary>
        public string LMS_NAME { get; set; }
        /// <summary>
        /// LMS提供的URL
        /// </summary>
        public string LMS_URL { get; set; }
        /// <summary>
        /// 印表機編號(1=印表機1;2=印表機2;3=快速標籤機
        /// </summary>
        [Required]
        public string PRINTER_NO { get; set; }
        /// <summary>
        /// 檔案項次
        /// </summary>
        [Required]
        public int REPORT_SEQ { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員編號
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
    /// <summary>
    /// 是否列印(0:否 1:是)
    /// </summary>
    public string ISPRINTED { get; set; }
    /// <summary>
    /// 列印時間
    /// </summary>
    public DateTime? PRINT_TIME { get; set; }

  }
}
