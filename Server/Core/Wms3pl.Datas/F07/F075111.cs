namespace Wms3pl.Datas.F07
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 驗收綁容器驗收單控管表
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F075111")]
  public class F075111 : IAuditInfo
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
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
		[Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }
    /// <summary>
    /// 狀態(0:人員綁定容器中 1:人員綁定容器完成 2: 更換人員作業)
    /// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
    /// <summary>
    /// 綁定工具(0: 電腦 1: PDA)
    /// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string DEVICE_TOOL { get; set; }
    /// <summary>
    /// 建立日期(鎖定日期)
    /// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }
    /// <summary>
    /// 建立人員編號(鎖定人員)
    /// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人員名稱(鎖定人名)
    /// </summary>
		[Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// 異動日期(解鎖日期/更換時間)
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// 異動人員編號(解鎖人員/更換人員)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人員名稱(解鎖人名/更換人名)
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

  }
}
