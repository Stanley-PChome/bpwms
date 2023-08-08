namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 來源單據對照表(不可修改)
  /// </summary>
  [Serializable]
  [DataServiceKey("SOURCE_TYPE")]
  [Table("F000902")]
  public class F000902 : IAuditInfo
  {

	  /// <summary>
	  /// 來源單據類別
	  /// </summary>
    [Key]
    [Required]
	  public string SOURCE_TYPE { get; set; }

	  /// <summary>
	  /// 來源單據名稱
	  /// </summary>
    [Required]
	  public string SOURCE_NAME { get; set; }

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
	  /// 單據頭碼
	  /// </summary>
	  public string ORD_HEADER { get; set; }

	  /// <summary>
	  /// 對應資料表
	  /// </summary>
	  public string TABLE_NAME { get; set; }

	  /// <summary>
	  /// 是否顯示
	  /// </summary>
    [Required]
	  public string ISVISABLE { get; set; }
  }
}
        