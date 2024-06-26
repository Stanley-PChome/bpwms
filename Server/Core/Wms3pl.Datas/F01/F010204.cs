namespace Wms3pl.Datas.F01
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

    /// <summary>
    /// 進倉驗收上架結果表
    /// </summary>
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F010204")]
  public class F010204 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "bigint")]
    public Int64 ID { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }

    /// <summary>
    /// 業主
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 貨主
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 進倉單號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string STOCK_NO { get; set; }

    /// <summary>
    /// 進倉序號
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 STOCK_SEQ { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 訂購數=預計收貨數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 STOCK_QTY { get; set; }

    /// <summary>
    /// 累積驗收數含不良品數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 TOTAL_REC_QTY { get; set; }

    /// <summary>
    /// 累積驗收不良品數
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public Int32 TOTAL_DEFECT_RECV_QTY { get; set; }

    /// <summary>
    /// 累積上架數含不良品上架數
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 TOTAL_TAR_QTY { get; set; }

    /// <summary>
    /// 累積不良品上架數
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 TOTAL_DEFECT_TAR_QTY { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建立人名
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
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
    }
}
        