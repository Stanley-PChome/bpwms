namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 作業鎖定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("WORK_CODE")]
  [Table("F0500")]
  public class F0500 
  {

	  /// <summary>
	  /// 作業類別(0配庫)
	  /// </summary>
    [Key]
    [Required]
	  public string WORK_CODE { get; set; }

	  /// <summary>
	  /// 執行狀態 (0待執行 1執行中)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }
  }
}
        