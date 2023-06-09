namespace Wms3pl.Datas.F25
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 序號展延記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LOG_SEQ")]
  [Table("F250105")]
  public class F250105 : IAuditInfo
  {

	  /// <summary>
	  /// 紀錄序號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
    [Required]
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 序號原效期
	  /// </summary>
    [Required]
	  public DateTime ORG_VALID_DATE { get; set; }

	  /// <summary>
	  /// 展延效期
	  /// </summary>
	  public DateTime? VALID_DATE { get; set; }

	  /// <summary>
	  /// 序號原狀態
	  /// </summary>
	  public string SERIAL_STATUS { get; set; }

	  /// <summary>
	  /// 是否通過(0否1是)
	  /// </summary>
    [Required]
	  public string ISPASS { get; set; }

	  /// <summary>
	  /// 刷驗訊息
	  /// </summary>
	  public string MESSAGE { get; set; }

	  /// <summary>
	  /// 狀態(0待展延1已展延9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 刷讀電腦IP
	  /// </summary>
    [Required]
	  public string CLIENT_IP { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
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
  }
}
        