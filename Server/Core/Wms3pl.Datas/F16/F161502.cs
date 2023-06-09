namespace Wms3pl.Datas.F16
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 退貨彙總表身檔
  /// </summary>
  [Serializable]
  [DataServiceKey("GATHER_NO","GATHER_SEQ","DC_CODE")]
  [Table("F161502")]
  public class F161502 : IAuditInfo
  {

	  /// <summary>
	  /// 彙總單號
	  /// </summary>
    [Key]
    [Required]
	  public string GATHER_NO { get; set; }

	  /// <summary>
	  /// 彙總單序號(項次,箱號)
	  /// </summary>
    [Key]
    [Required]
	  public string GATHER_SEQ { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

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
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

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
	  /// 商品名稱(匯入彙總表品名)
	  /// </summary>
	  public string ITEM_NAME { get; set; }

	  /// <summary>
	  /// 退貨數量(匯入彙總表退貨數)
	  /// </summary>
	  public string RTN_QTY { get; set; }
  }
}
        