namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉序號明細檔
  /// </summary>
  [Serializable]
  [DataServiceKey("FILE_NAME","SERIAL_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F020302")]
  public class F020302 : IAuditInfo
  {

	  /// <summary>
	  /// 匯入檔名
	  /// </summary>
    [Key]
    [Required]
	  public string FILE_NAME { get; set; }

	  /// <summary>
	  /// PO單號
	  /// </summary>
    [Required]
	  public string PO_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 序號
	  /// </summary>
    [Key]
    [Required]
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 序號長度
	  /// </summary>
    [Required]
	  public Int16 SERIAL_LEN { get; set; }

	  /// <summary>
	  /// 有效日期
	  /// </summary>
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 狀態(0待匯入1已匯入)
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
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 建檔日期
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
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 門號
	  /// </summary>
	  public string CELL_NUM { get; set; }

	  /// <summary>
	  /// PUK
	  /// </summary>
	  public string PUK { get; set; }

	  /// <summary>
	  /// 儲值卡盒號
	  /// </summary>
	  public string BATCH_NO { get; set; }
  }
}
        