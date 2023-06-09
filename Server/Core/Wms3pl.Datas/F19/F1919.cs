namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 儲區主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("AREA_CODE","WAREHOUSE_ID","DC_CODE")]
  [Table("F1919")]
  public class F1919 : IAuditInfo
  {

	  /// <summary>
	  /// 儲區編號
	  /// </summary>
    [Key]
    [Required]
	  public string AREA_CODE { get; set; }

	  /// <summary>
	  /// 儲區名稱
	  /// </summary>
	  public string AREA_NAME { get; set; }

	  /// <summary>
	  /// 儲區型態(F191901)
	  /// </summary>
    [Required]
	  public string ATYPE_CODE { get; set; }

	  /// <summary>
	  /// 倉別(F1980)
	  /// </summary>
    [Key]
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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
  }
}
        