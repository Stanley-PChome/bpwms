namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 快速加工-揀料與上架回倉調撥單關聯檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","PROCESS_NO","PICK_LOC","VALID_DATE","ENTER_DATE","MAKE_NO","BOX_CTRL_NO","PALLET_CTRL_NO","SERIAL_NO","ALLOCATION_NO")]
  [Table("F910209")]
  public class F910209 : IAuditInfo
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
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 加工單號
	  /// </summary>
    [Key]
    [Required]
	  public string PROCESS_NO { get; set; }

	  /// <summary>
	  /// 揀料儲位
	  /// </summary>
    [Key]
    [Required]
	  public string PICK_LOC { get; set; }

	  /// <summary>
	  /// 揀料效期
	  /// </summary>
    [Key]
    [Required]
	  public DateTime VALID_DATE { get; set; }

	  /// <summary>
	  /// 揀料入庫日
	  /// </summary>
    [Key]
    [Required]
	  public DateTime ENTER_DATE { get; set; }

	  /// <summary>
	  /// 揀料批號
	  /// </summary>
    [Key]
    [Required]
	  public string MAKE_NO { get; set; }

	  /// <summary>
	  /// 揀料箱號
	  /// </summary>
    [Key]
    [Required]
	  public string BOX_CTRL_NO { get; set; }

	  /// <summary>
	  /// 揀料板號
	  /// </summary>
    [Key]
    [Required]
	  public string PALLET_CTRL_NO { get; set; }

		/// <summary>
		/// 序號
		/// </summary>
		[Key]
		[Required]
		public string SERIAL_NO { get; set; }

		/// <summary>
		/// 回倉商品類型(0成品1原料)
		/// </summary>
		[Required]
	  public string BACK_ITEM_TYPE { get; set; }

	  /// <summary>
	  /// 回倉調撥單號
	  /// </summary>
    [Key]
    [Required]
	  public string ALLOCATION_NO { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

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
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }
  }
}
        