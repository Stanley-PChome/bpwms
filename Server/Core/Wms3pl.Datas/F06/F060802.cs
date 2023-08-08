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
    [Table("F060802")]
    public class F060802 : IAuditInfo
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
		/// 貨主編號
		/// </summary>
		public string SORTER_CODE { get; set; }
		/// <summary>
		/// 分揀機編號
		/// </summary>
		public int ABNORMAL_TYPE { get; set; }
		/// <summary>
		/// 異常類型(1=讀取失敗, 2=分揀設備異常, 3=流道滿載, 9=其他)
		/// </summary>
		public string ABNORMAL_MSG { get; set; }
		/// <summary>
		/// 異常訊息
		/// </summary>
		public string ABNORMAL_CODE { get; set; }
		/// <summary>
		/// 紀錄時間
		/// </summary>
		public string RECORD_TIME { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員
		/// </summary>
		public string CRT_STAFF { get; set; }
        /// <summary>
        /// 建立人名
        /// </summary>
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
