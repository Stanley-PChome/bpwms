namespace Wms3pl.Datas.F15
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 調撥序號刷讀紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ALLOCATION_NO","LOG_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F15100101")]
  public class F15100101 : IAuditInfo
  {

	  /// <summary>
	  /// 調撥單號
	  /// </summary>
    [Key]
    [Required]
	  public string ALLOCATION_NO { get; set; }

	  /// <summary>
	  /// 調撥單序號
	  /// </summary>
    [Required]
	  public Int16 ALLOCATION_SEQ { get; set; }

	  /// <summary>
	  /// 紀錄流水號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 序號狀態(F000904 Where Topic=F2501 and SubTopic=Status)
	  /// </summary>
	  public string SERIAL_STATUS { get; set; }

	  /// <summary>
	  /// 刷讀儲位
	  /// </summary>
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 是否通過
	  /// </summary>
    [Required]
	  public string ISPASS { get; set; }

	  /// <summary>
	  /// 狀態(0預設狀態 9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 刷驗訊息
	  /// </summary>
	  public string MESSAGE { get; set; }

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
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }
  }
}
        