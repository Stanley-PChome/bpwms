namespace Wms3pl.Datas.F01
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    [Serializable]
    [DataServiceKey("ID")]
    [Table("F010301_HISTORY")]
    public class F010301_HISTORY : IAuditInfo
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
        /// 收貨日期
        /// </summary>
        [Required]
        public DateTime? RECV_DATE { get; set; }

        /// <summary>
        /// 收貨時間
        /// </summary>
        [Required]
        public string RECV_TIME { get; set; }

        /// <summary>
        /// 收貨人員帳號
        /// </summary>
        [Required]
        public string RECV_USER { get; set; }

        /// <summary>
        /// 收貨人員名稱
        /// </summary>
        public string RECV_NAME { get; set; }

        /// <summary>
        /// 貨運單號
        /// </summary>
        [Required]
        public string SHIP_ORD_NO { get; set; }

        /// <summary>
        /// 箱數
        /// </summary>
        [Required]
        public Int16 BOX_CNT { get; set; }

        /// <summary>
        /// 簽單核對狀態(0:未核對;1:已核對)
        /// </summary>
        [Required]
        public string CHECK_STATUS { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string MEMO { get; set; }

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
