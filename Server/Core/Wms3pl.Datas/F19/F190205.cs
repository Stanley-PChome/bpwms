namespace Wms3pl.Datas.F19
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 商品類別檢驗項目設定
    /// </summary>
    [Serializable]
    [DataServiceKey("TYPE", "CHECK_TYPE", "CHECK_NO", "GUP_CODE", "CUST_CODE")]
    [Table("F190205")]
    public class F190205 : IAuditInfo
    {

        /// <summary>
        /// 商品類別(F000904 Where Topic=F1903)
        /// </summary>
        [Key]
        [Required]
        public string TYPE { get; set; }

        /// <summary>
        /// 檢驗類型(00:進倉 02:退貨)F000904 Where Topic=F190206
        /// </summary>
        [Key]
        [Required]
        public string CHECK_TYPE { get; set; }

        /// <summary>
        /// 檢驗項目編號(F1983)
        /// </summary>
        [Key]
        [Required]
        public string CHECK_NO { get; set; }

        /// <summary>
        /// 業主
        /// </summary>
        [Key]
        [Required]
        public string GUP_CODE { get; set; }

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
    /// 貨主
    /// </summary>
    [Required]
        public string CUST_CODE { get; set; }
    }
}
