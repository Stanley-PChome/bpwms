namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 標籤主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("LABEL_CODE","CUST_CODE","GUP_CODE")]
  [Table("F1970")]
  public class F1970 : IAuditInfo
  {

	  /// <summary>
	  /// 標籤編號(與報表編號相同)
	  /// </summary>
    [Key]
    [Required]
	  public string LABEL_CODE { get; set; }

	  /// <summary>
	  /// 標籤類型(0物料標1保固標2序號標4其他5託運單)F000904
	  /// </summary>
    [Required]
	  public string LABEL_TYPE { get; set; }

	  /// <summary>
	  /// 標籤名稱
	  /// </summary>
    [Required]
	  public string LABEL_NAME { get; set; }

	  /// <summary>
	  /// 狀態(0使用中9刪除)
	  /// </summary>
    [Required]
	  public string STATUS { get; set; }

	  /// <summary>
	  /// 是否列印商品資訊
	  /// </summary>
    [Required]
	  public string ITEM { get; set; }

	  /// <summary>
	  /// 是否列印廠商資訊
	  /// </summary>
    [Required]
	  public string VNR { get; set; }

	  /// <summary>
	  /// 是否列印保固期限
	  /// </summary>
    [Required]
	  public string WARRANTY { get; set; }

	  /// <summary>
	  /// 是否列印保固年份
	  /// </summary>
    [Required]
	  public string WARRANTY_Y { get; set; }

	  /// <summary>
	  /// 是否列印保固月份
	  /// </summary>
    [Required]
	  public string WARRANTY_M { get; set; }

	  /// <summary>
	  /// 是否列印保固日期
	  /// </summary>
    [Required]
	  public string WARRANTY_D { get; set; }

	  /// <summary>
	  /// 是否列印委外商
	  /// </summary>
    [Required]
	  public string OUTSOURCE { get; set; }

	  /// <summary>
	  /// 是否列印檢驗員
	  /// </summary>
    [Required]
	  public string CHECK_STAFF { get; set; }

	  /// <summary>
	  /// 是否列印物料說明
	  /// </summary>
    [Required]
	  public string ITEM_DESC { get; set; }

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
  }
}
        