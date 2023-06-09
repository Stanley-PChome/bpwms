namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 電子訂單貨物配送狀態檔
  /// </summary>
  [Serializable]
  [DataServiceKey("STATUS_ID","ALL_ID")]
  [Table("F194714")]
  public class F194714 : IAuditInfo
  {

	  /// <summary>
	  /// 狀態ID
	  /// </summary>
    [Key]
    [Required]
	  public string STATUS_ID { get; set; }

	  /// <summary>
	  /// 狀態說明
	  /// </summary>
	  public string STATUS_DESC { get; set; }

	  /// <summary>
	  /// 速達顯示說明
	  /// </summary>
	  public string TCAT_DESC { get; set; }

	  /// <summary>
	  /// 0:訂單處理中未配送、2:配送中、3:配送完成、4:退貨、5:異常
	  /// </summary>
	  public string STATUS { get; set; }

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

	  /// <summary>
	  /// 配送商
	  /// </summary>
    [Key]
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 是否為最終貨態(0:否,1:是)
	  /// </summary>
    [Required]
	  public string ISLASTSTATUS { get; set; }
  }
}
        