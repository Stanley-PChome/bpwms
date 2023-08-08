namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 物流中心行事曆
  /// </summary>
  [Serializable]
  [DataServiceKey("SCHEDULE_NO","DC_CODE")]
  [Table("F700501")]
  public class F700501 : IAuditInfo
  {

	  /// <summary>
	  /// 行事曆單號
	  /// </summary>
    [Key]
    [Required]
	  public string SCHEDULE_NO { get; set; }

	  /// <summary>
	  /// 行事曆日期
	  /// </summary>
    [Required]
	  public DateTime SCHEDULE_DATE { get; set; }

	  /// <summary>
	  /// 行事曆時間(HH:mm)
	  /// </summary>
    [Required]
	  public string SCHEDULE_TIME { get; set; }

	  /// <summary>
	  /// 事項種類(M會議W待辦事項S建議事項H假日)F000904
	  /// </summary>
    [Required]
	  public string SCHEDULE_TYPE { get; set; }

	  /// <summary>
	  /// 重要性(0低1一般2高)F000904
	  /// </summary>
    [Required]
	  public string IMPORTANCE { get; set; }

	  /// <summary>
	  /// 標題
	  /// </summary>
    [Required]
	  public string SUBJECT { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立時間
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 附件檔案名稱
	  /// </summary>
	  public string FILE_NAME { get; set; }

	  /// <summary>
	  /// 0待處理1已處理
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 訊息池Id
	  /// </summary>
	  public Decimal? MESSAGE_ID { get; set; }
	  public string CONTENT { get; set; }
  }
}
        