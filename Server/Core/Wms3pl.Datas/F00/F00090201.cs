namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 來源單據更新狀態表(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("WORK_TYPE","SOURCE_TYPE","THIS_STATUS")]
  [Table("F00090201")]
  public class F00090201 : IAuditInfo
  {

	  /// <summary>
	  /// 作業類別(1進倉 2出貨 3退貨 4調撥 5歸還)
	  /// </summary>
    [Key]
    [Required]
	  public Int16 WORK_TYPE { get; set; }

	  /// <summary>
	  /// 來源單據類別F000902
	  /// </summary>
    [Key]
    [Required]
	  public string SOURCE_TYPE { get; set; }

	  /// <summary>
	  /// 原作業單據狀態
	  /// </summary>
    [Key]
    [Required]
	  public string THIS_STATUS { get; set; }

	  /// <summary>
	  /// 來源單據變更後狀態
	  /// </summary>
    [Required]
	  public string UPDATE_STATUS { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔日期
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
	  /// 建檔人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }
  }
}
        