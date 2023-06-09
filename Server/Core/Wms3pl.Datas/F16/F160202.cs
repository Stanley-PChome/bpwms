namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 廠退單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("RTN_VNR_NO","RTN_VNR_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F160202")]
  public class F160202 : IAuditInfo
  {

	  /// <summary>
	  /// 廠退單號
	  /// </summary>
    [Key]
    [Required]
	  public string RTN_VNR_NO { get; set; }

	  /// <summary>
	  /// 廠退單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 RTN_VNR_SEQ { get; set; }

	  /// <summary>
	  /// EDI指定廠退倉別 F1980
	  /// </summary>
	  public string ORG_WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 來源倉別 F1980
	  /// </summary>
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 儲位編號
	  /// </summary>
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 廠退數量
	  /// </summary>
    [Required]
	  public Int32 RTN_VNR_QTY { get; set; }

	  /// <summary>
	  /// 廠退出貨數量
	  /// </summary>
    [Required]
	  public Int32 RTN_WMS_QTY { get; set; }

		/// <summary>
		/// 廠退原因說明
		/// </summary>
		public string MEMO { get; set; }

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
		/// 廠退原因
		/// </summary>
		public string RTN_VNR_CAUSE { get; set; }

		/// <summary>
		/// 指定商品驗收批號
		/// </summary>
		public string MAKE_NO { get; set; }
	}
}
        