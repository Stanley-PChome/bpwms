namespace Wms3pl.Datas.F02
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 驗收上傳檔案紀錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("RT_NO","UPLOAD_TYPE","UPLOAD_NO","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F02020105")]
  public class F02020105 : IAuditInfo
  {

	  /// <summary>
	  /// 驗收單號
	  /// </summary>
    [Key]
    [Required]
	  public string RT_NO { get; set; }

	  /// <summary>
	  /// 進倉單號
	  /// </summary>
    [Required]
	  public string PURCHASE_NO { get; set; }

	  /// <summary>
	  /// 上傳檔案類型
	  /// </summary>
    [Key]
    [Required]
	  public string UPLOAD_TYPE { get; set; }

	  /// <summary>
	  /// 上傳檔案編號
	  /// </summary>
    [Key]
    [Required]
	  public Int16 UPLOAD_NO { get; set; }

	  /// <summary>
	  /// 上傳檔案主機路徑
	  /// </summary>
    [Required]
	  public string UPLOAD_S_PATH { get; set; }

	  /// <summary>
	  /// 上傳檔案原路徑
	  /// </summary>
    [Required]
	  public string UPLOAD_C_PATH { get; set; }

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
  }
}
        