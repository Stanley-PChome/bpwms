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
    [Table("F060208")]
    public class F060208 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE	{ get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(12)")]
    public string CUST_CODE	{ get; set; }
		/// <summary>
		/// 容器編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string CONTAINER_CODE { get; set; }
		/// <summary>
		/// 容器類型
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(2)")]
    public string CONTAINER_TYPE { get; set; }
		/// <summary>
		/// 目前抵達位置
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string POSITION_CODE { get; set; }
		/// <summary>
		/// 目的位置
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string TARGET_POS_CODE { get; set; }
		/// <summary>
		/// 紀錄時間
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime CREATE_TIME { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string ORI_ORDER_CODE{ get; set; }
		/// <summary>
		/// 單據容器總箱數
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int BOX_TOTAL { get; set; }
		/// <summary>
		/// 單據容器箱序
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int BOX_SERIAL { get; set; }
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
		/// <summary>
		/// 作業狀態((0: 等待中、1:已抵達、2:已完成))
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int PROC_FLAG { get; set; }
	}
}
