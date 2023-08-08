namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 項目類別主檔(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("ITEM_TYPE_ID")]
  [Table("F910003")]
  public class F910003 : IAuditInfo
  {

	  /// <summary>
	  /// 項目類別編號
	  /// </summary>
    [Key]
    [Required]
	  public string ITEM_TYPE_ID { get; set; }

	  /// <summary>
	  /// 項目類別
	  /// </summary>
    [Required]
	  public string ITEM_TYPE { get; set; }

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
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 是否顯示
	  /// </summary>
    [Required]
	  public string ISVISABLE { get; set; }
  }
}
        