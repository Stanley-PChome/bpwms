namespace Wms3pl.Datas.F06
{
    using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;
    /// <summary>
    /// 儲位異常回報
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F060802")]
    public class F060802 : IAuditInfo
    {
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(10)")]
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
    /// 貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string SORTER_CODE { get; set; }
    /// <summary>
    /// 分揀機編號
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int ABNORMAL_TYPE { get; set; }
    /// <summary>
    /// 異常類型(1=讀取失敗, 2=分揀設備異常, 3=流道滿載, 9=其他)
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string ABNORMAL_MSG { get; set; }
    /// <summary>
    /// 異常訊息
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string ABNORMAL_CODE { get; set; }
    /// <summary>
    /// 紀錄時間
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(19)")]
    public string RECORD_TIME { get; set; }
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
