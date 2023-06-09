namespace Wms3pl.Datas.F70
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 派車單明細
  /// </summary>
  [Serializable]
  [DataServiceKey("DISTR_CAR_NO","DISTR_CAR_SEQ","DC_CODE","GUP_CODE","CUST_CODE")]
  [Table("F700102")]
  public class F700102 : IAuditInfo
  {

	  /// <summary>
	  /// 派車單號
	  /// </summary>
    [Key]
    [Required]
	  public string DISTR_CAR_NO { get; set; }

	  /// <summary>
	  /// 派車單序號
	  /// </summary>
    [Key]
    [Required]
	  public Int32 DISTR_CAR_SEQ { get; set; }

	  /// <summary>
	  /// 客戶代號F1910
	  /// </summary>
	  public string RETAIL_CODE { get; set; }

	  /// <summary>
	  /// 客戶名稱
	  /// </summary>
    [Required]
	  public string CUST_NAME { get; set; }

	  /// <summary>
	  /// 委託單位
	  /// </summary>
	  public string ENTRUST_DEPT { get; set; }

	  /// <summary>
	  /// 地址
	  /// </summary>
    [Required]
	  public string ADDRESS { get; set; }

	  /// <summary>
	  /// 聯絡人
	  /// </summary>
    [Required]
	  public string CONTACT { get; set; }

	  /// <summary>
	  /// 連絡電話
	  /// </summary>
    [Required]
	  public string CONTACT_TEL { get; set; }

	  /// <summary>
	  /// 取件時間(F194701)
	  /// </summary>
    [Required]
	  public string TAKE_TIME { get; set; }

	  /// <summary>
	  /// 商品數量
	  /// </summary>
    [Required]
	  public Int32 ITEM_QTY { get; set; }

	  /// <summary>
	  /// 派車用途(02取件(逆物流),01送件(正物流))F000904
	  /// </summary>
    [Required]
	  public string DISTR_USE { get; set; }

	  /// <summary>
	  /// 物流單據類別F000901
	  /// </summary>
	  public string ORD_TYPE { get; set; }

	  /// <summary>
	  /// 物流單號
	  /// </summary>
	  public string WMS_NO { get; set; }

	  /// <summary>
	  /// 備註
	  /// </summary>
	  public string MEMO { get; set; }

	  /// <summary>
	  /// 物流中心
	  /// </summary>
    [Key]
    [Required]
	  public string DC_CODE { get; set; }

	  /// <summary>
	  /// 業主編號
	  /// </summary>
    [Key]
    [Required]
	  public string GUP_CODE { get; set; }

	  /// <summary>
	  /// 貨主編號
	  /// </summary>
    [Key]
    [Required]
	  public string CUST_CODE { get; set; }

	  /// <summary>
	  /// 建立人員
	  /// </summary>
    [Required]
	  public string CRT_STAFF { get; set; }

	  /// <summary>
	  /// 建立日期
	  /// </summary>
    [Required]
	  public DateTime CRT_DATE { get; set; }

	  /// <summary>
	  /// 異動人員
	  /// </summary>
	  public string UPD_STAFF { get; set; }

	  /// <summary>
	  /// 異動日期
	  /// </summary>
	  public DateTime? UPD_DATE { get; set; }

	  /// <summary>
	  /// 建立人名
	  /// </summary>
    [Required]
	  public string CRT_NAME { get; set; }

	  /// <summary>
	  /// 異動人名
	  /// </summary>
	  public string UPD_NAME { get; set; }

	  /// <summary>
	  /// 裝車車號A
	  /// </summary>
	  public string CAR_NO_A { get; set; }

	  /// <summary>
	  /// 裝車車號B
	  /// </summary>
	  public string CAR_NO_B { get; set; }

	  /// <summary>
	  /// 裝車車號C
	  /// </summary>
	  public string CAR_NO_C { get; set; }

	  /// <summary>
	  /// 是否已上傳封條(0否1是)
	  /// </summary>
    [Required]
	  public string ISSEAL { get; set; }

	  /// <summary>
	  /// 配送效率F190102
	  /// </summary>
    [Required]
	  public string DELV_EFFIC { get; set; }

	  /// <summary>
	  /// 是否快速到貨(0非當配1當配)F000904 Where Topic=F194701
	  /// </summary>
    [Required]
	  public string CAN_FAST { get; set; }

	  /// <summary>
	  /// 配送溫層(A常溫 B低溫)F000904 Where Topic=F194701
	  /// </summary>
    [Required]
	  public string DELV_TMPR { get; set; }

	  /// <summary>
	  /// 郵遞區號F1934
	  /// </summary>
	  public string ZIP_CODE { get; set; }

	  /// <summary>
	  /// 材積
	  /// </summary>
	  public decimal? VOLUMN { get; set; }

	  /// <summary>
	  /// 派車類型F1951 Where UctId=DR
	  /// </summary>
	  public string DISTR_TYPE { get; set; }

	  /// <summary>
	  /// 成本中心
	  /// </summary>
	  public string COST_CENTER { get; set; }

	  /// <summary>
	  /// 路線代碼F194705 Where AllId
	  /// </summary>
	  public string ROUTE_CODE { get; set; }

	  /// <summary>
	  /// 回件單號
	  /// </summary>
	  public string BACK_PAST_NO { get; set; }

	  /// <summary>
	  /// 回倉日期
	  /// </summary>
	  public DateTime? BACK_DATE { get; set; }

	  /// <summary>
	  /// 外部單號(貨主單號)
	  /// </summary>
	  public string CUST_ORD_NO { get; set; }

	  /// <summary>
	  /// 配次F194701
	  /// </summary>
    [Required]
	  public string DELV_TIMES { get; set; }

	  /// <summary>
	  /// 預計配達日/收貨日
	  /// </summary>
	  public DateTime? DELV_DATE { get; set; }

	  /// <summary>
	  /// 希望送達時段/取件時段
	  /// </summary>
	  public string DELV_PERIOD { get; set; }
  }
}
        