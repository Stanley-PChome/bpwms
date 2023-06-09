namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 檔案分割欄位定義檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F190010")]
  public class F190010 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int64 ID { get; set; }

	  /// <summary>
	  /// 檔名關鍵字
	  /// </summary>
	  public string FILE_KEYWORD { get; set; }

	  /// <summary>
	  /// 副檔名
	  /// </summary>
	  public string FILE_EXTENSION { get; set; }

	  /// <summary>
	  /// 欄位名稱
	  /// </summary>
    [Required]
	  public string COL_NAME { get; set; }

	  /// <summary>
	  /// 欄位說明
	  /// </summary>
	  public string COL_DESC { get; set; }

	  /// <summary>
	  /// 欄位型態(N:數字 C:字串 D:日期)
	  /// </summary>
	  public string COL_TYPE { get; set; }

	  /// <summary>
	  /// 欄位長度
	  /// </summary>
	  public Int64? COL_LENGTH { get; set; }

	  /// <summary>
	  /// 開始索引位置或分隔索引位置
	  /// </summary>
    [Required]
	  public Int64 BEGIN_INDEX { get; set; }

	  /// <summary>
	  /// 結束索引位置
	  /// </summary>
	  public Int64? END_INDEX { get; set; }

	  /// <summary>
	  /// 對齊補的字元
	  /// </summary>
	  public string PAD_ADDCHAR { get; set; }

	  /// <summary>
	  /// 對齊方式(L:左靠;R:右靠)
	  /// </summary>
	  public string PAD_TYPE { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人員名稱
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        