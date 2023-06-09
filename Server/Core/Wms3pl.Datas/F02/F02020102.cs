namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉商品檢驗紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PURCHASE_NO","PURCHASE_SEQ","CHECK_NO","DC_CODE","GUP_CODE","CUST_CODE","RT_NO")]
  [Table("F02020102")]
  public class F02020102 : IAuditInfo
  {

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Key]
    [Required]
	  public string PURCHASE_NO { get; set; }

	  /// <summary>
	  /// 進倉單序號
	  /// </summary>
    [Key]
    [Required]
	  public string PURCHASE_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 商品檢驗項目編號
	  /// </summary>
    [Key]
    [Required]
	  public string CHECK_NO { get; set; }

	  /// <summary>
	  /// 是否通過
	  /// </summary>
    [Required]
	  public string ISPASS { get; set; }

	  /// <summary>
	  /// 原因編號(F1951)
	  /// </summary>
	  public string UCC_CODE { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

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

	  /// <summary>
	  /// 驗收單號
	  /// </summary>
    [Key]
    [Required]
	  public string RT_NO { get; set; }
  }
}
        