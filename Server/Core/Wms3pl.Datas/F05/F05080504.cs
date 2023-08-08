namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配庫試算各儲區出貨比例
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","CAL_NO","WAREHOUSE_ID","AREA_CODE")]
  [Table("F05080504")]
  public class F05080504 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 試算編號
	  /// </summary>
    [Key]
    [Required]
	  public string CAL_NO { get; set; }

	  /// <summary>
	  /// 倉庫編號
	  /// </summary>
    [Key]
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 儲區編號
	  /// </summary>
    [Key]
    [Required]
	  public string AREA_CODE { get; set; }

	  /// <summary>
	  /// 出貨數
	  /// </summary>
    [Required]
	  public Int32 DELV_QTY { get; set; }

	  /// <summary>
	  /// 出貨占比(出貨數/總出貨數)
	  /// </summary>
    [Required]
	  public Single DELV_RATIO { get; set; }

	  /// <summary>
	  /// 建立日期
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
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        