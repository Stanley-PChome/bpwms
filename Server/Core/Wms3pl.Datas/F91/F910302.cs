namespace Wms3pl.Datas.F91
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 合約單項目明細
  /// </summary>
  [Serializable]
  [DataServiceKey("CONTRACT_NO","CONTRACT_SEQ","DC_CODE","GUP_CODE")]
  [Table("F910302")]
  public class F910302 : IAuditInfo
  {

	  /// <summary>
	  /// 合約單編號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CONTRACT_NO { get; set; }

	  /// <summary>
	  /// 合約單項目明細序號
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public Int32 CONTRACT_SEQ { get; set; }

    /// <summary>
    /// 附約合約編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SUB_CONTRACT_NO { get; set; }

	  /// <summary>
	  /// 合約分類(0主約1附約)
	  /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CONTRACT_TYPE { get; set; }

	  /// <summary>
	  /// 生效日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime ENABLE_DATE { get; set; }

	  /// <summary>
	  /// 失效日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime DISABLE_DATE { get; set; }

	  /// <summary>
	  /// 項目類別 F910003
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string ITEM_TYPE { get; set; }

    /// <summary>
    /// 項目編號(報價單編號)F910401,F500101,F500102,F500103,F500104,F500105
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string QUOTE_NO { get; set; }

	  /// <summary>
	  /// 計量單位(由報價單原資料帶出)F91000302
	  /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string UNIT_ID { get; set; }

	  /// <summary>
	  /// 作業單價
	  /// </summary>
    [Column(TypeName = "decimal(11, 2)")]
    public decimal? TASK_PRICE { get; set; }

	  /// <summary>
	  /// 標準工時(秒)
	  /// </summary>
    [Column(TypeName = "int")]
    public Int32? WORK_HOUR { get; set; }

	  /// <summary>
	  /// 加工動作 F910001
	  /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string PROCESS_ID { get; set; }

	  /// <summary>
	  /// 委外商成本價
	  /// </summary>
    [Column(TypeName = "decimal(11, 2)")]
    public decimal? OUTSOURCE_COST { get; set; }

	  /// <summary>
	  /// 貨主核定價
	  /// </summary>
    [Column(TypeName = "decimal(11, 2)")]
    public decimal? APPROVE_PRICE { get; set; }

	  /// <summary>
	  /// 物流中心(000:不指定)
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 費用(由報價單原費用帶出供合約單UI顯示用)
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string CONTRACT_FEE { get; set; }
  }
}
        