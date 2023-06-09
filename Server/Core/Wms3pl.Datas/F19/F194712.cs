namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// EGS業主託運設定
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","ALL_ID","CUSTOMER_ID","CONSIGN_TYPE","ISTEST","CUST_CODE","CHANNEL")]
  [Table("F194712")]
  public class F194712 : IAuditInfo
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
	  /// 配送商編號(F1947)
	  /// </summary>
    [Key]
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 契客代號
	  /// </summary>
    [Key]
    [Required]
	  public string CUSTOMER_ID { get; set; }

	  /// <summary>
	  /// 託單類別(A:一般託運單,B:代收託運單)
	  /// </summary>
    [Key]
    [Required]
	  public string CONSIGN_TYPE { get; set; }

	  /// <summary>
	  /// 安全存量
	  /// </summary>
    [Required]
	  public Int32 SAVE_QTY { get; set; }

	  /// <summary>
	  /// 每次產生託運單數量
	  /// </summary>
    [Required]
	  public Int32 PATCH_QTY { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人員名稱
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 是否測試(0:否 1:是)
	  /// </summary>
    [Key]
    [Required]
	  public string ISTEST { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 通路
	  /// </summary>
    [Key]
    [Required]
	  public string CHANNEL { get; set; }
  }
}
        