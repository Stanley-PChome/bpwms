namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 路線配次與配送商對應表
  /// </summary>
  [Serializable]
  [DataServiceKey("ROUTE_NO","DC_CODE")]
  [Table("F194705")]
  public class F194705 : IAuditInfo
  {

	  /// <summary>
	  /// 路線對應編號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 ROUTE_NO { get; set; }

	  /// <summary>
	  /// 路線編號
	  /// </summary>
    [Required]
	  public string ROUTE_CODE { get; set; }

	  /// <summary>
	  /// 配送商編號
	  /// </summary>
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 配次
	  /// </summary>
    [Required]
	  public string DELV_TIMES { get; set; }

	  /// <summary>
	  /// 路線
	  /// </summary>
    [Required]
	  public string ROUTE { get; set; }

	  /// <summary>
	  /// 地址參考1
	  /// </summary>
	  public string ADDRESS_A { get; set; }

	  /// <summary>
	  /// 地址參考2
	  /// </summary>
	  public string ADDRESS_B { get; set; }

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
	  /// 流道
	  /// </summary>
	  public string PASSWAY { get; set; }
  }
}
        