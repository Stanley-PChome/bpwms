namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 程式下拉選單參數設定語系對應檔
  /// </summary>
  [Serializable]
  [DataServiceKey("TOPIC","SUBTOPIC","VALUE","LANG")]
  [Table("F000904_I18N")]
  public class F000904_I18N 
  {

	  /// <summary>
	  /// 程式編號(資料表)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
	  public string TOPIC { get; set; }

	  /// <summary>
	  /// 選單ID
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(30)")]
	  public string SUBTOPIC { get; set; }

	  /// <summary>
	  /// 參數值
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
	  public string VALUE { get; set; }

	  /// <summary>
	  /// 參數名稱
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(300)")]
	  public string NAME { get; set; }

    /// <summary>
    /// 選單名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(300)")]
	  public string SUB_NAME { get; set; }

    /// <summary>
    /// 語系
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
	  public string LANG { get; set; }
  }
}
        