namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 工作群組程式設定匯入紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_ID")]
  [Table("F195301_IMPORTLOG")]
  public class F195301_IMPORTLOG 
  {

	  /// <summary>
	  /// 紀錄ID
	  /// </summary>
    [Key]
    [Required]
	  public Decimal LOG_ID { get; set; }

	  /// <summary>
	  /// 工作群組名稱
	  /// </summary>
	  public string GRP_NAME { get; set; }

	  /// <summary>
	  /// 功能編號
	  /// </summary>
	  public string FUN_CODE { get; set; }

	  /// <summary>
	  /// 工作群組是否顯示個資
	  /// </summary>
	  public string SHOWINFO { get; set; }

	  /// <summary>
	  /// 錯誤訊息
	  /// </summary>
	  public string ERRMSG { get; set; }

	  /// <summary>
	  /// 是否匯入成功
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        