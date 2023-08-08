namespace Wms3pl.Datas.F00
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 使用者資料庫使用記錄檔
  /// </summary>
  [Serializable]
  [DataServiceKey("SEQ_NO")]
  [Table("F005001")]
  public class F005001 : IAuditInfo
  {

	  /// <summary>
	  /// 紀錄流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Decimal SEQ_NO { get; set; }

	  /// <summary>
	  /// 紀錄時間
	  /// </summary>
    [Required]
	  public DateTime LOG_DATE { get; set; }

	  /// <summary>
	  /// 電腦IP
	  /// </summary>
    [Required]
	  public string MACHINE { get; set; }

	  /// <summary>
	  /// 資料庫執行語法
	  /// </summary>
	  public string SQLSTR { get; set; }

	  /// <summary>
	  /// 程式編號
	  /// </summary>
	  public string FUN_ID { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建檔人名
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
        