namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;
    /// <summary>
    /// 儲位異常回報
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F060801")]
    public class F060801 : IAuditInfo
    {
        public Int64 ID { get; set; }
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DC_CODE { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GUP_CODE { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CUST_CODE { get; set; }
        /// <summary>
        /// 倉庫代碼
        /// </summary>
        public string WAREHOUSE_ID { get; set; }
        /// <summary>
        /// 異常類型(1=揀缺, 2=商品錯誤)
        /// </summary>
        public int ABNORMALTYPE { get; set; }
        /// <summary>
        /// 貨架編號
        /// </summary>
        public string SHELFCODE { get; set; }
        /// <summary>
        /// 儲位編號
        /// </summary>
        public string BINCODE { get; set; }
        /// <summary>
        /// 異常任務單號
        /// </summary>
        public string ORDERCODE { get; set; }
        /// <summary>
        /// 異常品號
        /// </summary>
        public string SKUCODE { get; set; }
        /// <summary>
        /// 異常數量
        /// </summary>
        public int SKUQTY { get; set; }
        /// <summary>
        /// 作業人員編號
        /// </summary>
        public string OPERATOR { get; set; }
        /// <summary>
        /// 建立人員
        /// </summary>
        public string CRT_STAFF { get; set; }
        /// <summary>
        /// 建立人名
        /// </summary>
        public string CRT_NAME { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CRT_DATE { get; set; }
        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }
        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }
        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }
    }
}
