namespace Wms3pl.Datas.F05
{
            using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 揀貨單儲區揀貨設定檔
  /// </summary>
  [Serializable]
  [DataServiceKey("DC_CODE","GUP_CODE","CUST_CODE","PICK_ORD_NO")]
  [Table("F05120101")]
  public class F05120101 : IAuditInfo
  {

	  /// <summary>
	  /// 物流中心編號
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
	  /// 揀貨單號F051201
	  /// </summary>
    [Key]
    [Required]
	  public string PICK_ORD_NO { get; set; }

	  /// <summary>
	  /// 倉庫編號F1980
	  /// </summary>
    [Required]
	  public string WAREHOUSE_ID { get; set; }

	  /// <summary>
	  /// 儲區編號F1919
	  /// </summary>
    [Required]
	  public string AREA_CODE { get; set; }

	  /// <summary>
	  /// 揀貨方式F000904 TOPIC=F191902 SUBTOPIC=PICK_TYPE
	  /// </summary>
    [Required]
	  public string PICK_TYPE { get; set; }

	  /// <summary>
	  /// 摘取工具F000904 TOPIC=F191902 SUBTOPIC=PICK_TOOL
	  /// </summary>
    [Required]
	  public string PICK_TOOL { get; set; }

	  /// <summary>
	  /// 播種工具F000904 TOPIC=F191902 SUBTOPIC=PUT_TOOL
	  /// </summary>
	  public string PUT_TOOL { get; set; }

	  /// <summary>
	  /// 揀貨順序F000904 TOPIC=F191902 SUBTOPIC=PICK_SEQ
	  /// </summary>
    [Required]
	  public string PICK_SEQ { get; set; }

	  /// <summary>
	  /// 排序方式F000904 TOPIC=F191902 SUBTOPIC=SORT_BY
	  /// </summary>
    [Required]
	  public string SORT_BY { get; set; }

	  /// <summary>
	  /// 單箱計算(0:否 1:是)
	  /// </summary>
    [Required]
	  public string SINGLE_BOX { get; set; }

	  /// <summary>
	  /// 揀貨稽核(0:否 1:是)
	  /// </summary>
    [Required]
	  public string PICK_CHECK { get; set; }

	  /// <summary>
	  /// 揀貨單位F91000302 ITEM_TYPE_ID=001 的計價單位編號(ACC_UNIT)
	  /// </summary>
    [Required]
	  public string PICK_UNIT { get; set; }

	  /// <summary>
	  /// 揀貨載具F1944
	  /// </summary>
    [Required]
	  public string PICK_MARTERIAL { get; set; }

	  /// <summary>
	  /// 出貨資材類型F1903.ISCARTON=1 的商品編號
	  /// </summary>
    [Required]
	  public string DELIVERY_MARTERIAL { get; set; }

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
        