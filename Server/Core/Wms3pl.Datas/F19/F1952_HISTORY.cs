namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 人員密碼修改歷史紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("EMP_ID","CRT_DATE")]
  [Table("F1952_HISTORY")]
  public class F1952_HISTORY
  {

	  /// <summary>
	  /// 員工編號
	  /// </summary>
    [Key]
    [Required]
	  public string EMP_ID { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 登入密碼
	  /// </summary>
	  public string PASSWORD { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        