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
    [Table("F06020701")]
    public class F06020701 : IAuditInfo
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
		/// 出貨任務單號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(32)")]
    public string ORDERCODE { get; set; }

		/// <summary>
		/// 明細項次
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int ROWNUM { get; set; }

		/// <summary>
		/// 庫內品號
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string SKUCODE { get; set; }

		/// <summary>
		/// 裝箱數量
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int SKUQTY { get; set; }

		/// <summary>
		/// 商品等級(0=殘品 1=正品)
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int SKULEVEL { get; set; }

    /// <summary>
    /// 商品效期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? EXPIRYDATE { get; set; }

    /// <summary>
    /// 外部批次號(入庫日(yyMMdd)+序號(3碼數字) 或)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string OUTBATCHCODE { get; set; }

    /// <summary>
    /// 商品序號清單 (紀錄本箱中的序號)
    /// </summary>
    [Column(TypeName = "varchar(MAX)")]
    public string SERIALNUMLIST { get; set; }

    /// <summary>
    /// 容器分隔編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string BINCODE { get; set; }

		/// <summary>
		/// 揀貨完成時間
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime COMPLETE_TIME { get; set; }

		/// <summary>
		/// 是否出庫單最後一箱(0=否 1=是)
		/// </summary>
		[Required]
    [Column(TypeName = "int")]
    public int ISLASTCONTAINER { get; set; }

    /// <summary>
    /// 出貨單總箱數((出庫單最後一箱時顯示))
    /// </summary>
    [Column(TypeName = "int")]
    public int? CONTAINER_TOTAL { get; set; }

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
