namespace Wms3pl.Datas.F19
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 商品材積資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ITEM_CODE","CUST_CODE", "GUP_CODE")]
    [Table("F1905")]
    public class F1905 : IAuditInfo
    {

        /// <summary>
        /// 商品編號
        /// </summary>
        [Key]
        [Required]
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// 商品長
        /// </summary>
        [Required]
        public decimal PACK_LENGTH { get; set; }

        /// <summary>
        /// 商品寬
        /// </summary>
        [Required]
        public decimal PACK_WIDTH { get; set; }

        /// <summary>
        /// 商品高
        /// </summary>
        [Required]
        public decimal PACK_HIGHT { get; set; }

        /// <summary>
        /// 商品重量
        /// </summary>
        [Required]
        public decimal PACK_WEIGHT { get; set; }

        /// <summary>
        /// 業主編號
        /// </summary>
        [Key]
        [Required]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

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
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }

				/// <summary>
				/// 貨主編號
				/// </summary>
				[Key]
				[Required]
        public string CUST_CODE { get; set; }
    }
}
