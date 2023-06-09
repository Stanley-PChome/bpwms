namespace Wms3pl.Datas.F02
{
    using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 進倉上架歷程表
  /// </summary>
  [Serializable]
  [DataServiceKey("ID")]
  [Table("F020202")]
  public class F020202 : IAuditInfo
  {

    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public Int64 ID { get; set; }

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
    /// 進倉單號
    /// </summary>
    [Required]
	public string STOCK_NO { get; set; }

    /// <summary>
    /// 進倉序號
    /// </summary>
    [Required]
	public Int32 STOCK_SEQ { get; set; }

    /// <summary>
    /// 驗收單號
    /// </summary>
    [Required]
	public string RT_NO { get; set; }

    /// <summary>
    /// 驗收序號
    /// </summary>
    [Required]
	public string RT_SEQ { get; set; }

    /// <summary>
    /// 調撥單號
    /// </summary>
    [Required]
    public string ALLOCATION_NO { get; set; }

    /// <summary>
    /// 調撥序號
    /// </summary>
    [Required]
    public Int16 ALLOCATION_SEQ { get; set; }

    /// <summary>
    /// 倉別
    /// </summary>
    [Required]
    public string WAREHOUSE_ID { get; set; }

    /// <summary>
    /// 儲位
    /// </summary>
    [Required]
    public string LOC_CODE { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 有效期限
    /// </summary>
    [Required]
    public DateTime VALID_DATE { get; set; }

    /// <summary>
    /// 入庫日
    /// </summary>
    [Required]
    public DateTime ENTER_DATE { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    [Required]
    public string MAKE_NO { get; set; }

    /// <summary>
    /// 廠商編號
    /// </summary>
    [Required]
    public string VNR_CODE { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    public string SERIAL_NO { get; set; }

    /// <summary>
    /// 箱號
    /// </summary>
    [Required]
    public string BOX_CTRL_NO { get; set; }

    /// <summary>
    /// 板號
    /// </summary>
    [Required]
    public string PALLET_CTRL_NO { get; set; }

    /// <summary>
    /// 上架數
    /// </summary>
    [Required]
    public Int64 TAR_QTY { get; set; }
    /// <summary>
    /// 0:建立;1:已回傳;9:取消
    /// </summary>
    [Required]
    public string STATUS { get; set; }

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
        