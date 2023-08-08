namespace Wms3pl.Datas.F02
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 驗收分播檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F0205")]
    public class F0205: IAuditInfo
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
        /// 進倉單號
        /// </summary>
        public string STOCK_NO { get; set; }
        /// <summary>
        /// 進倉項次
        /// </summary>
        public string STOCK_SEQ { get; set; }
        /// <summary>
        /// 驗收單號
        /// </summary>
        [Required]
        public string RT_NO { get; set; }
        /// <summary>
        /// 驗收序號
        /// </summary>
        [Required]
        public string RT_SEQ { get; set; }
        /// <summary>
        /// 上架倉別
        /// </summary>
        [Required]
        public string PICK_WARE_ID { get; set; }
        /// <summary>
        /// 上架區域(A:揀區 C:補區 R:不良品區)
        /// </summary>
        [Required]
        public string TYPE_CODE { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [Required]
        public string ITEM_CODE { get; set; }
        /// <summary>
        /// 是否需要複驗(0:否 1:是)
        /// </summary>
        [Required]
        public int NEED_DOUBLE_CHECK { get; set; }
        /// <summary>
        /// 預計分播數
        /// </summary>
        [Required]
        public Int32 B_QTY { get; set; }
        /// <summary>
        /// 實際分播數
        /// </summary>
        public Int32? A_QTY { get; set; }
        /// <summary>
        /// 狀態(0: 開箱、1: 已關箱待複驗、2: 可上架、3: 不可上架、4: 上架移動中、5: 移動完成、6:上架完成、7:已關箱待產生調撥單)
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
