namespace Wms3pl.Datas.F07
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 混和型容器內序號資料
  /// </summary>
  [Serializable]
	[DataServiceKey("ID")]
	[Table("F07010401")]
	public class F07010401 : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }

    /// <summary>
    /// F070104的流水ID
    /// </summary>
    [Required]
		public long F070104_ID { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    [Required]
		public string ITEM_CODE { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    [Required]
    public string SERIAL_NO { get; set; }

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
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }

		/// <summary>
		/// 異動人員
		/// </summary>
		public string UPD_STAFF { get; set; }

		/// <summary>
		/// 異動人名
		/// </summary>
		public string UPD_NAME { get; set; }
    
	}
}
