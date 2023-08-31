namespace Wms3pl.Datas.F51
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 庫存比對資料
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F51010501")]
  public class F51010501 : IAuditInfo
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
    [Column(TypeName = "varchar(20)")]
    public string GUP_CODE { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 倉庫代碼
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }
    /// <summary>
    /// 備份日期
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(10)")]
    public string CAL_DATE { get; set; }
    /// <summary>
    /// 儲位編號
    /// </summary>
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime VALID_DATE { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// WMS的庫存
    /// </summary>
    [Column(TypeName = "int")]
    public int? WMS_QTY { get; set; }
    /// <summary>
    /// 自動倉的庫存
    /// </summary>
    [Column(TypeName = "int")]
    public int? WCS_QTY { get; set; }
    /// <summary>
    /// 虛擬帳的預約數
    /// </summary>
    [Column(TypeName = "int")]
    public int? BOOKING_QTY { get; set; }
    /// <summary>
    /// 庫存差異數量
    /// </summary>
    [Column(TypeName = "int")]
    public int? DIFF_QTY { get; set; }
    /// <summary>
    /// 處理狀態 (0: 待處理 1: 比對中 2: 比對完成)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_FLAG { get; set; }
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
