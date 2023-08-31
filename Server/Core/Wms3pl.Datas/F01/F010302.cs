namespace Wms3pl.Datas.F01
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    [Serializable]
    [DataServiceKey("ID")]
    [Table("F010302")]
    public class F010302 : IAuditInfo
    {
        /// <summary>
        /// 流水號
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
        /// 物流商編號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(10)")]
        public string ALL_ID { get; set; }

        /// <summary>
        /// 刷單核對日期
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime? CHECK_DATE { get; set; }

        /// <summary>
        /// 刷單核對時間
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(8)")]
        public string CHECK_TIME { get; set; }

        /// <summary>
        /// 刷單人員帳號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string CHECK_USER { get; set; }

        /// <summary>
        /// 刷單人員名稱
        /// </summary>
        [Column(TypeName = "nvarchar(16)")]
        public string CHECK_NAME { get; set; }

        /// <summary>
        /// 貨運單號
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string SHIP_ORD_NO { get; set; }

        /// <summary>
        /// 核對箱數
        /// </summary>
        [Required]
        [Column(TypeName = "smallint")]
        public Int16 CHECK_BOX_CNT { get; set; }

        /// <summary>
        /// 貨單箱數
        /// </summary>
        [Required]
        [Column(TypeName = "smallint")]
        public Int16 SHIP_BOX_CNT { get; set; }

        /// <summary>
        /// 核對箱數結果 0:核對失敗;1:核對成功
        /// </summary>
        [Required]
        [Column(TypeName = "char(1)")]
        public string CHECK_STATUS { get; set; }

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
