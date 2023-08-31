namespace Wms3pl.Datas.F19
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using Wms3pl.WebServices.DataCommon;

  /// <summary>
  /// 貨主檔
  /// </summary>
  [Serializable]
  [DataServiceKey("CUST_CODE", "GUP_CODE")]
  [Table("F1909")]
  public class F1909 : IAuditInfo
  {

    /// <summary>
    /// 貨主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }

    /// <summary>
    /// 貨主名稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(80)")]
    public string CUST_NAME { get; set; }

    /// <summary>
    /// 業主編號
    /// </summary>
    [Key]
    [Required]
    [Column(TypeName = "varchar(2)")]
    public string GUP_CODE { get; set; }

    /// <summary>
    /// 簡稱
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(20)")]
    public string SHORT_NAME { get; set; }

    /// <summary>
    /// 負責人
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string BOSS { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    [Column(TypeName = "nvarchar(40)")]
    public string CONTACT { get; set; }

    /// <summary>
    /// 電話號碼
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string TEL { get; set; }

    /// <summary>
    /// 通訊地址
    /// </summary>
    [Column(TypeName = "nvarchar(120)")]
    public string ADDRESS { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    [Column(TypeName = "varchar(16)")]
    public string UNI_FORM { get; set; }

    /// <summary>
    /// 貨物聯絡人
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string ITEM_CONTACT { get; set; }

    /// <summary>
    /// 貨物聯絡人電話
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string ITEM_TEL { get; set; }

    /// <summary>
    /// 貨物聯絡人手機
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string ITEM_CEL { get; set; }

    /// <summary>
    /// 貨物聯絡人信箱
    /// </summary>
    [Column(TypeName = "varchar(800)")]
    public string ITEM_MAIL { get; set; }

    /// <summary>
    /// 帳務聯絡人
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    public string BILL_CONTACT { get; set; }

    /// <summary>
    /// 帳務聯絡人電話
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string BILL_TEL { get; set; }

    /// <summary>
    /// 帳務聯絡人手機
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string BILL_CEL { get; set; }

    /// <summary>
    /// 帳務聯絡人信箱
    /// </summary>
    [Column(TypeName = "varchar(800)")]
    public string BILL_MAIL { get; set; }

    /// <summary>
    /// 幣別 F000904
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(5)")]
    public string CURRENCY { get; set; }

    /// <summary>
    /// 付款條件 F000904
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PAY_FACTOR { get; set; }

    /// <summary>
    /// 付款方式 F000904
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PAY_TYPE { get; set; }

    /// <summary>
    /// 銀行代碼
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string BANK_CODE { get; set; }

    /// <summary>
    /// 銀行名稱
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string BANK_NAME { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string BANK_ACCOUNT { get; set; }

    /// <summary>
    /// 是否允許訂單地址解析(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ORDER_ADDRESS { get; set; }

    /// <summary>
    /// 是否允許儲位混批(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string MIX_LOC_BATCH { get; set; }

    /// <summary>
    /// 是否允許儲位混商品(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string MIX_LOC_ITEM { get; set; }

    /// <summary>
    /// 是否允許跨DC調撥(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string DC_TRANSFER { get; set; }

    /// <summary>
    /// 是否允許商品序號綁儲位(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string BOUNDLE_SERIALLOC { get; set; }

    /// <summary>
    /// 指定退貨物流中心
    /// </summary>
    [Column(TypeName = "varchar(3)")]
    public string RTN_DC_CODE { get; set; }

    /// <summary>
    /// 是否允許同質性商品轉換(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SAM_ITEM { get; set; }

    /// <summary>
    /// 是否允許內部交易(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string INSIDER_TRADING { get; set; }

    /// <summary>
    /// 內部交易最大交易數量
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? INSIDER_TRADING_LIM { get; set; }

    /// <summary>
    /// 是否允許訂單明細超過指定數做訂單切割(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPILT_ORDER { get; set; }

    /// <summary>
    /// 訂單明細超過N筆作訂單切割
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? SPILT_ORDER_LIM { get; set; }

    /// <summary>
    /// 是否允許B2C訂單在配庫缺貨時部分釋放(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string B2C_CAN_LACK { get; set; }

    /// <summary>
    /// 是否提供快速到貨(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CAN_FAST { get; set; }

    /// <summary>
    /// 是否可代開發票(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string INSTEAD_INVO { get; set; }

    /// <summary>
    /// 是否允許部分到貨和分批檢驗(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPILT_INCHECK { get; set; }

    /// <summary>
    /// 進貨箱入數與商品主檔不吻合 F000904
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPECIAL_IN { get; set; }

    /// <summary>
    /// 商品抽驗比例
    /// </summary>
    [Column(TypeName = "decimal(14,11)")]
    public decimal? CHECK_PERCENT { get; set; }

    /// <summary>
    /// 出車是否需加貼封條(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string NEED_SEAL { get; set; }

    /// <summary>
    /// 是否放置(無料號)DM(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string DM { get; set; }

    /// <summary>
    /// 是否需要緞帶包裝(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string RIBBON { get; set; }

    /// <summary>
    /// 緞帶使用起始時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? RIBBON_BEGIN_DATE { get; set; }

    /// <summary>
    /// 緞帶使用結束時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? RIBBON_END_DATE { get; set; }

    /// <summary>
    /// 是否提供貨主指定紙箱(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CUST_BOX { get; set; }

    /// <summary>
    /// 是否使用特殊紙箱(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SP_BOX { get; set; }

    /// <summary>
    /// 特殊紙箱編號(F1903.ISCARTON = '1')
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string SP_BOX_CODE { get; set; }

    /// <summary>
    /// 特殊紙箱使用起始時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? SPBOX_BEGIN_DATE { get; set; }

    /// <summary>
    /// 特殊紙箱使用結束時間
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? SPBOX_END_DATE { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string STATUS { get; set; }

    /// <summary>
    /// 異動人員
    /// </summary>
    [Column(TypeName = "varchar(40)")]
    public string UPD_STAFF { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 異動日期
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }

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
    /// 異動人名
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 發票郵遞區號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string INVO_ZIP { get; set; }

    /// <summary>
    /// 是否含稅(0:否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string TAX_TYPE { get; set; }

    /// <summary>
    /// 發票地址
    /// </summary>
    [Column(TypeName = "nvarchar(120)")]
    public string INVO_ADDRESS { get; set; }

    /// <summary>
    /// 發票到期N天前通知
    /// </summary>
    [Column(TypeName = "decimal(18,0)")]
    public Decimal? DUE_DAY { get; set; }

    /// <summary>
    /// 發票號碼少於N組前通知
    /// </summary>
    [Column(TypeName = "int")]
    public Int32? INVO_LIM_QTY { get; set; }

    /// <summary>
    /// 換貨是否自動產生退貨單(0:否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string AUTO_GEN_RTN { get; set; }

    /// <summary>
    /// 外部系統貨主編號
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(20)")]
    public string SYS_CUST_CODE { get; set; }

    /// <summary>
    /// 發票報表檔名(含副檔名)
    /// </summary>
    [Column(TypeName = "varchar(30)")]
    public string INVO_REPORT { get; set; }

    /// <summary>
    /// 盤盈是否可回沖(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string FLUSHBACK { get; set; }

    /// <summary>
    /// 序號綁儲位商品是否可混序號(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string MIX_SERIAL_NO { get; set; }

    /// <summary>
    /// 商品是否同業主共用(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string GUPSHARE { get; set; }

    /// <summary>
    /// 擴增預留欄位A
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_A { get; set; }

    /// <summary>
    /// 擴增預留欄位B
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_B { get; set; }

    /// <summary>
    /// 擴增預留欄位C
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_C { get; set; }

    /// <summary>
    /// 擴增預留欄位D
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_D { get; set; }

    /// <summary>
    /// 擴增預留欄位E
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string EXTENSION_E { get; set; }

    /// <summary>
    /// 出貨裝箱明細備註
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string CUST_MEMO { get; set; }

    /// <summary>
    /// 是否列印送貨單(0:不列印,1:列印)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPRINTDELV { get; set; }

    /// <summary>
    /// 是否列印箱明細
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPRINTDELVDTL { get; set; }

    /// <summary>
    /// 是否建議儲位取得揀貨區優先儲位
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPICKLOCFIRST { get; set; }

    /// <summary>
    /// 允許序號商品缺貨驗收
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISOUTOFSTOCKRECV { get; set; }

    /// <summary>
    /// 是否不出貨裝車 0:否(裝車) 1:是(不裝車)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISDELV_NOLOADING { get; set; }

    /// <summary>
    /// 是否自取領貨需刷讀提領單驗證碼(0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SELFTAKE_CHECKCODE { get; set; }

    /// <summary>
    /// 是否列印自取單(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPRINT_SELFTAKE { get; set; }

    /// <summary>
    /// 解鎖功能 (1: 提示錯誤訊息 3:主管解鎖)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string MANAGER_LOCK { get; set; }

    /// <summary>
    /// 是否揀貨單顯示收貨人姓名 0:不顯示,1:顯示
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPICKSHOWCUSTNAME { get; set; }

    /// <summary>
    /// 是否允許逆物流配送(0否1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISBACK_DISTR { get; set; }

    /// <summary>
    /// 是否驗收後上傳檔案 (0 否 1是) [ 1 為原本流程，0為新流程 ]
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ISUPLOADFILE { get; set; }

    /// <summary>
    /// 商品條碼允許重覆(0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOWREPEAT_ITEMBARCODE { get; set; }

    /// <summary>
    /// 以匯入日為訂單日期(0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IS_ORDDATE_TODAY { get; set; }

    /// <summary>
    /// 是否業主商品分類共用(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOWGUP_ITEMCATEGORYSHARE { get; set; }

    /// <summary>
    /// 是否業主廠商共用(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOWGUP_VNRSHARE { get; set; }

    /// <summary>
    /// 是否業主門市共用(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOWGUP_RETAILSHARE { get; set; }

    /// <summary>
    /// 是否允許驗收進行特殊採購(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOWRT_SPECIALBUY { get; set; }

    /// <summary>
    /// 允許不列印揀貨單(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_NOPRINTPICKORDER { get; set; }

    /// <summary>
    /// 允許不出貨包裝(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_NOSHIPPACKAGE { get; set; }

    /// <summary>
    /// 允許加箱後不做稽核(0:否;1是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_ADDBOXNOCHECK { get; set; }

    /// <summary>
    /// 是否揀貨單顯示效期 0:否 1:是
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISPICKSHOWVALIDDATE { get; set; }

    /// <summary>
    /// 是否調撥單顯示效期 0:否 1:是
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISALLOCATIONSHOWVALIDDATE { get; set; }

    /// <summary>
    /// 是否允許B2B單張訂單出貨
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISB2B_ALONE_OUT { get; set; }

    /// <summary>
    /// 是否判斷允出天數
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISALLOW_DELV_DAY { get; set; }

    /// <summary>
    /// 顯示包裝參考(0:否;1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SHOW_UNIT_TRANS { get; set; }

    /// <summary>
    /// 是否計算單據材積，0否1是
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CAL_CUFT { get; set; }

    /// <summary>
    /// 材積係數(整數箱)，預設為 27000
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 CUFT_FACTOR { get; set; }

    /// <summary>
    /// 材積(零數箱)，預設為 2
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public Int64 CUFT_BLUK { get; set; }

    /// <summary>
    /// 是否裝車稽核檢查出貨驗證碼(0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISDELV_LOADING_CHECKCODE { get; set; }

    /// <summary>
    /// 是否單箱稽核(0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IS_SINGLEBOXCHECK { get; set; }

    /// <summary>
    /// 預設郵遞區號(00000台灣找不到郵遞區號,99999海外郵遞區號)
    /// </summary>
    [Column(TypeName = "varchar(5)")]
    public string ZIP_CODE { get; set; }

    /// <summary>
    /// 列印工具(0: Crystal report 1: Bartend)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PRINT_TYPE { get; set; }

    /// <summary>
    /// 允許部分出貨 (0: 否 1: 是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPILT_OUTCHECK { get; set; }

    /// <summary>
    /// 部分出貨方式 (0:先進先出 1: 平均分攤 )
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPILT_OUTCHECKWAY { get; set; }

    /// <summary>
    /// 是否只允許最新的效期進倉(比以往的驗收單效期晚)(0: 否 1: 是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISLATEST_VALID_DATE { get; set; }

    /// <summary>
    /// 允許部分廠退出貨
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SPILT_VENDER_ORD { get; set; }

    /// <summary>
    /// 是否原廠退單出貨
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string CHG_VENDER_ORD { get; set; }

    /// <summary>
    /// 是否使用替代品號(0:否;1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ISSHIFTITEM { get; set; }

    /// <summary>
    /// 替代品號
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string SHIFTITEMCODE { get; set; }

    /// <summary>
    /// 允許匯出出貨明細資料(0: 否 1:是) (預設: 台空為1，其他為0)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOWOUTSHIPDETLOG { get; set; }

    /// <summary>
    /// 是否允許預先配庫(0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_ADVANCEDSTOCK { get; set; }

    /// <summary>
    /// 包裝數量換算最大單位
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(1)")]
    public string PACKCOUNT_MAX_UNIT { get; set; }

    /// <summary>
    /// 允許強制測量新品規格 (0: 否 1: 是) 祥和=1，其他=0
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(1)")]
    public string NEED_ITEMSPEC { get; set; }

    /// <summary>
    /// 是否列印進倉棧板標籤 (0: 都不印 1:驗收前必須列印 2: 驗收後必須列印(notyet))
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IS_PRINT_INSTOCKPALLETSTICKER { get; set; }

    /// <summary>
    /// 進倉單自動結案天數(0:不自動結案，整數:N天後系統自動將進倉單結案)
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(38,0)")]
    public Decimal INSTOCKAUTOCLOSED { get; set; }

    /// <summary>
    /// 允許快速進貨檢驗(0:否1:是) 若為是，在商品檢驗中可進行快驗
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string IS_QUICK_CHECK { get; set; }

    /// <summary>
    /// 允許報表自動轉成PDF檔案 (0:否 1:是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string PRINT2PDF { get; set; }

    /// <summary>
    /// 客戶指定共用路徑
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string SHARED_FOLDER { get; set; }

    /// <summary>
    /// 建議上架儲位方法 (0:一品一儲、1:多品一儲)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string SUGGEST_LOC_TYPE { get; set; }

    /// <summary>
    /// 是否取消缺貨訂單
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOW_CANCEL_LACKORD { get; set; }

    /// <summary>
    /// 批號效期檢核年份
    /// </summary>
    [Column(TypeName = "decimal(18,0)")]
    public Decimal? VALID_DATE_CHKYEAR { get; set; }

    /// <summary>
    /// 商品允退提醒天數
    /// </summary>
    [Column(TypeName = "decimal(38,0)")]
    public Decimal? ITEM_RET_REMIND_DAYS { get; set; }

    /// <summary>
    /// 不可退商品提醒天數
    /// </summary>
    [Column(TypeName = "decimal(38,0)")]
    public Decimal? ITEM_NOTRET_REMIND_DAYS { get; set; }

    /// <summary>
    /// 允許調撥修改效期 (0: 否 1: 是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOCATIONCHANGVALIDATE { get; set; }

    /// <summary>
    /// 允許調撥修改批號 (0: 否 1: 是)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string ALLOCATIONCHANGMAKENO { get; set; }

    /// <summary>
    /// 廠退方式(0:出貨模式;1:直接扣帳)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string VNR_RTN_TYPE { get; set; }

    /// <summary>
    /// 報廢銷毀方式(0:出貨模式;1:直接扣帳)
    /// </summary>
    [Required]
    [Column(TypeName = "char(1)")]
    public string DESTROY_TYPE { get; set; }

    /// <summary>
    /// 允許修改箱入數
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string ALLOW_EDIT_BOX_QTY { get; set; }

    /// <summary>
    /// 顯示上架確認訊息
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string SHOW_MESSAGE { get; set; }

    /// <summary>
    /// 是否預帶確認數量
    /// </summary>
    [Column(TypeName = "varchar(1)")]
    public string SHOW_QTY { get; set; }

    /// <summary>
    /// 是否上架可取消 1:是 0:否
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IS_UP_SHELF_CANCEL { get; set; }

    /// <summary>
    /// 是否允許訂單設定不出貨
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ALLOW_ORDER_NO_DELV { get; set; }

    /// <summary>
    /// 允許進倉單強制結案 (0: 否、1: 是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ALLOW_WAREHOUSEIN_CLOSED { get; set; }

    /// <summary>
    /// 是否啟動即期品排程(0:否: 1是)
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string IS_EXECIMMEDIATEITEM { get; set; }

    /// <summary>
    /// 是否允許訂單缺品出貨
    /// </summary>
    [Column(TypeName = "char(1)")]
    public string ALLOW_ORDER_LACKSHIP { get; set; }

  }
}
