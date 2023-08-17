namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 調撥明細貨架資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F060102")]
    public class F060102 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
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
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

        /// <summary>
        /// 調撥單號
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(32)")]
    public string ALLOCATION_NO { get; set; }

        /// <summary>
        /// 調撥單序號
        /// </summary>
        [Required]
    [Column(TypeName = "int")]
    public int ALLOCATION_SEQ { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 貨架編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SHELF_CODE { get; set; }

    /// <summary>
    /// 儲位編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string BIN_CODE { get; set; }

    /// <summary>
    /// 商品數量
    /// </summary>
    [Column(TypeName = "int")]
    public int SKU_QTY { get; set; }

    /// <summary>
    /// 作業人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string OPERATOR { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
    }
}
