using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 商品主檔_傳入
	/// </summary>
	public class PostItemDataReq
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public PostItemDataResultModel Result { get; set; }
	}

	public class PostItemDataResultModel
	{
		/// <summary>
		/// 總筆數
		/// </summary>
		public int? Total { get; set; }
		/// <summary>
		/// 商品主檔物件清單
		/// </summary>
		public List<PostItemDataItemsModel> Items { get; set; }
	}

	/// <summary>
	/// 商品主檔物件
	/// </summary>
	public class PostItemDataItemsModel
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 商品名稱
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 商品英文名稱
		/// </summary>
		public string ItemEngName { get; set; } = null;
		/// <summary>
		/// 客戶品名
		/// </summary>
		public string CustItemName { get; set; } = null;
		/// <summary>
		/// 國際條碼
		/// </summary>
		public string ItemBarCode1 { get; set; } = null;
		/// <summary>
		/// 條碼2
		/// </summary>
		public string ItemBarCode2 { get; set; } = null;
		/// <summary>
		/// 條碼3
		/// </summary>
		public string ItemBarCode3 { get; set; } = null;
		/// <summary>
		/// 重量
		/// </summary>
		public string PickWarehouse { get; set; }
		/// <summary>
		/// 上架倉別(G:良品倉)
		/// </summary>
		public string PickWarehouseId { get; set; } = null;
		/// <summary>
		/// 揀貨倉庫編號
		/// </summary>
		public string Ltype { get; set; }
		/// <summary>
		/// 商品大分類
		/// </summary>
		public string Mtype { get; set; }
		/// <summary>
		/// 商品中分類
		/// </summary>
		public string Stype { get; set; }
		/// <summary>
		/// 商品小分類
		/// </summary>
		public string UnitId { get; set; }
		/// <summary>
		/// 最小單位數量
		/// </summary>
		public int? UnitQty { get; set; }
		/// <summary>
		/// 商品負責人員編號
		/// </summary>
		public string ItemStaff { get; set; } = null;
		/// <summary>
		/// 商品顏色
		/// </summary>
		public string ItemColor { get; set; } = null;
		/// <summary>
		/// 商品尺寸
		/// </summary>
		public string ItemSize { get; set; } = null;
		/// <summary>
		/// 商品規格
		/// </summary>
		public string ItemSpec { get; set; } = null;
		/// <summary>
		/// 溫層(01:常溫26-30、02:恆溫8-18、03:冷藏-2~10、04:冷凍-18~-25)
		/// </summary>
		public string TmprType { get; set; }
		/// <summary>
		/// 商品抽驗比例(%)
		/// </summary>
		public decimal? CheckPercent { get; set; }
		/// <summary>
		/// 長(cm)
		/// </summary>
		public decimal? PackLength { get; set; }
		/// <summary>
		/// 寬(cm)
		/// </summary>
		public decimal? PackWidth { get; set; }
		/// <summary>
		/// 高(cm)
		/// </summary>
		public decimal? PackHeight { get; set; }
		/// <summary>
		/// 重量(g)
		/// </summary>
		public decimal? PackWeight { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		public string Memo { get; set; } = null;
		/// <summary>
		/// 採購安全庫存量
		/// </summary>
		public long? POSafetyQty { get; set; }
		/// <summary>
		/// 補貨安全庫存量
		/// </summary>
		public long? PickSafetyQty { get; set; }
		/// <summary>
		/// 補貨最小單位數
		/// </summary>
		public int? PickSaveOrd { get; set; }
		/// <summary>
		/// 允收天數
		/// </summary>
		public short? AllDln { get; set; }
		/// <summary>
		/// 放寬允收期限
		/// </summary>
		public short? AllowAllDln { get; set; }
		/// <summary>
		/// 保存天數
		/// </summary>
		public int? SaveDay { get; set; }
		/// <summary>
		/// 供應商最低訂量
		/// </summary>
		public int? VenOrd { get; set; }
		/// <summary>
		/// 進倉單訂購倍數
		/// </summary>
		public int? RetOrd { get; set; }
		/// <summary>
		/// 易碎品(0: 否, 1: 是)
		/// </summary>
		public string IsFragile { get; set; } = null;
		/// <summary>
		/// 防溢漏(0: 否, 1: 是)
		/// </summary>
		public string IsSpill { get; set; } = null;
		/// <summary>
		/// 組合商品註記(0:否;1:是)
		/// </summary>
		public string MultiFlag { get; set; } = null;
		/// <summary>
		/// 是否紙箱 (0: 否 1: 是)
		/// </summary>
		public string IsCarton { get; set; } = null;
		/// <summary>
		/// 是否可混批 (0: 否 1: 是)
		/// </summary>
		public string MixBatchno { get; set; } = null;
		/// <summary>
		/// 是否儲位混商品 (0: 否 1: 是)
		/// </summary>
		public string LocMixItem { get; set; } = null;
		/// <summary>
		/// 序號綁儲位 (0: 否 1: 是)
		/// </summary>
		public string BundleSerialloc { get; set; } = null;
		/// <summary>
		/// 序號商品 (0: 否 1: 是)
		/// </summary>
		public string BundleSerialno { get; set; } = null;
		/// <summary>
		/// 序號碼數
		/// </summary>
		public short? SerialnoDigit { get; set; }
		/// <summary>
		/// 序號開頭
		/// </summary>
		public string SerialBegin { get; set; }
		/// <summary>
		/// 序號檢查規則  (0: 純數 1: 非純數)
		/// </summary>
		public string SerialRule { get; set; } = null;
		/// <summary>
		/// 是否為越庫商品 (0: 否 1: 是)
		/// </summary>
		public string CDFlag { get; set; } = null;
		/// <summary>
		/// 是否原箱出貨(0: 否 1: 是)
		/// </summary>
		public string AllowOrdItem { get; set; } = null;
		/// <summary>
		/// 批號管控商品(0: 否 1: 是)
		/// </summary>
		public string MakenoRequ { get; set; } = null;
		/// <summary>
		/// 是否可退貨(0: 否1: 是)
		/// </summary>
		public string ItemReturn { get; set; } = null;
		/// <summary>
		/// 是否為效期商品(null: 未選擇、0: 否、1: 是)
		/// </summary>
		public string NeedExpired { get; set; } = null;
		/// <summary>
		/// 警示天數(原本的允售天數)
		/// </summary>
		public int? AllShp { get; set; } = null;
		/// <summary>
		/// EAN/ISBN
		/// </summary>
		public string ItemBarCode4 { get; set; } = null;
		/// <summary>
		/// 首次進貨日
		/// </summary>
		public DateTime? FirstInDate { get; set; } = null;
		/// <summary>
		/// 廠商代碼
		/// </summary>
		public string VnrCode { get; set; } = null;
		/// <summary>
		/// 貨主品號
		/// </summary>
		public string CustItemCode { get; set; } = null;
		/// <summary>
		/// 是否易遺失
		/// </summary>
		public string IsEasyLose { get; set; }
		/// <summary>
		/// 貴重品標示
		/// </summary>
		public string IsPrecious { get; set; }
		/// <summary>
		/// 強磁標示
		/// </summary>
		public string IsMagnetic { get; set; }
        /// <summary>
        /// 易變質標示(0: 否, 1: 是)
        /// </summary>
        public string IsPerishable { get; set; }
		/// <summary>
		/// 需溫控標示(0: 否, 1: 是)
		/// </summary>
		public string IsTempControl { get; set; }
    /// <summary>
    /// 廠商料號 (料號)
    /// </summary>
    public string VnrItemCode { get; set; }
    /// <summary>
    /// 原廠商編號
    /// </summary>
    public string OriVnrCode { get; set; }
  }

	public class PostItemGroupModel
	{
		/// <summary>
		/// 同商品編號最後一筆
		/// </summary>
		public PostItemDataItemsModel LastData { get; set; }
		/// <summary>
		/// 同商品編號&商品單位編號筆數
		/// </summary>
		public int Count { get; set; }
	}

  /// <summary>
  /// 批次商品新增API(PostItemData)新增用結構 (簡化的F1913)
  /// </summary>
  public class PostItemData
  {
    //ORD_SAVE_QTY = 0,
    //BORROW_DAY = 7,
    //CAN_SELL = "0",
    //CAN_SPILT_IN = "0",
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public string ITEM_CODE { get; set; }
    public string VEN_ORD { get; set; }
    public string RET_ORD { get; set; }
    public Int16? ALL_DLN { get; set; }
    public string PICK_WARE { get; set; }
    public string C_D_FLAG { get; set; }
    public string MULTI_FLAG { get; set; }
    public string MIX_BATCHNO { get; set; }
    public string ALLOWORDITEM { get; set; }
    public string BUNDLE_SERIALLOC { get; set; }
    public string BUNDLE_SERIALNO { get; set; }
    public Int64 PICK_SAVE_QTY { get; set; }
    public string ITEM_RETURN { get; set; }
    public string LOC_MIX_ITEM { get; set; }
    public Int16? SERIALNO_DIGIT { get; set; }
    public string SERIAL_BEGIN { get; set; }
    public string SERIAL_RULE { get; set; }
    public Int32? SAVE_DAY { get; set; }
    public string ITEM_STAFF { get; set; }
    public decimal? CHECK_PERCENT { get; set; }
    public Int32 PICK_SAVE_ORD { get; set; }
    public string ISCARTON { get; set; }
    public string LTYPE { get; set; }
    public string MTYPE { get; set; }
    public string STYPE { get; set; }
    public string ITEM_NAME { get; set; }
    public string EAN_CODE1 { get; set; }
    public string EAN_CODE2 { get; set; }
    public string EAN_CODE3 { get; set; }
    public string ITEM_ENGNAME { get; set; }
    public string ITEM_COLOR { get; set; }
    public string ITEM_SIZE { get; set; }
    public Int16? ITEM_HUMIDITY { get; set; }
    public string ITEM_SPEC { get; set; }
    public string TMPR_TYPE { get; set; }
    public string FRAGILE { get; set; }
    public string SPILL { get; set; }
    public string ITEM_UNIT { get; set; }
    public string MEMO { get; set; }
    public string PICK_WARE_ID { get; set; }
    public string CUST_ITEM_NAME { get; set; }
    public string MAKENO_REQU { get; set; }
    public string TYPE { get; set; }
    public string NEED_EXPIRED { get; set; }
    public Int32? ALL_SHP { get; set; }
    public string EAN_CODE4 { get; set; }
    public DateTime? FIRST_IN_DATE { get; set; }
    public string VNR_CODE { get; set; }
    public string CUST_ITEM_CODE { get; set; }
    public string IS_PRECIOUS { get; set; }
    public string IS_MAGNETIC { get; set; }
    public string IS_PERISHABLE { get; set; }
    public string IS_TEMP_CONTROL { get; set; }
    public string VNR_ITEM_CODE { get; set; }
    public string ORI_VNR_CODE { get; set; }
  }
}
