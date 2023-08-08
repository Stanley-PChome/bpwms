using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
  #region 進貨檢驗-驗收資料查詢
  /// <summary>
  /// 進貨檢驗-驗收資料查詢_傳入
  /// </summary>
  public class GetStockReceivedDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號/貨主單據號碼
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 品號/國際條碼/貨主品編
    /// </summary>
    public string ItemNo { get; set; }

  }

  /// <summary>
  /// 進貨檢驗-驗收資料查詢_傳回
  /// </summary>
  public class GetStockReceivedDataRes
  {
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 貨主單號  
    /// </summary>
    public string CustOrdNo { get; set; }
    /// <summary>
    /// 進倉單狀態
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// 進倉單狀態中文說明
    /// </summary>
    public string StatusDesc { get; set; }
    /// <summary>
    /// 廠商編號
    /// </summary>
    public string VnrCode { get; set; }
    /// <summary>
    /// 廠商名稱
    /// </summary>
    public string VnrName { get; set; }
    /// <summary>
    /// 快速通關分類
    /// </summary>
    public string FastPassType { get; set; }
    /// <summary>
    /// 快速通關分類中文說明
    /// </summary>
    public string FastPassTypeDesc { get; set; }
    /// <summary>
    /// 商品號碼
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 貨主品編
    /// </summary>
    public string CustItemCode { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// 訂購數量
    /// </summary>
    public int Qty { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public int StockSeq { get; set; }
    /// <summary>
    /// PO單號
    /// </summary>
    public string PoNo { get; set; }
    /// <summary>
    /// 是否虛擬商品
    /// </summary>
    public Boolean IsVirtualItem { get; set; }
    public Boolean CanOperator { get; set; }
		/// <summary>
		/// 貨主自訂分類
		/// </summary>
		public string CustCost { get; set; }
  }
  #endregion 進貨檢驗-驗收資料查詢

  #region 進貨檢驗-驗收明細資料查詢
  /// <summary>
  /// 進貨檢驗-驗收明細資料查詢_傳入
  /// </summary>
  public class GetStockReceivedDetDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public string WmsSeq { get; set; }
    /// <summary>
    /// 棧板容器儲位條碼
    /// </summary>
    public string PalletLocation { get; set; }
    /// <summary>
    /// 更換人員
    /// </summary>
    public Boolean IsChangeUser { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 是否虛擬商品
    /// </summary>
    public Boolean IsVirtualItem { get; set; }
    /// <summary>
    /// 工作站編號
    /// </summary>
    public string WorkStationCode { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-驗收明細資料查詢_傳回
  /// </summary>
  public class GetStockReceivedDetDataRes
  {
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public string StockSeq { get; set; }
    /// <summary>
    /// 商品品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 料號
    /// </summary>
    public string VnrItemCode { get; set; }
    /// <summary>
    /// 進貨數
    /// </summary>
    public int OrderQty { get; set; }
    /// <summary>
    /// 累積驗收數
    /// </summary>
    public int TotalRecvQty { get; set; }
    /// <summary>
    /// 本次驗收數
    /// </summary>
    public int RecvQty { get; set; }
    /// <summary>
    /// 良品數
    /// </summary>
    public int RecvGoodQty { get; set; }
    /// <summary>
    /// 不良品數
    /// </summary>
    public int RecvBadQty { get; set; }
    /// <summary>
    /// 序號商品
    /// </summary>
    public string BundleSerialNo { get; set; }
    /// <summary>
    /// 商品檢驗狀態(0:未完成、1:已完成)
    /// </summary>
    public string CheckItem { get; set; }
    /// <summary>
    /// 商品序號檢驗狀態(0:未完成、1:已完成)
    /// </summary>
    public string CheckSerial { get; set; }
    /// <summary>
    /// 上架倉別
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 上架倉別名稱
    /// </summary>
    public string TarWarehouseName { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }

		/// <summary>
		/// 採購單號
		/// </summary>
		public string PoNo { get; set; }

		/// <summary>
		/// 今日已驗收數
		/// </summary>
		public int TodayRecvQty { get; set; }

    /// <summary>
    /// 錯誤類型(1:工作站欄位錯誤)
    /// </summary>
    public string ErrorType { get; set; }

    /// <summary>
    /// 外部串接Api回傳訊息內容清單
    /// </summary>
    public List<string> ApiFailureMsgList { get; set; }
    
  }
  #endregion 進貨檢驗-驗收明細資料查詢

  #region 進貨檢驗-驗收商品查詢
  /// <summary>
  /// 進貨檢驗-驗收商品查詢_傳入
  /// </summary>
  public class RecvItemDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public string WmsSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-驗收商品查詢_傳回
  /// </summary>
  public class RecvItemDataRes
  {
    /// <summary>
    /// 商品編號
    /// </summary>
    public String ItemCode { get; set; }
    /// <summary>
    /// 貨主品編
    /// </summary>
    public String CustItemCode { get; set; }
    /// <summary>
    /// 廠商品編
    /// </summary>
    public String VnrItemCode { get; set; }
    /// <summary>
    /// 國條
    /// </summary>
    public String EanCode1 { get; set; }
    /// <summary>
    /// 條碼二
    /// </summary>
    public String EanCode2 { get; set; }
    /// <summary>
    /// 條碼三
    /// </summary>
    public String EanCode3 { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    public String ItemName { get; set; }
    /// <summary>
    /// 尺寸
    /// </summary>
    public String ItemSize { get; set; }
    /// <summary>
    /// 顏色
    /// </summary>
    public String ItemColor { get; set; }
    /// <summary>
    /// 規格
    /// </summary>
    public String ItemSpec { get; set; }
    /// <summary>
    /// 抽驗數量
    /// </summary>
    public int CheckQty { get; set; }
    /// <summary>
    /// 驗收註記
    /// </summary>
    public String RcvMemo { get; set; }
    /// <summary>
    /// 是否效期商品
    /// </summary>
    public String NeedExpired { get; set; }
    /// <summary>
    /// 保存天數
    /// </summary>
    public int? SaveDay { get; set; }
    /// <summary>
    /// 允收天數
    /// </summary>
    public int? AllowDelvDay { get; set; }
    /// <summary>
    /// 警示天數
    /// </summary>
    public int? AllowShipDay { get; set; }
    /// <summary>
    /// 是否序號商品
    /// </summary>
    public String BundleSerialNo { get; set; }
    /// <summary>
    /// 商品長
    /// </summary>
    public Decimal PackLength { get; set; }
    /// <summary>
    /// 商品寬
    /// </summary>
    public Decimal PackWidth { get; set; }
    /// <summary>
    /// 商品高
    /// </summary>
    public decimal PackHight { get; set; }
    /// <summary>
    /// 商品重量
    /// </summary>
    public decimal PackWeight { get; set; }
    /// <summary>
    /// 商品溫層
    /// </summary>
    public String TmprType { get; set; }
    /// <summary>
    /// 是否蘋果商品
    /// </summary>
    public String IsApple { get; set; }
    /// <summary>
    /// 驗收數量
    /// </summary>
    public int RecvQty { get; set; }
    /// <summary>
    /// 是否列印商品ID標
    /// </summary>
    public String IsPrintItemId { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    public String ValidDate { get; set; }
    /// <summary>
    /// 是否貴重品
    /// </summary>
    public String IsPrecious { get; set; }
    /// <summary>
    /// 是否易碎品
    /// </summary>
    public String IsFragile { get; set; }
    /// <summary>
    /// 是否易遺失
    /// </summary>
    public String IsEasyLose { get; set; }
    /// <summary>
    /// 是否強磁標示
    /// </summary>
    public String IsMagentic { get; set; }
    /// <summary>
    /// 是否防溢漏包裝
    /// </summary>
    public String IsSpill { get; set; }
    /// <summary>
    /// 是否易變質標示
    /// </summary>
    public String IsPerishable { get; set; }
    /// <summary>
    /// 是否需溫控標示
    /// </summary>
    public String IsTempControl { get; set; }
    /// <summary>
    /// 是否首次驗收商品
    /// </summary>
    public String IsFirstInDate { get; set; }
    /// <summary>
    /// 商品檢驗項目清單
    /// </summary>
    public List<CheckOpt> CheckOptList { get; set; }
    /// <summary>
    /// 商品溫層清單
    /// </summary>
    public List<TmprOpt> TmprOptList { get; set; }
    /// <summary>
    /// 上架倉別編號
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 上架倉別名稱
    /// </summary>
    public string TarWarehouseName { get; set; }
    /// <summary>
    /// 是否可修改上架倉別
    /// </summary>
    public string CanEditTareWarehouse { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-驗收商品查詢_商品溫層_傳回
  /// </summary>
  public class TmprOpt
  {
    /// <summary>
    /// 商品溫層編號
    /// </summary>
    public string TmprNo { get; set; }
    /// <summary>
    /// 商品溫層名稱
    /// </summary>
    public string TmprName { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-驗收商品查詢_商品檢驗項目_傳回
  /// </summary>
  public class CheckOpt
  {
    /// <summary>
    /// 檢項項目代碼
    /// </summary>
    public string CheckNo { get; set; }
    /// <summary>
    /// 檢驗項目名稱
    /// </summary>
    public string CheckName { get; set; }
  }


	#endregion 進貨檢驗-驗收商品查詢

	#region 進貨檢驗-序號刷讀查詢
	public class GetSerialItemDataReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }

		/// <summary>
		/// 進倉單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 進倉單序號
		/// </summary>
		public string WmsSeq { get; set; }
		/// <summary>
		/// 驗收單號
		/// </summary>
		public string RtNo { get; set; }
		/// <summary>
		/// 驗收項次
		/// </summary>
		public string RtSeq { get; set; }

		/// <summary>
		/// 採購單號
		/// </summary>
		public string PoNo { get; set; }

		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }

	}

	public class GetSerialItemDataRes
	{
		public string ItemCode { get; set; }
		/// <summary>
		/// 商品效期
		/// </summary>
		public string ValidDate { get; set; }
		/// <summary>
		/// 序號處理模式 (0: 序號抽驗模式、1: 序號收集模式)
		/// </summary>
		public string CheckMode { get; set; }
		/// <summary>
		/// 應刷數
		/// </summary>
		public int NeedSerialQty { get; set; }
		/// <summary>
		/// 已刷數
		/// </summary>
		public int ReadSerialQty { get; set; }

		/// <summary>
		/// 已刷序號清單
		/// </summary>
		public List<string> ItemSerialList { get; set; }
	}


	#endregion

	#region 進貨檢驗-序號登錄/序號刪除
	public class AddOrDelItemSerialNoReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// PO單號(採購單號)
		/// </summary>
		public string PoNo { get; set; }
		/// <summary>
		/// 進倉單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 進倉單Seq
		/// </summary>
		public string WmsSeq { get; set; }
		/// <summary>
		/// 驗收單號碼
		/// </summary>
		public string RtNo { get; set; }

		/// <summary>
		/// 驗收單號碼
		/// </summary>
		public string RtSeq { get; set; }


		/// <summary>
		/// 序號處理模式(0:序號抽驗模式 1:序號收集模式)
		/// </summary>
		public string CheckMode { get; set; }

		/// <summary>
		/// 處理方式(0:新增 1:刪除)
		/// </summary>
		public string ProcMode { get; set; }

		/// <summary>
		/// 應刷數
		/// </summary>
		public int? NeedQty { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		
		/// <summary>
		/// 刷入序號
		/// </summary>
		public string SerialNo { get; set; }
	}

	#endregion

	#region 進貨檢驗-不良品查詢

	public class GetDefectItemDataReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }

		/// <summary>
		/// 進倉單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 進倉單序號
		/// </summary>
		public string WmsSeq { get; set; }
		/// <summary>
		/// 驗收單號
		/// </summary>
		public string RtNo { get; set; }
		/// <summary>
		/// 驗收項次
		/// </summary>
		public string RtSeq { get; set; }

	}

	public class GetDefectItemDataRes
	{
		public string ValidDate { get; set; }
		public List<DefectItem> DefectDetail { get; set; }
		public List<WarehouseItem> WarehouseList { get; set; }
		public List<CauseItem> CauseList { get; set; }
		public WarehouseItem DfWarehouse { get; set; }
	}
	public class DefectItem
	{
		public long Id { get; set; }
		public string WarehouseId { get; set; }
		public string WarehouseName { get; set; }

		public int Qty { get; set; }
		public string UccName { get; set; }
		public string Cause { get; set; }
		public string SerialNo { get; set; }
	}
	public class WarehouseItem
	{
		public string WarehouseId { get; set; }
		public string WarehouseName { get; set; }
	}

	public class CauseItem
	{
		public string CauseCode { get; set; }
		public string CauseName { get; set; }
	}
	#endregion

	public class AddOrDelDefectItemReq: StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 進倉單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 進倉單項次
		/// </summary>
		public string WmsSeq { get; set; }

		/// <summary>
		/// 驗收單號
		/// </summary>
		public string RtNo { get; set; }
		/// <summary>
		/// 驗收項次
		/// </summary>
		public string RtSeq { get; set; }

		/// <summary>
		/// 採購單號
		/// </summary>
		public string PoNo { get; set; }

		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }

		/// <summary>
		/// 商品序號
		/// </summary>
		public string SerialNo { get; set; }

		/// <summary>
		/// 不良品上架倉別
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 不良品原因代碼
		/// </summary>
		public string UccCode { get; set; }
		/// <summary>
		/// 備註說明
		/// </summary>
		public string Memo { get; set; }

		/// <summary>
		/// 不良品數量
		/// </summary>
		public int? Qty { get; set; }

		/// <summary>
		/// 處理方式(0: 新增、1: 刪除)
		/// </summary>
		public string ProcMode { get; set; }
		/// <summary>
		/// 不良品明細ID
		/// </summary>
		public long? DefectId { get; set; }
	}
	/// <summary>
	/// 
	/// </summary>
	public class InsertOrUpdateNGRecordReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 進倉單Seq
    /// </summary>
    public string StockSeq { get; set; }
    /// <summary>
    /// F02020109.ID (新增給0)
    /// </summary>
    public Int64 ID { get; set; }
    /// <summary>
    /// 異動模式 A:新增 D:刪除
    /// </summary>
    public string ChangeFlag { get; set; }
    /// <summary>
    /// 不良品數量
    /// </summary>
    public int? DeffectQty { get; set; }
    /// <summary>
    /// 不良品序號
    /// </summary>
    public string SerialNo { get; set; }
    /// <summary>
    /// 原因代碼
    /// </summary>
    public string UccCode { get; set; }
    /// <summary>
    /// 原因
    /// </summary>
    public string Cause { get; set; }
    /// <summary>
    /// 備註
    /// </summary>
    public string Memo { get; set; }
    /// <summary>
    /// 不良品倉別
    /// </summary>
    public string WarehouseId { get; set; }
  }

  



  #region 進貨檢驗-儲存商品驗收註記
  /// <summary>
  /// 進貨檢驗-儲存商品驗收註記_傳入
  /// </summary>
  public class SaveRecvItemMemoReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemNo { get; set; }
    /// <summary>
    /// 驗收註記
    /// </summary>
    public string RcvMemo { get; set; }
  }
  #endregion 進貨檢驗-儲存商品驗收註記

  #region 進貨檢驗-取得物流中心進貨可上架倉別清單
  /// <summary>
  /// 進貨檢驗-取得物流中心進貨可上架倉別清單_傳入
  /// </summary>
  public class GetWarhouseInDcTarWarehouseListReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-取得物流中心進貨可上架倉別清單_傳回
  /// </summary>
  public class GetWarhouseInDcTarWarehouseListRes
  {
    /// <summary>
    /// 倉庫編號
    /// </summary>
    public string WarehouseId { get; set; }
    /// <summary>
    /// 倉庫名稱
    /// </summary>
    public string WarehouseName { get; set; }
  }
  #endregion 進貨檢驗-取得物流中心進貨可上架倉別清單

  #region 進貨檢驗-儲存商品驗收結果
  /// <summary>
  /// 進貨檢驗-儲存商品驗收結果_傳入
  /// </summary>
  public class CheckandSaveRecvItemDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public string WmsSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }

		public string PoNo { get; set; }
    /// <summary>
    /// 商品編號
    /// </summary>
    public string ItemNo { get; set; }
    /// <summary>
    /// 國條
    /// </summary>
    public string EanCode1 { get; set; }
    /// <summary>
    /// 條碼二
    /// </summary>
    public string EanCode2 { get; set; }
    /// <summary>
    /// 條碼三
    /// </summary>
    public string EanCode3 { get; set; }
    /// <summary>
    /// 是否效期商品
    /// </summary>
    public string NeedExpired { get; set; }
    /// <summary>
    /// 保存天數
    /// </summary>
    public int? SaveDay { get; set; }
    /// <summary>
    /// 允收天數
    /// </summary>
    public int? AllowDelvDay { get; set; }
    /// <summary>
    /// 警示天數
    /// </summary>
    public int? AllowShipDay { get; set; }
    /// <summary>
    /// 是否序號商品
    /// </summary>
    public string BundleSerialNo { get; set; }
    /// <summary>
    /// 商品長
    /// </summary>
    public decimal PackLength { get; set; }
    /// <summary>
    /// 商品寬
    /// </summary>
    public decimal PackWidth { get; set; }
    /// <summary>
    /// 商品高
    /// </summary>
    public decimal PackHight { get; set; }
    /// <summary>
    /// 商品重量
    /// </summary>
    public decimal PackWeight { get; set; }
    /// <summary>
    /// 商品溫層
    /// </summary>
    public string TmprType { get; set; }
    /// <summary>
    /// 是否蘋果商品
    /// </summary>
    public string IsApple { get; set; }
    /// <summary>
    /// 驗收數量
    /// </summary>
    public int RecvQty { get; set; }
    /// <summary>
    /// 是否列印商品ID標
    /// </summary>
    public string IsPrintItemId { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    public string ValidDate { get; set; }
    /// <summary>
    /// 是否貴重品
    /// </summary>
    public string IsPrecious { get; set; }
    /// <summary>
    /// 是否易碎品
    /// </summary>
    public string IsFragile { get; set; }
    /// <summary>
    /// 是否易遺失
    /// </summary>
    public string IsEasyLose { get; set; }
    /// <summary>
    /// 是否強磁標示
    /// </summary>
    public string IsMagentic { get; set; }
    /// <summary>
    /// 是否防溢漏包裝
    /// </summary>
    public string IsSpill { get; set; }
    /// <summary>
    /// 是否易變質標示
    /// </summary>
    public string IsPerishable { get; set; }
    /// <summary>
    /// 是否需溫控標示
    /// </summary>
    public string IsTempControl { get; set; }
    /// <summary>
    /// 是否首次驗收商品
    /// </summary>
    public string IsFirstInDate { get; set; }
    /// <summary>
    /// 上架倉別
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 商品檢驗項目清單
    /// </summary>
    public List<CheckNoList> CheckNoList { get; set; }
  }

  public class CheckNoList
  {
    /// <summary>
    /// 檢項項目代碼
    /// </summary>
    public string CheckNo { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-儲存商品驗收結果_傳回
  /// </summary>
  public class CheckandSaveRecvItemDataRes
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemNo { get; set; }
    /// <summary>
    /// 驗收註記
    /// </summary>
    public string RcvMemo { get; set; }
  }
  #endregion 進貨檢驗-儲存商品驗收結果

  /// <summary>
  /// 進貨檢驗_計算允收天數與警示天數_傳入
  /// </summary>
  public class CountDelvDayAndShipDayReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }

    /// <summary>
    /// 登入者帳號
    /// </summary>
    public string AccNo { get; set; }

    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }

    /// <summary>
    /// 保存天數
    /// </summary>
    public int SaveDay { get; set; }
  }

  #region 進貨檢驗-驗收完成
  /// <summary>
  /// 進貨檢驗-驗收完成_傳入
  /// </summary>
  public class RecvItemCompletedReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public string WmsSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 工作站編號
    /// </summary>
    public string WorkStationCode { get; set; }
  }

  /// <summary>
  /// 進貨檢驗-驗收完成_傳回
  /// </summary>
  public class RecvItemCompletedRes
  {
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 是否要回到查詢頁
    /// </summary>
    public Boolean IsGoQueryPage { get; set; }

    /// <summary>
    /// 外部串接Api回傳訊息內容清單
    /// </summary>
    public List<string> ApiFailureMsgList { get; set; }
  }
  #endregion 進貨檢驗-驗收完成

  #region 進貨檢驗-刪除驗收紀錄
  /// <summary>
  /// 進貨檢驗-刪除驗收紀錄_傳入
  /// </summary>
  public class DelectRecvItemDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public string WmsSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
  }
  #endregion 進貨檢驗-刪除驗收紀錄
}
