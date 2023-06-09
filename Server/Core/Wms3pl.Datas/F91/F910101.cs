namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 組合商品BOM表
  /// </summary>
  [Serializable]
  [DataServiceKey("BOM_NO","CUST_CODE","GUP_CODE")]
  [Table("F910101")]
  public class F910101 : IAuditInfo
  {

	  /// <summary>
	  /// 組合編號
	  /// </summary>
    [Key]
    [Required]
	  public string BOM_NO { get; set; }

	  /// <summary>
	  /// 成品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 組合類別0組合 1拆解
	  /// </summary>
    [Required]
	  public string BOM_TYPE { get; set; }

	  /// <summary>
	  /// 組合名稱
	  /// </summary>
    [Required]
	  public string BOM_NAME { get; set; }

	  /// <summary>
	  /// 單位F910002
	  /// </summary>
    [Required]
	  public string UNIT_ID { get; set; }

	  /// <summary>
	  /// 抽驗比例
	  /// </summary>
    [Required]
	  public Single CHECK_PERCENT { get; set; }

	  /// <summary>
	  /// 規格說明
	  /// </summary>
	  public string SPEC_DESC { get; set; }

	  /// <summary>
	  /// 包裝敘述
	  /// </summary>
	  public string PACKAGE_DESC { get; set; }

	  /// <summary>
	  /// 組合商品狀態(0使用中 9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

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
	  /// 是否加工(0:不加工,1:加工)
	  /// </summary>
    [Required]
	  public string ISPROCESS { get; set; }
  }
}
        