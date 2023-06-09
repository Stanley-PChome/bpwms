namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 門市附屬欄位檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RETAIL_CODE","GUP_CODE","CUST_CODE")]
  [Table("F191002")]
  public class F191002 : IAuditInfo
  {

	  /// <summary>
	  /// 門市編號
	  /// </summary>
    [Key]
    [Required]
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 業務員
	  /// </summary>
	  public string SALESMAN { get; set; }

	  /// <summary>
	  /// 區域
	  /// </summary>
	  public string REGION { get; set; }

	  /// <summary>
	  /// 上樓層
	  /// </summary>
	  public string FLOOR { get; set; }

	  /// <summary>
	  /// 協助上架/翻堆
	  /// </summary>
	  public string HELP { get; set; }

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
	  /// 建立時間
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
        