namespace Wms3pl.Datas.F06
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;
    using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 出貨明細人員明細紀錄資料檔
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F06020702")]
    public class F06020702 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

		/// <summary>
		/// F060207流水號
		/// </summary>
		[Required]
    [Column(TypeName = "bigint")]
    public long F060207_ID { get; set; }

    /// <summary>
    /// 貨架編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SHELF_CODE { get; set; }

    /// <summary>
    /// 儲位編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string BIN_CODE { get; set; }

		/// <summary>
		/// 揀出數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int SKUQTY { get; set; }

		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 建立人員編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

		/// <summary>
		/// 建立人員名稱
		/// </summary>
		[Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }


    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

    /// <summary>
    /// 異動人員編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人員名稱
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
    }
}
