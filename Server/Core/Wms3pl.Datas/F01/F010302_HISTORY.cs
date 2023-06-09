namespace Wms3pl.Datas.F01
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    [Serializable]
    [DataServiceKey("ID")]
    [Table("F010302_HISTORY")]
    public class F010302_HISTORY : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public Int64 ID { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        [Required]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 物流商編號
        /// </summary>
        [Required]
        public string ALL_ID { get; set; }

        /// <summary>
        /// 刷單核對日期
        /// </summary>
        [Required]
        public DateTime? CHECK_DATE { get; set; }

        /// <summary>
        /// 刷單核對時間
        /// </summary>
        [Required]
        public string CHECK_TIME { get; set; }

        /// <summary>
        /// 刷單人員帳號
        /// </summary>
        [Required]
        public string CHECK_USER { get; set; }

        /// <summary>
        /// 刷單人員名稱
        /// </summary>
        public string CHECK_NAME { get; set; }

        /// <summary>
        /// 貨運單號
        /// </summary>
        [Required]
        public string SHIP_ORD_NO { get; set; }

        /// <summary>
        /// 核對箱數
        /// </summary>
        [Required]
        public Int16 CHECK_BOX_CNT { get; set; }

        /// <summary>
        /// 貨單箱數
        /// </summary>
        [Required]
        public Int16 SHIP_BOX_CNT { get; set; }

        /// <summary>
        /// 核對箱數結果 0:核對失敗;1:核對成功
        /// </summary>
        [Required]
        public string CHECK_STATUS { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [Required]
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }
    }
}
