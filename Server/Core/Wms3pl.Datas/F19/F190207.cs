namespace Wms3pl.Datas.F19
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 商品圖檔路徑檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ITEM_CODE", "IMAGE_NO", "GUP_CODE","CUST_CODE")]
    [Table("F190207")]
    public class F190207 : IAuditInfo
    {

        /// <summary>
        /// 商品編號
        /// </summary>
        [Key]
        [Required]
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// 圖檔編號
        /// </summary>
        [Key]
        [Required]
        public Int16 IMAGE_NO { get; set; }

        /// <summary>
        /// 圖檔路徑
        /// </summary>
        [Required]
        public string IMAGE_PATH { get; set; }

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

        public string CUST_CODE { get; set; }
    }
}
