namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 訂單指定挑卡檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F05010102")]
  public class F05010102 : IAuditInfo
  {

	  /// <summary>
	  /// 流水號
	  /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Decimal ID { get; set; }

	  /// <summary>
	  /// 貨主單號
	  /// </summary>
    [Required]
	  public string CUST_ORD_NO { get; set; }

	  /// <summary>
	  /// 卡號序號
	  /// </summary>
    [Required]
	  public string SERIAL_NO { get; set; }

	  /// <summary>
	  /// 商品編號
	  /// </summary>
    [Required]
	  public string ITEM_CODE { get; set; }

	  /// <summary>
	  /// 門號
	  /// </summary>
    [Required]
	  public string CELL_NUM { get; set; }

	  /// <summary>
	  /// 啟用日
	  /// </summary>
	  public DateTime? ENABLE_DATE { get; set; }

	  /// <summary>
	  /// 出貨日(集貨日)
	  /// </summary>
	  public DateTime? SHIPPED_DATE { get; set; }

	  /// <summary>
	  /// 儲值卡盒號
	  /// </summary>
	  public string BATCH_NO { get; set; }

	  /// <summary>
	  /// 進貨日
	  /// </summary>
	  public DateTime? DELIVER_DATE { get; set; }

	  /// <summary>
	  /// 申請型態
	  /// </summary>
	  public string APPLICATION_PATTERN { get; set; }

	  /// <summary>
	  /// SOA預約生效日
	  /// </summary>
	  public DateTime? SOA_DATE { get; set; }

	  /// <summary>
	  /// 時間
	  /// </summary>
	  public string SOA_TIME { get; set; }

	  /// <summary>
	  /// 出貨是否有SA(0:否，1:是)
	  /// </summary>
    [Required]
	  public string SA { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主
	  /// </summary>
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主
	  /// </summary>
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 建檔日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 建檔人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建檔人名
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
        