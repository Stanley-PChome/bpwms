namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨檢驗身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RETURN_NO","RETURN_AUDIT_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F161402")]
  public class F161402 : IAuditInfo
  {

	  /// <summary>
	  /// 退貨單號
	  /// </summary>
    [Key]
    [Required]
	  public string RETURN_NO { get; set; }

	  /// <summary>
	  /// 退貨檢驗序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 RETURN_AUDIT_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 預設上架儲位
	  /// </summary>
    [Required]
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 已過帳數(已上架數)
	  /// </summary>
    [Required]
	  public Int32 MOVED_QTY { get; set; }

	  /// <summary>
	  /// 退貨數量
	  /// </summary>
    [Required]
	  public Int32 RTN_QTY { get; set; }

	  /// <summary>
	  /// 檢驗數量
	  /// </summary>
    [Required]
	  public Int32 AUDIT_QTY { get; set; }

	  /// <summary>
	  /// 檢驗人員
	  /// </summary>
    [Required]
	  public string AUDIT_STAFF { get; set; }

	  /// <summary>
	  /// 檢驗人名
	  /// </summary>
    [Required]
	  public string AUDIT_NAME { get; set; }

	  /// <summary>
	  /// 備註
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
	  /// 異常原因(F1951 Where UCT_ID=RC)
	  /// </summary>
	  public string CAUSE { get; set; }

	  /// <summary>
	  /// 不加工組合成品名稱(虛擬組合)
	  /// </summary>
	  public string BOM_ITEM_CODE { get; set; }

	  /// <summary>
	  /// 此商品在每一個不加工成品組合數量
	  /// </summary>
	  public Int32? BOM_QTY { get; set; }

	  /// <summary>
	  /// 商品總件數(一般商品固定為1,組合商品由BOM表計算取得)
	  /// </summary>
    [Required]
	  public Int32 ITEM_QTY { get; set; }

	  /// <summary>
	  /// 實體組合商品註記(0:一般商品,1:實體組合商品)
	  /// </summary>
    [Required]
	  public string MULTI_FLAG { get; set; }
  }
}
        