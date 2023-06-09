namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 儲位異動LOG
  /// </summary>
  [Serializable]
  [DataServiceKey("SEQ")]
  [Table("F191301")]
  public class F191301 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
	  public Int64 SEQ { get; set; }

	  /// <summary>
	  /// 異動項目(F000904) (topic=F191301, subtopic=action) 新增/調整/刪除
	  /// </summary>
	  public string WH_FIELD { get; set; }

	  /// <summary>
	  /// 異動原因(F000904) (topic=F191301, subtopic=whmovement)
	  /// </summary>
	  public string WH_REASON { get; set; }

	  /// <summary>
	  /// 新值
	  /// </summary>
	  public string NEW_VALUE { get; set; }

	  /// <summary>
	  /// 儲位
	  /// </summary>
	  public string LOC_CODE { get; set; }

	  /// <summary>
	  /// 品號
	  /// </summary>
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 數量
	  /// </summary>
	  public Int64? QTY { get; set; }

	  /// <summary>
	  /// 效期
	  /// </summary>
	  public DateTime? VALID_DATE { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 商品序號
	  /// </summary>
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 箱號
	  /// </summary>
	  public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 板號
	  /// </summary>
	  public string PALLET_CTRL_NO { get; set; }

	  /// <summary>
	  /// 單據號碼(WMS_NO)
	  /// </summary>
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

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
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 批號
	  /// </summary>
	  public string MAKE_NO { get; set; }
  }
}
        