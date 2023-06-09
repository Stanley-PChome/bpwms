namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 查詢轉檔設定參數
  /// </summary>
  [Serializable]
  [DataServiceKey("QID","SEQ")]
  [Table("F190703")]
  public class F190703 : IAuditInfo
  {

	  /// <summary>
	  /// 查詢編號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 QID { get; set; }

	  /// <summary>
	  /// 查詢序號
	  /// </summary>
    [Key]
    [Required]
	  public Int16 SEQ { get; set; }

	  /// <summary>
	  /// 參數名稱
	  /// </summary>
	  public string PNAME { get; set; }

	  /// <summary>
	  /// 參數類型(0:TextBox,1:DatePicker,2:ComboBox)
	  /// </summary>
	  public string PTYPE { get; set; }

	  /// <summary>
	  /// 參數格式
	  /// </summary>
	  public string FORMAT { get; set; }

	  /// <summary>
	  /// 下拉選單查詢語法設定編號F190703
	  /// </summary>
	  public Int32? FUN_ID { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建立時間
	  /// </summary>
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
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        