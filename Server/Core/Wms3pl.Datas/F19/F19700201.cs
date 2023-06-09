namespace Wms3pl.Datas.F19
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 板箱標籤記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("SEQ_NO")]
  [Table("F19700201")]
  public class F19700201 : IAuditInfo
  {

	  /// <summary>
	  /// 序號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Decimal SEQ_NO { get; set; }

	  /// <summary>
	  /// 年度
	  /// </summary>
    [Required]
	  public string YEAR { get; set; }

	  /// <summary>
	  /// 箱板號起
	  /// </summary>
    [Required]
	  public string START_NO { get; set; }

	  /// <summary>
	  /// 箱板號迄
	  /// </summary>
    [Required]
	  public string END_NO { get; set; }

	  /// <summary>
	  /// 列印類型(0:板標 1:箱標)
	  /// </summary>
    [Required]
	  public string LABEL_TYPE { get; set; }

	  /// <summary>
	  /// 列印張數
	  /// </summary>
    [Required]
	  public Decimal QTY { get; set; }

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
        