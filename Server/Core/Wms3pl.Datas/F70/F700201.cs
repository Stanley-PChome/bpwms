namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 客訴登錄
  /// </summary>
  [Serializable]
  [DataServiceKey("COMPLAINT_NO","DC_CODE")]
  [Table("F700201")]
  public class F700201 : IAuditInfo
  {

	  /// <summary>
	  /// 客訴編號
	  /// </summary>
    [Key]
    [Required]
	  public string COMPLAINT_NO { get; set; }

	  /// <summary>
	  /// 客訴日期
	  /// </summary>
    [Required]
	  public DateTime COMPLAINT_DATE { get; set; }

	  /// <summary>
	  /// 客戶編號F1910
	  /// </summary>
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 客戶名稱
	  /// </summary>
    [Required]
	  public string CUST_NAME { get; set; }

	  /// <summary>
	  /// 反應人員
	  /// </summary>
	  public string COMPLAINT_NAME { get; set; }

	  /// <summary>
	  /// 反應主題F700203
	  /// </summary>
    [Required]
	  public string COMPLAINT_TYPE { get; set; }

	  /// <summary>
	  /// 問題商品描述
	  /// </summary>
	  public string COMPLAINT_DESC { get; set; }

	  /// <summary>
	  /// 數量
	  /// </summary>
    [Required]
	  public Int32 QTY { get; set; }

	  /// <summary>
	  /// 處理人員單位F1925
	  /// </summary>
	  public string DEP_ID { get; set; }

	  /// <summary>
	  /// 處理人員
	  /// </summary>
	  public string HANDLE_STAFF { get; set; }

	  /// <summary>
	  /// 問題反應描述
	  /// </summary>
	  public string RESPOND_DESC { get; set; }

	  /// <summary>
	  /// 處理狀況描述
	  /// </summary>
	  public string HANDLE_DESC { get; set; }

	  /// <summary>
	  /// 處理狀態(0處理中1結案)F000904
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立時間
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
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        