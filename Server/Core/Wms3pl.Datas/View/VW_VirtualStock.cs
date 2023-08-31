namespace Wms3pl.Datas.View
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;

  /// <summary>
  /// 虛擬庫存
  /// </summary>
  [Serializable]
  [DataServiceKey("ORDER_NO", "ORDER_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
  [Table("VW_VirtualStock")]
  public class VW_VirtualStock
  {
    /// <summary>
    /// 單據號碼
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string ORDER_NO { get; set; }
    /// <summary>
    /// 序號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string ORDER_SEQ { get; set; }
    /// <summary>
    /// 狀態(0:已配庫,1:已搬動 2:已扣帳  9:取消)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }
    /// <summary>
    /// 預計揀貨數量
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int B_PICK_QTY { get; set; }
    /// <summary>
    /// 實際揀貨數量
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int A_PICK_QTY { get; set; }
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
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }
    /// <summary>
    /// 貨主
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
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
    /// 建立人名
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
    /// <summary>
    /// 商品編號
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string ITEM_CODE { get; set; }
    /// <summary>
    /// 有效期限
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? VALID_DATE { get; set; }
    /// <summary>
    /// 入庫日
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? ENTER_DATE { get; set; }
    /// <summary>
    /// 商品序號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string SERIAL_NO { get; set; }
    /// <summary>
    /// 揀貨儲位
    /// </summary>
    [Column(TypeName = "varchar(14)")]
    public string LOC_CODE { get; set; }
    /// <summary>
    /// 箱號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string BOX_CTRL_NO { get; set; }
    /// <summary>
    /// 板號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string PALLET_CTRL_NO { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(40)")]
    public string MAKE_NO { get; set; }
  }
}
