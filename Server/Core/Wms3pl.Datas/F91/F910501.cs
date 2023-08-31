namespace Wms3pl.Datas.F91
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 加工Device設定
	/// </summary>
	[Serializable]
	[DataServiceKey("DEVICE_IP", "DC_CODE")]
	[Table("F910501")]
	public class F910501 : IAuditInfo
	{

		/// <summary>
		/// 加工線 IP
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(50)")]
    public string DEVICE_IP { get; set; }

    /// <summary>
    /// 秤重機路徑
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string WEIGHING { get; set; }

    /// <summary>
    /// 印表機路徑
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string PRINTER { get; set; }

    /// <summary>
    /// 標籤機路徑
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string LABELING { get; set; }

    /// <summary>
    /// 錄影主機網址
    /// </summary>
    [Column(TypeName = "varchar(150)")]
    public string VIDEO_URL { get; set; }

    /// <summary>
    /// 錄影機台
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string VIDEO_NO { get; set; }

		/// <summary>
		/// 是否忽略錄影錯誤0否1是
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string VIDEO_ERROR { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

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
		/// 建立人名
		/// </summary>
		[Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 點陣式印表機路徑
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string MATRIX_PRINTER { get; set; }

		/// <summary>
		/// 是否忽略秤重機錯誤0否1是
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(1)")]
    public string WEIGHING_ERROR { get; set; }

		/// <summary>
		/// 是否不執行AGV
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string NOTRUNAGV { get; set; }

		/// <summary>
		/// 不使用CAPS
		/// </summary>
		[Required]
    [Column(TypeName = "char(1)")]
    public string NOTCAPS { get; set; }
    /// <summary>
    /// 工作站類型
    /// </summary>
    [Column(TypeName = "varchar(4)")]
    public string WORKSTATION_TYPE { get; set; }
    /// <summary>
    /// 工作站編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string WORKSTATION_CODE { get; set; }
    /// <summary>
    /// 工作站群組 (撈F000904、topic=F1946、subtopic=group
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string WORKSTATION_GROUP { get; set; }
    /// <summary>
    /// 配箱站與封箱站分開
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string NO_SPEC_REPROTS { get; set; }
    /// <summary>
    /// 需刷讀紙箱條碼關箱
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string CLOSE_BY_BOXNO { get; set; }
	}
}
