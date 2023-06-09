namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 出庫明細貨架資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F060204")]
    public class F060204 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        public long ID { get; set; }

        /// <summary>
        /// 任務單號
        /// </summary>
        [Required]
        public string DOC_ID { get; set; }

        /// <summary>
        /// 單據項次
        /// </summary>
        [Required]
        public int ORD_SEQ { get; set; }

        /// <summary>
        /// 貨架編號
        /// </summary>
        public string SHELFCODE { get; set; }

        /// <summary>
        /// 儲位編號
        /// </summary>
        public string BINCODE { get; set; }

        /// <summary>
        /// 商品數量
        /// </summary>
        public int? SKUQTY { get; set; }

        /// <summary>
        /// 作業人員
        /// </summary>
        [Required]
        public string OPERATOR { get; set; }
        
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
