namespace Wms3pl.Datas.F00
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 作業求救紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("HELP_NO", "DC_CODE")]
  [Table("F0010")]
  public class F0010 : IAuditInfo
  {

    /// <summary>
    /// 求救類型(01:揀貨 A1:揀貨延遲 02包裝03退貨點收 04退貨檢驗 05退貨上架申請 06調撥)
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string HELP_TYPE { get; set; }

    /// <summary>
    /// 求救編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 HELP_NO { get; set; }

    /// <summary>
    /// 單據編號(依不同作業寫入不同單據號碼)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 處理狀態(0:待處理1:已處理)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 使用者電腦IP
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string DEVICE_PC { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

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
    /// 建檔人名
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
    /// 儲位
    /// </summary>
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }

    /// <summary>
    /// 樓層
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string FLOOR { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
  }
}
