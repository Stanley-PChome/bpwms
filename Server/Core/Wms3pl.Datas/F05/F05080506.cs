namespace Wms3pl.Datas.F05
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    [Serializable]
    [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "CAL_NO", "ORD_NO", "ITEM_CODE")]
    [Table("F05080506")]
    public class F05080506 : IAuditInfo
    {

        /// <summary>
        /// 物流中心
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

        /// <summary>
        /// 業主
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

        /// <summary>
        /// 試算編號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string CAL_NO { get; set; }
        /// <summary>
        /// 訂單單號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }
    /// <summary>
    /// 預計揀貨倉別名稱清單(用逗點分隔)
    /// </summary>
    [Column(TypeName = "nvarchar(400)")]
    public string WAREHOUSE_INFO { get; set; }
        /// <summary>
        /// 訂購數
        /// </summary>
        [Required]
    [Column(TypeName = "int")]
    public int B_QTY { get; set; }
        /// <summary>
        /// 配庫數
        /// </summary>
        [Required]
    [Column(TypeName = "int")]
    public int A_QTY { get; set; }
        /// <summary>
        /// 是否缺貨
        /// </summary>
        [Required]
    [Column(TypeName = "char(1)")]
    public string IS_LACK { get; set; }


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
