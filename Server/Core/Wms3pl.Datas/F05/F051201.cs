namespace Wms3pl.Datas.F05
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 揀貨單頭檔
  /// </summary>
  [Serializable]
  [DataServiceKey("PICK_ORD_NO", "CUST_CODE", "GUP_CODE", "DC_CODE")]
  [Table("F051201")]
  public class F051201 : IAuditInfo
  {

    /// <summary>
    /// 揀貨單號
    /// </summary>
    [Key]
    [Required]
    public string PICK_ORD_NO { get; set; }

    /// <summary>
    /// 批次日期
    /// </summary>
    public DateTime? DELV_DATE { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Key]
    [Required]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 訂單類別(0:B2B 1:B2C)
    /// </summary>
    public string ORD_TYPE { get; set; }

    /// <summary>
    /// 批次時段
    /// </summary>
    public string PICK_TIME { get; set; }

    /// <summary>
    /// 揀貨狀態(0待揀貨、1揀貨中、2揀貨完成、9取消)
    /// </summary>
    public Int16? PICK_STATUS { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Key]
    [Required]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 物流中心
    /// </summary>
    [Key]
    [Required]
    public string DC_CODE { get; set; }

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
    /// 是否為台車揀貨
    /// </summary>
    [Required]
    public string ISDEVICE { get; set; }

    /// <summary>
    /// 揀貨人員
    /// </summary>
    public string PICK_STAFF { get; set; }

    /// <summary>
    /// 揀貨人名
    /// </summary>
    public string PICK_NAME { get; set; }

    /// <summary>
    /// 是否已列印(0:否;1:是)
    /// </summary>
    [Required]
    public string ISPRINTED { get; set; }

    /// <summary>
    /// 台車是否已列印(0:否;1:是)
    /// </summary>
    [Required]
    public string DEVICE_PRINT { get; set; }

    /// <summary>
    /// 揀貨完成時間
    /// </summary>
    public DateTime? PICK_FINISH_DATE { get; set; }

    /// <summary>
    /// 拆單方式(01:倉庫;02:PK區;03:不拆單)
    /// </summary>
    public string SPLIT_TYPE { get; set; }

    /// <summary>
    /// 拆單代碼(依SPLIT_TYPE寫入對應的代碼)
    /// </summary>
    public string SPLIT_CODE { get; set; }
    /// <summary>
    /// 派發系統(0: WMS、1: 中介系統)
    /// </summary>
    public string DISP_SYSTEM { get; set; }

    /// <summary>
    /// 揀貨單類型
    /// 0: 人工倉單一揀貨
    ///		(1)人工倉1P1O，O單跨PK區揀貨
    ///		(2)預設下一個動作到包裝站
    /// 1: 人工倉一階彙整揀貨單
    ///		(1)O單人工倉同PK區自我滿足
    ///		(2)人工倉1P多O，所有O都只對到同一個P & 沒有其他的P
    ///		(3)預設下一個動作到分貨場
    /// 2: 自動倉揀貨單	
    ///		(1)1P1O，O單非自我滿足
    ///		(2)預設下一個動作到集貨場
    /// 3: 人工倉二階揀貨揀貨單
    ///		(1)1P多O，O單非自我滿足
    ///		(2)會產生總量揀貨單F051203
    ///		(3)預設下一個動作到分貨場
    /// 4: 特殊結構訂單(1單1品1PCS)
    ///		(1) 1P 1O 1品 1PCS
    ///		(2) 預設下一個動作到包裝站
    /// 5: 快速補揀單(揀缺用)
    ///		(1)揀貨優先度會調高
    ///		(2)預設下一個動作集貨場
    /// 6. 自動倉自我滿足揀貨單
    ///		(1)自動倉1P1O，O單自我滿足
    ///		(2)預設下一個動作包裝站
    /// 7: 特殊結構訂單(1單1品多PCS)
    ///		(1) 1P 1O 1品 多PCS
    ///		(2) 預設下一個動作到包裝站
    /// 8: 特殊結構訂單(1單2品1PCS)
    ///		(1) 1P 1O 2品 1PCS
    ///		(2) 預設下一個動作到包裝站
    /// </summary>
    public string PICK_TYPE { get; set; }
    /// <summary>
    /// 下一個作業(0: 待確認、1: 分貨站、2: 集貨場、3: 包裝站、4: 跨庫調撥出貨碼頭)
    /// </summary>
    public string NEXT_STEP { get; set; }
    /// <summary>
    /// 人工倉揀貨工具(0: 待確認、1: 紙本、2: PDA)
    /// </summary>
    public string PICK_TOOL { get; set; }
    /// <summary>
    /// 揀貨開始時間
    /// </summary>
    public DateTime? PICK_START_TIME { get; set; }
    /// <summary>
    /// 跨庫調撥的目的地名稱
    /// </summary>
    public string MOVE_OUT_TARGET { get; set; }

    /// <summary>
    /// 建議出貨包裝線類型(空白=不指定 PA1=小線 PA2=大線)
    /// </summary>
    public string PACKING_TYPE { get; set; }

    /// <summary>
    /// 指定容器(00=不限, 01=M-周轉箱, 02=2L周轉箱)
    /// </summary>
    public string CONTAINER_TYPE { get; set; }

    /// <summary>
    /// PK區編號/倉庫編號
    /// </summary>
    public string PK_AREA { get; set; }

    /// <summary>
    /// PK區名稱/倉庫名稱
    /// </summary>
    public string PK_AREA_NAME { get; set; }

    /// <summary>
    /// 集貨/分貨單號
    /// </summary>
    public string MERGE_NO { get; set; }

    /// <summary>
    /// 廠商編號
    /// </summary>
    public string RTN_VNR_CODE { get; set; }

    /// <summary>
    /// 預計指定容器數量
    /// </summary>
		public int? CONTAINER_B_CNT { get; set; }
    
    /// <summary>
    /// 訂單建立日期 (只存日期不存時間)
    /// </summary>
    public DateTime? ORDER_CRT_DATE { get; set; }

    /// <summary>
    /// 商品處理類別(0:一般 1:含安裝型商品)
    /// </summary>
    public string ORDER_PROC_TYPE { get; set; }

    /// <summary>
    /// 收貨郵遞區號
    /// </summary>
    public string ORDER_ZIP_CODE { get; set; }

    /// <summary>
    /// 是否北北基訂單(0:否 1:是)
    /// </summary>
    public string IS_NORTH_ORDER { get; set; }
    /// <summary>
    /// WMS優先權代碼(F1956.PRIORITY_CODE)
    /// </summary>
    public string PRIORITY_CODE { get; set; }
    /// <summary>
    /// 設備優先權值
    /// </summary>
    public int? PRIORITY_VALUE { get; set; }
    /// <summary>
    /// 是否為人員指定WMS優先權(0:否 1:是)
    /// </summary>
    public string IS_USER_DIRECT_PRIORITY { get; set; }
    /// <summary>
    /// 裝置類型
    /// </summary>
    public string DEVICE_TYPE { get; set; }
    /// <summary>
    /// 揀貨單列印人員
    /// </summary>
    public string PRINT_STAFF { get; set; }
    /// <summary>
    /// 揀貨單列印人名
    /// </summary>
    public string PRINT_NAME { get; set; }
    /// <summary>
    /// 揀貨單列印時間
    /// </summary>
    public DateTime? PRINT_TIME { get; set; }
    /// <summary>
    /// 建議物流商編號
    /// </summary>
    public string SUG_LOGISTIC_CODE { get; set; }

    /// <summary>
    /// 特別處理標記 (0: 無、1: Apple廠商的商品)
    /// </summary>
    public string NP_FLAG { get; set; }
  }
}
