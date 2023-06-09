namespace Wms3pl.Datas.F02
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 進倉驗收檔
    /// </summary>
    [Serializable]
    [DataServiceKey("RT_NO", "RT_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
    [Table("F020201")]
    public class F020201 : IAuditInfo
    {

        /// <summary>
        /// 驗收單號碼
        /// </summary>
        [Key]
        [Required]
        public string RT_NO { get; set; }

        /// <summary>
        /// 驗收單序號
        /// </summary>
        [Key]
        [Required]
        public string RT_SEQ { get; set; }

        /// <summary>
        /// 進倉單號F010201
        /// </summary>
        [Required]
        public string PURCHASE_NO { get; set; }

        /// <summary>
        /// 進倉單序號F010202
        /// </summary>
        [Required]
        public string PURCHASE_SEQ { get; set; }

        /// <summary>
        /// 廠編
        /// </summary>
        public string VNR_CODE { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// 驗收日
        /// </summary>
        public DateTime? RECE_DATE { get; set; }

        /// <summary>
        /// 商品有效日期
        /// </summary>
        public DateTime? VALI_DATE { get; set; }

        /// <summary>
        /// 商品製造日期
        /// </summary>
        public DateTime? MADE_DATE { get; set; }

        /// <summary>
        /// 訂貨數
        /// </summary>
        public Int32? ORDER_QTY { get; set; }

        /// <summary>
        /// 驗收數
        /// </summary>
        public Int32? RECV_QTY { get; set; }

        /// <summary>
        /// 抽驗數
        /// </summary>
        public Int32? CHECK_QTY { get; set; }

        /// <summary>
        /// 檢驗狀態(1待上傳,2已上傳,3綁容器)
        /// </summary>
        [Required]
        public string STATUS { get; set; }

        /// <summary>
        /// 已抽驗商品
        /// </summary>
        [Required]
        public string CHECK_ITEM { get; set; }

        /// <summary>
        /// 已抽驗序號
        /// </summary>
        [Required]
        public string CHECK_SERIAL { get; set; }

        /// <summary>
        /// 是否列印箱板標
        /// </summary>
        [Required]
        public string ISPRINT { get; set; }

        /// <summary>
        /// 是否上傳圖檔
        /// </summary>
        [Required]
        public string ISUPLOAD { get; set; }

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
        /// 建檔人員
        /// </summary>
        [Required]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建檔日期
        /// </summary>
        [Required]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [Required]
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }

        /// <summary>
        /// 特採原因
        /// </summary>
        public string SPECIAL_DESC { get; set; }

        /// <summary>
        /// 特採原因編碼
        /// </summary>
        public string SPECIAL_CODE { get; set; }

        /// <summary>
        /// 是否特採
        /// </summary>
        [Required]
        public string ISSPECIAL { get; set; }

        /// <summary>
        /// 到貨時間(年月日時間)
        /// </summary>
        public DateTime? IN_DATE { get; set; }

        /// <summary>
        /// 指定出貨倉別
        /// </summary>
        public string TARWAREHOUSE_ID { get; set; }

        /// <summary>
        /// 快驗
        /// </summary>
        [Required]
        public string QUICK_CHECK { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 驗收模式(0=單據綁定驗收,1=容器綁定驗收)
        /// </summary>
        public string RT_MODE { get; set; }
    }
}
