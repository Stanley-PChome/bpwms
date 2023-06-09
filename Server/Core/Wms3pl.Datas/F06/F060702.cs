namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 入庫任務清單
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F060702")]
    public class F060702 : IAuditInfo
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
        /// 出庫單號(揀貨單號)
        /// </summary>
        [Required]
        public string ORDER_CODE { get; set; }

        /// <summary>
        /// 上游出貨單號(WMS出貨單號)
        /// </summary>
        [Required]
        public string ORI_ORDER_CODE { get; set; }

        /// <summary>
        /// 集貨場狀態 0:到齊就出 1:等待補揀 2:異常處理
        /// </summary>
        [Required]
        public int STATUS { get; set; }

        /// <summary>
        /// 處理狀態 0: 待處理 1:處理中 2:完成  F:處理失敗 T: TimeOut 9:取消
        /// </summary>
        [Required]
        public String PROC_FLAG { get; set; }

        /// <summary>
        /// 傳送時間
        /// </summary>
        [Required]
        public DateTime? PROC_DATE { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public String MESSAGE { get; set; }

        /// <summary>
        /// 已派送次數
        /// </summary>
        public int RESENT_CNT { get; set; }

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
