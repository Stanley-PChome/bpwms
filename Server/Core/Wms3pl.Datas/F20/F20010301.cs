namespace Wms3pl.Datas.F20
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 商品庫存調整序號刷讀紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ADJUST_NO","LOG_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F20010301")]
  public class F20010301 : IAuditInfo
  {

	  /// <summary>
	  /// 調整單號
	  /// </summary>
    [Key]
    [Required]
	  public string ADJUST_NO { get; set; }

	  /// <summary>
	  /// 調整單序號
	  /// </summary>
    [Required]
	  public Int32 ADJUST_SEQ { get; set; }

	  /// <summary>
	  /// 紀錄流水號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 序號狀態(F000904 Where Topic=F2501 and SubTopic=Status)
	  /// </summary>
	  public string SERIAL_STATUS { get; set; }

	  /// <summary>
	  /// 門號
	  /// </summary>
	  public string CELL_NUM { get; set; }

	  /// <summary>
	  /// PUK
	  /// </summary>
	  public string PUK { get; set; }

	  /// <summary>
	  /// 是否通過
	  /// </summary>
    [Required]
	  public string ISPASS { get; set; }

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
        