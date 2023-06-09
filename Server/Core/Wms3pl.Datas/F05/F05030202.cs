namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 配庫後訂單明細與出貨明細對照表
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F05030202")]
  public class F05030202 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Decimal ID { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Required]
	  public string ORD_NO { get; set; }

	  /// <summary>
	  /// 訂單序號
	  /// </summary>
    [Required]
	  public string ORD_SEQ { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Required]
	  public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 出貨序號
	  /// </summary>
    [Required]
	  public string WMS_ORD_SEQ { get; set; }

	  /// <summary>
	  /// 預計出貨數量
	  /// </summary>
    [Required]
	  public Int32 B_DELV_QTY { get; set; }

	  /// <summary>
	  /// 實際出貨數量
	  /// </summary>
	  public Int32? A_DELV_QTY { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        