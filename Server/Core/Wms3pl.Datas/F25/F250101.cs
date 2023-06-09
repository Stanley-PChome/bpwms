namespace Wms3pl.Datas.F25
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 在庫序號主檔異動紀錄
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_SEQ")]
  [Table("F250101")]
  public class F250101 : IAuditInfo
  {

	  /// <summary>
	  /// 流水序號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 紀錄時間
	  /// </summary>
    [Required]
	  public DateTime LOG_DATE { get; set; }

	  /// <summary>
	  /// 序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 商品編號(F1903)
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 盒號
	  /// </summary>
	  public string BOX_SERIAL { get; set; }

	  /// <summary>
	  /// 3G序號
	  /// </summary>
	  public string TAG3G { get; set; }

	  /// <summary>
	  /// 門號
	  /// </summary>
	  public string CELL_NUM { get; set; }

	  /// <summary>
	  /// PUK碼
	  /// </summary>
	  public string PUK { get; set; }

	  /// <summary>
	  /// 狀態(F000904)
	  /// </summary>
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 儲值卡盒號
	  /// </summary>
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 效期
	  /// </summary>
	  public DateTime? VALID_DATE { get; set; }

	  /// <summary>
	  /// PO單號
	  /// </summary>
	  public string PO_NO { get; set; }

	  /// <summary>
	  /// 卡號是否已開通(0否1是)
	  /// </summary>
	  public string ACTIVATED { get; set; }

	  /// <summary>
	  /// 序號是否已上傳給客戶(0否1是)
	  /// </summary>
	  public string SEND_CUST { get; set; }

	  /// <summary>
	  /// 廠商F1908
	  /// </summary>
	  public string VNR_CODE { get; set; }

	  /// <summary>
	  /// 系統商
	  /// </summary>
	  public string SYS_VNR { get; set; }

	  /// <summary>
	  /// 系統單號
	  /// </summary>
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 加工單號
	  /// </summary>
	  public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 異動類型F000903
	  /// </summary>
	  public string ORD_PROP { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
	  public string CASE_NO { get; set; }

	  /// <summary>
	  /// 進貨日期
	  /// </summary>
	  public DateTime? IN_DATE { get; set; }

	  /// <summary>
	  /// 客戶代號
	  /// </summary>
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// BOUNDEL_ID組合商品編號
	  /// </summary>
	  public Int64? COMBIN_NO { get; set; }

	  /// <summary>
	  /// 攝影機編號F192404
	  /// </summary>
	  public string CAMERA_NO { get; set; }

	  /// <summary>
	  /// 電腦編號
	  /// </summary>
	  public string CLIENT_IP { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
	  public string CUST_CODE { get; set; }

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
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

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
	  /// 組合成品編號
	  /// </summary>
	  public string BOUNDLE_ITEM_CODE { get; set; }
  }
}
        