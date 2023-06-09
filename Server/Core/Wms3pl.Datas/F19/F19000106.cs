namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// EDI特殊單據類別主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("TICKET_ID","TICKET_NAME")]
  [Table("F19000106")]
  public class F19000106 : IAuditInfo
  {

	  /// <summary>
	  /// 單據主檔Id
	  /// </summary>
    [Key]
    [Required]
	  public Decimal TICKET_ID { get; set; }

	  /// <summary>
	  /// 單據名稱
	  /// </summary>
    [Key]
    [Required]
	  public string TICKET_NAME { get; set; }

	  /// <summary>
	  /// 貨主代號
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主代號
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 物流中心代號
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

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
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        