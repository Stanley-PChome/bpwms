namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 裝箱刷讀紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PROCESS_NO","LOG_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F910507")]
  public class F910507 : IAuditInfo
  {

	  /// <summary>
	  /// 加工單號
	  /// </summary>
    [Key]
    [Required]
	  public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 紀錄流水號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 LOG_SEQ { get; set; }

	  /// <summary>
	  /// 加工站IP
	  /// </summary>
    [Required]
	  public string PROCESS_IP { get; set; }

	  /// <summary>
	  /// 盒號
	  /// </summary>
	  public string BOX_NO { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
	  public string CASE_NO { get; set; }

	  /// <summary>
	  /// 狀態(0:預設狀態 9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

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

	  /// <summary>
	  /// 箱重量(Kg)
	  /// </summary>
	  public decimal? WEIGHT { get; set; }
  }
}
        