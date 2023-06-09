namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 單據匯入紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("IMPORT_LOG_ID")]
  [Table("F0060")]
  public class F0060 : IAuditInfo
  {

	  /// <summary>
	  /// 匯入流水號
	  /// </summary>
    [Key]
    [Required]
	  public Decimal IMPORT_LOG_ID { get; set; }

	  /// <summary>
	  /// 匯入檔名
	  /// </summary>
    [Required]
	  public string FILE_NAME { get; set; }

	  /// <summary>
	  /// 匯入類型(0商品主檔1進倉單2訂單3退貨單4派車單5亞東API 6廠退單)F000904
	  /// </summary>
    [Required]
	  public string FILE_TYPE { get; set; }

	  /// <summary>
	  /// 匯入主要值(ex.系統單號,商品編號)
	  /// </summary>
	  public string DATA_KEY { get; set; }

	  /// <summary>
	  /// 資料內容
	  /// </summary>
	  public string DATA_CONTENT { get; set; }

	  /// <summary>
	  /// 訊息
	  /// </summary>
	  public string MESSAGE { get; set; }

	  /// <summary>
	  /// 狀態(1成功9失敗)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
	  public string CUST_CODE { get; set; }

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
	  /// 建檔時間
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
	  /// 異動時間
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        