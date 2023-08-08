namespace Wms3pl.Datas.F02
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉容器關箱後產調撥上架單
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F020603")]
  public class F020603 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
    /// <summary>
    /// F020501的ID
    /// </summary>
    [Column(TypeName = "bigint")]
    public long? F020501_ID { get; set; }
    /// <summary>
    /// F0701的ID
    /// </summary>
    [Column(TypeName = "bigint")]
    public long? F0701_ID { get; set; }
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
    /// 作業方式 (0: PDA容器綁定、1:PDA關箱 2: PDA商品複驗成功 3. PC容器綁定  4.PC關箱 5.PC複驗異常處理)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PROC_TYPE { get; set; }
    /// <summary>
    /// 狀態 (0:待處理、1: 上架單處理中、2:上架單建立失敗、3:上架單建立完成、 9:取消)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [Column(TypeName = "nvarchar(255)")]
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
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人員名稱
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
    /// 異動日期
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
  }
}
