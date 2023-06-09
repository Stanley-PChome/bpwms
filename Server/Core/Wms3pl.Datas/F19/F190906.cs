namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 系統圖片設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("IMG_NO")]
  [Table("F190906")]
  public class F190906 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
	  public Decimal IMG_NO { get; set; }

	  /// <summary>
	  /// 查詢條件
	  /// </summary>
    [Required]
	  public string IMG_KEY { get; set; }

	  /// <summary>
	  /// 檔案名稱
	  /// </summary>
    [Required]
	  public string IMG_NAME { get; set; }

	  /// <summary>
	  /// 檔案大小
	  /// </summary>
    [Required]
	  public Decimal IMG_SIZE { get; set; }

	  /// <summary>
	  /// 物流中心名稱
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

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
  }
}
        