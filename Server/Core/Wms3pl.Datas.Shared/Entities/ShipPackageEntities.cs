using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.Shared.Entities
{
    #region 出貨容器條碼檢核
    public class CheckShipContainerCodeReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 容器條碼
        /// </summary>
        public string ContainerCode { get; set; }
        /// <summary>
        /// 出貨模式(1:單人包裝站 2:包裝線包裝站)
        /// </summary>
        public string ShipMode { get; set; }
    }

    public class CheckShipContainerCodeRes
    {
        public ExecuteResult Result { get; set; } = new ExecuteResult();
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 單號
        /// </summary>
        public string WmsNo { get; set; }
        /// <summary>
        /// 是否為特殊結構單
        /// </summary>
        public bool IsSpecialOrder { get; set; }
        /// <summary>
        /// 特殊結構揀貨類型
        /// </summary>
        public string PickType { get; set; }
    }
    #endregion

    #region 查詢與檢核出貨單資訊
    public class SearchAndCheckWmsOrderInfoReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
        /// <summary>
        /// 工作站編號
        /// </summary>
        public string WorkStationId { get; set; }
        /// <summary>
        /// 出貨模式(1:單人包裝站 2:包裝線包裝站)
        /// </summary>
        public string ShipMode { get; set; }
    }

    public class SearchAndCheckWmsOrderInfoRes
    {
        public ExecuteResult Result { get; set; } = new ExecuteResult();
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
        /// <summary>
        /// 出貨單狀態(0:未包裝2:已包裝 5:已出貨 9:已取消)
        /// </summary>
        public Decimal Status { get; set; }
        /// <summary>
        /// 批次日期
        /// </summary>
        public DateTime DelvDate { get; set; }
        /// <summary>
        /// 批次時段
        /// </summary>
        public string PickTime { get; set; }
        /// <summary>
        /// 商品是否過刷
        /// </summary>
        public string IsPackCheck { get; set; }
        /// <summary>
        /// 建議箱號
        /// </summary>
        public string SugBoxNo { get; set; }
        /// <summary>
        /// 箱數
        /// </summary>
        public int BoxCnt { get; set; }
        /// <summary>
        /// 單據類型(01:一般出貨 02:廠退出貨)
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// 工作站類型=建議出貨包裝線類型(空白=不指定; PA1=小線;PA2=大線 
        /// </summary>
        public string PackingType { get; set; }
        /// <summary>
        /// 出貨商品清單
        /// </summary>
        public List<ItemModel> ItemList { get; set; }
        /// <summary>
        /// 紙箱商品清單
        /// </summary>
        public List<BoxItemModel> CartonItemList { get; set; }
        /// <summary>
        /// 訂單取消或出貨單取消
        /// </summary>
        public Boolean IsCancelOrder { get; set; }
    }
    #endregion

    #region 查詢出貨商品包裝明細
    public class SearchWmsOrderPackingDetailReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("RowNum")]
    public class SearchWmsOrderPackingDetailRes
    {
        [DataMember]
        public Decimal RowNum { get; set; }
        /// <summary>
        /// 是否原箱商品
        /// </summary>
        [DataMember]
        public string IsOriItem { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [DataMember]
        public string ItemCode { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        [DataMember]
        public string ItemName { get; set; }
        /// <summary>
        /// 出貨數
        /// </summary>
        [DataMember]
        public int ShipQty { get; set; }
        /// <summary>
        /// 包裝數
        /// </summary>
        [DataMember]
        public int PackageQty { get; set; }
        /// <summary>
        /// 累積包裝數
        /// </summary>
        [DataMember]
        public int TotalPackageQty { get; set; }
        /// <summary>
        /// 差異數
        /// </summary>
        [DataMember]
        public int DiffQty { get; set; }
        /// <summary>
        /// 商品特徵
        /// </summary>
        [DataMember]
        public string Feature { get; set; }
        /// <summary>
        /// 商品溫層
        /// </summary>
        [DataMember]
        public string TmprTypeName { get; set; }
    }
    #endregion

    #region 查詢出貨單刷讀紀錄
    public class SearchWmsOrderScanLogReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("RowNum")]
    public class SearchWmsOrderScanLogRes
    {
        [DataMember]
        public Decimal RowNum { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        [DataMember]
        public string SerialNo { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [DataMember]
        public string ItemCode { get; set; }
        /// <summary>
        /// 通過
        /// </summary>
        [DataMember]
        public string IsPass { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
    #endregion

    #region 刷讀商品條碼
    public class ScanItemBarcodeReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
        /// <summary>
        /// 批次日期
        /// </summary>
        public DateTime DelvDate { get; set; }
        /// <summary>
        /// 批次時段
        /// </summary>
        public string PickTime { get; set; }
        /// <summary>
        /// 刷讀商品條碼
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 數量(預設為1)
        /// </summary>
        public int Qty { get; set; } = 1;
        /// <summary>
        /// 單據類型(01:一般出貨 02:廠退出貨)
        /// </summary>
        public string OrdType { get; set; }
        /// <summary>
        /// 出貨品號商品清單
        /// </summary>
        public List<ItemModel> ItemList { get; set; }
        /// <summary>
        /// 紙箱商品清單
        /// </summary>
        public List<BoxItemModel> BoxItemList { get; set; }
        /// <summary>
        /// 動作 01:刷讀條碼 02:數量點擊確認按鈕
        /// </summary>
        public string Action { get; set; }
    }

    public class ScanItemBarcodeRes
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessed { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 箱序
        /// </summary>
        public int? PackageBoxNo { get; set; }
        /// <summary>
        /// 是否關箱
        /// </summary>
        public bool IsCloseBox { get; set; }
    }
    #endregion

    #region 關箱處理
    public class CloseShipBoxReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
        /// <summary>
        /// 指定箱序
        /// </summary>
        public int? PackageBoxNo { get; set; }
        /// <summary>
        /// 單據類型(01:一般出貨 02:廠退出貨) 
        /// </summary>
        public string OrdType { get; set; }
        /// <summary>
        /// 包裝模式(01:配箱+封箱,02:純配箱)
        /// </summary>
        public string PackageMode { get; set; }
        /// <summary>
        /// 是否要刷讀紙箱(0:否1:是) 
        /// </summary>
        public string IsScanBox { get; set; }
        /// <summary>
        /// 建議紙箱箱號
        /// </summary>
        public string SugBoxNo { get; set; }
        /// <summary>
        /// 出貨模式(1:單人包裝站 2:包裝線包裝站)
        /// </summary>
        public string ShipMode { get; set; }
        /// <summary>
        /// 工作站編號
        /// </summary>
        public string WorkStationId { get; set; }
        /// <summary>
        /// 動作是否為加箱
        /// </summary>
        public bool IsAppendBox { get; set; }
    }

    public class CloseShipBoxRes
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessed { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 最後一箱未關箱箱序
        /// </summary>
        public Int16? LastPackageBoxNo { get; set; }
        /// <summary>
        /// 報表清單
        /// </summary>
        public List<ShipPackageReportModel> ReportList { get; set; }
    }

    public class ShipPackageReportModel
    {
        /// <summary>
        /// 箱序
        /// </summary>
        public Int16 PackageBoxNo { get; set; }
        /// <summary>
        /// 貨主單號
        /// </summary>
        public string CustOrdNo { get; set; }
        /// <summary>
        /// 報表編號
        /// </summary>
        public string ReportCode { get; set; }
        /// <summary>
        /// 報表名稱
        /// </summary>
        public string ReportName { get; set; }
        /// <summary>
        /// 報表下載檔案位置
        /// </summary>
        public string ReportUrl { get; set; }
        /// <summary>
        /// 印表機編號(1=印表機1;2=印表機2;3=快速標籤機
        /// </summary>
        public string PrinterNo { get; set; }
        /// <summary>
        /// 報表項次
        /// </summary>
        public int ReportSeq { get; set; }

    public string ISPRINTED { get; set; }
    public DateTime? PRINT_TIME { get; set; }
    }

  /// <summary>
  /// 系統設定的建議箱號
  /// </summary>
  public class CloseShipSysBox
  {
    /// <summary>
    /// 箱號
    /// </summary>
    public string BOX_CODE { get; set; }
    /// <summary>
    /// 排序用
    /// </summary>
    public int OrderByValue { get; set; }
  }
    #endregion

  #region 使用出貨單容器資料產生箱明細
  public class UseShipContainerToBoxDetailReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
        /// <summary>
        /// 批次日期
        /// </summary>
        public DateTime DelvDate { get; set; }
        /// <summary>
        /// 批次時段
        /// </summary>
        public string PickTime { get; set; }
        /// <summary>
        /// 建議箱號(只有出貨單不需刷入出貨紙箱時才帶入建議紙箱)
        /// </summary>
        public string SubBoxNo { get; set; }
        /// <summary>
        /// 出貨模式(1:單人包裝站 2:包裝線包裝站)
        /// </summary>
        public string ShipMode { get; set; }
    /// <summary>
    /// 工作站編號
    /// </summary>
    public string WorkStationId { get; set; }
    }

    public class UseShipContainerToBoxDetailRes
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessed { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 出貨箱序
        /// </summary>
        public Int16 PackageBoxNo { get; set; }
    }
    #endregion

    #region 取消包裝
    public class CancelShipOrderReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
    }

    public class CancelShipOrderRes
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessed { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// F050801.STATUS 單據狀態(0:待處理 1:已包裝 2:已稽核待出貨  5:已出貨 6:已扣帳 9:不出貨)
        /// </summary>
        public decimal Status { get; set; }
    }
    #endregion

    #region 取得出貨單所有出貨箱要列印報表清單
    public class SearchShipReportListReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo { get; set; }
    }
	#endregion

	#region 取得箱明細報表資料
	public class GetBoxDetailReportReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號 
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 貨主編號 
		/// </summary>
		public string WmsOrdNo { get; set; }
		/// <summary>
		/// 箱序
		/// </summary>
		public short PackageBoxNo { get; set; }
	}

	public class GetBoxDetailReportRes
	{
		public BoxHeaderData BoxHeader { get; set; }
		public List<BoxDetailData> BoxDetail { get; set; }
	}
	public class BoxHeaderData : PcHomeDeliveryReport { }

	public class BoxDetailData : DeliveryReport { }
	#endregion

	#region 取得一般出貨小白標報表資料
	public class GetShipLittleLabelReportReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WmsOrdNo { get; set; }
		/// <summary>
		/// 箱序
		/// </summary>
		public short PackageBoxNo { get; set; }
	}

	public class GetShipLittleLabelReportRes
	{
		public List<Box> BoxLittleLabelDetail { get; set; }
	}

	public class Box
	{
		public string BoxBarCode { get; set; }
		public string BoxCode { get; set; }
	}
	#endregion

	#region 取得廠退出貨小白標報表資料
	public class GetRtnShipLittleLabelReportReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WmsOrdNo { get; set; }
		/// <summary>
		/// 箱序
		/// </summary>
		public short PackageBoxNo { get; set; }
	}

	public class GetRtnShipLittleLabelReportRes
	{
		public List<BoxRtnLittleLabel> BoxRtnLittleLabelDetail { get; set; }
	}

	public class BoxRtnLittleLabel : LittleWhiteReport { }
	#endregion

	#region 取得訂單出貨宅配單檔案
	public class GeShipFileReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 下載檔案url路徑
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// 是否補印(0=否 1=是)
		/// </summary>
		public int Reprint { get; set; }
	}

	public class GeShipFileRes
	{
		public bool IsSuccessed { get; set; }
		public string Message { get; set; }
		public string ContentType { get; set; }
		public byte[] FileBytes { get; set; }
	}
	#endregion

	#region 包裝站開站/關站紀錄
	public class SetPackageStationStatusLogReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WorkstationCode { get; set; }
		/// <summary>
		/// 包裝站狀態(01=開站 ,02關站)	
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 作業功能(1: 單人包裝站、2: 包裝線包裝站)
		/// </summary>
		public string WorkType { get; set; }
		/// <summary>
		/// 是否配箱站與封箱站分開
		/// </summary>
		public string NoSpecReport { get; set; }
		/// <summary>
		/// 是否需刷讀紙箱條碼關箱
		/// </summary>
		public string CloseByBoxNo { get; set; }
		/// <summary>
		/// 裝置名稱
		/// </summary>
		public string DeviceIp { get; set; }
	}

	public class SetPackageStationStatusLogRes : ExecuteResult { }

    #endregion

    #region 商品 紙箱商品
    public class ItemModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string IsOriItem { get; set; }
        public string EanCode1 { get; set; }
        public string EanCode2 { get; set; }
        public string EanCode3 { get; set; }
        public string BundleSerialNo { get; set; }
        public string BundleSerialLoc { get; set; }
    }

    public class BoxItemModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }
    #endregion

	#region
	public class SetPackageLineStationStatusReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WorkstationCode { get; set; }
		/// <summary>
		/// 工作站類型(PA1大線、PA小線)
		/// </summary>
		public string WorkstationType { get; set; }
		/// <summary>
		/// 包裝線狀態(0=關站;1=開站;2暫停;3=關站中;4=繼續)
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 是否配箱站與封箱站分開
		/// </summary>
		public string NoSpecReports { get; set; }
		/// <summary>
		/// 否需刷讀紙箱條碼關箱
		/// </summary>
		public string CloseByBoxno { get; set; }
		/// <summary>
		/// 裝置名稱
		/// </summary>
		public string DeviceIp { get; set; }
        /// <summary>
        /// 是否還有未完成單據
        /// </summary>
        public bool HasUndone { get; set; }
	}

	public class SetPackageLineStationStatusRes: ExecuteResult { }
	#endregion

	#region 變更出貨單為所有商品都須過刷
	public class ChangeShipPackCheckReq
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string WmsOrdNo { get; set; }
	}

	public class ChangeShipPackCheckRes:ExecuteResult { }
	#endregion

	#region 取得包裝線工作站分配結果
	public class GetWorkStataionShipDataReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string workstationCode { get; set; }
	}

	public class GetWorkStataionShipDataRes
	{
		/// <summary>
		/// 是否有抵達工作站容器
		/// </summary>
		public bool IsArrialContainer { get; set; }
		/// <summary>
		/// 已抵達工作站第一箱容器條碼
		/// </summary>
		public string ArrivalContainerCode { get; set; }
		/// <summary>
		/// 已抵達工作站的單號
		/// </summary>
		public string ArrivalWmsNo { get; set; }
		/// <summary>
		/// 已抵達工作站的單號
		/// </summary>
		public int WaitWmsOrderCnt { get; set; }
		/// <summary>
		/// 工作站等待容器數量
		/// </summary>
		public int WaitContainerCnt { get; set; }
	}
	#endregion

    #region 取得出貨單容器資料請求參數
    public class GetShipLogisticBoxReq
    {
        public string DcCode { get; set; }
        public string GupCode { get; set; }
        public string CustCode { get; set; }
        public string WmsOrdNo { get; set; }
    }
    #endregion 取得出貨單容器資料請求參數

    #region 
    public class GetShipLogisticBoxRes : ExecuteResult
    { public List<GetShipLogisticBoxData> Datas { get; set; } }

    public class GetShipLogisticBoxData
    {
        public string ContainerCode { get; set; }
        public Boolean IsScan { get; set; } = false;
    }
  #endregion

  #region CheckPackageModeResult
  public class CheckPackageModeResult : ApiResult
  { public F050801 f050801 { get; set; } }
  #endregion
}
