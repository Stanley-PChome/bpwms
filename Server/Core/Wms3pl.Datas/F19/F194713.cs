namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 超取服務商主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","ALL_ID","ESERVICE")]
  [Table("F194713")]
  public class F194713 : IAuditInfo
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
	  /// 配送商F1947 只允許TYPE=1(超取配送商)
	  /// </summary>
    [Key]
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 服務商編號
	  /// </summary>
    [Key]
    [Required]
	  public string ESERVICE { get; set; }

	  /// <summary>
	  /// 服務商名稱
	  /// </summary>
    [Required]
	  public string ESERVICE_NAME { get; set; }

	  /// <summary>
	  /// 母廠商編號
	  /// </summary>
    [Required]
	  public string ESHOP { get; set; }

	  /// <summary>
	  /// 子廠商編號
	  /// </summary>
	  public string ESHOP_ID { get; set; }

	  /// <summary>
	  ///  門市進貨日
	  /// </summary>
	  public Int16? SHOP_DELV_DAY { get; set; }

	  /// <summary>
	  /// 門市退貨日
	  /// </summary>
	  public Int16? SHOP_RETURN_DAY { get; set; }

	  /// <summary>
	  /// 平台名稱
	  /// </summary>
	  public string PLATFORM_NAME { get; set; }

	  /// <summary>
	  /// 廠商名稱
	  /// </summary>
	  public string VNR_NAME { get; set; }

	  /// <summary>
	  /// 客服資訊
	  /// </summary>
	  public string CUST_INFO { get; set; }

	  /// <summary>
	  /// 出貨標籤說明1
	  /// </summary>
	  public string NOTE1 { get; set; }

	  /// <summary>
	  /// 出貨標籤說明2
	  /// </summary>
	  public string NOTE2 { get; set; }

	  /// <summary>
	  /// 出貨標籤說明3
	  /// </summary>
	  public string NOTE3 { get; set; }

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

	  /// <summary>
	  /// 已付款說明
	  /// </summary>
	  public string PAID_NOTE { get; set; }

	  /// <summary>
	  /// 未付款說明
	  /// </summary>
	  public string UNPAID_NOTE { get; set; }

	  /// <summary>
	  /// 條碼類型(0:CODE39;1:CODE128) F000904.TOPIC=F194713 SUBTOPIC=BARCODE_TYPE
	  /// </summary>
    [Required]
	  public string BARCODE_TYPE { get; set; }

	  /// <summary>
	  /// 是否加印條碼前後星號 0:否 1:是
	  /// </summary>
    [Required]
	  public string ISPRINTSTAR { get; set; }

	  /// <summary>
	  /// 廠商名稱標籤
	  /// </summary>
	  public string LAB_VNR_NAME { get; set; }

	  /// <summary>
	  /// 出貨標籤1
	  /// </summary>
	  public string LAB_NOTE1 { get; set; }

	  /// <summary>
	  /// 出貨標籤2
	  /// </summary>
	  public string LAB_NOTE2 { get; set; }

	  /// <summary>
	  /// 出貨標籤3
	  /// </summary>
	  public string LAB_NOTE3 { get; set; }

	  /// <summary>
	  /// 客服資訊標籤
	  /// </summary>
	  public string LAB_CUST_INFO { get; set; }

	  /// <summary>
	  /// 託運單報表檔名稱
	  /// </summary>
	  public string DELV_FORMAT { get; set; }
  }
}
        