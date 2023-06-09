using System.Collections.Generic;
using Wms3pl.Datas.F05;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 批次訂單資料_傳入
	/// </summary>
	public class PostCreateOrdersReq
	{
		/// <summary>
		/// 目的物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號/客戶編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 訂單主檔
		/// </summary>
		public PostCreateOrdersResultModel Result { get; set; }
	}

	/// <summary>
	/// 訂單主檔
	/// </summary>
	public class PostCreateOrdersResultModel
	{
		public int Total { get; set; }
		public List<PostCreateOrdersModel> Orders { get; set; }
	}

	/// <summary>
	/// 訂單主檔
	/// </summary>
	public class PostCreateOrdersModel
	{
		/// <summary>
		/// 貨主訂單單號(唯一值)
		/// </summary>
		public string CustOrdNo { get; set; }
		/// <summary>
		/// 貨主訂購日期
		/// </summary>
		public string OrderDate { get; set; }
		/// <summary>
		/// 訂單類型
		/// </summary>
		public string OrderType { get; set; }
		/// <summary>
		/// 批次編號
		/// </summary>
		public string BatchNo { get; set; }
		/// <summary>
		/// 配送方式
		/// </summary>
		public string ShipWay { get; set; }
		/// <summary>
		/// 交易類型代號(單據性質)
		/// </summary>
		public string TranCode { get; set; }
		/// <summary>
		/// 通路編號
		/// </summary>
		public string ChannelCode { get; set; }
		/// <summary>
		/// 子通路編號
		/// </summary>
		public string SubChannelCode { get; set; }
		/// <summary>
		/// 物流商
		/// </summary>
		public string LogisticsProvider { get; set; }
		/// <summary>
		/// 指定到貨日
		/// </summary>
		public string ExpDeliveryDate { get; set; }
		/// <summary>
		/// 方便到貨時段
		/// </summary>
		public byte? DeliveryPeriod { get; set; }
		/// <summary>
		/// 是否代收
		/// </summary>
		public bool? IsCOD { get; set; }
		/// <summary>
		/// 代收金額
		/// </summary>
		public decimal? CODAmount { get; set; }
		/// <summary>
		/// 廠商門市編號
		/// </summary>
		public string SalesBaseNo { get; set; }
		/// <summary>
		/// 購買人姓名/聯絡人
		/// </summary>
		public string ShopperName { get; set; }
		/// <summary>
		/// 購買人電話
		/// </summary>
		public string ShopperPhone { get; set; }
		/// <summary>
		/// 收件人姓名
		/// </summary>
		public string ReceiverName { get; set; }
		/// <summary>
		/// 收件人電話
		/// </summary>
		public string ReceiverPhone { get; set; }
		/// <summary>
		/// 收件人行動電話
		/// </summary>
		public string ReceiverMobile { get; set; }
		/// <summary>
		/// 收件人郵遞區號
		/// </summary>
		public string ReceiverZip { get; set; }
		/// <summary>
		/// 收件人地址
		/// </summary>
		public string ReceiverAddress { get; set; }
		/// <summary>
		/// 訂單備註
		/// </summary>
		public string Memo { get; set; }
		/// <summary>
		/// 易碎標籤
		/// </summary>
		public bool? FragileLabel { get; set; }
		/// <summary>
		/// 出貨倉別
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 超取服務商
		/// </summary>
		public string EServiceNo { get; set; }
		/// <summary>
		/// 託運單號
		/// </summary>
		public string ConsignNo { get; set; }
		/// <summary>
		/// 超商物流出貨日期/門市進貨日
		/// </summary>
		public string ShipDate { get; set; }
		/// <summary>
		/// 超取門市店代碼
		/// </summary>
		public string StoreId { get; set; }
		/// <summary>
		/// 超取門市店名稱
		/// </summary>
		public string StoreName { get; set; }
		/// <summary>
		/// 超取門市店退日期
		/// </summary>
		public string ReturnDate { get; set; }
		/// <summary>
		/// 發票號碼
		/// </summary>
		public string InvoiceNo { get; set; }
		/// <summary>
		/// 發票日期 (日期格式)
		/// </summary>
		public string InvoiceDate { get; set; }
		/// <summary>
		/// 狀態(0:待處理 ;D:刪除)
		/// </summary>
		public string ProcFlag { get; set; }
		/// <summary>
		/// 顯示貨主訂單編號
		/// </summary>
		public string PrintCustOrdNo { get; set; }
		/// <summary>
		/// 顯示貨主宅配類別說明
		/// </summary>
		public string PrintMemo { get; set; }
		/// <summary>
		/// 貨主自訂義分類
		/// </summary>
		public string CustCost { get; set; }
		/// <summary>
		/// 建議紙箱編號
		/// </summary>
		public string SuggestBoxNo { get; set; }
		/// <summary>
		/// 跨庫調撥的目的地名稱
		/// </summary>
		public string MoveOutTarget { get; set; }
    /// <summary>
    /// 優先處理旗標 (1: 一般、2: 優先 3 = 急件)
    /// </summary>
    public string FastDealType { get; set; }
    /// <summary>
    /// 建議出貨包裝線類型 (空白=不指定、PA1=小線、PA2=大線)
    /// </summary>
    public string PackingType { get; set; }
    /// <summary>
    /// 是否出庫稽核 (0=否、1=是)
    /// </summary>
    public int? IsPackCheck { get; set; }
    /// <summary>
    /// 商品處理類型(庫內作業) 0=一般 1=含安裝型商品
    /// </summary>
    public string ItemOpType { get; set; }
    /// <summary>
    /// 建議物流商編號
    /// </summary>
    public string SuggestLogisticCode { get; set; }
    /// <summary>
    /// 收貨/店取額外資訊
    /// </summary>
    public ReceiverOptModel ReceiverOpt { get; set; } = new ReceiverOptModel();
    /// <summary>
    /// 進貨檔明細
    /// </summary>
    public List<PostCreateOrderItemsModel> Details { get; set; } = new List<PostCreateOrderItemsModel>();
	}

  /// <summary>
  /// 收貨/店取額外資訊
  /// </summary>
  public class ReceiverOptModel
  {
    /// <summary>
    /// 是否北北基 0=否 1=是
    /// </summary>
    public string IsTPEKEL { get; set; }
  }

	/// <summary>
	/// 進貨檔明細
	/// </summary>
	public class PostCreateOrderItemsModel
	{
		/// <summary>
		/// 通路品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 品項編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 通路品號
		/// </summary>
		public string ChannelItemCode { get; set; }
		/// <summary>
		/// 指定廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 通路品號說明
		/// </summary>
		public string ItemDesc { get; set; }
		/// <summary>
		/// 數量
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 指定批號
		/// </summary>
		public string MakeNo { get; set; }
    /// <summary>
    /// 指定序號(僅針對序號綁儲位的商品才可以使用)
    /// </summary>
    public string SerialNo { get; set; }
    /// <summary>
    /// 服務型品號資料
    /// </summary>
    public List<PostCreateOrderServiceItemsModel> ServiceItemDetails { get; set; } = new List<PostCreateOrderServiceItemsModel>();
	}

	/// <summary>
	/// 服務型品號資料
	/// </summary>
	public class PostCreateOrderServiceItemsModel
	{
		/// <summary>
		/// 服務型品項編號
		/// </summary>
		public string ServiceItemCode { get; set; }
		/// <summary>
		/// 服務型品項名稱
		/// </summary>
		public string ServiceItemName { get; set; }
	}
}
