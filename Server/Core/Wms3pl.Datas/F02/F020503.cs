namespace Wms3pl.Datas.F02
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 容器等待上架明細檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F020503")]
    public class F020503: IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 ID { get; set; }

        /// <summary>
        /// F020501.ID
        /// </summary>
        [Required]
        [Column(TypeName = "bigint")]
        public Int64 F020501_ID { get; set; }

        /// <summary>
        /// 上架倉別
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(3)")]
        public string PICK_WARE_ID { get; set; }

        /// <summary>
        /// 容器編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(32)")]
        public string CONTAINER_CODE { get; set; }

        /// <summary>
        /// 人員帳號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(16)")]
        public string EMP_ID { get; set; }

        /// <summary>
        /// 狀態 (0: 移動中、1: 移動完成)
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string STATUS { get; set; }

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
