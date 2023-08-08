namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 組合商品BOM表明細
  /// </summary>
  [Serializable]
  [DataServiceKey("BOM_NO","MATERIAL_CODE","CUST_CODE","GUP_CODE")]
  [Table("F910102")]
  public class F910102 : IAuditInfo
  {

	  /// <summary>
	  /// BOM表編號
	  /// </summary>
    [Key]
    [Required]
	  public string BOM_NO { get; set; }

	  /// <summary>
	  /// 組合商品料號(F1903.ITEM_CODE)
	  /// </summary>
    [Key]
    [Required]
	  public string MATERIAL_CODE { get; set; }

	  /// <summary>
	  /// 組合順序
	  /// </summary>
    [Required]
	  public Int16 COMBIN_ORDER { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

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
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

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
	  /// 組合數量
	  /// </summary>
    [Required]
	  public Int32 BOM_QTY { get; set; }
  }
}
        