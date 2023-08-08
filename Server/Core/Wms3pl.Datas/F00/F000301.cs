using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    /// <summary>
    /// 系統設定檔預設資料
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F000301")]
    public class F000301 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 類型(1: DC共用 2:貨主共用)
        /// </summary>
        [Required]
        public string SYS_TYPE { get; set; }

        /// <summary>
        /// 設定名稱
        /// </summary>
        [Required]
        public string AP_NAME { get; set; }

        /// <summary>
        /// 設定值
        /// </summary>
        [Required]
        public string SYS_PATH { get; set; }

        /// <summary>
        /// 設定描述
        /// </summary>
        [Required]
        public string DESCRIPT { get; set; }

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
    }
}
