namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 計價項目設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ITEM_TYPE_ID","ACC_ITEM_KIND_ID","DELV_ACC_TYPE")]
  [Table("F91000301")]
  public class F91000301 : IAuditInfo
  {

	  /// <summary>
	  /// 項目類別ID(F910003)
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_TYPE_ID { get; set; }

	  /// <summary>
	  /// 計價項目ID
	  /// </summary>
    [Key]
    [Required]
	  public string ACC_ITEM_KIND_ID { get; set; }

	  /// <summary>
	  /// 計價項目名稱
	  /// </summary>
    [Required]
	  public string ACC_ITEM_KIND_NAME { get; set; }

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
	  /// 配送計價類別 F000904(01:無 02:宅配 03:統倉 04:門店)
	  /// </summary>
    [Key]
    [Required]
	  public string DELV_ACC_TYPE { get; set; }
  }
}
        