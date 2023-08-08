﻿namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出庫容器資料檔
	/// </summary>
	[Serializable]
    [DataServiceKey("ID")]
    [Table("F060205")]
    public class F060205 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 任務單號
        /// </summary>
        [Required]
        public string DOC_ID { get; set; }

        /// <summary>
        /// 容器編號
        /// </summary>
        public string CONTAINERCODE { get; set; }

        /// <summary>
        /// 人員帳號
        /// </summary>
        public string OPERATOR { get; set; }

        /// <summary>
        /// 工作站編號
        /// </summary>
        public string WORKSTATIONNO { get; set; }

        /// <summary>
        /// 播種牆格口號
        /// </summary>
        public string SEEDBINCODE { get; set; }

        /// <summary>
        /// 品項數
        /// </summary>
        [Required]
        public int SKUTOTAL { get; set; }
        
        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [Required]
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }
    }
}
