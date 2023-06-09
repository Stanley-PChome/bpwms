namespace Wms3pl.Datas.Schedule
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

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
	  public string EMP_ID { get; set; }
	  public Byte[] DATA { get; set; }
	  public string DC_CODE { get; set; }
	  public string GUP_CODE { get; set; }
	  public string CUST_CODE { get; set; }
  }
}
        