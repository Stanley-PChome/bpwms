namespace Wms3pl.Datas.F05
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 原始出貨訂單關聯服務型商品資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ORD_NO", "ORD_SEQ", "SERVICE_ITEM_CODE")]
    [Table("F050104")]
    public class F050104 : IAuditInfo
    {

        /// <summary>
        /// 訂單編號
        /// </summary>
        [Key]
        [Required]
        public string ORD_NO { get; set; }

        /// <summary>
        /// 訂單明細序號
        /// </summary>
        [Key]
        [Required]
        public string ORD_SEQ { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        [Key]
        [Required]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 業主編號
        /// </summary>
        [Key]
        [Required]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        [Key]
        [Required]
        public string CUST_CODE { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// 服務型商品品號
        /// </summary>
        [Key]
        [Required]
        public string SERVICE_ITEM_CODE { get; set; }

        /// <summary>
        /// 服務型商品品名
        /// </summary>
        public string SERVICE_ITEM_NAME { get; set; }

        /// <summary>
        /// 狀態(0: 正常 9:取消)
        /// </summary>
        [Required]
        public string STATUS { get; set; }

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
