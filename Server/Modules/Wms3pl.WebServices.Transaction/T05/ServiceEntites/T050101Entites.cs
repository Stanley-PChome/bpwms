using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Transaction.T05.ServiceEntites
{
	

	public class DistrCar
	{
		public F700101 Master { get; set; }
		public List<F700102> Details { get; set; }
	}

	public class PickOrdRelatedWmsOrd
	{
		public string PickOrdNo { get; set; }
		public string Floor { get; set; }
		public string TmprType { get; set; }

		public string WarehouseId { get; set; }
		public string Area { get; set; }
		public bool IsVirtualItem { get; set; }
		public Dictionary<string,List<AllotedStockOrder>> WmsAllotedStockOrders { get; set; }
	}

	public class WmsOrdTakeTime
	{
		public F050801 F050801 { get; set; }
		public DateTime TakeDate { get; set; }
		public string TakeTime { get; set; }
		public string Address { get; set; }
		public bool IsKeyIn { get; set; }
		public bool CanFast { get; set; }
		public string DelvTimes { get; set; }
		public string DelvEffic { get; set; }

		public string OrdNo { get; set; }
	}

	public class AllIdWmsOrdTakeTime
	{
		public string AllId { get; set; }
		public List<WmsOrdTakeTime> WmsOrdTakeTimes { get; set; }
	}

	public class AllIdDelvTimeArea
	{
		public string AllId { get; set; }
		public DateTime TakeDate { get; set; }
		public bool CanFast { get; set; }
		public DelvTimeArea DelvTimeArea { get; set; }

	}

	public class OrdNoQty
	{
		public string OrdNo { get; set; }
		public int SumOrdQty { get; set; }
		public string CarPeriod { get; set; }
	}

	public class OrdDetailPartItemQty
	{
		public string OrdNo { get; set; }
		public string ItemCode { get; set; }
		public int PartItemQty { get; set; }
	}

	public class AllotedPartOrdDetailInfo
	{
		public F050302 F050302 { get; set; }
		public int AllotedQty { get; set; }
	}
	
	//public class ItemSumQty
	//{
	//	public string ItemCode { get; set; }
	//	public long SumQty { get; set; }
	//}

	/// <summary>
	/// 拆揀貨單方式
	/// </summary>
	public enum SplitPickType
	{
		/// <summary>
		/// 溫層
		/// </summary>
		Tmpr=0,
		/// <summary>
		/// 溫層+樓層
		/// </summary>
		TmprAndFloor,
		/// <summary>
		/// 儲區
		/// </summary>
		Area
	}

	/// <summary>
	/// 庫存檢查參數
	/// </summary>
	public class OrderStockCheckParam
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 單據類別代號
		/// </summary>
		public decimal TicketId { get; set; }
		/// <summary>
		/// 是否配庫試算
		/// </summary>
		public bool IsTrialCalculation { get; set; }

		/// <summary>
		/// 試算編號
		/// </summary>
		public string TrialCalculationNo { get; set; }
		/// <summary>
		/// 需配庫訂單清單
		/// </summary>
		public List<AllotStockOrderDetail> AllotStockOrderDetails { get; set; } 
	}

	/// <summary>
	///  庫存試算-訂單明細
	/// </summary>
	public class AllotStockOrderDetail
	{
		/// <summary>
		/// 單據排序優先等級(小->大)
		/// </summary>
		public decimal Prioity { get; set; }
		
		/// <summary>
		/// 出車時段
		/// </summary>
		public string CarPeriod { get; set; }
		/// <summary>
		/// 訂單主檔
		/// </summary>
		public F050301 F050301 { get; set; }
		/// <summary>
		/// 訂單明細
		/// </summary>
		public F050302 F050302 { get; set; }

	}
	/// <summary>
	/// 訂單商品總庫
	/// </summary>
	public class OrderItemOutOfStock
	{
		/// <summary>
		/// 出貨倉別
		/// </summary>
		public string TypeId { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 總訂購數量
		/// </summary>
		public int OrderQty { get; set; }
		/// <summary>
		/// 揀貨庫存數
		/// </summary>
		public int PickStockQty { get; set; }
		/// <summary>
		/// 補貨區庫存數
		/// </summary>
		public int ReSupplyStockQty { get; set; }
		/// <summary>
		/// 虛擬儲位可回復數
		/// </summary>
		public int VirtualStockQty { get; set; }
		/// <summary>
		/// 缺貨數
		/// </summary>
		public int OutStockQty { get; set; }
		/// <summary>
		/// 建議補貨數
		/// </summary>
		public int SuggestReSupplyStockQty { get; set; }
		/// <summary>
		/// 建議虛擬儲位可回復數
		/// </summary>
		public int SuggestVirtualStockQty { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MakeNo { get; set; }

		/// <summary>
		/// 指定序號
		/// </summary>
		public string SerialNo { get; set; }
	}

	public class OrderStockCheckResult
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 是否B2B訂單
		/// </summary>
		public bool IsB2B { get; set; }
		/// <summary>
		/// 單據類別代號
		/// </summary>
		public decimal TicketId { get; set; }
		/// <summary>
		/// 是否配庫試算
		/// </summary>
		public bool IsTrialCalculation { get; set; }
		/// <summary>
		/// 試算編號
		/// </summary>
		public string TrialCalculationNo { get; set; }
		/// <summary>
		/// 業主主檔
		/// </summary>
		public F1909 F1909 { get; set; }
		/// <summary>
		/// 商品庫存不足清單
		/// </summary>
		public List<OrderItemOutOfStock> ItemNoEnougthList { get; set; }

		/// <summary>
		/// 可配庫訂單
		/// </summary>
		public List<AllotStockOrderDetail> CanAllotStockOrderDetails { get; set; }

		/// <summary>
		/// 釋放訂單不配庫
		/// </summary>
		public List<AllotStockOrderDetail> ReleaseAllotStockOrderDetails { get; set; }

		/// <summary>
		/// 各倉別揀貨區庫存資料
		/// </summary>
		public Dictionary<string, List<ItemLocPriorityInfo>> PickLocStocks { get; set; }

	}
}
