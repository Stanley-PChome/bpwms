namespace Wms3pl.Datas.F19
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 貨主商品主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("ITEM_CODE", "GUP_CODE", "CUST_CODE")]
  [Table("F1903")]
  public class F1903 : IAuditInfo
  {

    /// <summary>
    /// 商品編號
    /// </summary>
    [Key]
    [Required]
    public string ITEM_CODE { get; set; }

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
    /// 供應商最低訂量
    /// </summary>
    public Int32? VEN_ORD { get; set; }

    /// <summary>
    /// 客戶訂貨單位
    /// </summary>
    public string RET_UNIT { get; set; }

    /// <summary>
    /// 訂購倍數(商品訂購時須以多少數量為最小單位)
    /// </summary>
    public Int32? RET_ORD { get; set; }

    /// <summary>
    /// 允收天數
    /// </summary>
    public Int16? ALL_DLN { get; set; }

    /// <summary>
    /// 出貨狀態(預設狀態0  刪除:9)
    /// </summary>
    public string SND_TYPE { get; set; }

    /// <summary>
    /// 上架倉別型態
    /// </summary>
    [Required]
    public string PICK_WARE { get; set; }

    /// <summary>
    /// 越庫商品註記
    /// </summary>
    public string C_D_FLAG { get; set; }

    /// <summary>
    /// 客戶品號
    /// </summary>
    public string CUST_ITEM_CODE { get; set; }

    /// <summary>
    /// 放寬允收期限
    /// </summary>
    public Int16? ALLOW_ALL_DLN { get; set; }

    /// <summary>
    /// 組合商品註記
    /// </summary>
    public string MULTI_FLAG { get; set; }

    /// <summary>
    /// 是否可混批(效期)擺放於儲位
    /// </summary>
    public string MIX_BATCHNO { get; set; }

    /// <summary>
    /// 是否可原箱出貨(0:否1是)
    /// </summary>
    public string ALLOWORDITEM { get; set; }

    /// <summary>
    /// 序號綁儲位(只要有儲位異動就需刷序號)
    /// </summary>
    public string BUNDLE_SERIALLOC { get; set; }

    /// <summary>
    /// 序號商品(商品進出或跨倉(上架,揀貨(ex.良品倉移動至加工倉),包裝)需刷序號)
    /// </summary>
    public string BUNDLE_SERIALNO { get; set; }

    /// <summary>
    /// 採購安全庫存量
    /// </summary>
    [Required]
    public Int64 ORD_SAVE_QTY { get; set; }

    /// <summary>
    /// 補貨安全庫存量
    /// </summary>
    [Required]
    public Int64 PICK_SAVE_QTY { get; set; }

    /// <summary>
    /// 是否可換貨(0:否1是)
    /// </summary>
    public string ITEM_EXCHANGE { get; set; }

    /// <summary>
    /// 是否可退貨(0:否1是)
    /// </summary>
    public string ITEM_RETURN { get; set; }

    /// <summary>
    /// 是否可併貨包裝(0:否1是)
    /// </summary>
    public string ITEM_MERGE { get; set; }

    /// <summary>
    /// 最大可借出天數
    /// </summary>
    public Int16? BORROW_DAY { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }

    /// <summary>
    /// 建檔日期
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
    /// 是否可儲位混商品(0:否1是)
    /// </summary>
    [Required]
    public string LOC_MIX_ITEM { get; set; }

    /// <summary>
    /// 是否無價商品(0:否1:是)
    /// </summary>
    [Required]
    public string NO_PRICE { get; set; }

    /// <summary>
    /// 環保稅
    /// </summary>
    public Int32? EP_TAX { get; set; }

    /// <summary>
    /// 序號碼數
    /// </summary>
    public Int16? SERIALNO_DIGIT { get; set; }

    /// <summary>
    /// 序號開頭
    /// </summary>
    public string SERIAL_BEGIN { get; set; }

    /// <summary>
    /// 序號檢查規則F000904 : 0純數/1非純數
    /// </summary>
    [Required]
    public string SERIAL_RULE { get; set; }

    /// <summary>
    /// 是否允許銷售(0:否1是)
    /// </summary>
    [Required]
    public string CAN_SELL { get; set; }

    /// <summary>
    /// 是否允許分批進貨
    /// </summary>
    [Required]
    public string CAN_SPILT_IN { get; set; }

    /// <summary>
    /// 出貨是否附上保證書(0:否1是)
    /// </summary>
    [Required]
    public string LG { get; set; }

    /// <summary>
    /// 保存天數
    /// </summary>
    public Int32? SAVE_DAY { get; set; }

    /// <summary>
    /// 商品負責人員 F1924
    /// </summary>
    public string ITEM_STAFF { get; set; }

    /// <summary>
    /// 抽驗比例(%)
    /// </summary>
    public decimal? CHECK_PERCENT { get; set; }

    /// <summary>
    /// 補貨最小單位數
    /// </summary>
    [Required]
    public Int32 PICK_SAVE_ORD { get; set; }

    /// <summary>
    /// 出貨平均數(每日)
    /// </summary>
    [Required]
    public Int32 DELV_QTY_AVG { get; set; }

    /// <summary>
    /// 是否為紙箱(0否1是)
    /// </summary>
    [Required]
    public string ISCARTON { get; set; }

    /// <summary>
    /// 是否為蘋果商品(0否1是)
    /// </summary>
    public string ISAPPLE { get; set; }

    /// <summary>
    /// 大分類 F1915
    /// </summary>
    public string LTYPE { get; set; }

    /// <summary>
    /// 中分類 F1916
    /// </summary>
    public string MTYPE { get; set; }

    /// <summary>
    /// 小分類 F1917
    /// </summary>
    public string STYPE { get; set; }

    /// <summary>
    /// 計價類別
    /// </summary>
    [Required]
    public string ACC_TYPE { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ITEM_NAME { get; set; }

    /// <summary>
    /// 商品條碼一
    /// </summary>
    public string EAN_CODE1 { get; set; }

    /// <summary>
    /// 商品條碼二
    /// </summary>
    public string EAN_CODE2 { get; set; }

    /// <summary>
    /// 商品條碼三
    /// </summary>
    public string EAN_CODE3 { get; set; }

    /// <summary>
    /// 客戶商品名稱(英)
    /// </summary>
    public string ITEM_ENGNAME { get; set; }

    /// <summary>
    /// 商品顏色
    /// </summary>
    public string ITEM_COLOR { get; set; }

    /// <summary>
    /// 商品尺寸
    /// </summary>
    public string ITEM_SIZE { get; set; }

    /// <summary>
    /// 商品類別(F000904)
    /// </summary>
    public string TYPE { get; set; }

    /// <summary>
    /// 商品保存濕度
    /// </summary>
    public Int16? ITEM_HUMIDITY { get; set; }

    /// <summary>
    /// 商品替代名稱
    /// </summary>
    public string ITEM_NICKNAME { get; set; }

    /// <summary>
    /// 商品屬性(F000904)
    /// </summary>
    public string ITEM_ATTR { get; set; }

    /// <summary>
    /// 商品規格
    /// </summary>
    public string ITEM_SPEC { get; set; }

    /// <summary>
    /// 溫層(F000904：01:常溫26-30、02:恆溫8-18、03冷藏-2~10、04:冷凍-18~-25) 
    /// </summary>
    [Required]
    public string TMPR_TYPE { get; set; }

    /// <summary>
    /// 易碎品包裝(0否1是)
    /// </summary>
    public string FRAGILE { get; set; }

    /// <summary>
    /// 防溢漏包裝(0否1是)
    /// </summary>
    public string SPILL { get; set; }

    /// <summary>
    /// 物料類型
    /// </summary>
    public string ITEM_TYPE { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string ITEM_UNIT { get; set; }

    /// <summary>
    /// 產品階層
    /// </summary>
    public string ITEM_CLASS { get; set; }

    /// <summary>
    /// SIM卡規格說明
    /// </summary>
    public string SIM_SPEC { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string MEMO { get; set; }

    /// <summary>
    /// 虛擬商品類別
    /// </summary>
    public string VIRTUAL_TYPE { get; set; }

    /// <summary>
    /// 貨源
    /// </summary>
    public string ITEM_SOURCE { get; set; }

    /// <summary>
    /// 擴增預留欄位A
    /// </summary>
    public string EXTENSION_A { get; set; }

    /// <summary>
    /// 擴增預留欄位B
    /// </summary>
    public string EXTENSION_B { get; set; }

    /// <summary>
    /// 擴增預留欄位C
    /// </summary>
    public string EXTENSION_C { get; set; }

    /// <summary>
    /// 擴增預留欄位D
    /// </summary>
    public string EXTENSION_D { get; set; }

    /// <summary>
    /// 擴增預留欄位E
    /// </summary>
    public string EXTENSION_E { get; set; }

    /// <summary>
    /// 上架倉別
    /// </summary>
    public string PICK_WARE_ID { get; set; }

    /// <summary>
    /// 停售日期
    /// </summary>
    public DateTime? STOP_DATE { get; set; }

    /// <summary>
    /// 序號管控商品flag(0:非控管 1:控管)
    /// </summary>
    [Required]
    public string BOUNDLE_SERIALREQ { get; set; }

    /// <summary>
    /// 攤提總次數
    /// </summary>
    [Required]
    public Int16 AMORTIZATION_NO { get; set; }

    /// <summary>
    /// 客戶品名
    /// </summary>
    public string CUST_ITEM_NAME { get; set; }

    /// <summary>
    /// 海關稅則號別
    /// </summary>
    public string TARIFF_NO { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string CURRENCY { get; set; }

    /// <summary>
    /// 是否為自有商品 (0: 否 1: 是)
    /// </summary>
    public string ISOEM { get; set; }

    /// <summary>
    /// 只允許整箱出貨
    /// </summary>
    public string ISBOX { get; set; }

    /// <summary>
    /// 批號管控商品(0:否 1:是)
    /// </summary>
    [Required]
    public string MAKENO_REQU { get; set; }

    /// <summary>
    /// 最小撥量
    /// </summary>
    [Required]
    public Int64 PICK_ORD { get; set; }

    /// <summary>
    /// 箱入數
    /// </summary>
    [Required]
    public Int64 CTNS { get; set; }
    /// <summary>
    /// 是否為效期商品(null: 未選擇、0: 否、1: 是)
    /// </summary>
    public string NEED_EXPIRED { get; set; }
    /// <summary>
    /// 警示天數(原本的允售天數)
    /// </summary>
    public Int32? ALL_SHP { get; set; }
    /// <summary>
    /// EAN/ISBN
    /// </summary>
    public string EAN_CODE4 { get; set; }
    /// <summary>
    /// 首次進貨日
    /// </summary>
    public DateTime? FIRST_IN_DATE { get; set; }
    /// <summary>
    /// 廠商代碼
    /// </summary>
    public string VNR_CODE { get; set; }
    /// <summary>
    /// 是否易遺失(0: 否, 1: 是)
    /// </summary>
    public string IS_EASY_LOSE { get; set; }
    /// <summary>
    /// 貴重品標示(0: 否, 1: 是)
    /// </summary>
    public string IS_PRECIOUS { get; set; }
    /// <summary>
    /// 強磁標示(0: 否, 1: 是)
    /// </summary>
    public string IS_MAGNETIC { get; set; }
    /// <summary>
    /// 是否同步成功(0: 否, 1: 是)
    /// </summary>
    public string IS_ASYNC { get; set; }
    /// <summary>
    /// 易變質標示(0: 否, 1: 是)
    /// </summary>
    public string IS_PERISHABLE { get; set; }
    /// <summary>
    /// 需溫控標示(0: 否, 1: 是)
    /// </summary>
    public string IS_TEMP_CONTROL { get; set; }
    /// <summary>
    /// 廠商料號 (料號)
    /// </summary>
    public string VNR_ITEM_CODE { get; set; }
    /// <summary>
    /// 驗收註記
    /// </summary>
    public string RCV_MEMO { get; set; }


  }
}
