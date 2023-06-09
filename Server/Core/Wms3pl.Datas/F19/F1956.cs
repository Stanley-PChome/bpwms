namespace Wms3pl.Datas.F19
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 出貨優先權代碼檔 
  /// </summary>
  [Serializable]
	[DataServiceKey("PRIORITY_CODE")]
	[Table("F1956")]
	public class F1956 : IAuditInfo
	{
    /// <summary>
    /// 出貨優先權代碼
    /// </summary>
    [Key]
    [Required]
    public string PRIORITY_CODE { get; set; }
    /// <summary>
    /// 出貨優先權名稱
    /// </summary>
    [Required]
    public string PRIORITY_NAME { get; set; }
    /// <summary>
    /// 是否提供人員選擇(0:否;1:是)
    /// </summary>
    [Required]
    public string IS_SHOW { get; set; }
    /// <summary>
    /// 是否為系統預設代碼(0:否;1:是)
    /// </summary>
    [Required]
    public string IS_SYSTEM { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }
    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 異動人員編號
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 異動人員名稱
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
