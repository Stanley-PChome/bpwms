namespace Wms3pl.Datas.F02
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    [Serializable]
    [DataServiceKey("ID")]
    [Table("F02020109")]
    public class F02020109: IAuditInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 ID { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(3)")]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 業主
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(2)")]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(6)")]
        public string CUST_CODE { get; set; }

        /// <summary>
        /// 進倉單號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string STOCK_NO { get; set; }

        /// <summary>
        /// 進倉序號
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public Int32 STOCK_SEQ { get; set; }

        /// <summary>
        /// 不良品數量
        /// </summary>
        [Column(TypeName = "int")]
        public Int32? DEFECT_QTY { get; set; }

        /// <summary>
        /// 商品序號
        /// </summary>
        [Column(TypeName = "varchar(50)")]
        public string SERIAL_NO { get; set; }

        /// <summary>
        /// 原因代碼
        /// </summary>
        [Column(TypeName = "varchar(6)")]
        public string UCC_CODE { get; set; }

        /// <summary>
        /// 其他原因
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string CAUSE { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(16)")]
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        [Column(TypeName = "datetime2(7)")]
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        [Column(TypeName = "nvarchar(16)")]
        public string UPD_NAME { get; set; }

        /// <summary>
        /// 驗收單號
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string RT_NO { get; set; }

        /// <summary>
        /// 驗收序號
        /// </summary>
        [Column(TypeName = "varchar(4)")]
        public string RT_SEQ { get; set; }


        /// <summary>
        /// 倉別編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(3)")]
        public string WAREHOUSE_ID { get; set; }

    }
}
