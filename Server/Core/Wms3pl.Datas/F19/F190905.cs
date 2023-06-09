namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 通路配送商主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","ALL_ID","CHANNEL","SUBCHANNEL")]
  [Table("F190905")]
  public class F190905 : IAuditInfo
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
	  /// 貨主
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 配送商
	  /// </summary>
    [Key]
    [Required]
	  public string ALL_ID { get; set; }

	  /// <summary>
	  /// 通路上編號
	  /// </summary>
    [Key]
    [Required]
	  public string CHANNEL { get; set; }

	  /// <summary>
	  /// 通路商名稱
	  /// </summary>
    [Required]
	  public string CHANNEL_NAME { get; set; }

	  /// <summary>
	  /// 通路商地址
	  /// </summary>
    [Required]
	  public string CHANNEL_ADDRESS { get; set; }

	  /// <summary>
	  /// 通路商電話
	  /// </summary>
    [Required]
	  public string CHANNEL_TEL { get; set; }

	  /// <summary>
	  /// 預設配送箱子尺寸(cm)
	  /// </summary>
	  public Int16? DEFAULT_BOXSIZE { get; set; }

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
	  /// 子通路編號
	  /// </summary>
    [Key]
    [Required]
	  public string SUBCHANNEL { get; set; }

	  /// <summary>
	  /// 箱明細格式(空值表示預設報表)
	  /// </summary>
	  public string DELVDTL_FORMAT { get; set; }

	  /// <summary>
	  /// 是否自動列印箱明細 0:否 1:是
	  /// </summary>
    [Required]
	  public string AUTO_PRINT_DELVDTL { get; set; }

	  /// <summary>
	  /// 是否自動列印託運單
	  /// </summary>
    [Required]
	  public string AUTO_PRINT_CONSIGN { get; set; }
  }
}
        