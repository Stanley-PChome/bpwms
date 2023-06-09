namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 揀貨單身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PICK_ORD_NO","PICK_ORD_SEQ","GUP_CODE","CUST_CODE","DC_CODE")]
  [Table("F051202")]
  public class F051202 : IAuditInfo
  {

	  /// <summary>
	  /// 揀貨單號
	  /// </summary>
    [Key]
    [Required]
	  public string PICK_ORD_NO { get; set; }

	  /// <summary>
	  /// 揀貨單序號
	  /// </summary>
    [Key]
    [Required]
	  public string PICK_ORD_SEQ { get; set; }

	  /// <summary>
	  /// 揀貨儲位
	  /// </summary>
    [Required]
	  public string PICK_LOC { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 預定揀貨數量
	  /// </summary>
    [Required]
	  public Int32 B_PICK_QTY { get; set; }

	  /// <summary>
	  /// 實際揀貨數量
	  /// </summary>
    [Required]
	  public Int32 A_PICK_QTY { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

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

	  /// <summary>
	  /// 有效日期
	  /// </summary>
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 揀貨狀態0待揀貨1揀貨完成
	  /// </summary>
    [Required]
	  public string PICK_STATUS { get; set; }

	  /// <summary>
	  /// 入庫日
	  /// </summary>
    [Required]
	  public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 出貨單號
	  /// </summary>
    [Required]
	  public string WMS_ORD_NO { get; set; }

	  /// <summary>
	  /// 廠商編號
	  /// </summary>
    [Required]
	  public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
    [Required]
	  public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
    [Required]
	  public string PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
    [Required]
	  public string MAKE_NO { get; set; }

		/// <summary>
		/// 出貨項次
		/// </summary>
		
		public string WMS_ORD_SEQ { get; set; }


		/// <summary>
		/// 路順
		/// </summary>
		public int? ROUTE_SEQ { get; set; }

		/// <summary>
		/// PK區編號/倉庫編號
		/// </summary>
		public string PK_AREA { get; set; }

		/// <summary>
		/// PK區名稱/倉庫名稱
		/// </summary>
		public string PK_AREA_NAME { get; set; }
	}
}
        