using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
  public class CheckOrderService
  {
    private TransApiBaseService _tacService;
    private CommonService _commonService;
    public CommonService CommonService
    {
      get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
      set { _commonService = value; }
    }

    private List<F000904Data> _f000904Datas;

    public CheckOrderService(CommonService _commService = null)
		{
			_tacService = new TransApiBaseService();
			CommonService = _commService;
		}

		#region 主檔欄位資料檢核

		/// <summary>
		/// 檢查ProcFlag
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		/// <returns></returns>
		public void CheckProcFlag(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			List<string> procFlags = new List<string> { "0", "D" };
			if (!procFlags.Contains(order.ProcFlag))
				res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20961", MsgContent = string.Format(_tacService.GetMsg("20961"), order.CustOrdNo) });
		}

		/// <summary>
		/// 檢查刪除狀態貨主單號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="thirdPartOrdersList"></param>
		/// <param name="warehouseIns"></param>
		/// <returns></returns>
		public bool CheckCustExistForThirdPart(List<ApiResponse> res, List<P8202050000_F050101> custWmsList, PostCreateOrdersModel order, string custCode)
		{
			var notCancelStatus = new List<decimal> { 2, 5, 6 };// 2(已包裝)或5(已裝車)或6(已扣帳)
			var isAddF075102 = false;
			var f075102Repo = new F075102Repository(Schemas.CoreSchema);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var currCustData = custWmsList.Where(x => x.CUST_ORD_NO == order.CustOrdNo).FirstOrDefault();
			if (order.ProcFlag == "D")
			{
				if (currCustData == null)
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20962", MsgContent = string.Format(_tacService.GetMsg("20962"), order.CustOrdNo) });
				else
				{
					// 檢核是否有一筆出貨單狀態為[F050801.STATUS] = 2(已包裝)或5(已裝車)或6(已扣帳)則不允許訂單取消
					if (currCustData.F050801_STATUS_List.Any() && currCustData.F050801_STATUS_List.Where(x => notCancelStatus.Contains(x)).Any())
						res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20965", MsgContent = string.Format(_tacService.GetMsg("20965"), order.CustOrdNo) });
					else
						f075102Repo.DelF075102ByKey(custCode, order.CustOrdNo);// 刪除F075102
				}
			}
			else if (order.ProcFlag == "0")
			{
				if (currCustData != null)
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20964", MsgContent = string.Format(_tacService.GetMsg("20964"), order.CustOrdNo) });
				else
				{
					#region 新增出貨單匯入控管紀錄表
					var f075102Res = f075102Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
						new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
					() =>
					{
						var lockF075102 = f075102Repo.LockF075102();
						var f075102 = f075102Repo.Find(o => o.CUST_CODE == custCode && o.CUST_ORD_NO == order.CustOrdNo, isForUpdate: true, isByCache: false);
						if (f075102 == null)
						{
							f075102 = new F075102 { CUST_CODE = custCode, CUST_ORD_NO = order.CustOrdNo };
							f075102Repo.Add(f075102);
							isAddF075102 = true;
						}
						else
						{
							f075102 = null; // 代表F075102已存在資料
						}
						return f075102;
					});
					if (f075102Res == null)// 代表F075102已存在資料
						res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20964", MsgContent = string.Format(_tacService.GetMsg("20964"), order.CustOrdNo) });
					#endregion
				}
			}

			return isAddF075102;
		}

		/// <summary>
		/// 1-1. 檢查訂單類型
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		public void CheckOrderType(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			if (order.ProcFlag == "0")
			{
				List<string> orderTypes = new List<string> { "0", "1" };

				if (!orderTypes.Contains(order.OrderType))
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20751", MsgContent = string.Format(_tacService.GetMsg("20751"), order.CustOrdNo) });
			}
		}

		/// <summary>
		/// 1.2. 檢查配送方式
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		public void CheckShipWay(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			if (order.ProcFlag == "0")
			{
				List<string> shipWays = new List<string> { "1", "2", "3", "4" };

				if (!shipWays.Contains(order.ShipWay))
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20752", MsgContent = string.Format(_tacService.GetMsg("20752"), order.CustOrdNo) });
			}
		}

		/// <summary>
		/// 1.3. 檢查方便到貨時段
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		public void CheckDeliveryPeriod(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			if (order.ProcFlag == "0")
			{
				List<byte> deliveryPeriods = new List<byte> { 1, 2, 3, 4 };

				if (order.DeliveryPeriod == null || order.DeliveryPeriod == 0 || !deliveryPeriods.Contains(Convert.ToByte(order.DeliveryPeriod)))
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20754", MsgContent = string.Format(_tacService.GetMsg("20754"), order.CustOrdNo) });
			}
		}

		/// <summary>
		/// 1.4. 檢核出貨倉別
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		/// <param name="warhouseList"></param>
		public void CheckWarehouseId(List<ApiResponse> res, PostCreateOrdersModel order, List<F1980> warhouseList)
		{
			if (order.ProcFlag == "0")
			{
				if (!warhouseList.Where(x => x.WAREHOUSE_TYPE == order.WarehouseId).Any())
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20759", MsgContent = string.Format(_tacService.GetMsg("20759"), order.CustOrdNo, order.WarehouseId) });
			}
		}

		/// <summary>
		/// 1.5. 檢查物流商是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		/// <param name="f1947List"></param>
		public void CheckLogisticsProvider(List<ApiResponse> res, PostCreateOrdersModel order, List<F1947> f1947List)
		{
			if (order.ProcFlag == "0" && order.ShipWay != "3" && !f1947List.Where(x => x.ALL_ID == order.LogisticsProvider).Any())
				res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20761", MsgContent = string.Format(_tacService.GetMsg("20761"), order.CustOrdNo, order.LogisticsProvider) });
		}

		/// <summary>
		/// 1.6. 檢查超取配送商
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		/// <param name="eServiceList"></param>
		public void CheckEService(List<ApiResponse> res, PostCreateOrdersModel order, List<F194713> eServiceList)
		{
			if (order.ProcFlag == "0" && order.ShipWay == "1" && !eServiceList.Where(x => x.ALL_ID == order.LogisticsProvider && x.ESERVICE == order.EServiceNo).Any())
				res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20757", MsgContent = string.Format(_tacService.GetMsg("20757"), order.CustOrdNo, order.EServiceNo) });
		}

		/// <summary>
		/// 1.7. 檢查門市編號
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		/// <param name="retailList"></param>
		public void CheckRetail(List<ApiResponse> res, PostCreateOrdersModel order, List<F1910> retailList)
		{
			if (order.ProcFlag == "0" && order.OrderType == "0" && !retailList.Where(x => x.RETAIL_CODE == order.SalesBaseNo).Any())
				res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20758", MsgContent = string.Format(_tacService.GetMsg("20758"), order.CustOrdNo, order.SalesBaseNo) });
		}

		/// <summary>
		/// 檢查貨主自訂義分類
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		public void CheckCustCost(List<ApiResponse> res, PostCreateOrdersModel order)
		{
      if (_f000904Datas == null)
      {
        _f000904Datas = new List<F000904Data>();
        var f000904Repo = new F000904Repository(Schemas.CoreSchema);
        _f000904Datas = f000904Repo.GetF000904Data("F050101", "CUST_COST").ToList();
      }

      List<string> custCost = _f000904Datas.Select(x => x.VALUE).Distinct().ToList();

      if (order.ProcFlag == "0" && (!string.IsNullOrWhiteSpace(order.CustCost) && !custCost.Contains(order.CustCost)))
				res.Add(new ApiResponse {
          No = order.CustOrdNo,
          MsgCode = "20023",
          MsgContent = string.Format(_tacService.GetMsg("20023"), order.CustOrdNo, string.Join(", ", _f000904Datas.Select(x => $"{x.VALUE}:{x.NAME}")))
        });
		}

		/// <summary>
		/// 檢查優先處理旗標
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		public void CheckFastDealType(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			List<string> fastDealType = new List<string> { "1", "2", "3" };

			if (order.ProcFlag == "0" && (!string.IsNullOrWhiteSpace(order.FastDealType) && !fastDealType.Contains(order.FastDealType)))
				res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20113", MsgContent = string.Format(_tacService.GetMsg("20113"), order.CustOrdNo) });
		}

    /// <summary>
    /// 檢查商品處理類型
    /// </summary>
    /// <param name="res"></param>
    /// <param name="warehouseIns"></param>
    public void CheckItemOpType(List<ApiResponse> res, PostCreateOrdersModel order)
    {
      List<string> ItemOpType = new List<string> { "0", "1" };

      if (order.ProcFlag == "0" && (!string.IsNullOrWhiteSpace(order.ItemOpType) && !ItemOpType.Contains(order.ItemOpType)))
        res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20129", MsgContent = string.Format(_tacService.GetMsg("20129"), order.CustOrdNo) });
    }

    /// <summary>
    /// 檢查收貨/店取額外資訊
    /// </summary>
    /// <param name="res"></param>
    /// <param name="warehouseIns"></param>
    public void CheckReceiverOpt(List<ApiResponse> res, PostCreateOrdersModel order)
    {
      List<string> IsTPEKEL = new List<string> { "0", "1" };

      //如果檢查收貨/店取額外資訊為空後續內容就不檢查
      if (order.ProcFlag == "0" && order.ReceiverOpt == null)
        return;

      //檢查是否為北北基欄位資料是否正確
      if (order.ProcFlag == "0" && !string.IsNullOrWhiteSpace(order.ReceiverOpt.IsTPEKEL) && !IsTPEKEL.Contains(order.ReceiverOpt.IsTPEKEL))
        res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20130", MsgContent = string.Format(_tacService.GetMsg("20130"), order.CustOrdNo) });
    }

    /// <summary>
    /// 1-8.檢查貨主單號是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="order"></param>
    public void CheckCustOrdNo(PostCreateOrdersModel order, ref List<F050101> f050101List, ref List<F050102> f050102List,
				ref List<F05010301> f050101301List, ref List<F050103> f050103List, ref List<F050104> f050104List,
				ref List<F050304> f050304List, ref List<F050001> f050001List, ref List<F050002> f050002List)
		{
			/* 檢查貨主單號是否存在[之前Cache已新增的訂單]:
			 * [找到的WMS單]=檢查<參數4>.CustOrdNo是否存在於[之前Cache已新增的訂單] 中找CUST_ORD_NO =< 參數4 >.CustOrdNo
			 * 若存在 
			 * 移除之前Cache此CustOrdNo已新增的訂單、訂單明細、訂單延伸檔等(參考6-6 新增的資料表) */

			var ordNo = string.Empty;

			#region F050101
			var repeatedF050101 = f050101List.Where(x => x.CUST_ORD_NO == order.CustOrdNo).SingleOrDefault();

			if (repeatedF050101 != null)
			{
				ordNo = repeatedF050101.ORD_NO;
				f050101List.Remove(repeatedF050101);
			}
			#endregion

			#region F050102
			if (repeatedF050101 != null)
			{
				var repeatedF050102 = f050101List.Where(x => x.ORD_NO == ordNo);
				if (repeatedF050102.Any())
					f050101List = f050101List.Except(repeatedF050102).ToList();
			}
			#endregion

			#region F05010301
			if (repeatedF050101 != null)
			{
				var repeatedF050101301 = f050101301List.Where(x => x.ORD_NO == ordNo);
				if (repeatedF050101301.Any())
					f050101301List = f050101301List.Except(repeatedF050101301).ToList();
			}
			#endregion

			#region F050103
			if (repeatedF050101 != null)
			{
				var repeatedF050103 = f050103List.Where(x => x.ORD_NO == ordNo).SingleOrDefault();
				if (repeatedF050103 != null)
					f050103List.Remove(repeatedF050103);
			}
			#endregion

			#region F050104
			if (repeatedF050101 != null)
			{
				var repeatedF050104 = f050104List.Where(x => x.ORD_NO == ordNo);
				if (repeatedF050104.Any())
					f050104List = f050104List.Except(repeatedF050104).ToList();
			}
			#endregion

			#region F050304
			if (repeatedF050101 != null)
			{
				var repeatedF050304 = f050304List.Where(x => x.ORD_NO == ordNo).SingleOrDefault();
				if (repeatedF050304 != null)
					f050304List.Remove(repeatedF050304);
			}
			#endregion

			#region F050001
			if (repeatedF050101 != null)
			{
				var repeatedF050001 = f050001List.Where(x => x.ORD_NO == ordNo).SingleOrDefault();
				if (repeatedF050001 != null)
					f050001List.Remove(repeatedF050001);
			}
			#endregion

			#region F050002
			if (repeatedF050101 != null)
			{
				var repeatedF050002 = f050002List.Where(x => x.ORD_NO == ordNo);
				if (repeatedF050002.Any())
					f050002List = f050002List.Except(repeatedF050002).ToList();
			}
			#endregion
		}
		#endregion

		#region 明細欄位資料檢核
		/// <summary>
		/// 檢查明細筆數
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		public void CheckDetailCnt(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			// 如果<參數4>.ProcFlag<> D且檢查明細是否為null或明細筆數是否為0，若是則回傳&訊息內容[20071, <參數4>.CustInNo]
			if (order.ProcFlag == "0" && (order.Details == null || (order.Details != null && order.Details.Count == 0)))
			{
				res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20071", MsgContent = string.Format(_tacService.GetMsg("20071"), order.CustOrdNo) });
			}
		}

		/// <summary>
		/// 檢核項次必須大於0，且同一張單據內的序號不可重複
		/// </summary>
		/// <param name="res"></param>
		/// <param name="returns"></param>
		public void CheckDetailSeq(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			if (order.ProcFlag == "0" && order.Details != null && order.Details.Count > 0)
			{
				if (order.Details.Count > order.Details.Select(x => x.ItemSeq).Distinct().Count())
				{
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20070", MsgContent = _tacService.GetMsg("20070") });
				}
			}
		}

		/// <summary>
		/// 檢核明細Qty是否大於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		public void CheckDetailQty(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			// 檢核明細InQty是否大於0,若否則回傳&訊息內容[20060, < 參數4 >.CustInNO, 去重複品號清單(格式:[品號1]、[品號2])]
			if (order.ProcFlag == "0" && order.Details != null && order.Details.Count > 0)
			{
				var itemSeqList = order.Details.Where(x => x.Qty <= 0).Select(x => x.ItemSeq).Distinct().ToList();

				if (itemSeqList.Count > 0)
				{
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20060", MsgContent = string.Format(_tacService.GetMsg("20060"), order.CustOrdNo, string.Join("、", itemSeqList)) });
				}
			}
		}
		#endregion

		#region 明細服務欄位檢核
		/// <summary>
		/// 明細服務欄位檢核
		/// </summary>
		/// <param name="res"></param>
		/// <param name="order"></param>
		public void CheckServiceItemCode(List<ApiResponse> res, PostCreateOrdersModel order)
		{
			if (order.ProcFlag == "0" && order.Details != null && order.Details.Any())
			{
				for (int i = 0; i < order.Details.Count; i++)
				{
					var currDetail = order.Details[i];
					if (currDetail.ServiceItemDetails.Any())
					{
						var repeatServiceCode = currDetail.ServiceItemDetails.GroupBy(x => x.ServiceItemCode)
								.Select(x => new { ServiceItemCode = x.Key, IsPass = x.Count() == 1 })
								.Where(x => !x.IsPass).Select(x => x.ServiceItemCode).ToList();

						if (repeatServiceCode.Any())
							res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20963", MsgContent = string.Format(_tacService.GetMsg("20963"), $"{order.CustOrdNo}第{i + 1}筆明細", string.Join("、", repeatServiceCode)) });
					}
				}
			}
		}
		#endregion

		#region 檢查資料是否完整
		/// <summary>
		/// 檢查資料是否完整
		/// </summary>
		/// <param name="res"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public void CheckOrderData(List<ApiResponse> res, string gupCode, string custCode, PostCreateOrdersModel order)
		{
			if (order.ProcFlag == "0")
			{

				var itemCodeList = order.Details.Select(x => x.ItemCode).Distinct().ToList();

				//2.[商品資料] = 取得商品資料[< 參數1 >.GUP_CODE,< 參數1 >.CUST_CODE, DISTINCT<參數2>.ITEM_CODE]
				var productList = _commonService.GetProductList(gupCode, custCode, order.Details.Select(x => x.ItemCode).Distinct().ToList());

				//3.[差異品號清單] = DISTINCT<參數2>.ITEM_CODE 比較[商品資料].ITEM_CODE 是否有差異(訂單品號無商品主檔資料)
				var differentData = itemCodeList.Except(productList.Select(x => x.ITEM_CODE));

				//  IF[差異品號清單].Any() then
				if (differentData.Any())
				{
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20086", MsgContent = string.Format(_tacService.GetMsg("20086"), order.CustOrdNo, string.Join("、", differentData)) });
				}

        #region 指定序號出貨檢查
        if (!differentData.Any())
        {
          var chkSerialNoColumn = order.Details.Where(x => !string.IsNullOrWhiteSpace(x.SerialNo));
          if (chkSerialNoColumn.Any())
          {
            //如果有給序號，數量不可為1以外的值
            if (chkSerialNoColumn.Any(x => x.Qty != 1))
              res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20756", MsgContent = _tacService.GetMsg("20756") });

            //如果有給序號，只能序號綁儲位商品
            var chkBundleLoc = productList.Where(x => chkSerialNoColumn.Any(y => y.ItemCode == x.ITEM_CODE) && x.BUNDLE_SERIALLOC != "1");
            if (chkBundleLoc.Any())
            {
              res.Add(
                new ApiResponse
                {
                  No = order.CustOrdNo,
                  MsgCode = "20790",
                  MsgContent = string.Format(_tacService.GetMsg("20790"), string.Join("、", chkBundleLoc.Select(x => x.ITEM_CODE)))
                });
            }
          }
        }
        #endregion 指定序號出貨檢查


        // 3.[停售品號清單] = 檢查[商品資料]的停售日期(STOP_DATE) <= 系統日(DateTime.Today)
        var cessationOfSaleList = productList.Where(x => x.STOP_DATE <= DateTime.Today).Select(x => x.ITEM_CODE);

				// IF[停售品號清單].Any() then
				if (cessationOfSaleList.Any())
				{
					res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20087", MsgContent = string.Format(_tacService.GetMsg("20087"), order.CustOrdNo, string.Join("、", differentData)) });
				}
			}
		}

    /// <summary>
    /// 檢查通路編號是否存在，有判斷大小寫！！
    /// </summary>
    /// <param name="res"></param>
    /// <param name="order"></param>
    public void CheckChannelCode(List<ApiResponse> res, PostCreateOrdersModel order)
    {
      if (order.ProcFlag == "0")
      {
        var channels = _commonService.GetF000904s("F050101", "CHANNEL");

        if (!channels.Any(x => x.VALUE == order.ChannelCode))
          res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20053", MsgContent = _tacService.GetMsg("20053"), ErrorColumn = "ChannelCode" });
      }
    }

    /// <summary>
    /// 檢查子通路編號是否存在，有判斷大小寫！！
    /// </summary>
    /// <param name="res"></param>
    /// <param name="order"></param>
    public void CheckSubChannelCode(List<ApiResponse> res, PostCreateOrdersModel order)
    {
      if (order.ProcFlag == "0")
      {
        var subChannels = _commonService.GetF000904s("F050101", "SUBCHANNEL");

        if (!subChannels.Any(x => x.VALUE == order.SubChannelCode))
          res.Add(new ApiResponse { No = order.CustOrdNo, MsgCode = "20131", MsgContent = _tacService.GetMsg("20131"), ErrorColumn = "SubChannelCode" });
      }
    }

    /// <summary>
    /// 檢查建議物流商編號
    /// </summary>
    /// <param name="res"></param>
    /// <param name="order"></param>
    public void CheckSuggestLogisticCod(List<ApiResponse> res, PostCreateOrdersModel order, List<F0002> f0002List)
    {
      if (order.ProcFlag == "0")
      {
        if (!string.IsNullOrWhiteSpace(order.SuggestLogisticCode))
        {
          var f0002 = f0002List.FirstOrDefault(x => x.LOGISTIC_CODE.ToUpper() == order.SuggestLogisticCode.ToUpper());
          if (f0002 == null)
          {
            res.Add(new ApiResponse
            {
              No = order.CustOrdNo,
              MsgCode = "20761",
              MsgContent = string.Format(_tacService.GetMsg("20761"), order.CustOrdNo, order.SuggestLogisticCode),
              ErrorColumn = "SuggestLogisticCode"
            });
          }
          else
          {
            order.SuggestLogisticCode = f0002.LOGISTIC_CODE;
          }
        }
      }
    }

    #endregion
  }
}
