namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 貨主指定配送商設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ALL_ID","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F194704")]
  public class F194704 : IAuditInfo
  {

	  /// <summary>
	  /// 配送商編號
	  /// </summary>
    [Key]
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

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
	  /// 列印託運單方式(0:CrystalReport 1:Bartend)
	  /// </summary>
    [Required]
	  public string PRINTER_TYPE { get; set; }

	  /// <summary>
	  /// 託運單格式(01:熱感式 02:二模格式)
	  /// </summary>
    [Required]
	  public string CONSIGN_FORMAT { get; set; }

	  /// <summary>
	  /// 取號方式 F000904 TOPIC=F194704 SUBTOPIC=GET_CONSIGN_NO
	  /// </summary>
    [Required]
	  public string GET_CONSIGN_NO { get; set; }

	  /// <summary>
	  /// 是否代印託運單(0:否 1:是)
	  /// </summary>
    [Required]
	  public string PRINT_CONSIGN { get; set; }

	  /// <summary>
	  /// 是否自動列印託運單(0:否 1:是)
	  /// </summary>
    [Required]
	  public string AUTO_PRINT_CONSIGN { get; set; }

	  /// <summary>
	  /// 指定配送商集貨所郵遞區號(統一速達北二特販所:00301)
	  /// </summary>
	  public string ZIP_CODE { get; set; }

	  /// <summary>
	  /// 加箱托單取號模式F000904 TOPIC=F194704 SUBTOPIC=ADDBOX_GET_CONSIGN_NO
	  /// </summary>
    [Required]
	  public string ADDBOX_GET_CONSIGN_NO { get; set; }
  }
}
        