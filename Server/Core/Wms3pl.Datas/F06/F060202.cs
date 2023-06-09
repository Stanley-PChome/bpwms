namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 出庫任務完成結果回傳
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F060202")]
    public class F060202 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }
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
        /// 倉庫代碼
        /// </summary>
        [Required]
        public string WAREHOUSE_ID { get; set; }

        /// <summary>
        /// 原任務單號
        /// </summary>
        [Required]
        public string DOC_ID { get; set; }

        /// <summary>
        /// 原WMS單號
        /// </summary>
        [Required]
        public string WMS_NO { get; set; }

        /// <summary>
        /// 原揀貨單號
        /// </summary>
        [Required]
        public string PICK_NO { get; set; }

        /// <summary>
        /// 狀態 0: 待處理 1:處理中 2:完成  9:取消
        /// </summary>
        [Required]
        public string STATUS { get; set; }

        /// <summary>
        /// 傳送時間
        /// </summary>
        public DateTime? PROC_DATE { get; set; }

        /// <summary>
        /// 回傳單據狀態 中介層回傳之出庫單狀態(3:揀貨完成 9:取消)
        /// </summary>
        [Required]
        public string M_STATUS { get; set; }

        /// <summary>
        /// 揀貨開始時間(yyyy/MM/dd HH:mm:ss)
        /// </summary>
        [Required]
        public string STARTTIME { get; set; }

        /// <summary>
        /// 揀貨完成時間(yyyy/MM/dd HH:mm:ss)
        /// </summary>
        [Required]
        public string COMPLETETIME { get; set; }

        /// <summary>
        /// 登錄人員=作業人員
        /// </summary>
        [Required]
        public string OPERATOR { get; set; }

        /// <summary>
        /// 是否異常(0:正常  1:異常)
        /// </summary>
        [Required]
        public int ISEXCEPTION { get; set; }

        /// <summary>
        /// 明細數
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
