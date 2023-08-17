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
    [Table("F060207")]
    public class F060207 : IAuditInfo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }

		/// <summary>
		///物流中心編號(DCCODE)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號(OWNERCODE)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CUST_CODE { get; set; }

		/// <summary>
		/// 倉別編號(ZONECODE)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string WAREHOUSE_ID { get; set; }

		/// <summary>
		/// 容器編號(周轉箱號)
		/// </summary
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string CONTAINERCODE { get; set; }

		/// <summary>
		/// 工作站人員(工號)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string OPERATOR { get; set; }

    /// <summary>
    /// 工作站編號
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public string WORKSTATION_NO { get; set; }

    /// <summary>
    /// 播種格口編號
    /// </summary>
    [Column(TypeName = "varchar(32)")]
    public string SEED_BINCODE { get; set; }

		/// <summary>
		/// 明細筆數
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int SKUTOTAL { get; set; }

		/// <summary>
		/// 處理狀態(0:待處理;1:成功;2:失敗;3:不處理)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [Column(TypeName = "nvarchar(255)")]
    public string MSG_CONTENT { get; set; }
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
