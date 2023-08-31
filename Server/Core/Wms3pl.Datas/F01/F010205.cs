namespace Wms3pl.Datas.F01
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 進倉驗收上架結果表
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F010205")]
    public class F010205 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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
        /// 驗收單號
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string RT_NO { get; set; }

        /// <summary>
        /// 調撥單號
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public string ALLOCATION_NO { get; set; }

        /// <summary>
        /// 歷程狀態(0: 訂單成立、1: 收貨、2: 驗收、3: 上架、4: 單據結案、9: 訂單取消)
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string STATUS { get; set; }

        /// <summary>
        /// 明細資料處理狀態(0:待處理 1:已處理)
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string PROC_FLAG { get; set; }

        /// <summary>
        /// 紀錄轉出時間
        /// </summary>
        [Column(TypeName = "datetime2(0)")]
        public DateTime? TRANS_DATE { get; set; }

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
