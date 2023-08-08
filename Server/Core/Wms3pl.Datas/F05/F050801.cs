namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 出貨頭檔
	/// </summary>
	[Serializable]
	[DataServiceKey("WMS_ORD_NO", "CUST_CODE", "GUP_CODE", "DC_CODE")]
	[Table("F050801")]
	public class F050801 : IAuditInfo
	{

		/// <summary>
		/// 出貨單號
		/// </summary>
		[Key]
		[Required]
		public string WMS_ORD_NO { get; set; }

		/// <summary>
		/// 批次日期
		/// </summary>
		[Required]
		public DateTime DELV_DATE { get; set; }

		/// <summary>
		/// 貨主
		/// </summary>
		[Key]
		[Required]
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 訂單類別(0:B2B 1:B2C)
		/// </summary>
		[Required]
		public string ORD_TYPE { get; set; }

		/// <summary>
		/// 批次時段
		/// </summary>
		[Required]
		public string PICK_TIME { get; set; }

		/// <summary>
		/// 門市編號(F1910)
		/// </summary>
		public string RETAIL_CODE { get; set; }

		/// <summary>
		/// 出貨單列印註記
		/// </summary>
		[Required]
		public string PRINT_FLAG { get; set; }

		/// <summary>
		/// 業主
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
		/// 揀貨單號
		/// </summary>
		public string PICK_ORD_NO { get; set; }

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
		/// 單據狀態(0:待處理 1:已包裝 2:已稽核待出貨  5:已出貨 6:已扣帳 9:不出貨)
		/// </summary>
		[Required]
		public Decimal STATUS { get; set; }

		/// <summary>
		/// 溫層(01常溫02冷藏03冷凍)
		/// </summary>
		[Required]
		public string TMPR_TYPE { get; set; }

		/// <summary>
		/// 自取
		/// </summary>
		[Required]
		public string SELF_TAKE { get; set; }

		/// <summary>
		/// 易碎標籤(包裝用)
		/// </summary>
		[Required]
		public string FRAGILE_LABEL { get; set; }

		/// <summary>
		/// 保證書(包裝用)
		/// </summary>
		[Required]
		public string GUARANTEE { get; set; }

		/// <summary>
		/// WelcomeLetter(包裝用)
		/// </summary>
		[Required]
		public string HELLO_LETTER { get; set; }

		/// <summary>
		/// 門號申請書(包裝用)
		/// </summary>
		[Required]
		public string SA { get; set; }

		/// <summary>
		/// 客戶名稱(B2C用)
		/// </summary>
		public string CUST_NAME { get; set; }

		/// <summary>
		/// 發票列印次數
		/// </summary>
		[Required]
		public Int16 INVOICE_PRINT_CNT { get; set; }

		/// <summary>
		/// 性別
		/// </summary>
		public string GENDER { get; set; }

		/// <summary>
		/// 年紀
		/// </summary>
		public Int16? AGE { get; set; }

		/// <summary>
		/// 異動類型(F000903)
		/// </summary>
		[Required]
		public string ORD_PROP { get; set; }

		/// <summary>
		/// 不裝車(0否 1是)
		/// </summary>
		[Required]
		public string NO_LOADING { get; set; }

		/// <summary>
		/// SA份數
		/// </summary>
		[Required]
		public Int16 SA_QTY { get; set; }

		/// <summary>
		/// 不稽核(0否 1是)
		/// </summary>
		[Required]
		public string NO_AUDIT { get; set; }

		/// <summary>
		/// 是否列印託運單(0否1是)
		/// </summary>
		public string PRINT_PASS { get; set; }

		/// <summary>
		/// 是否列印送貨單(0否1是)
		/// </summary>
		[Required]
		public string PRINT_DELV { get; set; }

		/// <summary>
		/// 是否列印箱明細(0否1是)
		/// </summary>
		[Required]
		public string PRINT_BOX { get; set; }

		/// <summary>
		/// 過帳日期(年月日時分秒)
		/// </summary>
		public DateTime? APPROVE_DATE { get; set; }

		/// <summary>
		/// 裝車時間(年月日時分秒)
		/// </summary>
		public DateTime? INCAR_DATE { get; set; }

		/// <summary>
		/// 指定到貨日期
		/// </summary>
		public DateTime? ARRIVAL_DATE { get; set; }

		/// <summary>
		/// 是否為虛擬商品出貨單
		/// </summary>
		[Required]
		public string VIRTUAL_ITEM { get; set; }

		/// <summary>
		/// 來源單據F000902
		/// </summary>
		public string SOURCE_TYPE { get; set; }

		/// <summary>
		/// 配送商編號F1947
		/// </summary>
		public string ALL_ID { get; set; }

		/// <summary>
		/// 專車(0否1是)
		/// </summary>
		[Required]
		public string SPECIAL_BUS { get; set; }

		/// <summary>
		/// 郵遞區號
		/// </summary>
		public string ZIP_CODE { get; set; }

		/// <summary>
		/// 是否快速到貨(0否1是)
		/// </summary>
		[Required]
		public string CAN_FAST { get; set; }

		/// <summary>
		/// 是否含有原箱商品出貨單(0否1是)
		/// </summary>
		[Required]
		public string ALLOWORDITEM { get; set; }

		/// <summary>
		/// 合流作業是否已列印出貨單明細(0否1是)
		/// </summary>
		[Required]
		public string PRINT_DETAIL_FLAG { get; set; }

		/// <summary>
		/// 使用材積(cm)
		/// </summary>
		public decimal? VOLUMN { get; set; }

		/// <summary>
		/// 商品總重量
		/// </summary>
		public decimal? WEIGHT { get; set; }

		/// <summary>
		/// 裝車人員
		/// </summary>
		public string INCAR_STAFF { get; set; }

		/// <summary>
		/// 裝車人名
		/// </summary>
		public string INCAR_NAME { get; set; }

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
		/// 代收金額
		/// </summary>
		public decimal? COLLECT_AMT { get; set; }

		/// <summary>
		/// SA檢核表數量
		/// </summary>
		[Required]
		public Int16 SA_CHECK_QTY { get; set; }

		/// <summary>
		/// 設定不出貨(0否1是)
		/// </summary>
		public string NO_DELV { get; set; }

		/// <summary>
		/// 方便到貨時段
		/// </summary>
		[Required]
		public string DELV_PERIOD { get; set; }

		/// <summary>
		/// 是否超取
		/// </summary>
		[Required]
		public string CVS_TAKE { get; set; }

		/// <summary>
		/// 入庫狀態(0:待處理;1:已入庫,2:已入庫確認)
		/// </summary>
		[Required]
		public string SELFTAKE_CHECKCODE { get; set; }

		/// <summary>
		/// 自取驗證碼
		/// </summary>
		public string CHECK_CODE { get; set; }

		/// <summary>
		/// 自取確認時間
		/// </summary>
		public DateTime? SELFTAKE_DATE { get; set; }

		/// <summary>
		/// 出貨驗證碼(ex: 報關單號)
		/// </summary>
		public string DELV_CHECKCODE { get; set; }

		/// <summary>
		/// 實際指定到貨日期
		/// </summary>
		public DateTime? A_ARRIVAL_DATE { get; set; }

		/// <summary>
		/// 來回件(0:否;1:是)
		/// </summary>
		[Required]
		public string ROUND_PIECE { get; set; }
		/// <summary>
		/// 揀貨開始時間
		/// </summary>
		public DateTime? START_TIME { get; set; }
		/// <summary>
		/// 揀貨完成時間
		/// </summary>
		public DateTime? COMPLETE_TIME { get; set; }
		/// <summary>
		/// 作業人員
		/// </summary>
		public string OPERATOR { get; set; }
		/// <summary>
		/// 是否異常(0=正常, 1=異常)
		/// </summary>
		public string ISEXCEPTION { get; set; }
		/// <summary>
		/// 客戶自訂分類(Out:一般出貨 MoveOut:集貨出貨(跨庫調撥) CVS:超商出貨)
		/// </summary>
		public string CUST_COST { get; set; }
		/// <summary>
		/// 優先處理旗標(1:一般 2:優先 3:急件)
		/// </summary>
		public string FAST_DEAL_TYPE { get; set; }
		/// <summary>
		/// 建議紙箱編號
		/// </summary>
		public string SUG_BOX_NO { get; set; }
		/// <summary>
		/// 跨庫調撥的目的地名稱
		/// </summary>
		public string MOVE_OUT_TARGET { get; set; }

		/// <summary>
		/// 是否出貨稽核(0=不需稽核  1=需稽核  2=人員變更需稽核)
		/// </summary>
		public int? ISPACKCHECK { get; set; }

		/// <summary>
		/// 建議出貨包裝線類型(空白=不指定 PA1=小線 PA2=大線)
		/// </summary>
		public string PACKING_TYPE { get; set; }

		/// <summary>
		/// 出貨模式(0: 未開始包裝、1: 使用原出貨包裝流程、2: 使用新出貨包裝流程、3: 外部出貨包裝紀錄、4: 稽核出庫)
		/// </summary>
		public string SHIP_MODE { get; set; }

    /// <summary>
    /// 指定容器(00=不限, 01=M-周轉箱, 02=2L周轉箱)
    /// </summary>
		public string CONTAINER_TYPE { get; set; }

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
    /// 建議物流商編號
    /// </summary>
    public string SUG_LOGISTIC_CODE { get; set; }

    /// <summary>
    /// 特別處理標記 (0: 無、1: Apple廠商的商品)
    /// </summary>
    public string NP_FLAG { get; set; }


    /// <summary>
    /// 人員包裝開始時間
    /// </summary>
    public DateTime? PACK_START_TIME { get; set; }

    /// <summary>
    /// 人員包裝完成時間
    /// </summary>
    public DateTime? PACK_FINISH_TIME { get; set; }

    /// <summary>
    /// 人員取消包裝時間
    /// </summary>
    public DateTime? PACK_CANCEL_TIME { get; set; }

    /// <summary>
    /// 配箱站與封箱站是否分開(0:否 1:是)
    /// </summary>
    public string NO_SPEC_REPROTS { get; set; }

    /// <summary>
    /// 是否刷讀紙箱關箱(0:否 1:是)
    /// </summary>
    public string CLOSE_BY_BOXNO { get; set; }
  }
}