namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 儲位計價項目設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","TMPR_TYPE","LOC_TYPE_ID","ACC_UNIT","ACC_ITEM_KIND_ID")]
  [Table("F199001")]
  public class F199001 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 溫層(01:常溫 02:低溫 03:冷凍)F000904 Where Topic=F1980 Subtopic=TMPR_TYPE
	  /// </summary>
    [Key]
    [Required]
	  public string TMPR_TYPE { get; set; }

	  /// <summary>
	  /// 料架編號(F1942)
	  /// </summary>
    [Key]
    [Required]
	  public string LOC_TYPE_ID { get; set; }

	  /// <summary>
	  /// 計價單位 F91000302 Where ItemTypeId=002
	  /// </summary>
    [Key]
    [Required]
	  public string ACC_UNIT { get; set; }

	  /// <summary>
	  /// 計價數量
	  /// </summary>
    [Required]
	  public Int32 ACC_NUM { get; set; }

	  /// <summary>
	  /// 單位費用
	  /// </summary>
    [Required]
	  public decimal UNIT_FEE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 含稅(0:未稅 1:含稅)F000904
	  /// </summary>
    [Required]
	  public string IN_TAX { get; set; }

	  /// <summary>
	  /// 狀態(0使用中,9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 計價類別編號(固定為002)
	  /// </summary>
    [Required]
	  public string ITEM_TYPE_ID { get; set; }

	  /// <summary>
	  /// 計價項目F91000301 where ITEM_TYPE_ID=002
	  /// </summary>
    [Key]
    [Required]
	  public string ACC_ITEM_KIND_ID { get; set; }

	  /// <summary>
	  /// 計價項目名稱(同DC不得重複)
	  /// </summary>
    [Required]
	  public string ACC_ITEM_NAME { get; set; }
  }
}
        