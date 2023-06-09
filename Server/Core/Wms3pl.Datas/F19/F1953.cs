namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 工作群組檔
  /// </summary>
  [Serializable]
  [DataServiceKey("GRP_ID")]
  [Table("F1953")]
  public class F1953 : IAuditInfo
  {

	  /// <summary>
	  /// 工作群組編號
	  /// </summary>
    [Key]
    [Required]
	  public Decimal GRP_ID { get; set; }

	  /// <summary>
	  /// 群組名稱
	  /// </summary>
    [Required]
	  public string GRP_NAME { get; set; }

	  /// <summary>
	  /// 說明
	  /// </summary>
	  public string GRP_DESC { get; set; }

	  /// <summary>
	  /// 是否顯示個資
	  /// </summary>
	  public string SHOWINFO { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 註記是否已刪除(1:已刪除)
	  /// </summary>
	  public string ISDELETED { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        