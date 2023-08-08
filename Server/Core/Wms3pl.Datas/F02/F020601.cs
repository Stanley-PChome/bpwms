namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 驗收完成作業主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F020601")]

  public class F020601 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>

    [Column(TypeName = "string(20)")]
    public string RT_NO { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>

    [Column(TypeName = "string(20)")]
    public string STOCK_NO { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    [Required]
    [Column(TypeName = "string(3)")]
    public string DC_CODE { get; set; }
    /// <summary>
    /// 業主編號
    /// </summary>
    [Required]
    [Column(TypeName = "string(20)")]
    public string GUP_CODE { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "string(20)")]
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    [Required]
    [Column(TypeName = "string(25)")]
    public string CUST_ORD_NO { get; set; }
    /// <summary>
    /// 作業方式 (0: 商品檢驗、1: 跨庫調撥入人工倉)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_TYPE { get; set; }
    /// <summary>
    /// 狀態 (0:待處理、1:驗收處理中、2: 驗收處理失敗、3: 驗收處理完成、4: 上架單處理中、5:上架單建立失敗、6:上架單建立完成、 9:取消)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
    /// <summary>
    /// 是否將驗收結果回傳給LMS(0:否、1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string TRANS_TO_LMS { get; set; }
    /// <summary>
    /// 錯誤訊息
    /// </summary>

    [Column(TypeName = "string(255)")]
    public string MESSAGE { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }
    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    [Column(TypeName = "string(20)")]
    public string CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人員名稱
    /// </summary>
    [Required]
    [Column(TypeName = "string(16)")]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// 異動日期
    /// </summary>

    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// 異動日期
    /// </summary>

    [Column(TypeName = "string(20)")]
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人員名稱
    /// </summary>

    [Column(TypeName = "string(16)")]
    public string UPD_NAME { get; set; }



  }
}
