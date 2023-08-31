namespace Wms3pl.Datas.F02
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 驗收容器上架頭檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F020501")]
    public class F020501: IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 ID { get; set; }

        /// <summary>
        /// 物流中心編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(3)")]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 業主編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string CUST_CODE { get; set; }

        /// <summary>
        /// 容器編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(32)")]
        public string CONTAINER_CODE { get; set; }

        /// <summary>
        /// F0701的流水ID
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 F0701_ID { get; set; }

        /// <summary>
        /// 上架倉別
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(3)")]
        public string PICK_WARE_ID { get; set; }

        /// <summary>
        /// 上架區域(A:揀區 C:補區 R:不良品區)
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string TYPE_CODE { get; set; }

        /// <summary>
        /// 狀態(0: 開箱、1: 已關箱待複驗、2: 可上架、3: 不可上架、4: 已上架、7:已關箱待產生調撥單)
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string STATUS { get; set; }

        /// <summary>
        /// 調撥單號
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string ALLOCATION_NO { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
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
        [Column(TypeName = "datetime2(0)")]
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
    }
}
