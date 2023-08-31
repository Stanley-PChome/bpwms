namespace Wms3pl.Datas.Schedule
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;

  /// <summary>
  /// 使用者登入設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("EMP_ID")]
  [Table("PREFERENCE")]
  public class PREFERENCE
  {
    [Key]
    [Required]
    [Column(TypeName = "varchar(24)")]
    public string EMP_ID { get; set; }
    [Column(TypeName = "varbinary(max)")]
    public Byte[] DATA { get; set; }
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
  }
}
