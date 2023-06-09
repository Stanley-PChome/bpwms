using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F0006")]
    public class F0006 : IAuditInfo
    {
        [Key]
        [Required]
        public Int64 ID { get; set; }
        /// <summary>
        /// 物流中心編號
        /// </summary>
        [Required]
        public string DC_CODE { get; set; }
        /// <summary>
        /// 介接系統編號 (0: LMS系統、1: WCS系統、2: WCSPR系統、3: PDA系統)
        /// </summary>
        [Required]
        public string EXTERNAL_SYSTEM { get; set; }
        /// <summary>
        /// 顯示名稱(非程式名稱，方便人員看得懂的名稱)
        /// </summary>
        [Required]
        public string SHOW_NAME { get; set; }
        /// <summary>
        /// 排程名稱
        /// </summary>
        public string PROG_NAME { get; set; }
        /// <summary>
        /// 排程資料表
        /// </summary>
        public string PROG_LOG_TABLE { get; set; }
        /// <summary>
        /// API名稱
        /// </summary>
        public string API_NAME { get; set; }
        /// <summary>
        /// API資料表
        /// </summary>
        public string API_LOG_TABLE { get; set; }
        /// <summary>
        /// 功能說明
        /// </summary>
        public string MEMO { get; set; }
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
    /// 篩選規則
    /// </summary>
    public string FILTER_RULE { get; set; }
    }
}
