namespace Wms3pl.Datas.F25
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 在庫序號主檔異動紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_SEQ")]
  [Table("F250101")]
  public class F250101 : IAuditInfo
  {

    /// <summary>
    /// 流水序號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 LOG_SEQ { get; set; }

    /// <summary>
    /// 紀錄時間
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime LOG_DATE { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }

    /// <summary>
    /// 商品編號(F1903)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 盒號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BOX_SERIAL { get; set; }

    /// <summary>
    /// 3G序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string TAG3G { get; set; }

    /// <summary>
    /// 門號
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string CELL_NUM { get; set; }

    /// <summary>
    /// PUK碼
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string PUK { get; set; }

    /// <summary>
    /// 狀態(F000904)
    /// </summary>
    [Column(TypeName = "char(2)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 儲值卡盒號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BATCH_NO { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? VALID_DATE { get; set; }

    /// <summary>
    /// PO單號
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string PO_NO { get; set; }

    /// <summary>
    /// 卡號是否已開通(0否1是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ACTIVATED { get; set; }

    /// <summary>
    /// 序號是否已上傳給客戶(0否1是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string SEND_CUST { get; set; }

    /// <summary>
    /// 廠商F1908
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 系統商
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SYS_VNR { get; set; }

    /// <summary>
    /// 系統單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }

    /// <summary>
    /// 加工單號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string PROCESS_NO { get; set; }

    /// <summary>
    /// 異動類型F000903
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string ORD_PROP { get; set; }

    /// <summary>
    /// 箱號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string CASE_NO { get; set; }

    /// <summary>
    /// 進貨日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? IN_DATE { get; set; }

    /// <summary>
    /// 客戶代號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string RETAIL_CODE { get; set; }

    /// <summary>
    /// BOUNDEL_ID組合商品編號
    /// </summary>
    [Column(TypeName = "bigint")]
    public Int64? COMBIN_NO { get; set; }

    /// <summary>
    /// 攝影機編號F192404
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string CAMERA_NO { get; set; }

    /// <summary>
    /// 電腦編號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string CLIENT_IP { get; set; }

    /// <summary>
    /// 業主
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

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
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 組合成品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string BOUNDLE_ITEM_CODE { get; set; }
  }
}
