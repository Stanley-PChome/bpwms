namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 入庫任務清單
    /// </summary>
    [Serializable]
    [DataServiceKey("DOC_ID", "CMD_TYPE")]
    [Table("F060101")]
    public class F060101 : IAuditInfo
    {
        /// <summary>
        /// 任務單號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string DOC_ID { get; set; }
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
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }

        /// <summary>
        /// 倉庫代碼
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }

        /// <summary>
        /// 命令類別 1:入庫任務  2:  入庫任務取消
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "vachar(3)")]
    public string CMD_TYPE { get; set; }

        /// <summary>
        /// 單據編號/容器編號
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

        /// <summary>
        /// 狀態 0: 待處理 1:處理中 2:完成  F:處理失敗
        /// </summary>
        [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 傳送時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? PROC_DATE { get; set; }

    /// <summary>
    /// 已派送次數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int RESENT_CNT { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [Column(TypeName = "nvarchar(MAX)")]
    public string MESSAGE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
    [Column(TypeName = "varchar(40)")]
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
    [Column(TypeName = "varchar(40)")]
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
