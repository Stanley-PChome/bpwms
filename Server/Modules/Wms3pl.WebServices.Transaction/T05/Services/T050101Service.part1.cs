using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Common.Extensions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Helper;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Transaction.T05.ServiceEntites;

namespace Wms3pl.WebServices.Transaction.T05.Services
{
	public partial class T050101Service
	{
		#region 暫存快取
		public F1903 GetF1903(string gupCode,string custCode,string itemCode)
		{
			return GetF1903s(gupCode, custCode, new List<string> { itemCode }).FirstOrDefault();
		}

		private List<RetailCarPeriod> _retailCarPeriodList;
		public List<RetailCarPeriod> GetRetailCarPeriods(string dcCode,string gupCode,string custCode,List<string> retailCodes)
		{
			var list = new List<RetailCarPeriod>(); 
			if (_retailCarPeriodList == null)
				_retailCarPeriodList = new List<RetailCarPeriod>();
			var datas = _retailCarPeriodList.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && retailCodes.Contains(x.RETAIL_CODE)).ToList();
			list.AddRange(datas);
			var existsRetails = datas.Select(x => x.RETAIL_CODE).Distinct().ToList();
			var noExistsRetails = retailCodes.Except(existsRetails).ToList();
			if(noExistsRetails.Any())
			{
				var repF194716 = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
				var retailCarPeriods = repF194716.GetRetailCarPeriods(dcCode, gupCode, custCode, noExistsRetails);
				_retailCarPeriodList.AddRange(retailCarPeriods);
				list.AddRange(retailCarPeriods);
			}
			return list;
		}

		private List<F198001> _f198001List;
		public F198001 GetF198001(string typeId)
		{
			if (_f198001List == null)
				_f198001List = new List<F198001>();
			var f198001 = _f198001List.FirstOrDefault(x => x.TYPE_ID == typeId);
			if(f198001 == null)
			{
				var repo = new F198001Repository(Schemas.CoreSchema);
				f198001 = repo.Find(x => x.TYPE_ID == typeId);
				if (f198001 != null)
					_f198001List.Add(f198001);
			}
			return f198001;
		}

		#endregion

		#region 設定出貨時段
		/// <summary>
		/// 設定出貨時段
		/// </summary>
		/// <param name="osap"></param>
		private void SetCarPeriod(OrderStockCheckParam osap)
		{
			var retailCarPeriods = GetRetailCarPeriods(osap.DcCode, osap.GupCode, osap.CustCode, osap.AllotStockOrderDetails.Select(x => x.F050301.RETAIL_CODE).Distinct().ToList());
			osap.AllotStockOrderDetails.ForEach(x =>
			{
				var retailCarPeriod = retailCarPeriods.FirstOrDefault(y => y.RETAIL_CODE == x.F050301.RETAIL_CODE);
				x.CarPeriod = retailCarPeriod == null ? "Z" : retailCarPeriod.CAR_PERIOD;
			});
		}
		#endregion

		#region 依單據類別優先順序排序(小->大)
		/// <summary>
		/// 依單據類別優先順序排序(小->大)
		/// </summary>
		/// <param name="osap"></param>
		private void SortTicketPrioity(string dcCode,string gupCode,string custCode,List<AllotStockOrderDetail> allotStockOrderDetails)
		{
			var f190001Rep = new F190001Repository(Schemas.CoreSchema);
			var f190001s = f190001Rep.GetDatas(dcCode, gupCode, custCode).ToList();
			allotStockOrderDetails.ForEach(x =>
			{
				var f190001 = f190001s.FirstOrDefault(y => y.TICKET_ID == x.F050301.TICKET_ID);
				x.Prioity = f190001 == null ? int.MaxValue : f190001.PRIORITY;
			});
			allotStockOrderDetails = allotStockOrderDetails.OrderBy(x => x.Prioity).ToList();
		}
		#endregion

		#region 配庫試算
		/// <summary>
		/// 配庫試算
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="ordNos">訂單編號</param>
		/// <returns></returns>
		public ExecuteResult AllotStockTrialCalculation(string dcCode,string gupCode,string custCode,List<string> ordNos)
		{
			_wmsLogHelper = new WmsLogHelper();
			_wmsLogHelper.StartRecord(WmsLogProcType.AllotStockTC);
			var allotedStockOrderList = new List<AllotedStockOrder>();

			var allotStockOrderDetails = new List<AllotStockOrderDetail>();

			#region 取得訂單池要配庫的訂單
			var f050001s = new List<F050001>();
			var f050002s = new List<F050002>();
			GetDirectOrders(ordNos,true, ref f050001s, ref f050002s);

			//排除明細設定不為不出貨
			f050002s = f050002s.Where(x => x.NO_DELV == "0").ToList();

            var f050001CheckDetails = (from m in f050001s
                                       join d in f050002s
                                       on new { m.DC_CODE, m.GUP_CODE, m.CUST_CODE, m.ORD_NO } equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.ORD_NO }
                                       select new AllotStockOrderDetail
                                       {
                                           F050301 = AutoMapper.Mapper.DynamicMap<F050301>(m),
                                           F050302 = AutoMapper.Mapper.DynamicMap<F050302>(d)
                                       }).ToList();
            allotStockOrderDetails.AddRange(f050001CheckDetails);
			#endregion

			SortTicketPrioity(dcCode, gupCode, custCode, allotStockOrderDetails);

			var gTicket = allotStockOrderDetails.GroupBy(x => x.F050301.TICKET_ID);
			var calNo = DateTime.Now.ToString("yyyyMMddHHmmss");

			//依單據類別分批配庫
			foreach (var ticket in gTicket)
			{
				var osap = new OrderStockCheckParam
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					TicketId = ticket.Key,
					AllotStockOrderDetails = ticket.ToList(),
					IsTrialCalculation = true,
					TrialCalculationNo = calNo
				};
				if (!ticket.ToList().Any())
					continue;
				var orderStockCheckResult = NewOrderStockCheck(osap);
				var allotedStockOrders = NewOrderAllot(orderStockCheckResult);
				allotedStockOrderList.AddRange(allotedStockOrders);
			}
			CreateAllotStockTrialCalculationLog(dcCode,gupCode,custCode,calNo,allotStockOrderDetails, allotedStockOrderList);
			_wmsLogHelper.StopRecord();
			return new ExecuteResult(true,"", calNo);
		}

		/// <summary>
		/// 建立試算Log
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="calNo"></param>
		/// <param name="allotStockOrderDetails"></param>
		/// <param name="allotedStockOrderList"></param>
		private void CreateAllotStockTrialCalculationLog(string dcCode,string gupCode,string custCode,string calNo,List<AllotStockOrderDetail> allotStockOrderDetails, List<AllotedStockOrder> allotedStockOrderList)
		{
			var f05080503Repo = new F05080503Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05080504Repo = new F05080504Repository(Schemas.CoreSchema, _wmsTransaction);

			#region 試算頭檔
			var beforeOrdNos = allotStockOrderDetails.Select(x => x.F050301.ORD_NO).Distinct().ToList();
			var afterOrdNos = allotedStockOrderList.Select(x => x.F050302.ORD_NO).Distinct().ToList();
			var beforeRetailCodes = allotStockOrderDetails.Where(x=> !string.IsNullOrWhiteSpace(x.F050301.RETAIL_CODE)).Select(x => x.F050301.RETAIL_CODE).Distinct().ToList();
			var afterRetailCodes = allotStockOrderDetails.Where(x => !string.IsNullOrWhiteSpace(x.F050301.RETAIL_CODE) && afterOrdNos.Contains(x.F050301.ORD_NO)).Select(x => x.F050301.RETAIL_CODE).Distinct().ToList();
			var beforeItemCodes = allotStockOrderDetails.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
			var afterItemCodes = allotedStockOrderList.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
			var beforeDelvQty = allotStockOrderDetails.Select(x => x.F050302.ORD_QTY).Sum();
			var afterDelvQty = allotedStockOrderList.Select(x => x.F050302.ORD_QTY).Sum();

			
			var f05080503 = new F05080503
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CAL_NO = calNo,
				TTL_B_ORD_CNT = beforeOrdNos.Count,
				TTL_A_ORD_CNT = afterOrdNos.Count,
				TTL_B_RETAIL_CNT = beforeRetailCodes.Count,
				TTL_A_RETAIL_CNT = afterRetailCodes.Count,
				TTL_B_ITEM_CNT = beforeItemCodes.Count,
				TTL_A_ITEM_CNT = afterItemCodes.Count,
				TTL_B_DELV_QTY = beforeDelvQty,
				TTL_A_DELV_QTY = afterDelvQty,
				TTL_A_SHELF_CNT = 0
			};

			#endregion

			#region 試算各儲區出貨占比
			var addF05080504List = new List<F05080504>();
			var g = allotedStockOrderList.GroupBy(x => new { x.WarehouseId, x.AreaCode });
			foreach(var item in g)
			{
				var f05080504 = new F05080504
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,	
					CAL_NO = calNo,
					WAREHOUSE_ID = item.Key.WarehouseId,
					AREA_CODE = item.Key.AreaCode,
					DELV_QTY = item.Sum(x=> x.F050302.ORD_QTY),
				};
				f05080504.DELV_RATIO = (float)Math.Round(((float)f05080504.DELV_QTY / (float)f05080503.TTL_A_DELV_QTY), 2);
				addF05080504List.Add(f05080504);
			}

			//針對加總比例不足1 將最大比例加上差異
			var sumDelvRatio = addF05080504List.Sum(x => x.DELV_RATIO);
			if(addF05080504List.Any() && sumDelvRatio < 1)
			{
				var diff = 1 - sumDelvRatio;
				var item  = addF05080504List.OrderByDescending(x => x.DELV_RATIO).First();
				item.DELV_RATIO += diff;
			}
			//針對加總比例大於1 將最大比例減掉差異
			else if (addF05080504List.Any() && sumDelvRatio > 1)
			{
				var diff =sumDelvRatio - 1;
				var item = addF05080504List.OrderByDescending(x => x.DELV_RATIO).First();
				item.DELV_RATIO -= diff;
			}
			#endregion

			f05080504Repo.BulkInsert(addF05080504List);
			f05080503Repo.Add(f05080503);
			
		}

		#endregion

		#region 新-訂單庫存檢查
		/// <summary>
		/// 新-訂單庫存檢查
		/// </summary>
		/// <param name="osap"></param>
		/// <returns></returns>
		public OrderStockCheckResult NewOrderStockCheck(OrderStockCheckParam osap)
		{
			var result = new OrderStockCheckResult();
			result.DcCode = osap.DcCode;
			result.GupCode = osap.GupCode;
			result.CustCode = osap.CustCode;
			result.IsTrialCalculation = osap.IsTrialCalculation;
			result.TicketId = osap.TicketId;
			result.TrialCalculationNo = osap.TrialCalculationNo;
			result.ReleaseAllotStockOrderDetails = new List<AllotStockOrderDetail>();
			result.ItemNoEnougthList = new List<OrderItemOutOfStock>();
			result.CanAllotStockOrderDetails = new List<AllotStockOrderDetail>();
			//是否B2B訂單
			var isB2B = osap.AllotStockOrderDetails.First().F050301.ORD_TYPE == "0";
			result.IsB2B = isB2B;
			var f1909 = GetF1909(osap.GupCode, osap.CustCode);
			if (result.IsTrialCalculation) // 試算時以允許部分出貨來計算
				f1909.SPILT_OUTCHECK = "1";
			result.F1909 = f1909;
			//設定出或時段
			SetCarPeriod(osap);
			//取得揀貨儲位庫存
			var pickLocStocks = GetPickLocStocks(osap);
			
			var gTypes = osap.AllotStockOrderDetails.GroupBy(x => x.F050301.TYPE_ID);
			//依出貨倉別配庫
			foreach (var type in gTypes)
			{
				var typeList = type.ToList();
				//取得此出貨倉別揀貨庫存
				var typePickLocStocks = pickLocStocks[type.Key];
				//取得訂購商品在揀貨儲位總庫存缺貨資料
				var itemNoEnougthList = CheckItemTotalQtyEnough(typePickLocStocks, typeList,type.Key);
				//釋放缺貨訂單(不允許部分出貨且B2C訂單或貨主允許B2B單張訂單出貨)
				if (f1909.SPILT_OUTCHECK == "0" && (!isB2B || f1909.ISB2B_ALONE_OUT == "1"))
				{
					var releaseOrderNos = RelaseOutOfStockOrders(osap.DcCode, osap.GupCode, osap.CustCode, type.Key, ref itemNoEnougthList, ref typeList);
					result.ReleaseAllotStockOrderDetails.AddRange(typeList.Where(x => releaseOrderNos.Contains(x.F050301.ORD_NO)));
					typeList.RemoveAll(x => releaseOrderNos.Contains(x.F050301.ORD_NO));
				}
				//取得缺貨商品補貨區可補貨數量
				GetReSupplyStockQty(osap.DcCode, osap.GupCode, osap.CustCode, type.Key,ref itemNoEnougthList);
				//取得缺貨商品虛擬儲位可回復庫存
				GetVirtualStockQty(osap.DcCode, osap.GupCode, osap.CustCode, type.Key, ref itemNoEnougthList);

				result.ItemNoEnougthList.AddRange(itemNoEnougthList);

				result.CanAllotStockOrderDetails.AddRange(typeList);
			}
			result.PickLocStocks = pickLocStocks;
			//寫入庫存Log
			WriteStockLog(result);
			return result;
		}

		#region 庫存與缺貨處理

		#region 取得各出貨倉別揀貨儲位數量
		private Dictionary<string, List<ItemLocPriorityInfo>> _pickLocStocks;
		/// <summary>
		/// 取得各出貨倉別揀貨儲位數量
		/// </summary>
		/// <param name="osap"></param>
		/// <returns></returns>
		private Dictionary<string, List<ItemLocPriorityInfo>> GetPickLocStocks(OrderStockCheckParam osap)
		{
            _f1913Rep = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
            if (_pickLocStocks == null)
				_pickLocStocks = new Dictionary<string, List<ItemLocPriorityInfo>>();
			//依出貨倉別取得揀貨儲位庫存
			var gTypes = osap.AllotStockOrderDetails.GroupBy(x => x.F050301.TYPE_ID);
			foreach (var type in gTypes)
			{
				var item = _pickLocStocks.FirstOrDefault(x => x.Key == type.Key);
				if (item.Equals(default(KeyValuePair<string, List<ItemLocPriorityInfo>>)))
				{
					var pickLocPriorityInfos = _f1913Rep.GetItemPickLocPriorityInfo(osap.DcCode, osap.GupCode, osap.CustCode, new List<string>(), false, type.Key).Where(a => a.QTY > 0).ToList();
					_pickLocStocks.Add(type.Key, pickLocPriorityInfos);
				}
			}
			return _pickLocStocks;
		}

		#endregion

		#region 取得各出貨倉別補貨儲位數量
		private Dictionary<string, List<ItemLocPriorityInfo>> _reSupplyLocStocks;
		/// <summary>
		/// 取得各出貨倉別補貨儲位數量
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="typeId">出貨倉別</param>
		/// <returns></returns>
		public List<ItemLocPriorityInfo> GetReSupplyLocStocks(string dcCode, string gupCode, string custCode, string typeId)
		{
			if (_reSupplyLocStocks == null)
				_reSupplyLocStocks = new Dictionary<string, List<ItemLocPriorityInfo>>();
			var item = _reSupplyLocStocks.FirstOrDefault(x => x.Key == typeId);
			if (item.Equals(default(KeyValuePair<string, List<ItemLocPriorityInfo>>)))
			{
				var pickLocPriorityInfos = _f1913Rep.GetItemResupplyLocPriorityInfo(dcCode, gupCode, custCode, new List<string>(), false, typeId).Where(a => a.QTY > 0).ToList();
				_reSupplyLocStocks.Add(typeId, pickLocPriorityInfos);
			}
			return _reSupplyLocStocks[typeId];
		}

		#endregion

		#region 取得訂購商品在揀貨儲位總庫存缺貨資料
		/// <summary>
		/// 取得訂購商品在揀貨儲位總庫存缺貨資料
		/// </summary>
		/// <param name="stocks">出貨倉別庫存</param>
		/// <param name="details">訂單明細</param>
		private List<OrderItemOutOfStock> CheckItemTotalQtyEnough(List<ItemLocPriorityInfo> stocks, List<AllotStockOrderDetail> details, string typeId)
		{
			var ItemNoEnoughList = new List<OrderItemOutOfStock>();
			var gOrderItems = details.GroupBy(x => x.F050302.ITEM_CODE).ToList();
			foreach (var item in gOrderItems)
			{
				var ordQty = item.Sum(x => x.F050302.ORD_QTY);
				var stockQty = (int)stocks.Where(x => x.ITEM_CODE == item.Key).Sum(x => x.QTY);
				if (ordQty > stockQty)
					ItemNoEnoughList.Add(new OrderItemOutOfStock { TypeId = typeId, ItemCode = item.Key, OrderQty = ordQty, PickStockQty = stockQty, OutStockQty = ordQty - stockQty });
			}
			return ItemNoEnoughList;
		}

		#endregion

		#region 釋放缺貨訂單
		/// <summary>
		/// 釋放缺貨訂單
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="typeId">出貨倉別代碼</param>
		/// <param name="itemNoEnougthList">缺貨清單</param>
		/// <param name="allotStockOrderDetails"></param>
		private List<string> RelaseOutOfStockOrders(string dcCode, string gupCode, string custCode, string typeId, ref List<OrderItemOutOfStock> itemNoEnougthList, ref List<AllotStockOrderDetail> allotStockOrderDetails)
		{
			var releaseOrders = new List<string>();
			//非配庫試算且有商品缺貨
			if (itemNoEnougthList.Any())
			{
				var itemCodes = itemNoEnougthList.Select(x => x.ItemCode).ToList();
				//取得有缺貨商品訂單單號
				var hasOutOfStockItemOrderNos = allotStockOrderDetails.Where(x => itemCodes.Contains(x.F050302.ITEM_CODE)).Select(x => x.F050301.ORD_NO).Distinct().ToList();
				//取得有缺貨商品訂單明細
				var gOutOfStockOrderDetails = allotStockOrderDetails.Where(x => hasOutOfStockItemOrderNos.Contains(x.F050301.ORD_NO))
					.GroupBy(x => x.F050301.ORD_NO).Select(x => new { OrdNo = x.Key, items = x.ToList(), ItemCount = x.Select(y => y.F050302.ITEM_CODE).Distinct().Count() });

				//一單一品訂單(排序依訂購數(小->大)再依照訂單編號(大->小))
				var singleItemOrders = gOutOfStockOrderDetails.Where(x => x.ItemCount == 1).OrderBy(x => x.items.Sum(y => y.F050302.ORD_QTY)).ThenByDescending(x => x.OrdNo).ToList();
				foreach (var item in singleItemOrders)
				{
					var itemCode = item.items.First().F050302.ITEM_CODE;
					var itemNoEnougth = itemNoEnougthList.FirstOrDefault(x => x.ItemCode == itemCode);
					if (itemNoEnougth != null)
					{
						var orderQty = item.items.Where(x => x.F050302.ITEM_CODE == itemCode).Sum(x => x.F050302.ORD_QTY);
						if (itemNoEnougth.OutStockQty >= orderQty)
						{
							itemNoEnougth.OrderQty -= orderQty;
							itemNoEnougth.OutStockQty -= orderQty;
							releaseOrders.Add(item.OrdNo);

						}
					}
				}

				//一單多品(排序依品項數(小->大)再依照訂單編號(大->小))
				var multiItemOrders = gOutOfStockOrderDetails.Where(x => x.ItemCount > 1).OrderBy(x => x.ItemCount).ThenByDescending(x => x.OrdNo).ToList();
				foreach (var item in multiItemOrders)
				{
					var orderItemCodes = item.items.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
					var itemNoEnougths = itemNoEnougthList.Where(x => orderItemCodes.Contains(x.ItemCode));
					foreach (var itemNoEnougth in itemNoEnougths)
					{
						var orderQty = item.items.Where(x => x.F050302.ITEM_CODE == itemNoEnougth.ItemCode).Sum(x => x.F050302.ORD_QTY);
						if (itemNoEnougth.OutStockQty >= orderQty)
						{
							itemNoEnougth.OrderQty -= orderQty;
							itemNoEnougth.OutStockQty -= orderQty;
						}
					}
					releaseOrders.Add(item.OrdNo);
				}
				itemNoEnougthList = itemNoEnougthList.Where(x => x.OutStockQty > 0).ToList();
			}
			return releaseOrders;
		}
		#endregion

		#region 取得缺貨商品補貨區庫存

		/// <summary>
		/// 取得缺貨商品補貨區庫存
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="typeId">出貨倉別代碼</param>
		/// <param name="itemNoEnougthList">缺貨清單</param>
		/// <returns></returns>
		private void GetReSupplyStockQty(string dcCode, string gupCode, string custCode, string typeId, ref List<OrderItemOutOfStock> itemNoEnougthList)
		{
			if (itemNoEnougthList.Any())
			{
				var pickLocPriorityInfos = GetReSupplyLocStocks(dcCode,gupCode,custCode,typeId).Where(a => a.QTY > 0).ToList();
				foreach (var item in itemNoEnougthList)
				{
					//商品補貨區數量
					var reSupplyQty = (int)pickLocPriorityInfos.Where(x => x.ITEM_CODE == item.ItemCode).Sum(x => x.QTY);
					item.ReSupplyStockQty = reSupplyQty;
					//商品缺貨數>=商品補貨數 則設定補貨數 並減少缺貨數
					if (item.OutStockQty >= reSupplyQty)
					{
						item.SuggestReSupplyStockQty = reSupplyQty;
					}
					//商品缺貨數<商品補貨數 則設定補貨數=缺貨數 並設定缺貨數=0
					else
					{
						item.SuggestReSupplyStockQty = item.OutStockQty;
					}
				}
			}
		}

		#endregion

		#region 取得缺貨商品虛擬儲位可回復庫存
		/// <summary>
		/// 取得缺貨商品虛擬儲位可回復庫存
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="typeId">出貨倉別代碼</param>
		/// <param name="itemNoEnougthList">缺貨清單</param>
		/// <returns></returns>
		private void GetVirtualStockQty(string dcCode, string gupCode, string custCode, string typeId, ref List<OrderItemOutOfStock> itemNoEnougthList)
		{
			var virtualQtyItems = new List<ItemQty>();
			if (itemNoEnougthList.Any())
			{
				virtualQtyItems = _f1913Rep.GetVirtualQtyItems(dcCode, gupCode, custCode, typeId, itemNoEnougthList.Where(x => x.OutStockQty > 0).Select(x => x.ItemCode).ToList()).ToList();
				foreach (var item in itemNoEnougthList)
				{
					//商品虛擬儲位庫存
					var virtualQtyItem = virtualQtyItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
					if (virtualQtyItem != null)
					{
						if (item.SuggestReSupplyStockQty >= item.OutStockQty)
							continue;
						var outStockQty = item.OutStockQty - item.SuggestReSupplyStockQty;
						//商品缺貨數>=商品虛擬儲位庫存數 設定虛擬儲位回復數 並減少缺貨數
						if (outStockQty >= virtualQtyItem.Qty)
						{
							item.SuggestVirtualStockQty = virtualQtyItem.Qty;
						}
						//商品缺貨數<商品虛擬儲位庫存數 設定虛擬儲位回復數=缺貨數 並將缺貨數設為0
						else
						{
							item.SuggestVirtualStockQty = outStockQty;
						}
					}
				}
			}
		}

		#endregion

		#endregion

		#region 寫入庫存Log
		private void WriteStockLog(OrderStockCheckResult oscr)
		{
			var f1901 = GetF1901(oscr.DcCode);
			var f1929 = GetF1929(oscr.GupCode);
			var f1909 = GetF1909(oscr.GupCode, oscr.CustCode);
			if(oscr.IsTrialCalculation)
			{
				var f050805Repo = new F050805Repository(Schemas.CoreSchema, _wmsTransaction);
				var addF050805List = oscr.ItemNoEnougthList.Select(x => new F050805
				{
					DC_CODE = oscr.DcCode,
					GUP_CODE = oscr.GupCode,
					CUST_CODE =oscr.CustCode,
					CAL_NO = oscr.TrialCalculationNo,
					TYPE_ID = x.TypeId,
					ITEM_CODE = x.ItemCode,
					TTL_ORD_QTY = x.OrderQty,
					TTL_PICK_STOCK_QTY = x.PickStockQty,
					TTL_RESUPPLY_STOCK_QTY = x.ReSupplyStockQty,
					TTL_VIRTUAL_STOCK_QTY = x.VirtualStockQty,
					TTL_OUTSTOCK_QTY = x.OutStockQty ,
					SUG_RESUPPLY_STOCK_QTY = x.SuggestReSupplyStockQty,
					SUG_VIRTUAL_STOCK_QTY = x.SuggestVirtualStockQty,
				});
				f050805Repo.BulkInsert(addF050805List, new string[] { "ID" });
			}
			else
			{
				var messageList = new Dictionary<string,List<string>>();
				var msgNo = string.Empty;
				F0020 f0020;

				#region 產生虛擬儲位回復Log
				var virtualItems = oscr.ItemNoEnougthList.Where(x => x.SuggestVirtualStockQty > 0).ToList();
				msgNo = "AAM00012"; //物流中心:「{0}」業主:「{1}」貨主:「{2}」倉別:「{3}」商品:「{4}」需回復虛擬儲位數量:{5}
				f0020 = GetF0020(msgNo);
				var virtualItemMsgList = new List<string>();
				foreach (var item in virtualItems)
				{
					var f198001 = GetF198001(item.TypeId);
					var f1903 = GetF1903(oscr.GupCode, oscr.CustCode, item.ItemCode);
					virtualItemMsgList.Add(string.Format(f0020.MSG_CONTENT,
									(f1901 == null) ? "" : f1901.DC_NAME,
									(f1929 == null) ? "" : f1929.GUP_NAME,
									(f1909 == null) ? "" : f1909.SHORT_NAME,
									(f198001 == null) ? "" : f198001.TYPE_NAME,
									(f1903 == null) ? "" : f1903.ITEM_CODE + f1903.ITEM_NAME,
								  item.SuggestVirtualStockQty));
				}
				if(virtualItemMsgList.Any())
					messageList.Add(msgNo, virtualItemMsgList);
				#endregion

				#region 良品倉揀貨區庫存不足Log
				//不允許部分出貨
				if (oscr.F1909.SPILT_OUTCHECK == "0")
				{
					msgNo = "AAM00025"; //物流中心:「{0}」業主:「{1}」貨主:「{2}」商品:「{3}」揀區庫存不足{4}，請執行配庫試算產生補貨調撥單
					f0020 = GetF0020(msgNo);
					var pickStockMsgList = new List<string>();
					var pickItemNoEnoughList = oscr.ItemNoEnougthList.Where(x => x.TypeId == "G").ToList();
					foreach (var item in pickItemNoEnoughList)
					{
						var f1903 = GetF1903(oscr.GupCode, oscr.CustCode, item.ItemCode);
						pickStockMsgList.Add(string.Format(f0020.MSG_CONTENT,
										(f1901 == null) ? "" : f1901.DC_NAME,
										(f1929 == null) ? "" : f1929.GUP_NAME,
										(f1909 == null) ? "" : f1909.SHORT_NAME,
										(f1903 == null) ? "" : f1903.ITEM_CODE + f1903.ITEM_NAME
										, item.OutStockQty));
					}
					if (pickStockMsgList.Any())
						messageList.Add(msgNo, pickStockMsgList);
				}
				#endregion

				foreach (var item in messageList)
				{
					AddMessagePoolForInside("9", oscr.DcCode, oscr.GupCode, oscr.CustCode, item.Key, string.Join("\n", item.Value));
				}
			}
		}

		#endregion

		#endregion

		#region 新-訂單配庫
		public List<AllotedStockOrder> NewOrderAllot(OrderStockCheckResult oscr)
		{
			var serialNoService = new SerialNoService();
			var allotedStockOrders = new List<AllotedStockOrder>();
			//非配庫試算且B2B揀貨儲位數量不足且或貨主設定不允許B2B單張訂單出貨 不配庫
			if (!oscr.IsTrialCalculation && oscr.ItemNoEnougthList.Any() && oscr.IsB2B && oscr.F1909.ISB2B_ALONE_OUT == "0")
				return allotedStockOrders;

			var gTypes = oscr.CanAllotStockOrderDetails.GroupBy(x => x.F050301.TYPE_ID);
			var itemCodes = oscr.CanAllotStockOrderDetails.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
			var f190301Repo = new F190301Repository(Schemas.CoreSchema);
			var f190301Datas = f190301Repo.GetUnitQtyDatas(oscr.GupCode, new List<string> { "箱", "盒" }, itemCodes).ToList();
			
			foreach(var type in gTypes)
			{
				IEnumerable<OrdNoQty> oOrdNos = new List<OrdNoQty>();
				List<OrdDetailPartItemQty> ordDetailPartItemQtys = null;

				//取得此倉別揀貨庫存資料
				var pickLocStocks = oscr.PickLocStocks[type.Key];
				//這樣寫才能去重複(無法用Distinct)
				var f050301s = type.GroupBy(x=> new { x.F050301.DC_CODE, x.F050301.GUP_CODE, x.F050301.CUST_CODE, x.F050301.ORD_NO }).Select(x => x.First().F050301).ToList();
				var f050302s = type.Select(x => x.F050302).ToList();
				var gF050301 = f050301s.GroupBy(x => x.TYPE_ID).First();

				//有允出天數限制的平均分攤配庫
				if (oscr.IsB2B && oscr.F1909.ISALLOW_DELV_DAY == "1" && oscr.F1909.SPILT_OUTCHECK == "1" && oscr.F1909.SPILT_OUTCHECKWAY == "1")
				{
					IEnumerable<RetailCarPeriod> retailCarPeriods;
					oOrdNos = SortOrderPartAllowedByCarPeriod(oscr.DcCode, oscr.GupCode, oscr.CustCode, f050302s, gF050301, out retailCarPeriods);
					AddNoCarPeriodMessage(oscr.GupCode, oscr.CustCode, oscr.DcCode, f050301s, retailCarPeriods);
					var tmpF050302s = f050302s.Select(a => AutoMapper.Mapper.DynamicMap<F050302>(a)).ToList();
					AllotedEquallyDistributedAllowDelvDay(oOrdNos, f050301s, tmpF050302s, pickLocStocks, ref allotedStockOrders);
				}
				else
				{
					if (oscr.F1909.SPILT_OUTCHECK == "0") //Robin 20180503 不允許部分出貨
					{
						//依有指定序號、總揀次多及訂單編號小排序做配庫
						oOrdNos = SortOrderNotPartAllowed(f050302s, gF050301);
					}
					//允許部分出貨，訂單先進先出
					else if (oscr.F1909.SPILT_OUTCHECK == "1" && oscr.F1909.SPILT_OUTCHECKWAY == "0")
					{
						oOrdNos = SortOrderPartAllowedByOrdNo(f050302s, gF050301);
					}
					else if (oscr.F1909.SPILT_OUTCHECK == "1" && oscr.F1909.SPILT_OUTCHECKWAY == "1")
					{
						IEnumerable<RetailCarPeriod> retailCarPeriods;
						oOrdNos = SortOrderPartAllowedByCarPeriod(oscr.DcCode, oscr.GupCode, oscr.CustCode, f050302s, gF050301, out retailCarPeriods);
						AddNoCarPeriodMessage(oscr.GupCode, oscr.CustCode, oscr.DcCode, f050301s, retailCarPeriods);
						ordDetailPartItemQtys = (from a in f050302s
																		 group a by new { a.ORD_NO, a.ITEM_CODE } into g
																		 let qty = g.Sum(s => s.ORD_QTY)
																		 select new OrdDetailPartItemQty { OrdNo = g.Key.ORD_NO, ItemCode = g.Key.ITEM_CODE, PartItemQty = qty }).ToList();
						CalcEquallyDistributed(oOrdNos, pickLocStocks, f050301s, f050302s, oscr.IsB2B, oscr.F1909, ref ordDetailPartItemQtys);
					}
					//暫存庫存資料檢核允出天數效期
					var tempPickLocStocks = new List<ItemLocPriorityInfo>();
					pickLocStocks.ForEach((x) =>
					{
						ItemLocPriorityInfo y = new ItemLocPriorityInfo();
						x.CloneProperties(y);
						tempPickLocStocks.Add(y);
					});
					foreach (var oOrdNo in oOrdNos)
					{
						var ordF050302s = f050302s.Where(a => a.ORD_NO == oOrdNo.OrdNo);
						//Robin 20180508 增加不允許部分出貨才需踢單
						if (oscr.IsB2B && oscr.F1909.ISALLOW_DELV_DAY == "1" && !CheckValidDate(gF050301.First(x => x.ORD_NO == oOrdNo.OrdNo), ordF050302s, tempPickLocStocks) && oscr.F1909.SPILT_OUTCHECK == "0")
						{
							oscr.ReleaseAllotStockOrderDetails.AddRange(type.Where(x => x.F050301.ORD_NO == oOrdNo.OrdNo).ToList());
							oscr.CanAllotStockOrderDetails.RemoveAll(x => x.F050301.ORD_NO == oOrdNo.OrdNo);
							continue;
						}

						foreach(var f050302 in ordF050302s)
						{
							var isBatchItem = serialNoService.IsBatchNoItem(f050302.GUP_CODE, f050302.CUST_CODE, f050302.ITEM_CODE);
							var ordQty = f050302.ORD_QTY;
							//允許部分出貨平均分攤，依分攤的數量進行配庫
							if (ordDetailPartItemQtys != null && ordDetailPartItemQtys.Any())
							{
								var ordDetailPartItemQty = ordDetailPartItemQtys.Single(a => a.OrdNo == f050302.ORD_NO && a.ItemCode == f050302.ITEM_CODE);
								if (ordQty > ordDetailPartItemQty.PartItemQty)
									ordQty = ordDetailPartItemQty.PartItemQty;
								else
									ordDetailPartItemQty.PartItemQty = ordDetailPartItemQty.PartItemQty - ordQty;
							}
							var q = pickLocStocks.Where(a => a.ITEM_CODE == f050302.ITEM_CODE && a.QTY > 0);
							if (!string.IsNullOrEmpty(f050302.SERIAL_NO))
								q = q.Where(a => a.SERIAL_NO == f050302.SERIAL_NO);

							//如果有找到商品限制效期天數則篩選效期必須大於等於今天日期+商品限制效期天數
							var limitValidDay = _itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == f050302.ITEM_CODE);
							if (limitValidDay != null)
								q = q.Where(x => x.VALID_DATE >= DateTime.Today.AddDays(limitValidDay.DELIVERY_DAY));

							var oItemPickLocStocks = q
								.OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
								.ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL)
								.ThenBy(a => a.CASE_NO).ThenBy(a => a.BATCH_NO).ThenBy(a => a.BOX_CTRL_NO).ThenBy(a => a.PALLET_CTRL_NO).ThenBy(a=> a.MAKE_NO);

							// Robin 20180503 不允許部分出貨才須依箱盒配庫
							if (oscr.F1909.SPILT_OUTCHECK == "0")
							{
								//優先順序 箱=>盒=>散裝去配
								var caseItem = f190301Datas.Find(
										o => o.GUP_CODE == f050302.GUP_CODE && o.ITEM_CODE == f050302.ITEM_CODE && o.ACC_UNIT_NAME == "箱");
								var boxItem = f190301Datas.Find(
									o => o.GUP_CODE == f050302.GUP_CODE && o.ITEM_CODE == f050302.ITEM_CODE && o.ACC_UNIT_NAME == "盒");
								var boxQty = boxItem == null ? 0 : boxItem.UNIT_QTY;  //取得商品一盒數量是多少
								var caseQty = caseItem == null ? 0 : caseItem.UNIT_QTY; //取得商品一箱數量是多少
								const int batchQty = 200; //儲值卡盒號 固定一盒200個

								//分配箱
								ShareQty(oItemPickLocStocks, f050302, caseQty * boxQty, ShareUnitQtyType.Case, false, ref ordQty, ref allotedStockOrders);
								//分配盒(無箱號)
								ShareQty(oItemPickLocStocks, f050302, boxQty, ShareUnitQtyType.BoxNoCase, false, ref ordQty, ref allotedStockOrders);
								//分配儲值卡盒
								if (isBatchItem)
									ShareQty(oItemPickLocStocks, f050302, batchQty, ShareUnitQtyType.BatchBox, false, ref ordQty, ref allotedStockOrders);
							}

							//散裝
							ShareQty(oItemPickLocStocks, f050302, 1, ShareUnitQtyType.Bulk, false, ref ordQty, ref allotedStockOrders);

							//如過還不夠 才去拆箱、盒、儲值卡盒號 優先順序 盒=>箱
							// Robin 20180503 不允許部分出貨才須依箱盒配庫
							if (oscr.F1909.SPILT_OUTCHECK == "0" && ordQty > 0)
							{
								//拆儲值卡盒號
								if (isBatchItem)
									ShareQty(oItemPickLocStocks,f050302, 1, ShareUnitQtyType.BatchBox, true, ref ordQty, ref allotedStockOrders);
								//拆盒(無箱號)
								ShareQty(oItemPickLocStocks, f050302, 1, ShareUnitQtyType.BoxNoCase, true, ref ordQty, ref allotedStockOrders);
								//拆盒(有箱號)
								ShareQty(oItemPickLocStocks, f050302, 1, ShareUnitQtyType.BoxHasCase, true, ref ordQty, ref allotedStockOrders);
								//拆箱
								ShareQty(oItemPickLocStocks, f050302, 1, ShareUnitQtyType.Case, true, ref ordQty, ref allotedStockOrders);
							}
						}
					}
				}
			}
			//產生訂單分配Log
			CreateOrderAllotLog(oscr, allotedStockOrders);
			return allotedStockOrders;
		}

		#region 分配庫存數
		/// <summary>
		/// 分配庫存數
		/// </summary>
		/// <param name="oItemPickLocStocks">商品揀貨庫存資料</param>
		/// <param name="f050302">訂單明細</param>
		/// <param name="unitQty">商品容積單位最小數量</param>
		/// <param name="shareUnitType">容積單位類型</param>
		/// <param name="isUnBoxing">是否拆盒拆箱</param>
		/// <param name="ordQty">訂購數量</param>
		/// <param name="allotedStockOrders">回傳訂單分配明細</param>
		private void ShareQty(IOrderedEnumerable<ItemLocPriorityInfo> oItemPickLocStocks,F050302 f050302, int unitQty, ShareUnitQtyType shareUnitType,bool isUnBoxing,ref int ordQty,ref List<AllotedStockOrder> allotedStockOrders)
		{
			if (unitQty <= 0 || ordQty < unitQty)
				return;
			//已分配的序號
			var sharedSerialNos = allotedStockOrders.Where(x => x.SerialNo != "0").Select(x => x.SerialNo).Distinct().ToList();
			var stocks = new List<IGrouping<string, ItemLocPriorityInfo>>();
			switch (shareUnitType)
			{
				case ShareUnitQtyType.Case:
					stocks = oItemPickLocStocks.Where(x => !string.IsNullOrEmpty(x.CASE_NO) && x.QTY > 0 &&
																				 sharedSerialNos.All(o => o != x.SERIAL_NO))
														 .OrderByDescending(a => a.ATYPE_CODE)
														 .ThenBy(a => a.VALID_DATE)
														 .ThenBy(a => a.ENTER_DATE)
														 .ThenByDescending(a => a.HANDY)
														 .ThenBy(a => a.HOR_DISTANCE)
														 .ThenBy(a => a.LOC_CODE)
														 .ThenBy(a => a.CASE_NO)
														 .GroupBy(c => c.CASE_NO).ToList();
					break;
				case ShareUnitQtyType.BoxNoCase:
					stocks = oItemPickLocStocks.Where(x => !string.IsNullOrEmpty(x.BOX_SERIAL) && string.IsNullOrEmpty(x.CASE_NO) &&
																				 x.QTY > 0 && sharedSerialNos.All(o => o != x.SERIAL_NO))
														 .OrderByDescending(a => a.ATYPE_CODE)
														 .ThenBy(a => a.VALID_DATE)
														 .ThenBy(a => a.ENTER_DATE)
														 .ThenByDescending(a => a.HANDY)
														 .ThenBy(a => a.HOR_DISTANCE)
														 .ThenBy(a => a.LOC_CODE)
														 .ThenBy(a => a.BOX_SERIAL)
														 .GroupBy(c => c.BOX_SERIAL).ToList();
					break;
				case ShareUnitQtyType.BoxHasCase:
					stocks = oItemPickLocStocks.Where(x => !string.IsNullOrEmpty(x.BOX_SERIAL) && !string.IsNullOrEmpty(x.CASE_NO) &&
																				 x.QTY > 0 && sharedSerialNos.All(o => o != x.SERIAL_NO))
														 .OrderByDescending(a => a.ATYPE_CODE)
														 .ThenBy(a => a.VALID_DATE)
														 .ThenBy(a => a.ENTER_DATE)
														 .ThenByDescending(a => a.HANDY)
														 .ThenBy(a => a.HOR_DISTANCE)
														 .ThenBy(a => a.LOC_CODE)
														 .ThenBy(a => a.BOX_SERIAL)
														 .GroupBy(c => c.BOX_SERIAL).ToList();
					break;
				case ShareUnitQtyType.BatchBox:
					stocks = oItemPickLocStocks.Where(x => !string.IsNullOrEmpty(x.BATCH_NO) && string.IsNullOrEmpty(x.CASE_NO) &&
																				 x.QTY > 0 && sharedSerialNos.All(o => o != x.SERIAL_NO))
														 .OrderByDescending(a => a.ATYPE_CODE)
														 .ThenBy(a => a.VALID_DATE)
														 .ThenBy(a => a.ENTER_DATE)
														 .ThenByDescending(a => a.HANDY)
														 .ThenBy(a => a.HOR_DISTANCE)
														 .ThenBy(a => a.LOC_CODE)
														 .ThenBy(a => a.BATCH_NO)
														 .GroupBy(c => c.BATCH_NO).ToList();
					break;
				case ShareUnitQtyType.Bulk:
					stocks = oItemPickLocStocks.Where(o => string.IsNullOrEmpty(o.CASE_NO) && string.IsNullOrEmpty(o.BOX_SERIAL) &&
																				 string.IsNullOrEmpty(o.BATCH_NO) && o.QTY > 0 &&
																				 sharedSerialNos.All(c => c != o.SERIAL_NO))
														 .OrderByDescending(a => a.ATYPE_CODE)
														 .ThenBy(a => a.VALID_DATE)
														 .ThenBy(a => a.ENTER_DATE)
														 .ThenByDescending(a => a.HANDY)
														 .ThenBy(a => a.HOR_DISTANCE)
														 .ThenBy(a => a.LOC_CODE)
														 .GroupBy(c => c.ATYPE_CODE + "|" + c.VALID_DATE.ToString("yyyy/MM/dd") + "|" + c.ENTER_DATE.ToString("yyyy/MM/dd") + "|" +c.HANDY + "|" + c.HOR_DISTANCE).ToList();
					break;
			}
			foreach (var stock in stocks)
			{
				if (ordQty >= unitQty)
				{
					if (((!isUnBoxing && stock.Sum(x => x.QTY) == unitQty) || isUnBoxing || shareUnitType == ShareUnitQtyType.Bulk))
						CreateAllotedStockOrder(stock,f050302,ref ordQty, ref allotedStockOrders);
				}
				else
					break;
			}
		}
		#endregion

		#region 產生訂單分配Log
		/// <summary>
		/// 產生訂單分配Log
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="f050302s">原始訂單明細</param>
		/// <param name="allotedStockOrders">已分配訂單明細</param>
		/// <param name="isTrialCalculation">是否試算</param>
		private void CreateOrderAllotLog(OrderStockCheckResult oscr,List<AllotedStockOrder> allotedStockOrders)
		{
			var messageList = new Dictionary<string, List<string>>();
		  if(oscr.IsTrialCalculation)
			{
				var f05080501Repo = new F05080501Repository(Schemas.CoreSchema, _wmsTransaction);
				var f05080502Repo = new F05080502Repository(Schemas.CoreSchema, _wmsTransaction);
				var addF05080501List = new List<F05080501>();
				var addF05080502List = new List<F05080502>();

				#region 產生釋放訂單配庫試算紀錄
				var gRelaseAllotStockOrders = oscr.ReleaseAllotStockOrderDetails
					.GroupBy(x => new { x.F050301.DC_CODE, x.F050301.GUP_CODE, x.F050301.CUST_CODE, x.F050301.ORD_NO });
				foreach (var item in gRelaseAllotStockOrders)
				{
					var f05080501 = new F05080501
					{
						DC_CODE = item.Key.DC_CODE,
						GUP_CODE = item.Key.GUP_CODE,
						CUST_CODE = item.Key.CUST_CODE,
						CAL_NO = oscr.TrialCalculationNo,
						ORD_NO = item.Key.ORD_NO,
					  RESULT_CODE = "03"  //試算結果代碼:01:全件出貨  02:部分出貨 03:無法出貨
					};
					addF05080501List.Add(f05080501);
					addF05080502List.AddRange(
					item.Select(x => new F05080502
					{
						DC_CODE = x.F050302.DC_CODE,
						GUP_CODE = x.F050302.GUP_CODE,
						CUST_CODE = x.F050302.CUST_CODE,
						CAL_NO = oscr.TrialCalculationNo,
						ORD_NO = x.F050302.ORD_NO,
						ORD_SEQ = x.F050302.ORD_SEQ,
						ITEM_CODE = x.F050302.ITEM_CODE,
						ORD_QTY = x.F050302.ORD_QTY,
						ALLOT_QTY = 0
					}).ToList());
				}
				#endregion

				#region 產生可配庫訂單試算紀錄
				var gCanAllotStockOrders = oscr.CanAllotStockOrderDetails
					.GroupBy(x => new { x.F050301.DC_CODE, x.F050301.GUP_CODE, x.F050301.CUST_CODE, x.F050301.ORD_NO });
				foreach(var item in gCanAllotStockOrders)
				{
					var f05080502s = item.Select(x => new F05080502
					{
						DC_CODE = x.F050302.DC_CODE,
						GUP_CODE = x.F050302.GUP_CODE,
						CUST_CODE = x.F050302.CUST_CODE,
						CAL_NO = oscr.TrialCalculationNo,
						ORD_NO = x.F050302.ORD_NO,
						ORD_SEQ = x.F050302.ORD_SEQ,
						ITEM_CODE = x.F050302.ITEM_CODE,
						ORD_QTY = x.F050302.ORD_QTY,
						ALLOT_QTY = allotedStockOrders.Where(y => y.F050302.DC_CODE == x.F050302.DC_CODE &&
						y.F050302.GUP_CODE == x.F050302.GUP_CODE && y.F050302.CUST_CODE == x.F050302.CUST_CODE &&
						y.F050302.ORD_NO == x.F050302.ORD_NO && y.F050302.ORD_SEQ == x.F050302.ORD_SEQ).Sum(z => z.F050302.ORD_QTY)
					}).ToList();

					var resultCode = string.Empty;
					if (f05080502s.All(x => x.ALLOT_QTY == 0))
						resultCode = "03"; //03:無法出貨
					else if (f05080502s.Any(x => x.ORD_QTY != x.ALLOT_QTY))
						resultCode = "02"; //02:部分出貨
					else
						resultCode = "01";//01:全件出貨 

					if(resultCode != "01")
					{
						var f05080501 = new F05080501
						{
							DC_CODE = item.Key.DC_CODE,
							GUP_CODE = item.Key.GUP_CODE,
							CUST_CODE = item.Key.CUST_CODE,
							CAL_NO = oscr.TrialCalculationNo,
							ORD_NO = item.Key.ORD_NO,
							RESULT_CODE = resultCode  //試算結果代碼:01:全件出貨  02:部分出貨 03:無法出貨
						};
						addF05080501List.Add(f05080501);
						addF05080502List.AddRange(f05080502s.Where(x=> x.ORD_QTY!= x.ALLOT_QTY));
					}
				}
				#endregion

				

				f05080501Repo.BulkInsert(addF05080501List);
				f05080502Repo.BulkInsert(addF05080502List);

			}
			else
			{
				var msgNo = string.Empty;
				F0020 f0020;
				
				//貨主允許部分出貨
				if (oscr.F1909.SPILT_OUTCHECK == "1")
				{

					#region 產生配庫後訂單部分庫存不足商品
					var f050302s = oscr.CanAllotStockOrderDetails.Select(x => x.F050302).ToList();
					msgNo = "AAM00022"; //訂單「{0}」商品「{1}」訂購「{2}」庫存不足無法出貨
					f0020 = GetF0020(msgNo);
					var unAllotOrderMsgList = new List<string>();
					var orderItems = f050302s.Select(x => new { x.ORD_NO, x.ITEM_CODE, x.ORD_SEQ, x.ORD_QTY });
					var allotedStockOrderItems = allotedStockOrders.Select(x => new { x.F050302.ORD_NO, x.F050302.ITEM_CODE, x.F050302.ORD_SEQ, x.F050302.ORD_QTY });
					var hasNoStocksItem = from o in orderItems
																join a in allotedStockOrderItems
																on new { o.ORD_NO,o.ITEM_CODE,o.ORD_SEQ } equals new{ a.ORD_NO,a.ITEM_CODE,a.ORD_SEQ } into g
																from a in g.DefaultIfEmpty()
																where a == null
																group o by new { o.ORD_NO, o.ITEM_CODE } into g2
																select new { g2.Key.ORD_NO,g2.Key.ITEM_CODE,OutOfStockQty = g2.Sum(x=> x.ORD_QTY) };
					foreach (var item in hasNoStocksItem)
					{
						//訂單「{0}」商品「{1}」訂購「{2}」庫存不足無法出貨
						var message = string.Format(f0020.MSG_CONTENT,item.ORD_NO,item.ITEM_CODE,item.OutOfStockQty);
						unAllotOrderMsgList.Add(message);
					}
					if(unAllotOrderMsgList.Any())
						messageList.Add(msgNo, unAllotOrderMsgList);
					#endregion
				}

				foreach (var item in messageList)
				{
					AddMessagePoolForInside("9", oscr.DcCode, oscr.GupCode, oscr.CustCode, item.Key, string.Join("\n", item.Value));
				}
			}
		}
		#endregion

		#endregion

		#region 建立訂單明細與出貨明細對應表F05030202
		/// <summary>
		/// 建立訂單明細與出貨明細對應表F05030202
		/// </summary>
		/// <param name="allotedStockOrders"></param>
		/// <param name="f050802"></param>
		/// <param name="f05030202s"></param>
		private void CreateF05030202(List<F050302> f050302s,F050802 f050802,ref List<F05030202> f05030202s)
		{
			var g = f050302s.GroupBy(x => new {x.DC_CODE,x.GUP_CODE,x.CUST_CODE, x.ORD_NO, x.ORD_SEQ });
			foreach (var item in g)
			{
				var f05030202 = new F05030202
				{
					DC_CODE = item.Key.DC_CODE,
					GUP_CODE = item.Key.GUP_CODE,
					CUST_CODE = item.Key.CUST_CODE,
					ORD_NO = item.Key.ORD_NO,
					ORD_SEQ = item.Key.ORD_SEQ,
					WMS_ORD_NO = f050802.WMS_ORD_NO,
					WMS_ORD_SEQ = f050802.WMS_ORD_SEQ,
					B_DELV_QTY = item.Sum(x=> x.ORD_QTY)
				};
				f05030202s.Add(f05030202);
			}
		}
		#endregion

	}
}
