namespace Wms3pl.Datas.F05
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 配庫試算-訂單預計揀貨倉別
    /// </summary>
    [Serializable]
    [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "CAL_NO", "ORD_NO")]
    [Table("F05080505")]
    public class F05080505 : IAuditInfo
    {
        /// <summary>
        /// 物流中心
        /// </summary>
        [Key]
        [Required]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 業主
        /// </summary>
        [Key]
        [Required]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主
        /// </summary>
        [Key]
        [Required]
        public string CUST_CODE { get; set; }

        /// <summary>
        /// 試算編號
        /// </summary>
        [Key]
        [Required]
        public string CAL_NO { get; set; }
        /// <summary>
        /// 訂單單號
        /// </summary>
        [Key]
        [Required]
        public string ORD_NO { get; set; }
        /// <summary>
        /// 貨主單號
        /// </summary>
        public string CUST_ORD_NO { get; set; }
        /// <summary>
        /// 貨主自訂分類
        /// </summary>
        public string CUST_COST { get; set; }
        /// <summary>
        /// 優先處理旗標
        /// </summary>
        public string FAST_DEAL_TYPE { get; set; }
        /// <summary>
        /// 跨庫目的地
        /// </summary>
        public string MOVE_OUT_TARGET { get; set; }
        /// <summary>
        /// 預計揀貨倉別名稱清單(用逗點分隔)
        /// </summary>
        public string WAREHOUSE_INFO { get; set; }
        /// <summary>
        /// 是否缺貨訂單
        /// </summary>
        [Required]
        public string IS_LACK_ORDER { get; set; }
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
