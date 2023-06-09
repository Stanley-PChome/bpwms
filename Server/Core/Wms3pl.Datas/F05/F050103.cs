namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 原始出貨訂單關聯資料頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ORD_NO","CUST_ORD_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F050103")]
  public class F050103 : IAuditInfo
  {

	  /// <summary>
	  /// 訂單編號
	  /// </summary>
    [Key]
    [Required]
	  public string ORD_NO { get; set; }

	  /// <summary>
	  /// 貨主訂單編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_ORD_NO { get; set; }

	  /// <summary>
	  /// 訂單總金額
	  /// </summary>
	  public decimal? AMT { get; set; }

	  /// <summary>
	  /// 付款方式
	  /// </summary>
	  public string PAY_WAY { get; set; }

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
	  /// 超取(不)付款
	  /// </summary>
	  public string STORE_PAY { get; set; }

	  /// <summary>
	  /// 發票號碼
	  /// </summary>
	  public string INVOICE { get; set; }

	  /// <summary>
	  /// 發票日期
	  /// </summary>
	  public DateTime? INVOICE_DATE { get; set; }

	  /// <summary>
	  /// 個人識別碼
	  /// </summary>
	  public string IDENTIFIER { get; set; }

	  /// <summary>
	  /// 宅單備註
	  /// </summary>
	  public string DELV_MEMO { get; set; }

	  /// <summary>
	  /// 預定配達日
	  /// </summary>
	  public DateTime? EXP_DELV_DATE { get; set; }

	  /// <summary>
	  /// 客戶簡稱
	  /// </summary>
	  public string RETAIL_SHORTNAME { get; set; }

	  /// <summary>
	  /// 客戶全名
	  /// </summary>
	  public string RETAIL_NAME { get; set; }

	  /// <summary>
	  /// 部門名稱
	  /// </summary>
	  public string DEPT_NAME { get; set; }

	  /// <summary>
	  /// 業務員
	  /// </summary>
	  public string SALESMAN { get; set; }

	  /// <summary>
	  /// 發票聯數
	  /// </summary>
	  public string INVOICE_DESC { get; set; }

	  /// <summary>
	  /// 統一編號
	  /// </summary>
	  public string UNI_FORM { get; set; }

	  /// <summary>
	  /// 廠別名稱
	  /// </summary>
	  public string SITENAME { get; set; }

	  /// <summary>
	  /// 本幣銷貨稅額
	  /// </summary>
	  public decimal? ORDERTAXAMOUNT { get; set; }

	  /// <summary>
	  /// 本幣含稅銷貨金額
	  /// </summary>
	  public decimal? ORDERTOTALAMOUNT { get; set; }

	  /// <summary>
	  /// 集包地名稱
	  /// </summary>
	  public string CONCENTRATED { get; set; }

	  /// <summary>
	  /// 集包地編號
	  /// </summary>
	  public string CONCENTRATED_NO { get; set; }

	  /// <summary>
	  /// 到貨區碼
	  /// </summary>
	  public string SHIPPING_AREA_NO { get; set; }

	  /// <summary>
	  /// 收件人地址省市
	  /// </summary>
	  public string SHIPPINGCITY { get; set; }

	  /// <summary>
	  /// 報單號碼
	  /// </summary>
	  public string DECLARATION_NO { get; set; }

	  /// <summary>
	  /// 客戶車次或路線編號
	  /// </summary>
	  public string DELV_NO { get; set; }

	  /// <summary>
	  /// 賣家名稱
	  /// </summary>
	  public string SELLER_NAME { get; set; }

	  /// <summary>
	  /// 出貨模式 0: 虛出 1: 實出(物流中心需揀貨包裝)
	  /// </summary>
    [Required]
	  public string SHIPPING_FLAG { get; set; }

	  /// <summary>
	  /// 包裹重量(毛重)
	  /// </summary>
	  public string PACK_WEIGHT { get; set; }

	  /// <summary>
	  /// 保價金額
	  /// </summary>
	  public string PACK_INSURANCE { get; set; }

      public string PRINT_CUST_ORD_NO { get; set; }
      public string PRINT_MEMO { get; set; }
    }
}
        